using System.ComponentModel.DataAnnotations;
using MycroCloud.WebMvc.Areas.Services.Models.WebApp.Routes.Shared;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp
{
    public class RouteSaveModel
    {
        public int RouteId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public RouteMatchSaveModel Match { get; set; }
        public RouteAuthorizationSaveModel Authorization { get; set; }
        public RouteValidationSaveModel Validation { get; set; }
        public RouteResponseSaveModel Response { get; set; }
    }

    public class RouteMatchSaveModel
    {
        public int? Order { get; set; }

        public List<string> Methods { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
        public string Path { get; set; }
    }
    public class RouteAuthorizationSaveModel
    {
        [Required]
        public RouteAuthorizationType Type { get; set; }
        public IList<int> PolicyIds { get; set; }
        public IList<RouteAuthorizationClaimSaveModel> Claims { get; set; }
    }

    public class RouteValidationSaveModel {
        public List<RouteValidationQueryRuleSaveModel> QueryParamerters { get; set; }
        public List<RouteValidationHeaderRuleSaveModel> Headers { get; set; }
        public List<RouteValidationBodyFieldRuleSaveModel> Body { get; set; }
    }

    public class RouteResponseSaveModel
    {
    }

    public class RouteValidationBodyFieldRuleSaveModel
    {
    }

    public class RouteValidationHeaderRuleSaveModel
    {
    }

    public class RouteValidationQueryRuleSaveModel
    {
    }

    public class RouteAuthorizationClaimSaveModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}