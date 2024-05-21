namespace WebApp.Domain.Entities;

public class Variable : BaseEntity
{
    public int Id { get; set; }
    public int AppId { get; set; }
    public string Name { get; set; }
    public string StringValue { get; set; }
    public VariableValueType ValueType { get; set; }
    public bool IsSecret { get; set; }
    public App App { get; set; }
}

/// <summary>
/// Based on JSON primitive types
/// </summary>
public enum VariableValueType
{
    String,
    Number,
    Boolean,
    Null
}