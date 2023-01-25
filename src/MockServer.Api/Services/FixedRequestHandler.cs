using System.Net;
using System.Text;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class FixedRequestHandler : IRequestHandler
{
    private readonly IRequestRepository _requestRepository;

    public FixedRequestHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<ResponseMessage> GetResponseMessage(AppRequest request)
    {
        var headers = await _requestRepository.GetResponseHeaders(request.Id);
        foreach (var header in headers)
        {
            request.HttpContext.Response.Headers.Add(header.Name, header.Value);
        }
        var res = await _requestRepository.GetResponse(request.Id);
        string body = "";
        if (res.BodyTextRenderEngine == 1)
        {
            body = res.BodyText;
        }
        else if (res.BodyTextRenderEngine == 2)
        {
            //Handlebars
            IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer();
            var req = await HttpContextExtentions.GetRequestDictionary(request.HttpContext);
            var ctx = new
            {
                request = req
            };
            body = renderService.Render(ctx, res.BodyText, res.BodyRenderScript);
        }
        else if (res.BodyTextRenderEngine == 3)
        {
            var ctx = new
            {
                request = await HttpContextExtentions.GetRequestDictionary(request.HttpContext)
            };
            IExpressionTemplateWithScriptRenderer renderService = new ExpressionTemplateWithScriptRenderer();
            body = renderService.Render(ctx, res.BodyText, res.BodyRenderScript);
        }
        return new ResponseMessage
        {
            StatusCode = (HttpStatusCode)res.StatusCode,
            Content = new StringContent(body, Encoding.UTF8)
        };
    }
}