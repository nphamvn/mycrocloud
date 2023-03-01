namespace MockServer.Api.TinyFramework;

public class LengthConstraint : IConstraint
{
    public int Length { get; set; }

    public string ErrorMessage => throw new NotImplementedException();

    public LengthConstraint(int length)
    {
        Length = length;
    }

    public bool Match(object value)
    {
        throw new NotImplementedException();
    }
}
