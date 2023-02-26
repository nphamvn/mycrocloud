namespace MockServer.Web.Models.WebApplications.Authentications.JwtBearer;

public class JwtBearerSchemeSaveModel : AuthenticationSchemeSaveModel
{
    public JwtBearerSchemeOptionsSaveModel Options { get; set; }
}
