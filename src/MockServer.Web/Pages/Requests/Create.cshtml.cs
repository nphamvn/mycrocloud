using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MockServer.Web.Models;

namespace MockServer.Web.Pages.Requests;

public class CreateModel : PageModel
{
    [BindProperty]
    public Request? Request { get; set; }

    public IActionResult OnPost()
    {

        if (Request!.FixedRequest is FixedRequest fixedRequest)
        {
            Console.WriteLine("FixedRequest: " + fixedRequest.StatusCode + ": " + fixedRequest.Body);
        }
        if (Request!.ForwardRequest is ForwardRequest forwardRequest)
        {
            Console.WriteLine("ForwardRequest: " + forwardRequest.Host);
        }
        return RedirectToPage("Create");
    }
}