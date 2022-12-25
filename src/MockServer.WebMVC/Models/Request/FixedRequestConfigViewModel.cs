using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Entities.Requests;

namespace MockServer.WebMVC.Models.Request;

public class FixedRequestConfigViewModel
{
    public IList<RequestParam> RequestParams { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public RequestBody RequestBody { get; set; }
    public int ResponseStatusCode { get; set; }
    public string ResponseBody { get; set; }
    public Dictionary<string, string> ResponseHeaders { get; set; }
    public int RequestId { get; set; }
    public string ResponseContentType { get; set; }
    public int Delay { get; set; }
    public static ICollection<SelectListItem> CommonHttpStatusCode
         => GetCommonHttpStatusCode();
    private static ICollection<SelectListItem> GetCommonHttpStatusCode()
    {
        return new List<SelectListItem>
        {
            new SelectListItem(nameof(HttpStatusCode.OK), ((int)HttpStatusCode.OK).ToString()),
            new SelectListItem(nameof(HttpStatusCode.Created), ((int)HttpStatusCode.Created).ToString()),
            new SelectListItem(nameof(HttpStatusCode.NoContent), ((int)HttpStatusCode.NoContent).ToString()),
            new SelectListItem(nameof(HttpStatusCode.NotModified), ((int)HttpStatusCode.NotModified).ToString()),
            new SelectListItem(nameof(HttpStatusCode.BadRequest), ((int)HttpStatusCode.BadRequest).ToString()),
            new SelectListItem(nameof(HttpStatusCode.Unauthorized), ((int)HttpStatusCode.Unauthorized).ToString()),
            new SelectListItem(nameof(HttpStatusCode.Forbidden), ((int)HttpStatusCode.Forbidden).ToString()),
            new SelectListItem(nameof(HttpStatusCode.NotFound), ((int)HttpStatusCode.NotFound).ToString()),
            new SelectListItem(nameof(HttpStatusCode.RequestTimeout), ((int)HttpStatusCode.RequestTimeout).ToString()),
            new SelectListItem(nameof(HttpStatusCode.InternalServerError), ((int)HttpStatusCode.InternalServerError).ToString()),
        };
    }
}