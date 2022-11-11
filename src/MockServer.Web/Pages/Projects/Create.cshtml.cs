using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MockServer.Web.Models;
using MockServer.Web.Services;

namespace MockServer.Web.Pages.Projects;

public class CreateModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IProjectService workspacesService;

    public CreateModel(ILogger<IndexModel> logger, IProjectService workspacesService)
    {
        _logger = logger;
        this.workspacesService = workspacesService;
    }

    public void OnGet()
    {
    }
    [BindProperty]
    public Project? Project { get; set; }
    public async Task<IActionResult> OnPost()
    {
        _logger.LogInformation(nameof(OnPost));
        _logger.LogInformation(nameof(Project.Name) + ": " + Project!.Name);
        return Redirect("./Index");
    }
}
