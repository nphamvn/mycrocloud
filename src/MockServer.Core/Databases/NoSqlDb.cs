using System.Dynamic;
using System.Text.Json;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;

public class NoSqlDb : Db
{
    private readonly int _dbId;
    private readonly IDatabaseRepository _databaseRespository;
    public NoSqlDb(int dbId, IDatabaseRepository databaseRespository)
    {
        _dbId = dbId;
        _databaseRespository = databaseRespository;
    }

    public override string readJson()
    {
        var task = _databaseRespository.Get(_dbId);
        task.Wait();
        var db = task.Result;
        return db.Data;
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
        var task = _databaseRespository.UpdateData(_dbId, jsonString);
        task.Wait();
    }
}