namespace MockServer.Core.WebApplications.Security
{
    public class Policy
    {
        public int PolicyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Claims Claims { get; set; }
        public List<string> ConditionalExpressions { get; set; }
        public int WebApplicationId { get; set; }
        public WebApplication WebApplication { get; set; }
    }

    public class Claims : Dictionary<string, string>
    {

    }
}