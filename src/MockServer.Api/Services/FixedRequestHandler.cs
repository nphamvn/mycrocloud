using System.Net;
using System.Text;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;

namespace MockServer.Api.Services;

public class FixedRequestHandler : IRequestHandler
{
    private readonly IRequestRepository _requestRepository;
    private int _userId;
    public FixedRequestHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }
    
    public async Task<ResponseMessage> GetResponseMessage(Request incomingRequest)
    {
        var request = await _requestRepository.GetById(incomingRequest.Id);
        _userId = request.Project.User.Id;
        var handlerContext = new HandlerContext(incomingRequest.HttpContext);
        handlerContext.Setup();
        var script =
                    """
                    const db = connectDb('blogs');
                    db.read();
                    db.data.
                    const newPost = {
                        name: 'new post'
                    };
                    const post = db.write('posts', newPost);
                    const posts = db.read('posts');
                    """;
        if (!string.IsNullOrEmpty(script))
        {
            handlerContext.JintEngine.Execute(script);
        }
        var headers = await _requestRepository.GetResponseHeaders(incomingRequest.Id);
        foreach (var header in headers)
        {
            incomingRequest.HttpContext.Response.Headers.Add(header.Name, header.Value);
        }
        var res = await _requestRepository.GetResponse(incomingRequest.Id);
        string body = "";
        if (res.BodyTextRenderEngine == 1)
        {
            body = res.BodyText;
        }
        else if (res.BodyTextRenderEngine == 2)
        {
            //Handlebars
            IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer(handlerContext.JintEngine);
            body = renderService.Render(res.BodyText);
        }
        else if (res.BodyTextRenderEngine == 3)
        {
            IExpressionTemplateWithScriptRenderer renderService = new ExpressionTemplateWithScriptRenderer(handlerContext.JintEngine);
            body = renderService.Render(res.BodyText);
        }

        return new ResponseMessage
        {
            StatusCode = (HttpStatusCode)res.StatusCode,
            Content = new StringContent(body, Encoding.UTF8)
        };
    }
}