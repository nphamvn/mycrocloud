namespace MockServer.ReverseProxyServer.Constraints;

public class LengthConstraint : IConstraint
{
    public int Length { get; set; }
    public LengthConstraint(int length)
    {
        Length = length;
    }
    public bool Match(object value, out string message)
    {
        message = "";
        return true;
    }
}
