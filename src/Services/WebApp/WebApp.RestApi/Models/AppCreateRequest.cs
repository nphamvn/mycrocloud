using System.ComponentModel.DataAnnotations;

namespace WebApp.RestApi.Models;

public class AppCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [MaxLength(400)]
    public string? Description { get; set; }
}