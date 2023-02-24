namespace MockServer.Core.Settings;

public class GlobalSettings
{
    public GlobalSettings()
    {
    }
    public SqlSettings Sqlite { get; set; }
}
public class SqlSettings
{
    public string ConnectionString { get; set; }
}