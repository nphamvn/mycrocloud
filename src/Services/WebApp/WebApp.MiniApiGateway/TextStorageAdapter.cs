using System.Text;

namespace WebApp.MiniApiGateway;

public class TextStorageAdapter(string connectionString, HttpClient httpClient)
{
    public string Read()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5148");
        request.Headers.Add("X-Connection-String", connectionString);
        var response = httpClient.Send(request);
        using var reader = new StreamReader(response.Content.ReadAsStream());
        return reader.ReadToEnd();
    }
    
    public void Write(string text)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5148");
        request.Headers.Add("X-Connection-String", connectionString);
        request.Content = new StringContent(text, Encoding.UTF8, "text/plain");
        var response = httpClient.Send(request);
        response.EnsureSuccessStatusCode();
    }
}
