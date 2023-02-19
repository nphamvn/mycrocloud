using System.Net;

namespace MockServer.Core.Helpers;

public static class HttpProtocolExtensions
{
    public static List<string> CommonHttpMethods
        => new List<string>
            {
            "GET",
            "HEAD",
            "POST",
            "PUT",
            "DELETE",
            "CONNECT",
            "OPTIONS",
            "TRACE",
            "PATCH",
            };
    public static ICollection<HttpStatusCode> CommonHttpStatusCode
         => new List<HttpStatusCode>
        {
            HttpStatusCode.OK,
            //new SelectListItem(nameof(HttpStatusCode.OK), ((int)HttpStatusCode.OK).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.Created), ((int)HttpStatusCode.Created).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.NoContent), ((int)HttpStatusCode.NoContent).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.NotModified), ((int)HttpStatusCode.NotModified).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.BadRequest), ((int)HttpStatusCode.BadRequest).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.Unauthorized), ((int)HttpStatusCode.Unauthorized).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.Forbidden), ((int)HttpStatusCode.Forbidden).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.NotFound), ((int)HttpStatusCode.NotFound).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.RequestTimeout), ((int)HttpStatusCode.RequestTimeout).ToString()),
            // new SelectListItem(nameof(HttpStatusCode.InternalServerError), ((int)HttpStatusCode.InternalServerError).ToString()),
        };
}
