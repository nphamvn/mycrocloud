namespace MockServer.MockResponder.Services;

public interface IHttpResponseRetriever
{
    Task<HttpResponseMessage> GetResponseMessage(int routeId, HttpContext context);
}