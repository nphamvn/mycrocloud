namespace MockServer.Web.Shared;

public class BuiltInValdationAttributeDescription
{
    public string Name { get; set; }
    public string Descritption { get; set; }
    public string ParameterDescription { get; set; }
    public IList<BuiltInValdationAttributeParameterDescription> Parameters { get; set; }
}

