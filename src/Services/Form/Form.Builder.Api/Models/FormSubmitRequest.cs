namespace Form.Builder.Api.Models;

public class FormSubmitRequest
{
    public Guid FieldId { get; set; }
    public object Value { get; set; }
}