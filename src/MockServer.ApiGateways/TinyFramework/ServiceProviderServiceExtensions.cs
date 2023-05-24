namespace MockServer.Api.TinyFramework;

public static class ServiceProviderServiceExtensions
{
    public static T GetRequiredService<T>(this Request request){
        return request.HttpContext.RequestServices.GetRequiredService<T>();
    }
}
