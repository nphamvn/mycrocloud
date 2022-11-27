using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.WebMVC.Models.Request;

public class RequestItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public int Method { get; set; }
}
