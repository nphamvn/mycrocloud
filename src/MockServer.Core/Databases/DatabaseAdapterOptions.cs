using System.Text.Json;

namespace MockServer.Core.Databases;

public class DatabaseAdapterOptions
{
    public virtual JsonSerializerOptions JsonSerializerOptions { get; set; }
}