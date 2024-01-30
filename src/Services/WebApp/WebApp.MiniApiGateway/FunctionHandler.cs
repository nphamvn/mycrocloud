using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Jint;
using Jint.Native;
using WebApp.Domain.Entities;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;
public static class FunctionHandler
{
    public static async Task Handle(HttpContext context)
    {
        var app = (App)context.Items["_App"]!;
        var route = (Route)context.Items["_Route"]!;
        var request = await ReadRequest(context.Request);
        var scripts = context.RequestServices.GetRequiredService<ScriptCollection>();

        var engine = new Engine(options =>
        {
            if (app.Settings.CheckFunctionExecutionLimitMemory)
            {
                options.LimitMemory(app.Settings.FunctionExecutionLimitMemoryBytes ?? 10 * 1024 * 1024);
            }

            if (app.Settings.CheckFunctionExecutionTimeout)
            {
                options.TimeoutInterval(TimeSpan.FromSeconds(app.Settings.FunctionExecutionTimeoutSeconds ?? 15));
            }
        });

        //Inject global variables
        engine.SetValue("env", JsValue.FromObject(engine, new
        {
            CONNECTION_STRING = "Data Source=app.db;",
            APP_NAME = app.Name,
            APP_ID = app.Id,
        }));

        //Inject dependencies
        foreach (var dependency in route.FunctionHandlerDependencies ?? [])
        {
            if (scripts.TryGetValue(dependency, out var script))
            {
                engine.Execute(script);
            }
        }
        //Inject plugins
        engine.SetValue("useNoSqlDatabase", new Func<string, NoSqlDatabaseConnection>((connectionString) => {
            var httpClientFactory = context.RequestServices.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("NoSqlDbServer");
            var connection = new NoSqlDatabaseConnection(connectionString, httpClient, engine);
            return connection;
        }));

        JsValue jsResult;
        var result = new FunctionExecutionResult();
        //Start measuring time for function execution
        var stopwatch = Stopwatch.StartNew();
        try
        {
            engine.Execute(route.FunctionHandler ?? throw new InvalidOperationException("FunctionHandler is null"));
            var handler = engine.GetValue("handler");
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
            stopwatch.Stop();
        }

        result.Duration = stopwatch.Elapsed;
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