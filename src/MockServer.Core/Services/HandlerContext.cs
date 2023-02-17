using Jint;
using Microsoft.AspNetCore.Http;
using MockServer.Core.Extentions;

namespace MockServer.Core.Services;

public class HandlerContext
{
    private Engine _engine;
    public Engine JintEngine { get; }
    private readonly HttpContext _context;
    public HandlerContext(HttpContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
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
}

public class db {
    private readonly string _jsonFilePath;
    public db(string connectionString)
    {
        var dbOwner = connectionString.Split(':')[0];
        var fileName = connectionString.Split(':')[1] + ".json";
        _jsonFilePath = Path.Combine("db", dbOwner, fileName);
    }

    public async Task<object> read(string table) {
        
        return new object();
    }
    public async Task<object> write(string table, object obj) {

        return obj;
    }
}

public class Http {

}