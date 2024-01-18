using System.Diagnostics;
using System.Globalization;
using Jint;
using Jint.Native;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public static class FunctionHandler
{
    public static async Task Handle(HttpContext context)
    {
        var route = (Route)context.Items["_Route"]!;
        var request = context.Items["_Request"]!;
        var scripts = context.RequestServices.GetRequiredService<ScriptCollection>();
        //Start measuring time for function execution
        
        var engine = new Engine(options =>
        {
            options.LimitMemory(50 * 1024 * 1024);
            options.TimeoutInterval(TimeSpan.FromSeconds(30));
        });
        foreach (var dependency in route.FunctionHandlerDependencies ?? [])
        {
            if (scripts.TryGetValue(dependency, out var script))
            {
                engine.Execute(script);
            }
        }
        var stopwatch = Stopwatch.StartNew();
        JsValue jsResult;
        var result = new FunctionExecutionResult();
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
}