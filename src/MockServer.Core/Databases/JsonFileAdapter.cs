using System.Dynamic;
using System.Text.Json;
using MockServer.Core.Databases;

public class JsonFileAdapter : DatabaseAdapter
{
    private readonly string _filePath;

    public JsonFileAdapter(string filePath, JsonSerializerOptions options) : base(options)
    {
        _filePath = filePath;
    }
    public override string ReadJson()
    {
        return File.ReadAllText(_filePath);
    }

    public override void Write(object obj)
    {
        var json = GetJson(obj);
        File.WriteAllText(_filePath, json);
    }
}