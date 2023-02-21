using System.Dynamic;
using System.Text.Json;
using Jint;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Core.Databases;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Core.Services;

public class HandlerContext
{
    private Engine _engine;
    public Engine JintEngine => _engine;
    private readonly HttpContext _context;
    private readonly IDatabaseRepository _databaseRespository;
    private readonly string _username;
    private readonly GlobalSettings _settings;
    private readonly IFactoryService _factoryService;
    public HandlerContext(HttpContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _databaseRespository = _context.RequestServices.GetService<IDatabaseRepository>();
        _username = Convert.ToString(_context.Items["Username"]);
        _settings = _context.RequestServices.GetService<GlobalSettings>();
        _factoryService = _context.RequestServices.GetService<IFactoryService>();
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
        _engine.SetValue("connectDb", new Func<string, Db>(ConnectDatabase));
    }
    private Db ConnectDatabase(string name)
    {
        string username = Convert.ToString(_context.Items["Username"]);
        var task = _databaseRespository.Find(username, name);
        task.Wait();
        var db = task.Result;
        if (db != null)
        {
            _engine.Execute(
                """
                function read(db) {
                    let json = db.readJson();
                    log(json);
                    return JSON.parse(json); 
                }
                """);
            Db instance;
            if (_settings.DatabaseProvider == nameof(JsonFileBasedDb))
            {
                instance = _factoryService.Create<JsonFileBasedDb>(username, name);
            }
            else if (_settings.DatabaseProvider == nameof(NoSqlDb))
            {
                instance = _factoryService.Create<NoSqlDb>(db.Id, _databaseRespository);
            }
            else 
            {
                throw new DbException("Database provider not found");
            }
            return instance;
        }
        else 
        {
            throw new DbException("Database not found");
        }
    }
}