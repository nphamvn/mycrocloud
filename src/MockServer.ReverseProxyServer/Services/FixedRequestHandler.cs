using System.Net;
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
        var response = await _requestRepository.GetResponse(request.Id);
        string content = "";
        if (response.BodyTextRenderEngine == 1)
        {
            //static
            content = response.BodyText;
        }
        else if (response.BodyTextRenderEngine == 2)
        {
            //Handlebars
            IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer();
            var ctx = new
            {
                request = HttpContextExtentions.GetRequestDictionary(request.HttpContext)
            };
            content = renderService.Render(ctx, response.BodyText);
        }
        return new ResponseMessage
        {
            StatusCode = (HttpStatusCode)response.StatusCode,
            Content = new StringContent(content)
        };
    }
}