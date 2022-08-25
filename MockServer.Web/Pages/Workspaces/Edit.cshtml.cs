using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MockServer.Web.Pages.Workspaces;

public class EditModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public EditModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
}
