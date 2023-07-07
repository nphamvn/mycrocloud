namespace  MockServer.Domain.WebApplication.Route;
public class RouteMatch
{
    public int Order { get; set; }
    public List<string> Methods { get; set; } = new ();
    public string Path { get; set; }
}