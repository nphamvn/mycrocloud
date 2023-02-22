using System.ComponentModel.DataAnnotations;
using MockServer.Core.Models.Services;

namespace MockServer.Web.Models.Database;

public class SaveDatabaseViewModel
{
    public int? Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Data { get; set; }
    public IEnumerable<Service>? AllServices { get; set; }
    public IEnumerable<Service>? AllowedService { get; set; }
}
