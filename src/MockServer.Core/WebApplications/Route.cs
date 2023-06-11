﻿using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.WebApplications;
public class Route : BaseEntity
{
    public int RouteId { get; set; }
    public int WebApplicationId { get; set; }
    public ResponseProvider ResponseProvider { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
    public string Method { get; set; }
    public List<string> Methods { get; set; }
    public Authorization Authorization { get; set; }
    public IList<RequestQueryValidationItem> RequestQueryValidationItems { get; set; }
    public IList<RequestHeaderValidationItem> RequestHeaderValidationItems { get; set; }
    public IList<RequestBodyValidationItem> RequestBodyValidationItems { get; set; }
}

