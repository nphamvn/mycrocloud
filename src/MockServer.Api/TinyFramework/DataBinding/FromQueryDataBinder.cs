using MockServer.Api.TinyFramework.DataBinding;

public class FromQueryDataBinder : IDataBinder
{
    public string Query { get; set; }
    public FromQueryDataBinder()
    {

    }
    public FromQueryDataBinder(string query)
    {
        Query = query;
    }
    public object Get(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(Query);
        context.Request.Query.TryGetValue(Query, out var value);
        return value;
    }
}