using System.Text.Json;

namespace WebApplication.Domain.Databases;

public class DatabaseAdapterOptions
{
    public virtual JsonSerializerOptions JsonSerializerOptions { get; set; }
}
