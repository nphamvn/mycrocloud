namespace MockServer.Api.TinyFramework;

public class NonFileNameConstraint : IConstraint
{
    public string ErrorMessage => throw new NotImplementedException();

    public bool Match(object value)
    {
        throw new NotImplementedException();
    }
}
