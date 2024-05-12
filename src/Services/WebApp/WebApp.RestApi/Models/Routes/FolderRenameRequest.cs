using System.ComponentModel.DataAnnotations;

namespace WebApp.RestApi.Models.Routes;

public class FolderRenameRequest
{
    [Required]
    public string Name { get; set; }
}