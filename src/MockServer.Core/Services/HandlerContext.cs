using Jint;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;

namespace MockServer.Core.Services;

public class HandlerContext
{
    private Engine _engine;
    public Engine JintEngine { get; }
    private readonly HttpContext _context;
    private readonly IDatabaseRespository _databaseRespository;
    private readonly string _username;
    public HandlerContext(HttpContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _databaseRespository = _context.RequestServices.GetService<IDatabaseRespository>();
        _username = Convert.ToString(_context.Items["Username"]);
    }
    public void Setup() {
        _engine = new Engine();
        var task = HttpContextExtentions.GetRequestDictionary(_context);
        task.Wait();
        var request = task.Result;
        var ctx = new
        {
            request = request
        };

        _engine.SetValue("ctx", ctx);
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

public class db {
    private readonly string _jsonFilePath;
    public dynamic data { get;}
    public db(string username, string name)
    {
        var fileName = name.Split(':')[1] + ".json";
        _jsonFilePath = Path.Combine("db", username, fileName);
    }

    public async Task<dynamic> read(string table) {
        
        return new object();
    }
    public async Task<dynamic> write(string table, object obj) {

        return obj;
    }
}

public class Http {

}