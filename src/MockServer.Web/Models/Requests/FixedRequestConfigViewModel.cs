using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Models.Requests;

namespace MockServer.Web.Models.Requests;

public class FixedRequestConfigViewModel
{
    public IList<RequestQuery> RequestParams { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public Response Response { get; set; }
    public static ICollection<SelectListItem> CommonHttpStatusCode
         => new List<SelectListItem>
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