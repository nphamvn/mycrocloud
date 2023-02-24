using System.Dynamic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace MockServer.Core.Extentions;

public static class HttpContextExtentions
{
    public static async Task WriteResponse(this HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        var json = await response.Content.ReadAsStringAsync();
        var data = Encoding.UTF8.GetBytes(json);
        context.Response.Headers["Content-Length"] = data.Length.ToString();
        await context.Response.Body.WriteAsync(data, 0, data.Length);
    }

    public static async Task<Dictionary<string, object>> GetRequestDictionary(HttpContext context)
    {
        var request = context.Request;
        context.Request.EnableBuffering();
        var bodyText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        var body = !string.IsNullOrEmpty(bodyText) ? new Dictionary<string, object>(JsonSerializer.Deserialize<ExpandoObject>(bodyText)) : null;
        var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
        var query = request.Query.ToDictionary(q => q.Key, q => q.Value.FirstOrDefault());
        var routeValues = request.RouteValues.ToDictionary(rv => rv.Key, rv => rv.Value);
        return new Dictionary<string, object>()
            {
                { "method", request.Method},
                { "path", request.Path.Value},
                { "host", request.Host.Host},
                { "headers", headers},
                { "routeValues", routeValues},
                { "query", query},
                { "body",  body}
            };
    }

    public static async Task<object> InvokeAction(this HttpContext context, string code) {
        var scriptOptions = ScriptOptions.Default
            .WithImports("System", "Microsoft.AspNetCore.Http", "MockServer.Core.Extentions.HttpContextExtentions.Globals");
        var script = CSharpScript.Create(code, scriptOptions);
        var result = await script.RunAsync(new Globals(context));
        return result.ReturnValue;
    }

    public static async Task<object> Execute(this HttpContext context, string code)
    {
        var options = ScriptOptions.Default
        .WithImports("System")
        .WithImports("System.Threading.Tasks")
        .WithImports("Microsoft.AspNetCore.Http")
        .WithImports("Microsoft.AspNetCore.Http.Features")
        .AddReferences(typeof(HttpContext).Assembly)
        .AddReferences(typeof(Globals).Assembly)
        .AddReferences(typeof(object).Assembly);

        var state = await CSharpScript.RunAsync(code, options, new Globals(context));
        var result = state.ReturnValue;

        return result;
    }

    public class Globals
    {
        private readonly HttpContext _httpContext;
        public dynamic Request => _httpContext.GetType().GetProperty("Request")?.GetValue(_httpContext, null);
        public Globals(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }
    }
}