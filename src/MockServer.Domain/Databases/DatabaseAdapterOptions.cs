using System.Text.Json;

namespace MockServer.Domain.Databases;

public class DatabaseAdapterOptions
{
    public virtual JsonSerializerOptions JsonSerializerOptions { get; set; }
}
