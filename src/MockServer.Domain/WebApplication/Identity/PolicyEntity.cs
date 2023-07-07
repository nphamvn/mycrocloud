namespace MockServer.Domain.WebApplication.Entities
{
    public class PolicyEntity
    {
        public int PolicyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Claims Claims { get; set; }
        public List<string> ConditionalExpressions { get; set; }
        public int WebApplicationId { get; set; }
        public WebApplicationEntity WebApplicationEntity { get; set; }
    }

    public class Claims : Dictionary<string, string>
    {

    }
}