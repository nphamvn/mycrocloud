﻿public class ExpectionCallback : IExpectionCallback
{
    public async Task<HttpResponseMessage> Handle(HttpRequest request)
    {
        var message = new HttpResponseMessage();
        message.StatusCode = System.Net.HttpStatusCode.OK;
        message.Content = new StringContent("Hi nampham-02. Your requested path: " + request.Path.Value);
        return message;
    }
}