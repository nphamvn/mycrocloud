using System;
using Microsoft.AspNetCore.Http;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;
using MockServer.Api.Models.Docker;

namespace MockServer.Api.Services
{
    public class ExpectionCallbackExecutor : IRequestHandler
    {
        private readonly IDockerServices _dockerServices;

        public ExpectionCallbackExecutor(IDockerServices dockerServices)
        {
            _dockerServices = dockerServices;
        }

        public async Task<ResponseMessage> GetResponseMessage(Request request)
        {
            //1: Prepare source file by replacing user's class file to template source (username, requestId)

            //2: Send message to Go (or Python) GRPC service to build image, start container
            var runContainerResult = await _dockerServices.StartContainer(new RunContainerOptions
            {

            });


            //3: Send request to started container
            using var client = new HttpClient();
            var httpRequest = request.HttpContext.Request;
            var message = new HttpRequestMessage();
            var path = httpRequest.Path.Value.StartsWith("/") ? httpRequest.Path.Value.Remove(0, 1) : httpRequest.Path.Value;
            message.Method = new HttpMethod(request.HttpContext.Request.Method);
            string host = "ip";
            int port = 1000;
            message.RequestUri = new Uri(string.Format("http://{0}:{1}/{2}", host, port, path));
            var response = await client.SendAsync(message);

            return new ResponseMessage();
        }

        private HttpRequestMessage Create(ForwardingRequest request)
        {
            var httpRequest = request.HttpContext.Request;
            var path = httpRequest.Path.Value.StartsWith("/") ? httpRequest.Path.Value.Remove(0, 1) : httpRequest.Path.Value;
            var message = new HttpRequestMessage();
            //message.Content = request.co;
            //request.Headers.ToList().ForEach(header => message.Headers.Add(header.Key, header.Value));
            message.Method = new HttpMethod(request.HttpContext.Request.Method);//(HttpMethod)Enum.Parse(typeof(HttpMethod), request.Method);
            message.RequestUri = new Uri(string.Format("{0}://{1}/{2}", request.Scheme, request.Host, path));
            return message;
        }
    }
}

