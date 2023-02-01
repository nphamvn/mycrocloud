namespace MockServer.Api.Constraints;

public class NonFileNameConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        throw new NotImplementedException();
    }
}
