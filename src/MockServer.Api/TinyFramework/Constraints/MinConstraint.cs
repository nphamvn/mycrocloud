namespace MockServer.Api.TinyFramework;

public class MinConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        throw new NotImplementedException();
    }
}
