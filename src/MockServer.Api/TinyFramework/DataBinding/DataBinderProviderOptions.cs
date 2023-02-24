using System.Diagnostics.CodeAnalysis;

namespace MockServer.Api.TinyFramework.DataBinding;

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
