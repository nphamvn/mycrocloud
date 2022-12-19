using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public IEnumerable<RequestParam> Params { get; set; } = new List<RequestParam>
    {
        new RequestParam {
            Id = 1,
            Key = "page",
            Value = "1",
            Required = true,
            Exactly = true
        },
        new RequestParam {
            Id = 2,
            Key = "sort",
            Value = "name",
            Required = true,
            Exactly = true
        }
    };
    public IEnumerable<RequestHeader> Headers { get; set; } = new List<RequestHeader>
    {
        new RequestHeader {
            Id = 1,
            Name = "Accept",
            Value = "*/*",
            Required = true,
            Exactly = true
        }
    };
}

public class RequestHeader
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool Exactly { get; set; }
}
public class RequestParam
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public bool Required { get; set; }
    public bool Exactly { get; set; }
}
