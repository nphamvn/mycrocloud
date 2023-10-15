namespace WebApp.Domain.Entities;
public class AuthorizationPolicy : BaseEntity
{
    public int PolicyId { get; set; }
    public int AppId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}