using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockServer.Core.Enums;

namespace MockServer.WebMVC.Models.Request;

public class RequestItem
{
    public int Id { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public RequestMethod Method { get; set; }
    public string Description { get; set; }

    public string MethodBadgeColor
    => Method switch
    {
        RequestMethod.DELETE => "bg-red",
        RequestMethod.POST => "bg-orange",
        RequestMethod.PUT => "bg-yellow",
        RequestMethod.GET => "bg-green",
        _ => ""
    };
}