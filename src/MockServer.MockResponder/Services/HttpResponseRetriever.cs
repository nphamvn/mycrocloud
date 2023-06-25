using System.Net;
using System.Text.Json;
using MockServer.Core.Repositories;
using MockServer.Core.WebApplications;
using MockServer.MockResponder.Extensions;

namespace MockServer.MockResponder.Services {
    public class HttpResponseRetriever : IHttpResponseRetriever
    {
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;

        public HttpResponseRetriever(IWebApplicationRouteRepository webApplicationRouteRepository)
        {
            _webApplicationRouteRepository = webApplicationRouteRepository;
        }
        public async Task<HttpResponseMessage> GetResponseMessage(int routeId, HttpContext context)
        {
            var response = await _webApplicationRouteRepository.GetMockResponse(routeId);
            var message = new HttpResponseMessage();
            
            //Build StatusCode
            message.StatusCode = (HttpStatusCode)BuildStatusCode(response.StatusCode, context);
            
            //Build Header
            var headers = await BuildHeaders(response.Headers, context);
            foreach (var header in headers)
            {
                message.Headers.Add(header.Key, header.Value);
            } 
            
            //Build Body
            message.Content = new StringContent(BuildBodyContent(response.Body, context));
            
            return message;
        }

        private async Task<Dictionary<string, string>> BuildHeaders(ICollection<HeaderItem> headers, HttpContext context)
        {
            return headers.ToDictionary(header => header.Name, header => RenderTemplate(header.Value.RenderedValue, context));
        }

        private int BuildStatusCode(Value status, HttpContext context)
        {
            var stringResult = RenderTemplate(status.RenderedValue, context);

            if (int.TryParse(stringResult, out var code))
            {
                return code;
            }

            throw new Exception("Something went wrong");
        }

        private string BuildBodyContent(Value body, HttpContext context)
        {
            return RenderTemplate(body.RenderedValue, context);
        }

        private static string RenderTemplate(TemplateRenderedValue value, HttpContext context)
        {
            ITemplateEngine templateEngine = value.Engine switch
            {
                TemplateEngine.None => new NoneTemplateEngine(value.Template),
                TemplateEngine.JavaScript => new JavaScriptTemplateEngine(value.Template, context),
                TemplateEngine.Handlebars => new HandlebarsTemplateEngine(value.Template, context),
                TemplateEngine.Mustache => new MustacheTemplateEngine(value.Template, context),
                TemplateEngine.Ejs => new EjsTemplateEngine(value.Template, context),
                _ => throw new ArgumentOutOfRangeException()
            };

            return templateEngine.Render();
        }
    }

    public interface ITemplateEngine
    {
        public string Render();
    }

    public interface IJavaScriptEngine
    {
        
    }
    public class NoneTemplateEngine : ITemplateEngine
    {
        private readonly string _value;
        public NoneTemplateEngine(string value)
        {
            _value = value;
        }
        public string Render()
        {
            return _value;
        }
    }

    public class JavaScriptTemplateEngine : ITemplateEngine
    {
        private readonly string _expression;
        private readonly HttpContext _httpContext;

        public JavaScriptTemplateEngine(string expression, HttpContext httpContext)
        {
            _expression = expression;
            _httpContext = httpContext;
        }
        public string Render()
        {
            throw new NotImplementedException();
        }
    }

    public class HandlebarsTemplateEngine : ITemplateEngine
    {
        private readonly string _template;
        private readonly HttpContext _httpContext;

        public HandlebarsTemplateEngine(string template, HttpContext httpContext)
        {
            _template = template;
            _httpContext = httpContext;
        }
        public string Render()
        {
            throw new NotImplementedException();
        }
    }

    public class MustacheTemplateEngine : ITemplateEngine
    {
        private readonly string _template;
        private readonly HttpContext _httpContext;

        public MustacheTemplateEngine(string template, HttpContext httpContext)
        {
            _template = template;
            _httpContext = httpContext;
        }
        public string Render()
        {
            throw new NotImplementedException();
        }
    }

    public class EjsTemplateEngine : ITemplateEngine
    {
        private readonly string _template;
        private readonly HttpContext _httpContext;

        public EjsTemplateEngine(string template, HttpContext httpContext)
        {
            _template = template;
            _httpContext = httpContext;
        }
        public string Render()
        {
            throw new NotImplementedException();
        }
    }
}