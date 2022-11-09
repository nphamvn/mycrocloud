using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MockServer.Web.Models;
using MockServer.Web.Services;

namespace MockServer.Web.Pages.Workspaces;

public class CreateModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IWorkspacesService workspacesService;

    public CreateModel(ILogger<IndexModel> logger, IWorkspacesService workspacesService)
    {
        _logger = logger;
        this.workspacesService = workspacesService;
    }

    public void OnGet()
    {
    }
    [BindProperty]
    public Workspace? Workspace { get; set; }
    public async Task<IActionResult> OnPost()
    {
        _logger.LogInformation(nameof(OnPost));
        _logger.LogInformation(nameof(Workspace.Name) + ": " + Workspace!.Name);
        return Redirect("./Index");
    }
}
