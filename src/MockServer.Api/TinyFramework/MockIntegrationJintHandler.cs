using System.Net;
using System.Text;
using Jint;
using Microsoft.Extensions.Options;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;

namespace MockServer.Api.TinyFramework;

public class MockIntegrationJintHandler : RequestHandler
{
    private readonly Engine _engine;
    private readonly CoreRoute _route;
    private readonly CoreWebApplication _webApplication;
    private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
    private readonly IDatabaseRepository _databaseRepository;
    private readonly DatabaseAdapterOptions _databaseAdapterOptions;

    public MockIntegrationJintHandler(
        CoreWebApplication webApplication,
        CoreRoute route,
        IWebApplicationRouteRepository webApplicationRouteRepository,
        IDatabaseRepository databaseRepository,
        Engine engine,
        IOptions<DatabaseAdapterOptions> databaseAdapterOptions)
    {
        _webApplication = webApplication;
        _route = route;
        _webApplicationRouteRepository = webApplicationRouteRepository;
        _databaseAdapterOptions = databaseAdapterOptions.Value;
        _engine = engine;
        _databaseRepository = databaseRepository;
    }
    
    public override async Task Handle(HttpContext context)
    {
        var integration = await _webApplicationRouteRepository.GetMockIntegration(_route.Id);
        Initialize(context);
        if (!string.IsNullOrEmpty(integration.Code))
        {
            _engine.Execute(integration.Code);
        }
        var headers = integration.ResponseHeaders;
        foreach (var header in headers)
        {
            context.Response.Headers.Add(header.Name, header.Value);
        }
        string body = "";
        if (integration.ResponseBodyTextRenderEngine == 1)
        {
            body = integration.ResponseBodyText;
        }
        else if (integration.ResponseBodyTextRenderEngine == 2)
        {
            var handlebarsCode = File.ReadAllText("handlebars.min-v4.7.7.js");
            var renderService = new JintHandlebarsTemplateRenderer(_engine, handlebarsCode) ;
            body = renderService.Render(integration.ResponseBodyText);
        }
        else if (integration.ResponseBodyTextRenderEngine == 3)
        {
            var renderService = new ExpressionTemplateWithScriptRenderer(_engine) ;
            body = renderService.Render(integration.ResponseBodyText);
        }

        await context.WriteResponse(new ResponseMessage
        {
            StatusCode = (HttpStatusCode)integration.ResponseStatusCode,
            Content = new StringContent(body, Encoding.UTF8)
        });
    }

    private void Initialize(HttpContext context) {
        var task = HttpContextExtentions.GetRequestDictionary(context);
        task.Wait();
        var request = task.Result;
        var ctx = new
        {
            request = request
        };
        _engine.SetValue("ctx", ctx);

        _engine.SetValue("log", new Action<object>(Console.WriteLine));

        var dbClassCode = File.ReadAllText("Contents/js/Db.js");
        _engine.Execute(dbClassCode);

        _engine.SetValue("createAdapter", new Func<string, IDatabaseAdapter>(CreateAdapter));
        
    }
    private IDatabaseAdapter CreateAdapter(string database)
    {
        var findTask = _databaseRepository.Find(_webApplication.UserId, database);
        findTask.Wait();
        var db = findTask.Result;
        if (db != null)
        {
            var getServiceTask = _databaseRepository.GetDatabaseUsingService(db.Id);
            getServiceTask.Wait();
            var services = getServiceTask.Result;
            if (services.Any(s => s.Type == ServiceType.WebApp && s.Id == _webApplication.Id))
            {
                if (db.Adapter == nameof(JsonFileAdapter))
                {
                    var jsonFileAdapter = new JsonFileAdapter(db.JsonFilePath, _databaseAdapterOptions);
                    return jsonFileAdapter;
                }
                else if (db.Adapter == nameof(NoSqlAdapter))
                {
                    var noSqlAdapter = new NoSqlAdapter(db.Id, _databaseRepository, _databaseAdapterOptions);
                    return noSqlAdapter;
                }
                else
                {
                    throw new DbException("Database provider not found");
                }
            }
            else
            {
                throw new DbException($"Application {_webApplication.Name} is not accessible to database {db.Name}");
            }
        }
        else
        {
            throw new DbException("Database not found");
        }
    }
}