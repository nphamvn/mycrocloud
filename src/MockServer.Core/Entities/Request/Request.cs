using System;
namespace MockServer.Core.Entities;
public class Request
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
}

