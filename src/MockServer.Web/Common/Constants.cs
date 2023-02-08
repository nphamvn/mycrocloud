namespace MockServer.Web.Common;

public class Constants
{
    public class RouteName
    {
        public const string ProjectId = "ProjectId";
        public const string ProjectName = "ProjectName";
        public const string RequestId = "RequestId";
    }
    public class Template
    {
        public const string ProjectsController = "[controller]";
        public const string ProjectsController_View = "{ProjectName}";
        public const string ProjectsController_Create = "create";
    }
}
