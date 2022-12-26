namespace MockServer.Core.Entities.Requests;
public class FixedRequest : BaseEntity
{
    public int RequestId { get; set; }
    public int ResponseStatusCode { get; set; }
    public string ResponseBodyText { get; set; }
    public string ResponseContentType { get; set; }
    public int Delay { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public IList<RequestParam> RequestParams { get; set; }
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public ResponseBody ResponseBody { get; set; }
}

public class ResponseConfiguration
{
    public int StatusCode { get; set; }
    public int Delay { get; set; }
    public ICollection<ResponseHeader> Headers { get; set; }
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
public class ResponseBody : BaseEntity
{
    public string Format { get; set; }
    public string Text { get; set; }
}
