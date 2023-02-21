namespace MockServer.Core.Models.Requests;

public class RequestBody
{
    public bool Required { get; set; }
    public bool MatchExactly { get; set; }
    public string Format { get; set; }
    public string Text { get; set; }
    public string Constraints { get; set; }
}