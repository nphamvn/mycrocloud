using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Models
{
    public class FormFieldCreateUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Type { get; set; }
        
        [Required]
        public bool Required { get; set; }
        
        [Required]
        public FormFieldDetailsCreateUpdateRequest Details { get; set; }
    }

    public class FormFieldDetailsCreateUpdateRequest
    {
        public FormFieldTextInputDetails? TextInput { get; set; }
        public FormFieldNumberInputDetails? NumberInput { get; set; }
    }

    public class FormFieldTextInputDetails
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }
    public class FormFieldNumberInputDetails
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}