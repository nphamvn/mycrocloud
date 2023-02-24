namespace MockServer.Api.TinyFramework;

public class MatchExactlyConstraint : IConstraint
{
    public object CompareValue { get; set; }
    public MatchExactlyConstraint(object compareValue)
    {
        CompareValue = compareValue;
    }
    public bool Match(object value, out string message)
    {
        message = "";
        return value == CompareValue;
    }

    public static MatchExactlyConstraint Create(object compareValue) {
        return new MatchExactlyConstraint(compareValue);
    }
}
