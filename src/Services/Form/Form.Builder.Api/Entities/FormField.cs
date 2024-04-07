using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Entities
{
    public class FormField: IValidatableObject 
    {
        [Key]
        public Guid Id { get; set; }
        
        public int FormId { get; set; }
        
        public Form Form { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public bool IsRequired { get; set; }
        
        public FormFieldDetails Details { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public ICollection<SelectListItem>? SelectListItems { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationResult.Success;
        }
    }
    
    public class FormFieldDetails
    {
        public TextInputDetails? TextInput { get; set; }
        
        public NumberInputDetails? NumberInput { get; set; }

        public DropdownDetails? DropdownDetails { get; set; }
    }

    public class DropdownDetails
    {
    }

    public class TextInputDetails
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
    
    public class NumberInputDetails
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}
