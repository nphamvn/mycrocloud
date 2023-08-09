using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class AuthorizationPolicySaveViewModel : IValidatableObject
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield return ValidationResult.Success; 
        if (Claims.Count is not > 0)
        {
            yield return new ValidationResult("Please specify at least one claim", new string[] { nameof(Claims) });
        }
        else if (Claims.Any(c => string.IsNullOrEmpty(c.Key) || string.IsNullOrEmpty(c.Value)))
        {
            yield return new ValidationResult("Expression must not be empty", new string[] { nameof(Claims) });
        }
    }
}