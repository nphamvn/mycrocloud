using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class PolicySaveModel : IValidatableObject
{
    public PolicySaveModel()
    {
        ConditionalExpressions = new();
    }

    [Required]
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public WebApplication? WebApplication { get; set; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        Console.WriteLine("Validate");
        if (ConditionalExpressions.Count is not > 0)
        {
            yield return new ValidationResult("Please specify at least one expression", new string[] { nameof(ConditionalExpressions) });
        }
        else if (ConditionalExpressions.Contains(null) || ConditionalExpressions.Contains(""))
        {
            Console.WriteLine("Expression must not be empty");
            yield return new ValidationResult("Expression must not be empty", new string[] { nameof(ConditionalExpressions) });
        }
    }
}
