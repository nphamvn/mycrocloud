namespace MockServer.Api.TinyFramework;

public interface IConstraint
{
    string ErrorMessage { get; }
    bool Match(object value);
}
