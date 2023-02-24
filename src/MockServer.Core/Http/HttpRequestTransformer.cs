using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Forwarder;

namespace MockServer.Core.Http;

public class HttpRequestTransformer
{
    private readonly IHttpForwarder _httpForwarder;

    public HttpRequestTransformer(IHttpForwarder httpForwarder)
    {
        _httpForwarder = httpForwarder;
    }

    public async Task<HttpResponseMessage> ProxyAsync(HttpContext context) {
        var transformer = HttpTransformer.Default;
        var proxyRequest = new HttpRequestMessage(HttpMethod.Get, context.Request.Host.Host);

        //await transformer.TransformRequestAsync(context, proxyRequest, "prefix", CancellationToken.None);
        HttpClientHandler handler = new MyHttpClientHandler();
        var client = new HttpMessageInvoker(handler);
        var config = new ForwarderRequestConfig();
        var error = await _httpForwarder.SendAsync(context, "prefix", client, config, transformer);
        return null;
    }
}

