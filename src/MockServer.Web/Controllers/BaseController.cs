using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Common;

namespace MockServer.Web.Controllers;

[Controller]
[Route("[controller]")]
public class BaseController : Controller
{
    protected void SetFormMode(FormMode formMode)
    {
        ViewBag._FormMode = formMode;
    }
}