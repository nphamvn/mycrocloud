using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Infrastructure.Repositories.EfCore;
using Route = WebApp.Domain.Entities.Route;
namespace MyApp.Namespace
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class RouteListModel(AppDbContext appDbContext) : PageModel
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        public App App { get; set; }
        public IEnumerable<Route> Routes { get; set; } = new List<Route>();
        public async Task OnGet(int appId)
        {
            App = await appDbContext.Apps.SingleAsync(x => x.Id == appId);
            Routes = await appDbContext.Routes
                        .Where(r => r.AppId == appId)
                        .OrderBy(r => r.CreatedAt)
                        .ToListAsync();
        }
        public async Task OnPostChangeStatus(int id, RouteStatus status)
        {
            var route = await appDbContext.Routes.FirstOrDefaultAsync(x => x.Id == id);
            if (route != null)
            {
                route.Status = status;
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
