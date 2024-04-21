namespace Api.Entities;

public class User
{
    public required string Id { get; set; }

    public required string FullName { get; set; }

    public string Picture { get; set; }
}