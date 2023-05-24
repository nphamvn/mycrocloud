using System.Globalization;

namespace MockServer.Api.TinyFramework;

public class RangeConstraint : IConstraint
{
    public int Min { get; set; }
    public int Max { get; set; }

    public string ErrorMessage => throw new NotImplementedException();

    public RangeConstraint(int min, int max)
    {
        Min = min;
        Max = max;
    }
    public bool Match(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var valueString = Convert.ToString(value, CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(valueString))
        {
            return false;
        }
        if (int.TryParse(valueString, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
        {
            return Min <= intValue && intValue <= Max;
        }
        return false;
    }
}
