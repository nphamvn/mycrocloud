using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MockServer.Web.Models;

namespace MockServer.Web.Pages.Requests;

public class CreateModel : PageModel
{
    [BindProperty]
    public Request? Request { get; set; }
}