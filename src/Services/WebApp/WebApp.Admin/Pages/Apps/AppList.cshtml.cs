using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;

namespace MyApp.Namespace
{
    public class AppListModel(AppDbContext appDbContext) : PageModel
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
