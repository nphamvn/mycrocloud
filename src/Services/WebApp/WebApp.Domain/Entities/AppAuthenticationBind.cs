namespace WebApp.Domain.Entities;

public class AppAuthenticationBind : BaseEntity
{
    public int AppId { get; set; }
    public App App { get; set; }
    public int SchemeId { get; set; }
    public AuthenticationScheme Scheme { get; set; }
}