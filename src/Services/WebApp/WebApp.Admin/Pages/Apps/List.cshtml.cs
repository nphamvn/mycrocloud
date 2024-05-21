using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure;

namespace WebApp.Admin.Pages.Apps
{
    public class ListPageModel(AppDbContext appDbContext) : PageModel
    {
        public IEnumerable<App> Apps { get; set; } = new List<App>();

        public async Task OnGet()
        {
            Apps = await appDbContext.Apps
                    .Include(a => a.Routes)
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
        }
    }
}
