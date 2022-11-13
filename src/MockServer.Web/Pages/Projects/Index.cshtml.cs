using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MockServer.Web.Pages.Projects;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    public string Name { get; set; }
    public IActionResult OnGet(string? name)
    {
        if (name is null)
        {
            _logger.LogInformation("Get all projects");
            return Page();
        }
        else
        {
            _logger.LogInformation("Get project detail");
            return Page();
        }
    }
}
