public class TestExpectionCallback: IExpectionCallback
{
    public TestExpectionCallback()
    {
    }

    public async Task<HttpResponseMessage> Handle(HttpRequest request)
    {
        var response = new HttpResponseMessage();

        response.StatusCode = System.Net.HttpStatusCode.OK;
        response.Content = new StringContent("ok"); 

        return response;
    }
}