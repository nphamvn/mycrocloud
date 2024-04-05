using System.ComponentModel.DataAnnotations;
using Form.Builder.Api.Models;

namespace Form.Builder.Api.Entities;

public class FormFieldValue
{
    [Key]
    public int Id { get; set; }
    
    public FormSubmission FormSubmission { get; set; } = null!;
    
    public FormField Field { get; set; } = null!;

    public string? StringValue { get; set; }
}