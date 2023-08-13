using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MycroCloud.WebMvc.Identity
{
    public sealed class MycroCloudIdentityDbContext(DbContextOptions<MycroCloudIdentityDbContext> options)
        : IdentityDbContext<MycroCloudIdentityUser, MycroCloudIdentityRole, string>(options)
    {
    }
}