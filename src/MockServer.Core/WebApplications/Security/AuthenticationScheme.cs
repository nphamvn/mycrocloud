namespace MockServer.Core.WebApplications.Security;

public class AuthenticationScheme : BaseEntity
{
    public string Name { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public int Order { get; set; }
    public AuthenticationSchemeOptions Options { get; set; }
    public string Description { get; set; }

    public override int GetHashCode()
    {
        return this.Id;
    }
    public override bool Equals(object obj)
    {
        return Equals(obj as AuthenticationScheme);
    }

    private bool Equals(AuthenticationScheme other) {
        return other != null && other.Id == this.Id;
    }
}
