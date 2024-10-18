using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using Jint;
using Jint.Native;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure;

namespace WebApp.MiniApiGateway.Middlewares;

public class FunctionInvokerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, Scripts scripts, IAppRepository appRepository)
    {
        var app = (App)context.Items["_App"]!;
        var route = (Route)context.Items["_Route"]!;
        var request = await ReadRequest(context.Request);

        var engine = new Engine(options =>
        {
            if (app.Settings.CheckFunctionExecutionLimitMemory)
            {
                var memoryLimit = app.Settings.FunctionExecutionLimitMemoryBytes ?? 1 * 1024 * 1024;
                memoryLimit += 10 * 1024 * 1024;
                
                options.LimitMemory(memoryLimit);
            }

            if (app.Settings.CheckFunctionExecutionTimeout)
            {
                options.TimeoutInterval(TimeSpan.FromSeconds(app.Settings.FunctionExecutionTimeoutSeconds ?? 3));
            }
        });

        //Inject global variables
        await InjectEnvironmentVariables(appRepository, app, engine);

        // Inject utility scripts
        InjectBuiltInUltilityScripts(scripts, engine);

        //Inject user-defined dependencies
        await InjectUserDefinedDependencies(route, engine);
        
        //Inject plugins
        InjectPlugIns(engine, app);

        JsValue jsResult;
        var result = new FunctionExecutionResult();
        //Start measuring time for function execution
        var startingTimestamp = Stopwatch.GetTimestamp();
        try
        {
            engine.Execute(route.FunctionHandler);
            var handler = engine.GetValue(route.FunctionHandlerMethod ?? "handler");
            jsResult = engine.Invoke(handler, request);
        }
        catch (Exception e)
        {
            jsResult = JsValue.FromObject(engine, new
            {
                statusCode = 500,
                body = e.Message,
                additionalLogMessage = e.Message
            });
            result.Exception = e;
        }
        finally
        {
            result.Duration = Stopwatch.GetElapsedTime(startingTimestamp);
        }
        
        var statusCode = jsResult.Get("statusCode");
        if (statusCode.IsNumber())
        {
            result.StatusCode = (int)statusCode.AsNumber();
        }

        var headers = jsResult.Get("headers");
        if (headers.IsObject())
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
        if (body.IsString())
        {
            result.Body = body.AsString();
        }

        var additionalLogMessage = jsResult.Get("additionalLogMessage");
        if (additionalLogMessage.IsString())
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

    private static void InjectPlugIns(Engine engine, App app)
    {
        engine.SetValue("useTextStorage", new Func<string, TextStorageAdapter>(name => {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(ConfigurationHelper.Configuration!.GetConnectionString("DefaultConnection"));
            var appDbContext = new AppDbContext(optionsBuilder.Options);
            var adapter = new TextStorageAdapter(app, name, appDbContext);
            return adapter;
        }));
    }

    private async Task InjectUserDefinedDependencies(Route route, Engine engine)
    {
        foreach (var dependency in route.FunctionHandlerDependencies ?? [])
        {
            var script = await LoadScript(dependency);
            if (!string.IsNullOrEmpty(script))
            {
                engine.Execute(script);
            }
        }
    }

    private static void InjectBuiltInUltilityScripts(Scripts scripts, Engine engine)
    {
        //engine.Execute(scripts.Faker);
        engine.Execute(scripts.Handlebars);
        engine.Execute(scripts.Lodash);
    }

    private static async Task InjectEnvironmentVariables(IAppRepository appRepository, App app, Engine engine)
    {
        var variables = new Dictionary<string, object?>();
        var appVariables = await appRepository.GetVariables(app.Id);
        foreach (var variable in appVariables)
        {
            object? value = variable.ValueType switch
            {
                VariableValueType.String => variable.StringValue,
                VariableValueType.Number => int.Parse(variable.StringValue),
                VariableValueType.Boolean => bool.Parse(variable.StringValue),
                VariableValueType.Null => null,
                _ => throw new ArgumentOutOfRangeException()
            };
            variables[variable.Name] = value;
        }
        engine.SetValue("env", variables);
    }

    private async Task<string> LoadScript(string dependency)
    {
        return string.Empty;
    }

    private static async Task<object> ReadRequest(HttpRequest request)
    {
        var bodyString = await new StreamReader(request.Body).ReadToEndAsync();
        return new
        {
            method = request.Method,
            path = request.Path.Value,
            @params = request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString()),
            query = request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            body = !string.IsNullOrEmpty(bodyString) ? JsonSerializer.Deserialize<dynamic>(bodyString) : null
        };
    }
}

public static class FunctionInvokerMiddlewareExtensions
{
    public static IApplicationBuilder UseFunctionInvokerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FunctionInvokerMiddleware>();
    }
}

public class FunctionExecutionResult
{
    public int? StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public string? Body { get; set; }
    public string? AdditionalLogMessage { get; set; }
    public TimeSpan Duration { get; set; }
    public Exception? Exception { get; set; }
}