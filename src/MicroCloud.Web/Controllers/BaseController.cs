using MicroCloud.Web.Common;
using Microsoft.AspNetCore.Mvc;

namespace MicroCloud.Web.Controllers;

[Controller]
[Route("[controller]")]
public class BaseController : Controller
{
    protected void SetFormMode(FormMode formMode)
    {
        ViewBag._FormMode = formMode;
    }
}