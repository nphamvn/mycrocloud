namespace MockServer.Core.Models.Requests;

public class Response
{
    public string BodyFormat { get; set; }
    public string BodyText { get; set; }
    public int BodyTextRenderEngine { get; set; }
    public string BodyRenderScript { get; set; }
    public int StatusCode { get; set; }
    public bool Delay { get; set; }
    public int DelayTime { get; set; }
}
