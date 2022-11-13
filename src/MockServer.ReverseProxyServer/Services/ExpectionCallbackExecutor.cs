using System;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services
{
    public class ExpectionCallbackExecutor : IRequestHandler
    {
        public ExpectionCallbackExecutor()
        {
        }

        public async Task<AppResponse> Handle(AppRequest request)
        {
            //1: Prepare source file by replacing user's class file to template source (username, requestId)

            //2: Send message to Go (or Python) service to build image, start container

            //3: Send request to started container

            return new AppResponse(request.HttpContext, new HttpResponseMessage());
        }
    }
}

