namespace MockServer.Api.TinyFramework;

public class MatchExactlyConstraint : IConstraint
{
    public object CompareValue { get; set; }

    public string ErrorMessage => throw new NotImplementedException();

    public MatchExactlyConstraint(object compareValue)
    {
        CompareValue = compareValue;
    }
    public bool Match(object value)
    {
        return value == CompareValue;
    }

    public static MatchExactlyConstraint Create(object compareValue) {
        return new MatchExactlyConstraint(compareValue);
    }
}
