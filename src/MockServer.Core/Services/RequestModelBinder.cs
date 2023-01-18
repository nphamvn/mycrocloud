using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MockServer.Core.Services;
public class DataBinderProviderOptions
{
    public Dictionary<string, Type> Map { get; set; }
    public DataBinderProviderOptions()
    {
        Map = GetDefaultMap();
    }
    public static DataBinderProviderOptions Default =>
        new DataBinderProviderOptions
        {
            Map = GetDefaultMap()
        };

    private static Dictionary<string, Type> GetDefaultMap()
    {
        Dictionary<string, Type> defaults = new();
        AddMap<FromHeaderDataBinder>(defaults, "header");
        return defaults;
    }
    private static void AddMap<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDataBinder>(Dictionary<string, Type> map, string key) where TDataBinder : IDataBinder
    {
        map[key] = typeof(TDataBinder);
    }
}

public class DataBinderProvider
{
    private readonly DataBinderProviderOptions Options;
    public DataBinderProvider(IOptions<DataBinderProviderOptions> options)
    {
        Console.WriteLine(nameof(DataBinderProvider));
        Options = options.Value;
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
        var map = Options.Map;
        if (!map.TryGetValue(key, out var type))
        {
            return default;
        }
        return (IDataBinder)CreateBinder(type, argumentString);
    }
    private static IDataBinder CreateBinder(Type type, string argumentString)
    {
        ConstructorInfo activationConstructor = null;
        object[] parameters = null;
        var constructors = type.GetConstructors();
        if (constructors.Length == 1 && GetNonConvertableParameterTypeCount(constructors[0].GetParameters()) == 1)
        {
            activationConstructor = constructors[0];
            parameters = ConvertArguments(activationConstructor.GetParameters(), new string[] { argumentString });
        }
        else
        {
            var arguments = argumentString?.Split(',', StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
            var matchingConstructors = constructors
                .Where(ci => GetNonConvertableParameterTypeCount(ci.GetParameters()) == arguments.Length)
                .OrderByDescending(ci => ci.GetParameters().Length)
                .ToArray();
            if (matchingConstructors.Length == 0)
            {
                throw new Exception("No constructor found");
            }
            else
            {
                // When there are multiple matching constructors, choose the one with the most service arguments
                if (matchingConstructors.Length == 1
                    || matchingConstructors[0].GetParameters().Length > matchingConstructors[1].GetParameters().Length)
                {
                    activationConstructor = matchingConstructors[0];
                }
                else
                {
                    throw new Exception("Constructors are ambiguous");
                }
                parameters = ConvertArguments(activationConstructor.GetParameters(), arguments);
            }
        }
        return (IDataBinder)activationConstructor.Invoke(parameters);
    }
    private static int GetNonConvertableParameterTypeCount(ParameterInfo[] parameters)
    {
        var count = 0;
        for (var i = 0; i < parameters.Length; i++)
        {
            if (typeof(IConvertible).IsAssignableFrom(parameters[i].ParameterType))
            {
                count++;
            }
        }

        return count;
    }
    private static object[] ConvertArguments(ParameterInfo[] parameterInfos, string[] arguments)
    {
        var parameters = new object[parameterInfos.Length];
        var argumentPosition = 0;
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            var parameter = parameterInfos[i];
            var parameterType = parameter.ParameterType;
            parameters[i] = Convert.ChangeType(arguments[argumentPosition], parameterType, CultureInfo.InvariantCulture);
            argumentPosition++;
        }

        return parameters;
    }
}
public interface IDataBinder
{
    object Get(HttpContext context);
}
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