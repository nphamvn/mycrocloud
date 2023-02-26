using System.Text.Json;
using Jint;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Core.Databases;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications;

namespace MockServer.Core.Services;

public class HandlerContext
{
    private Engine _engine;
    public Engine JintEngine => _engine;
    private readonly HttpContext _context;
    public WebApplication WebApplication { get; set; }
    private readonly IDatabaseRepository _databaseRespository;
    private readonly GlobalSettings _settings;
    private readonly IFactoryService _factoryService;
    public HandlerContext(HttpContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _databaseRespository = _context.RequestServices.GetService<IDatabaseRepository>();
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
        _engine.SetValue("createAdapter", new Func<string, IDatabaseAdapter>(CreateAdapter));
    }
    private IDatabaseAdapter CreateAdapter(string databaseName)
    {
        var code = File.ReadAllText("Contents/js/Db.js");
        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        return DatabaseAdapterUtilities.CreateAdapter(
            service: new Service {
                Type = ServiceType.WebApp,
                Id = WebApplication.Id
            },
            factoryService: _factoryService,
            databaseRepository: _databaseRespository,
            jsonSerializerOptions: jsonSerializerOptions,
            UserId: WebApplication.UserId,
            databaseName: databaseName,
            engine: _engine,
            codes: new string[] { code }
        );
    }
}