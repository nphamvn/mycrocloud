namespace MockServer.Domain.Databases;

public class DbException: Exception
{
    public DbException(string message) : base(message)
    {
        
    }
}
