using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockServer.Core.Entities.Requests;
using MockServer.Core.Enums;

namespace MockServer.WebMVC.Models.Project;

public class RequestOpenViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RequestMethod Method { get; set; }
    public string Path { get; set; }
    public string ProjectName { get; set; }
    public int ProjectId { get; set; }
    public RequestType Type { get; set; }
    public string Username { get; set; }
    public string Description { get; set; }
    public string Url => string.Format("https://{0}.mockserver.com/{1}/{2}", Username, ProjectName, Path);
    public string MethodTextColor
    => Method switch
    {
        RequestMethod.DELETE => "text-red",
        RequestMethod.POST => "text-orange",
        RequestMethod.PUT => "text-yellow",
        RequestMethod.GET => "text-green",
        _ => "text-red"
    };
    public IList<RequestParam> RequestParams { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public RequestBody RequestBody { get; set; }
}
