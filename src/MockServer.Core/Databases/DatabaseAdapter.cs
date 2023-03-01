using System.Dynamic;
using System.Text.Json;

namespace MockServer.Core.Databases;

public abstract class DatabaseAdapter : IDatabaseAdapter
{
    private readonly DatabaseAdapterOptions _options;

    public DatabaseAdapter(DatabaseAdapterOptions options)
    {
        _options = options;
    }
    public abstract string ReadJson();
    public abstract void Write(object value);
    
    public virtual object Read() {
        var json = ReadJson();
        return !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<ExpandoObject>(json) : null;
    }
    public string GetJson(object value){
        return JsonSerializer.Serialize(value, _options.JsonSerializerOptions);
    }
}
