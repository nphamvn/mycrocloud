namespace MockServer.Api.TinyFramework;

public class NonFileNameConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        throw new NotImplementedException();
    }
}
