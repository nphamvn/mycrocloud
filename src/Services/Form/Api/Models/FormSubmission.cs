using System.ComponentModel.DataAnnotations;

namespace Form.Builder.Api.Models;
using FormEntity = Form.Builder.Api.Entities.Form;

public class FormSubmission
{
    [Key]
    public int Id { get; set; }
    
    public FormEntity Form { get; set; }
}