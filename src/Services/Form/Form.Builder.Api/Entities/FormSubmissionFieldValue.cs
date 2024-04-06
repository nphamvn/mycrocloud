using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Entities;

public class FormSubmissionFieldValue
{
    [Key]
    public int Id { get; set; }
    
    public FormSubmission FormSubmission { get; set; } = null!;
    
    public FormField Field { get; set; } = null!;

    public string? StringValue { get; set; }
}