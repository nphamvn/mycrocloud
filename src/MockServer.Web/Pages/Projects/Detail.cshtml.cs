using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace MockServer.Web.Pages.Projects;
public class DetailModel : PageModel
{
    private readonly ILogger<DetailModel> _logger;

    public DetailModel(ILogger<DetailModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        _logger.LogInformation("Get project detail");
        return Page();
    }
}