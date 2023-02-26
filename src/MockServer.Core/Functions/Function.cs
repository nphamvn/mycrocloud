
using MockServer.Core.Services;

namespace MockServer.Core.Functions;

public class Function: Service
{
    public Runtime Runtime { get; set; }
    public string Code { get; set; }
    public Dictionary<string, string> ConfigurationSettings { get; set; }
    public Dictionary<string, string> ResourceAllocation { get; set; }
    public Dictionary<string, string> Security { get; set; }
}
