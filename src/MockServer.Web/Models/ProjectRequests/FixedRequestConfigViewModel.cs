using MockServer.Core.Models.Requests;

namespace MockServer.Web.Models.ProjectRequests;

public class RequestConfiguration
{
    public IList<RequestQuery> Query { get; set; }
    public IList<RequestHeader> Headers { get; set; }
    public RequestBody Body { get; set; }
}
public class HandlerConfiguration {
    public string Script { get; set; }
}

public class ResponseConfiguration {
    public string ContentType { get; set; }
    public int ContentLength { get; set; }
    public int BodyRenderType { get; set; }
    public string BodyTextFormat { get; set; }
    public string BodyText { get; set; }
    public IList<ResponseHeader> Headers { get; set; }
    public int StatusCode { get; set; }
    public bool Delay { get; set; }
    public int DelayTime { get; set; }
}