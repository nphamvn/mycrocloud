using System.Text;

namespace MockServer.ReverseProxyServer.Models;

public class RequestModel
{
    public string Username { get; set; }
    public string ProjectName { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }

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