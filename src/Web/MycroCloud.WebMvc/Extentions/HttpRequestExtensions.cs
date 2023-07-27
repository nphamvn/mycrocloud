namespace MycroCloud.WebMvc.Extentions;

public static class HttpRequestExtensions
{
    private const string RequestedWithHeader = "X-Requested-With";
    private const string XmlHttpRequest = "XMLHttpRequest";

    public static bool IsAjaxRequest(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Headers != null)
        {
            return request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }

        return false;
    }
}
