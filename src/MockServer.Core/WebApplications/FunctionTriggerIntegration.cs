using MockServer.Core.Functions;

namespace MockServer.Core.WebApplications;

public class FunctionTriggerIntegration : RouteIntegration
{
    public int FunctionId { get; set; }
    public Function Function { get; set; }
}
