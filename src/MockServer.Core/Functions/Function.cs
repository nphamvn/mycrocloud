
using MockServer.Core.Services;

namespace MockServer.Core.Functions;

public class Function: Service
{
    public int RuntimeId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public Dictionary<string, string> ConfigurationSettings { get; set; }
    public Dictionary<string, string> ResourceAllocation { get; set; }
    public Dictionary<string, string> Security { get; set; }
    public Runtime Runtime { get; set; }
}
