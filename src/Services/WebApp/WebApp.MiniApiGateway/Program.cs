using System.Diagnostics;
using System.Text.Json;
using Jint;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options =>
{
    options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"]);
});
builder.Services.AddHttpLogging(o => { });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting up...");
app.UseHttpLogging();
app.Use(async (context, next) =>
{
    var appRepository = context.RequestServices.GetService<IAppRepository>()!;
    var routeRepository = context.RequestServices.GetService<IRouteRepository>()!;
    var logRepository = context.RequestServices.GetService<ILogRepository>()!;

    int? appId = null;
    var fromHeader = true;
    if (fromHeader)
    {
        if (context.Request.Headers.TryGetValue("X-AppId", out var headerAppId)
            && int.TryParse(headerAppId.ToString()["App-".Length..], out var parsedAppId))
        {
            appId = parsedAppId;
        }
    }

    if (appId is null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    var app = await appRepository.FindByAppId(appId.Value);
    if (app is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    var routes = await routeRepository.List(app.Id, "", "");
    Route? route = null;
    foreach (var r in routes)
    {
        var matcher = new TemplateMatcher(TemplateParser.Parse(r.Path), []);
        if (matcher.TryMatch(context.Request.Path, context.Request.RouteValues) && context.Request.Method.Equals(r.Method, StringComparison.OrdinalIgnoreCase))
        {
            route = r;
            break;
        }
    }

    if (route is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    context.Items["_App"] = app;
    context.Items["_Route"] = route;

    await next(context);

    var functionExecutionResult = context.Items["_FunctionExecutionResult"] as FunctionExecutionResult;

    await logRepository.Add(new Log
    {
        App = app,
        Route = route,
        Method = context.Request.Method,
        Path = context.Request.Path,
        StatusCode = context.Response.StatusCode,
        AdditionalLogMessage = functionExecutionResult?.AdditionalLogMessage,
        FunctionExecutionDuration = functionExecutionResult?.Duration
    });
});

app.Run(async context =>
{
    var route = (Route)context.Items["_Route"]!;
    if (route.ResponseType == "static")
    {
        context.Response.StatusCode = route.ResponseStatusCode ?? throw new InvalidOperationException("ResponseStatusCode is null");
        if (route.ResponseHeaders is not null)
        {
            foreach (var header in route.ResponseHeaders)
            {
                context.Response.Headers.Append(header.Name, header.Value);
            }
        }
        await context.Response.WriteAsync(route.ResponseBody ?? throw new InvalidOperationException("ResponseBody is null"));
        return;
    }
    else if (route.ResponseType == "function")
    {
        object? reqBody = null;
        try
        {
            reqBody = JsonSerializer.Deserialize<Dictionary<string, object>>(await new StreamReader(context.Request.Body).ReadToEndAsync());
        }
        catch (Exception)
        {
        }
        var req = new
        {
            method = context.Request.Method,
            path = context.Request.Path.Value,
            @params = context.Request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString()),
            query = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            body = reqBody
        };
        //Start measuring time for function execution
        var stopwatch = Stopwatch.StartNew();

        var engine = new Engine()
                        .Execute(route.FunctionHandler ?? throw new InvalidOperationException("FunctionHandler is null"));
        var handler = engine.GetValue("handler");
        if (route.FunctionHandler == "AwsLamda")
        {

        }
        else
        {
            //Execute function and get response
            var result = new FunctionExecutionResult();
            var jsResult = engine.Invoke(handler, req);
            stopwatch.Stop();
            result.Duration = stopwatch.Elapsed;
            var statusCode = jsResult.Get("statusCode");
            if (!statusCode.IsNull())
            {
                result.StatusCode = (int)statusCode.AsNumber();
            }
            var headers = jsResult.Get("headers");
            if (!headers.IsNull())
            {
                var headersObject = headers.AsObject();
                var headersObjectProperties = headersObject.GetOwnProperties();
                foreach (var (k, v) in headersObjectProperties)
                {
                    var headerName = k.AsString();
                    string headerValue;

                    if (headerName is null)
                    {
                        continue;
                    }

                    var value = v.Value;

                    if (value.IsNull())
                    {
                        continue;
                    }
                    if (value.IsNumber())
                    {
                        headerValue = value.AsNumber().ToString();
                    }
                    else if (value.IsString())
                    {
                        headerValue = value.AsString();
                    }
                    else if (value.IsBoolean())
                    {
                        headerValue = value.AsBoolean().ToString();
                    }
                    else
                    {
                        continue;
                    }

                    if (!result.Headers.TryAdd(headerName, headerValue))
                    {
                        result.Headers[headerName] = headerValue;
                    }
                }
            }
            var body = jsResult.Get("body");
            if (!body.IsNull())
            {
                result.Body = body.AsString();
            }
            var additionalLogMessage = jsResult.Get("additionalLogMessage");
            if (!additionalLogMessage.IsNull())
            {
                result.AdditionalLogMessage = additionalLogMessage.AsString();
            }

            //Write response
            context.Response.StatusCode = result.StatusCode ?? 200;
            foreach (var (key, value) in result.Headers)
            {
                context.Response.Headers.Append(key, value);
            }
            await context.Response.WriteAsync(result.Body ?? "");
            context.Items["_FunctionExecutionResult"] = result;
        }

        return;
    }
    throw new NotImplementedException();
});

app.Run();

class FunctionExecutionResult
{
    public int? StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public string? Body { get; set; }
    public string? AdditionalLogMessage { get; set; }
    public TimeSpan Duration { get; set; }
}