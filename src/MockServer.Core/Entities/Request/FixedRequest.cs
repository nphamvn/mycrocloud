namespace MockServer.Core.Entities.Requests;
public class FixedRequest : BaseEntity
{
    public int RequestId { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public IList<RequestParam> RequestParams { get; set; }
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public Response Response { get; set; }
}

public class RequestHeader : BaseEntity
{
    public string Name { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
    public string Description { get; set; }
}
public class RequestParam : BaseEntity
{
    public string Key { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
    public string Description { get; set; }
}
public class RequestBody : BaseEntity
{
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
    public string Format { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
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
    public int StatusCode { get; set; }
    public bool Delay { get; set; }
    public int DelayTime { get; set; }
}
