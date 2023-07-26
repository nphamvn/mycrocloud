using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp
{
    public class RouteSaveModel
    {
        public int RouteId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
        public string Name { get; set; }
        public string Description { get; set; }
        public RouteMatchViewModel Match { get; set; }
        public AuthorizationViewModel Authorization { get; set; }
        public RouteValidationViewModel Validation { get; set; }
        public RouteResponseViewModel Response { get; set; }
    }

    public class RouteResponseViewModel
    {
    }

    public class RouteMatchViewModel
    {
        public int Order { get; set; }
        public ICollection<string> Methods { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
        public string Path { get; set; }
    }
    public class RouteValidationViewModel {
        public IList<QueryParamerterValidationRuleViewModel> QueryParamerters { get; set; }
        public IList<HeaderValidationRuleViewModel> Headers { get; set; }
        public IList<BodyFieldValidationRuleViewModel> Body { get; set; }
    }

    public class BodyFieldValidationRuleViewModel
    {
    }

    public class HeaderValidationRuleViewModel
    {
    }

    public class QueryParamerterValidationRuleViewModel
    {
    }

    public class ClaimViewModel {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Value { get; set; }
    }
}