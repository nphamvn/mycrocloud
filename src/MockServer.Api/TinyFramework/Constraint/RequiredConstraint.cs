using System.Globalization;

namespace MockServer.Api.TinyFramework;

public class RequiredConstraint : IConstraint
{
    public bool Match(object value, out string message)
    {
        message = "";
        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        return !string.IsNullOrEmpty(valueString);
    }
}
