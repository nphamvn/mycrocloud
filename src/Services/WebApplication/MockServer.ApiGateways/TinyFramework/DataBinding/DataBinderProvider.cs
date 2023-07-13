using MockServer.Core.Helpers;

namespace MockServer.Api.TinyFramework.DataBinding;

public class DataBinderProvider
{
    private readonly DataBinderProviderOptions _options;
    public DataBinderProvider(DataBinderProviderOptions options)
    {
        _options = options;
    }
    public IDataBinder GetBinder(string name)
    {
        string argumentString;
        string key;
        var indexOfFirstOpenParens = name.IndexOf('(');
        if (indexOfFirstOpenParens >= 0 && name.EndsWith(')'))
        {
            key = name.Substring(0, indexOfFirstOpenParens);
            argumentString = name.Substring(
                indexOfFirstOpenParens + 1,
                name.Length - indexOfFirstOpenParens - 2);
        }
        else
        {
            key = name;
            argumentString = null;
        }
        var map = _options.Map;
        if (!map.TryGetValue(key, out var type))
        {
            return default;
        }
        return ConstructorInfoUtilities.CreateInstance<IDataBinder>(type, argumentString);
    }
}
