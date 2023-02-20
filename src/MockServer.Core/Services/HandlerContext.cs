using System.Dynamic;
using System.Text.Json;
using Jint;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;

namespace MockServer.Core.Services;

public class HandlerContext
{
    private Engine _engine;
    public Engine JintEngine => _engine;
    private readonly HttpContext _context;
    private readonly IDatabaseRespository _databaseRespository;
    private readonly string _username;
    public HandlerContext(HttpContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _databaseRespository = _context.RequestServices.GetService<IDatabaseRespository>();
        _username = Convert.ToString(_context.Items["Username"]);
    }
    public void Setup()
    {
        _engine = new Engine();
        var task = HttpContextExtentions.GetRequestDictionary(_context);
        task.Wait();
        var request = task.Result;
        var ctx = new
        {
            request = request
        };

        _engine.SetValue("ctx", ctx);
        _engine.SetValue("log", new Action<object>(Console.WriteLine));
        _engine.SetValue("connectDb", new Func<string, db>(connectDb));
    }
    private db connectDb(string name)
    {
        string username = Convert.ToString(_context.Items["Username"]);
        var task = _databaseRespository.Find(username, name);
        task.Wait();
        var db = task.Result;
        return db != null ? new db(username, name, _engine) : throw new Exception("No database found");
    }
}

public class db
{
    private readonly string _path;
    public db(string username, string name, Engine engine)
    {
        //var fileName = name + ".json";
        //_path = Path.Combine("db", username, fileName);
        _path = name + ".json";
        if (!File.Exists(_path))
        {
            File.Create(_path);
        }
        engine.Execute("function read(db) { return JSON.parse(db.readJson()); }");
    }

    public string readJson()
    {
        // Read the JSON data from file
        return File.ReadAllText(_path);
    }

    public object read() {
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

    public void write(object obj)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        // Convert the dynamic object to a JSON string
        string jsonString = JsonSerializer.Serialize(obj, options);

        // Write the JSON string to file
        using (StreamWriter sw = File.CreateText(_path))
        {
            sw.Write(jsonString);
        }
    }
}

public class Http
{

}