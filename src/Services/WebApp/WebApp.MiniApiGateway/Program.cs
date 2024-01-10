using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Jint;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.MiniApiGateway;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options => { options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"]); });
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
if (app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        var subDomain = context.Request.Host.Host.Split(".")[0];
        context.Request.Headers.Append("X-AppId", subDomain);
        return next(context);
    });
}

app.Use(async (context, next) =>
{
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
    var appRepository = context.RequestServices.GetService<IAppRepository>()!;
    var foundApp = await appRepository.FindByAppId(appId.Value);
    if (foundApp is null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    context.Items["_App"] = foundApp;
    await next(context);
});

app.Use(async (context, next) =>
{
    var routeRepository = context.RequestServices.GetService<IRouteRepository>()!;
    var logRepository = context.RequestServices.GetService<ILogRepository>()!;
    var foundApp = (App)context.Items["_App"]!;
    var routes = await routeRepository.List(foundApp.Id, "", "");
    var matchedRoutes = new List<Route>();
    foreach (var r in routes)
    {
        var matcher = new TemplateMatcher(TemplateParser.Parse(r.Path), []);
        if (matcher.TryMatch(context.Request.Path, context.Request.RouteValues) && 
            (r.Method.Equals("ANY") || context.Request.Method.Equals(r.Method, StringComparison.OrdinalIgnoreCase)))
        {
            matchedRoutes.Add(r);
        }
    }

    switch (matchedRoutes.Count)
    {
        case 0:
            context.Response.StatusCode = 404;
            return;
        case > 1:
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("The request matched multiple endpoints");
            return;
    }

    var route = matchedRoutes.First();
    context.Items["_Route"] = route;
    object? reqBody = null;
    try
    {
        //TODO:
        reqBody = JsonSerializer.Deserialize<Dictionary<string, object>>(
            await new StreamReader(context.Request.Body).ReadToEndAsync());
    }
    catch (Exception)
    {
        // ignored
    }

    var request = new
    {
        method = context.Request.Method,
        path = context.Request.Path.Value,
        @params = context.Request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString()),
        query = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
        headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
        body = reqBody
    };
    context.Items["_Request"] = request;
    await next(context);

    var functionExecutionResult = context.Items["_FunctionExecutionResult"] as FunctionExecutionResult;

    await logRepository.Add(new Log
    {
        App = foundApp,
        Route = route,
        Method = context.Request.Method,
        Path = context.Request.Path,
        StatusCode = context.Response.StatusCode,
        AdditionalLogMessage = functionExecutionResult?.AdditionalLogMessage,
        FunctionExecutionDuration = functionExecutionResult?.Duration
    });
});

//Validation
app.Use(async (context, next) =>
{
    var route = (Route)context.Items["_Route"]!;
    var routeRepository = context.RequestServices.GetService<IRouteRepository>()!;
    var validations = await routeRepository.GetValidations(route.Id);
    if (validations.Count == 0)
    {
        await next(context);
        return;
    }
    var request = context.Items["_Request"]!;
    var engine = new Engine()
        .SetValue("request", request);
    var errors = new Dictionary<string,string>();
    foreach (var validation in validations)
    {
        var errorKey = $"{validation.Source.ToLower()}:{validation.Name}";
        switch (validation.Source.ToLower())
        {
            case "header":
            {
                foreach (var rule in validation.Rules)
                {
                    switch (rule.Key.ToLower())
                    {
                        case "required":
                            if (!context.Request.Headers.TryGetValue(validation.Name, out var value) || string.IsNullOrEmpty(value))
                            {
                                var property = rule.Value.GetType().GetProperty("message");
                                var message = property != null ? property.GetValue(rule.Value)?.ToString() ?? "" : $"header {validation.Name} is required";
                                errors.Add(errorKey, message);
                            }
                            break;
                    }
                }

                foreach (var expression in validation.Expressions ?? [])
                {
                    engine.Evaluate(expression);
                }
                break;
            }
        }
    }

    if (errors.Count == 0)
    {
        await next(context);
        return;
    }

    context.Response.StatusCode = 400;
    await context.Response.WriteAsync($"Bad request. {errors.First().Value}");
});

app.Run(async context =>
{
    var route = (Route)context.Items["_Route"]!;
    switch (route.ResponseType)
    {
        case "static":
        {
            context.Response.StatusCode = route.ResponseStatusCode ??
                                          throw new InvalidOperationException("ResponseStatusCode is null");
            if (route.ResponseHeaders is not null)
            {
                foreach (var header in route.ResponseHeaders)
                {
                    context.Response.Headers.Append(header.Name, header.Value);
                }
            }

            await context.Response.WriteAsync(route.ResponseBody ??
                                              throw new InvalidOperationException("ResponseBody is null"));
            return;
        }
        case "function":
        {
            var request = context.Items["_Request"]!;
            //Start measuring time for function execution
            var stopwatch = Stopwatch.StartNew();

            var engine = new Engine()
                .Execute(route.FunctionHandler ?? throw new InvalidOperationException("FunctionHandler is null"));
            var handler = engine.GetValue("handler");

            //Execute function and get response
            var result = new FunctionExecutionResult();
            var jsResult = engine.Invoke(handler, request);
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

                    var value = v.Value;

                    if (value.IsNull())
                    {
                        continue;
                    }

                    if (value.IsNumber())
                    {
                        headerValue = value.AsNumber().ToString(CultureInfo.InvariantCulture);
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

            return;
        }
        default:
            throw new NotImplementedException();
    }
});

app.Run();