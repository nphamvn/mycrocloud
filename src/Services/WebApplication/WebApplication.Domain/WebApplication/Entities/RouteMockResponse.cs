using WebApplication.Domain.Shared;

namespace WebApplication.Domain.WebApplication.Entities
{
    public class RouteMockResponse
    {
        public GeneratedValue StatusCode { get; set; }
        public List<HeaderItem> Headers { get; set; } = new ();
        public GeneratedValue Body { get; set; }
    }
    public class HeaderItem
    {
        public string Name { get; set; }
        public GeneratedValue Value { get; set; }
    }
}