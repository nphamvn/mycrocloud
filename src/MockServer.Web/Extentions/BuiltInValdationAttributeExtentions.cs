using MockServer.Web.Shared;

namespace MockServer.Web.Extentions;

public static class BuiltInValdationAttributeExtentions
{
    public static List<BuiltInValdationAttributeDescription> GetBuiltInValdationAttributeParameterDescriptions()
    {
        var attributes = new List<BuiltInValdationAttributeDescription>();
        attributes.Add(new()
        {
            Name = "Required"
        });
        attributes.Add(new()
        {
            Name = "Range",
            Parameters = new List<BuiltInValdationAttributeParameterDescription>()
            {
                new() {
                    Name = "Type"
                },
                new() {
                    Name = "Min",
                    Type = "Number"
                },
                new() {
                    Name = "Max",
                    Type = "Number"
                },
            }
        });
        return attributes;
    }
}
