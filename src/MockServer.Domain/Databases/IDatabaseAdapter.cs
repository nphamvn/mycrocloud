namespace MockServer.Domain.Databases;

public interface IDatabaseAdapter
{
    string ReadJson();
    void Write(object value);
}
