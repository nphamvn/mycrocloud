namespace MockServer.Core.Entities.Requests;
public class FixedRequest : BaseEntity
{
    public int RequestId { get; set; }
    public int ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public string ResponseContentType { get; set; }
    public int Delay { get; set; }
}

public class RequestConfiguration
{
    public ICollection<RequestHeader> Headers { get; set; }
    public ICollection<RequestParam> Params { get; set; }
}

public class ResponseConfiguration
{
    public int StatusCode { get; set; }
    public int Delay { get; set; }
    public ICollection<ResponseHeader> Headers { get; set; }
    public ICollection<ResponseParam> Params { get; set; }
}
public class ResponseHeader : BaseEntity
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
}

public class ResponseParam : BaseEntity
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
}

public class RequestHeader : BaseEntity
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
}
public class RequestParam : BaseEntity
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
}

