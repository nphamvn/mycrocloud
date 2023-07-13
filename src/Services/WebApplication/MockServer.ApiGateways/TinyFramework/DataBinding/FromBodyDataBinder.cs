using MockServer.Api.TinyFramework.DataBinding;

public class FromBodyDataBinder : IDataBinder
{
    public object Get(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        context.Request.EnableBuffering();
        var text = new StreamReader(context.Request.Body).ReadToEnd();
        context.Request.Body.Position = 0;
        return text;
    }
}