namespace MockServer.Api.TinyFramework;

public interface IConstraint
{
    bool Match(object value, out string message);
}
