namespace Form.Builder.Api.Entities;

public class SelectListItem
{
    public Guid Id { get; set; }
    
    public required string Text { get; set; }

    public FormField Field { get; set; }
}