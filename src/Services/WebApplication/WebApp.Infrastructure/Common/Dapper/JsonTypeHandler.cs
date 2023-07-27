using System.Text.Json;
namespace Dapper;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value, new JsonSerializerOptions());
    }
    public override T Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        return default(T);
    }
}
