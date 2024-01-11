using System.Diagnostics;
using System.Globalization;
using Jint;
using Jint.Runtime.Interop;
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
        var stopwatch = Stopwatch.StartNew();

        var engine = new Engine();
        engine.SetValue(nameof(DbConnection), TypeReference.CreateTypeReference<DbConnection>(engine));
        engine.Execute(await File.ReadAllTextAsync("Scripts/MycroCloudDb.js"));
        foreach (var dependency in route.FunctionHandlerDependencies ?? [])
        {
            if (scripts.TryGetValue(dependency, out var script))
            {
                engine.Execute(script);
            }
        }
        engine.Execute(route.FunctionHandler ?? throw new InvalidOperationException("FunctionHandler is null"));
        var handler = engine.GetValue("handler");

        //Execute function and get response
        var result = new FunctionExecutionResult();
        var jsResult = engine.Invoke(handler, request);
        stopwatch.Stop();
        result.Duration = stopwatch.Elapsed;
        var statusCode = jsResult.Get("statusCode");
        if (!statusCode.IsNull() && !statusCode.IsUndefined())
        {
            result.StatusCode = (int)statusCode.AsNumber();
        }

        var headers = jsResult.Get("headers");
        if (!headers.IsNull() && !headers.IsUndefined())
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
        if (!body.IsNull() && !body.IsUndefined())
        {
            result.Body = body.AsString();
        }

        var additionalLogMessage = jsResult.Get("additionalLogMessage");
        if (!additionalLogMessage.IsNull() && !additionalLogMessage.IsUndefined())
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