using System.ComponentModel.DataAnnotations;

namespace WebApp.RestApi.Models;

public class AppRenameRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
}
