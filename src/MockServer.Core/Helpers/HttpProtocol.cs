namespace MockServer.Core.Helpers;

public static class HttpProtocol
{
    public static List<string> CommonHttpMethods
        => new List<string>
            {
            "GET",
            "HEAD",
            "POST",
            "PUT",
            "DELETE",
            "CONNECT",
            "OPTIONS",
            "TRACE",
            "PATCH",
            };
}
