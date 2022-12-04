using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.WebMVC.Models.Request;

public class FixedRequestConfigViewModel
{
    public int ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; }
    public int RequestId { get; set; }
    public string ResponseContentType { get; set; }
    public int Delay { get; set; }
}
