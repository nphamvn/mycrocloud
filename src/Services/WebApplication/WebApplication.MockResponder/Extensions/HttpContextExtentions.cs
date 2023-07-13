namespace MockServer.MockResponder.Extensions;

public static class HttpContextExtentions
{
    public static async Task WriteResponseMessage(this HttpContext context, HttpResponseMessage message)
    {
        context.Response.StatusCode = (int)message.StatusCode;
        foreach (var header in message.Headers)
        {
            context.Response.Headers.Add(header.Key, header.Value.ToArray());
        }
        var contentStream = await message.Content.ReadAsStreamAsync();
        await contentStream.CopyToAsync(context.Response.Body);
    }

    public static async Task<object> ToJsonObject(this HttpContext context)
    {
        var obj = new
        {
            
        };
        return obj;
    }
}