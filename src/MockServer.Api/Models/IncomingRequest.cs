using System.Text;
using MockServer.Core.Enums;

namespace MockServer.Api.Models;

public class IncomingRequest
{
    public string Username { get; set; }
    public string ProjectName { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public RequestType RequestType { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(nameof(Username) + ": " + Username);
        sb.AppendLine();
        sb.Append(nameof(Method) + ": " + Method);
        sb.AppendLine();
        sb.Append(nameof(Path) + ": " + Path);
        return sb.ToString();
    }
}