namespace MockServer.DTOs;

public class ItemDto
{
    public ItemRequest Request { get; set; }
    public ItemResponse Response { get; set; }
    public class ItemRequest
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
    }
    public class ItemResponse
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }
    }
}
