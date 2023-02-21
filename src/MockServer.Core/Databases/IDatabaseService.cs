namespace MockServer.Core.Databases;

public abstract class Db
{
    public abstract string readJson();
    public abstract object read();
    public abstract void write(object obj);
}
