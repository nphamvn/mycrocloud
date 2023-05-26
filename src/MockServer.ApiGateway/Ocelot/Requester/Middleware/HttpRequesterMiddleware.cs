namespace Ocelot.Requester.Middleware
{
    using Microsoft.AspNetCore.Http;
    using System.Net;
    using System.Net.Http;
    using Ocelot.Logging;
    using Ocelot.Middleware;
    using System.Threading.Tasks;
    using Ocelot.Responses;

    public class RequestForwardingMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpRequester _requester;

        public RequestForwardingMiddleware(RequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            IHttpRequester requester)
                : base(loggerFactory.CreateLogger<RequestForwardingMiddleware>())
        {
            _next = next;
            _requester = requester;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var downstreamRoute = httpContext.Items.DownstreamRoute();

            var response = await _requester.GetResponse(httpContext);

            CreateLogBasedOnResponse(response);

            if (response.IsError)
            {
                Logger.LogDebug("IHttpRequester returned an error, setting pipeline error");

                httpContext.Items.UpsertErrors(response.Errors);
                return;
            }

            Logger.LogDebug("setting http response message");

            httpContext.Items.UpsertDownstreamResponse(new DownstreamResponse(response.Data));

            await _next.Invoke(httpContext);
        }

        private void CreateLogBasedOnResponse(Response<HttpResponseMessage> response)
        {
            if (response.Data?.StatusCode <= HttpStatusCode.BadRequest)
            {
                Logger.LogInformation(
                    $"{(int)response.Data.StatusCode} ({response.Data.ReasonPhrase}) status code, request uri: {response.Data.RequestMessage?.RequestUri}");
            } 
            else if (response.Data?.StatusCode >= HttpStatusCode.BadRequest)
            {
                Logger.LogWarning(
                    $"{(int) response.Data.StatusCode} ({response.Data.ReasonPhrase}) status code, request uri: {response.Data.RequestMessage?.RequestUri}");
            }
        }
    }
}
