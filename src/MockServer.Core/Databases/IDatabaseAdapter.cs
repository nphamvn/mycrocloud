namespace MockServer.Core.Databases;

public interface IDatabaseAdapter
{
    string ReadJson();
    void Write(object value);
}
