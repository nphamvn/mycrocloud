public class RequestHandler
{
    /// <summary>
    /// Handle the context(request) which is sent from reverse proxy 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Hanlde(HttpContext context)
    {
        IExpectionCallback callback = context.RequestServices.GetRequiredService<IExpectionCallback>();
        var response = await callback.Handle(context.Request);

    }
}

