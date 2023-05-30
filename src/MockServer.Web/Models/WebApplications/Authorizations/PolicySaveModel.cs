using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class PolicySaveModel : IValidatableObject
{
    public PolicySaveModel()
    {
        Claims = new();
    }

    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    public WebApplication? WebApplication { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield return ValidationResult.Success; 
        Console.WriteLine("Validate");
        if (Claims.Count is not > 0)
        {
            yield return new ValidationResult("Please specify at least one claim", new string[] { nameof(Claims) });
        }
        else if (Claims.Any(c => string.IsNullOrEmpty(c.Key) || string.IsNullOrEmpty(c.Value)))
        {
            Console.WriteLine("Expression must not be empty");
            yield return new ValidationResult("Expression must not be empty", new string[] { nameof(Claims) });
        }
    }
}
