namespace MockServer.Core.Models.Requests;
public class FixedRequest
{
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public Response Response { get; set; }
}
