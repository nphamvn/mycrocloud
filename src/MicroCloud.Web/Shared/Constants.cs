namespace MicroCloud.Web.Common;

public class Constants
{
    public const int TemporaryId = -9999;
    public class RouteName
    {
        public const string WebApplicationId = "WebApplicationId";
        public const string WebApplicationName = "WebApplicationName";
        public const string RouteId = "RouteId";
        public const string DatabaseId = "DatabaseId";
        public const string DatabaseName = "DatabaseName";
        public const string PolicyId = "PolicyId";
    }
    public class RouteTemplate
    {
        public const string ProjectsController = "[controller]";
        public const string ProjectsController_View = "{ProjectName}";
        public const string ProjectsController_Create = "create";
    }
}
