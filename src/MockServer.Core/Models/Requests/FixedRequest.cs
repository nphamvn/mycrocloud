namespace MockServer.Core.Models.Requests;
public class FixedRequest
{
    public IList<RequestHeader> RequestHeaders { get; set; }
    public IList<RequestQuery> RequestParams { get; set; }
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public Response Response { get; set; }
}

public class RequestHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Constraints { get; set; }
}
public class RequestQuery
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Constraints { get; set; }
    public int ConstraintsCount
    => !string.IsNullOrEmpty(Constraints) ? Constraints.Split(":").Length : 0;
}
public class RequestBody : BaseEntity
{
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
    public string Format { get; set; }
    public string Text { get; set; }
    public string Constraints { get; set; }
}
public class ResponseHeader : BaseEntity
{
    public string Name { get; set; }
    public string Value { get; set; }
}
public class Response : BaseEntity
{
    public string BodyFormat { get; set; }
    public string BodyText { get; set; }
    public int BodyTextRenderEngine { get; set; }
    public string BodyRenderScript { get; set; }
    public int StatusCode { get; set; }
    public bool Delay { get; set; }
    public int DelayTime { get; set; }
}
