namespace MockServer.Api.Constraints;

public interface IConstraint
{
    bool Match(object value, out string message);
}
