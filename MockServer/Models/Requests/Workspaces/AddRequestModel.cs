namespace MockServer.Models.Request.Workspaces;

public class AddRequestModel
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public RequestResponseModel Response { get; set; }
}

public class RequestResponseModel
{
    public int StatusCode { get; set; }
    public string Body { get; set; }
}