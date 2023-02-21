using System.Dynamic;
using System.Text.Json;
using MockServer.Core.Databases;

public class JsonFileBasedDb: Db
{
    private readonly string _path;
    public JsonFileBasedDb(string username, string name)
    {
        //var fileName = name + ".json";
        //_path = Path.Combine("db", username, fileName);
        _path = name + ".json";
        if (!File.Exists(_path))
        {
            File.Create(_path);
        }
    }

    public override string readJson()
    {
        // Read the JSON data from file
        return File.ReadAllText(_path);
    }

    public override object read()
    {
        var json = readJson();
        if (!string.IsNullOrEmpty(json))
        {
            // Deserialize the JSON data into a dynamic object
            return JsonSerializer.Deserialize<ExpandoObject>(json);
        }
        else
        {
            return null;
        }
    }

    public override void write(object obj)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        // Convert the dynamic object to a JSON string
        string jsonString = JsonSerializer.Serialize(obj, options);
        File.WriteAllText(_path, jsonString);
    }
}