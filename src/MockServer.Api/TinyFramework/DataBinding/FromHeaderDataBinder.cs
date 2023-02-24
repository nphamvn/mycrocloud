namespace MockServer.Api.TinyFramework.DataBinding;

public class FromHeaderDataBinder : IDataBinder
{
    public string Name { get; set; }
    public FromHeaderDataBinder()
    {

    }
    public FromHeaderDataBinder(string name)
    {
        Name = name;
    }
    public object Get(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(Name);
        context.Request.Headers.TryGetValue(Name, out var value);
        return value;
    }
}
