using System.Net;
using System.Text.Json;
using MockServer.Core.Repositories;

namespace MockServer.MockResponder.Services {
    public class HttpResponseRetriever : IHttpResponseRetriever
    {
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;

        public HttpResponseRetriever(IWebApplicationRouteRepository webApplicationRouteRepository)
        {
            _webApplicationRouteRepository = webApplicationRouteRepository;
        }
        public async Task<HttpResponseMessage> GetResponseMessage(int routeId)
        {
            var response = await _webApplicationRouteRepository.GetMockResponse(routeId);
            var message = new HttpResponseMessage();
            message.StatusCode = (HttpStatusCode)response.StatusCode;
            message.Content = new StringContent(JsonSerializer.Serialize(response));
            return message;
        }
    }
}