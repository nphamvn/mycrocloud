using WebApp.Domain.Shared;

namespace WebApp.Domain.Entities;
public class AuthenticationSchemeEntity : BaseEntity
{
    public int SchemeId { get; set; }
    public int AppId { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
}