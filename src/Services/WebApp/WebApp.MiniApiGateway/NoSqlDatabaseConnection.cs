using System.Text;
using Jint;
using Jint.Native;

namespace WebApp.MiniApiGateway;

public class NoSqlDatabaseConnection(string connectionString, HttpClient httpClient, Engine engine)
{
    public JsString Read(string name)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5148/_docs?name=" + name);
        request.Headers.Add("X-Connection-String", connectionString);
        var response = httpClient.Send(request);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        var content = reader.ReadToEnd();
        return new JsString(content);
    }
    
    public void Write(string name, string data)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5148/_docs?name=" + name);
        request.Headers.Add("X-Connection-String", connectionString);
        request.Content = new StringContent(data, Encoding.UTF8, "application/json");
        var response = httpClient.Send(request);
        response.EnsureSuccessStatusCode();
    }
}
