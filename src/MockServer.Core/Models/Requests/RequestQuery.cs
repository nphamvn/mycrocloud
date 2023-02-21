namespace MockServer.Core.Models.Requests;

public class RequestQuery
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Constraints { get; set; }
    public int ConstraintsCount
    => !string.IsNullOrEmpty(Constraints) ? Constraints.Split(":").Length : 0;
}
