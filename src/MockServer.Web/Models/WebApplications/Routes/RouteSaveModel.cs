using System.ComponentModel.DataAnnotations;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Models.WebApplications.Routes
{
    public class RouteSaveModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
        public string Path { get; set; }
        public List<string> Methods { get; set; }
        public int Order { get; set; }
        public Authorization Authorization { get; set; }
        public IList<ValidationItem> QueryParamsValidation { get; set; }
        public IList<ValidationItem> HeadersValidation { get; set; }
        public IList<ValidationItem> BodyValidation { get; set; }
    }
    public class Authorization
    {
        public AuthorizationType Type { get; set; }
        public List<int> Policies { get; set; }
        //public List<string> claims { get; set; }
    }
}