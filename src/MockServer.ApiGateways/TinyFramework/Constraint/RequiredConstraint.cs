using System.Globalization;

namespace MockServer.Api.TinyFramework;

public class RequiredConstraint : IConstraint
{
    public string ErrorMessage => throw new NotImplementedException();

    public bool Match(object value)
    {
        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        return !string.IsNullOrEmpty(valueString);
    }
}
