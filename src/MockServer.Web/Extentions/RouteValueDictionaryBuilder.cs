namespace MockServer.Web.Extentions;
public class RouteValueDictionaryBuilder {
    private readonly RouteValueDictionary _routeValues;

    public RouteValueDictionaryBuilder()
    {
        _routeValues = new RouteValueDictionary();
    }
    public RouteValueDictionaryBuilder(RouteValueDictionary values)
    {
        _routeValues = new RouteValueDictionary(values);
    }
    public RouteValueDictionaryBuilder Add(string key, object value)
    {
        _routeValues.Add(key, value);
        return this;
    }
    public RouteValueDictionaryBuilder AddRange(IDictionary<string, object> values)
    {
        foreach (var entry in values)
        {
            _routeValues.Add(entry.Key, entry.Value);
        }
        return this;
    }
    public RouteValueDictionary Build()
    {
        return _routeValues;
    }
}