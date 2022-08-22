namespace MockServer.Models;

public class Response
{
    public int Id { get; set; }
    public int StatusCode { get; set; }
    public string Body { get; set; }
    //public Dictionary<string, string>? Headers { get; set; }
    //public Dictionary<string, string>? Cookies { get; set; }
    public int RequestId { get; set; }
    public Request Request { get; set; }
}