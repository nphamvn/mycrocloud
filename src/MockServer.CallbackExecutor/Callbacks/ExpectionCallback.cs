public class ExpectionCallback : IExpectionCallback
{
    public Task<HttpResponseMessage> Handle(HttpRequest request)
    {
        throw new NotImplementedException();
    }
}