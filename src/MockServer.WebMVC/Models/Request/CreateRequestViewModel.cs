using System.ComponentModel.DataAnnotations;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.WebMVC.Models.Request;

public class CreateUpdateRequestModel
{
    public RequestType Type { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string Name { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
    public string Path { get; set; }
    public string Method { get; set; }
    public string? Description { get; set; }
    public AppAuthorization Authorization { get; set; } = new();
}
