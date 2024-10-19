using Jint;

namespace WebApp.MiniApiGateway;

public static class EngineExtensions
{
    public static async Task SetRequestValue(this Engine engine, HttpRequest request)
    {
        var requestMethod = request.Method;
        var requestPath = request.Path.Value;
        var requestParams = request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString());
        var requestQuery = request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var requestHeaders = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();

        engine
            .SetValue("requestMethod", requestMethod)
            .SetValue("requestPath", requestPath)
            .SetValue("requestParams", requestParams)
            .SetValue("requestQuery", requestQuery)
            .SetValue("requestHeaders", requestHeaders)
            .SetValue("bodyParser", "json")
            .SetValue("requestBody", requestBody)
            ;

        const string code = """
                            const request = {
                              method: requestMethod,
                              path: requestPath,
                              headers: requestHeaders,
                              query: requestQuery,
                              params: requestParams,
                            };

                            switch (bodyParser) {
                              case "json":
                                request.body = requestBody ? JSON.parse(requestBody) : null;
                                break;
                            }
                            """;

        engine.Execute(code);
    }
}