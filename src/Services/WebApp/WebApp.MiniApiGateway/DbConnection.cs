namespace WebApp.MiniApiGateway;

public class NoSqlDbConnection(string connectionString)
{
    public void Connect()
    {
        
    }
    
    public object[] Query(string collectionName)
    {
        return new[] { "foo", "bar" };
    }
    
    public void Insert(string collectionName, object data)
    {
        
    }
}