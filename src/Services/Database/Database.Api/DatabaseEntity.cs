namespace Database.Api;

public class DatabaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ICollection<DatabaseColumn> Columns { get; set; }
}

public class DatabaseColumn
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Properties { get; set; }
}