using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class JwtBearerAuthenticationSchemeSaveViewModel
{
    [Display(Name= "Name")]
    [Required]
    public string Name { get; set; }

    [Display(Name = "Display Name")]
    public string DisplayName { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }
    public JwtBearerSchemeOptionsSaveModel Options { get; set; }
}
public class JwtBearerSchemeOptionsSaveModel
{
    public bool RequireHttpsMetadata { get; set; } = false;
    public string MetadataAddress { get; set; }
    public string SecretKey { get; set; }
    public IList<string> SymmetricSecuritySecretKeys { get; set; }
    [Required]
    public string Authority { get; set; }
    public string Algorithm { get; set; }
    [Required]
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string BinderSource { get; set; } = "Authorization";
    public string TokenPrefix { get; set; } = "Bearer";
}