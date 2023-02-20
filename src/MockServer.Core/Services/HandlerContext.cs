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
        return db != null ? new db(username, name) : throw new Exception("No database found");
    }
}

public class db
{
    private readonly string _path;
    public object data { get; private set; }
    public db(string username, string name)
    {
        //var fileName = name + ".json";
        //_path = Path.Combine("db", username, fileName);
        _path = name + ".json";
        if (!File.Exists(_path))
        {
            File.Create(_path);
        }
    }

    public object read()
    {
        // Read the JSON data from file
        string jsonString = File.ReadAllText(_path);
        if (!string.IsNullOrEmpty(jsonString))
        {
            // Deserialize the JSON data into a dynamic object
            data = JsonSerializer.Deserialize<ExpandoObject>(jsonString);
        }
        else
        {
            data = new();
        }

        return data;
    }
    public void write(object obj)
    {
        data = obj;
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        // Convert the dynamic object to a JSON string
        string jsonString = JsonSerializer.Serialize(data, options);

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