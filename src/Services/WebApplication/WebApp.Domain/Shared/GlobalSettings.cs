namespace WebApp.Domain.Shared;

public class GlobalSettings
{
    public GlobalSettings()
    {
    }
    public SqlSettings Sqlite { get; set; }
}
public class SqlSettings : DatabaseOptions
{
}
public class PostgresSettings : DatabaseOptions
{
}

public class DatabaseOptions
{
    public string ConnectionString { get; set; }
}