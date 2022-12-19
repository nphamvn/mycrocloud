namespace MockServer.Core.Entities.Requests;
public class ForwardingRequest : BaseEntity
{
    public int RequestId { get; set; }
    public string Scheme { get; set; }
    public string Host { get; set; }
}

