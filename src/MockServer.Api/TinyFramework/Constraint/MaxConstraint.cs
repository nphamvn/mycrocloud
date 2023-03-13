using System.Globalization;

namespace MockServer.Api.TinyFramework;

/// <summary>
/// Constrains a route parameter to be an integer with a maximum value.
/// </summary>
public class MaxConstraint : IConstraint
{
    public MaxConstraint(long max)
    {
        Max = max;
    }

    public long Max { get; }

    public string ErrorMessage => throw new NotImplementedException();

    public bool Match(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (long.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
        {
            return longValue <= Max;
        }
        return false;
    }
}