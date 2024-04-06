using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Entities;

public class FormSubmission
{
    [Key]
    public int Id { get; set; }
    
    public Form Form { get; set; }

    public ICollection<FormSubmissionFieldValue> Values { get; set; } = [];
    
    public DateTime CreatedAt { get; set; }
}