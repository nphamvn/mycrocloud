public interface IExpectionCallback
{
    Task<HttpResponseMessage> Handle(HttpRequest request);
}
