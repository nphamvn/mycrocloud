namespace MockServer.Api.Constraints;

public class IntConstraint : IConstraint
{
    public IntConstraint()
    {

    }
    public bool Match(object value, out string message)
    {
        message = "";
        return true;
    }
}
