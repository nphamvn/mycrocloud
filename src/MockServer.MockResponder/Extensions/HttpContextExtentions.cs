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

        if (message.Content != null)
        {
            foreach (var header in message.Content.Headers)
            {
                context.Response.Headers.Add(header.Key, header.Value.ToArray());
            }

            var contentStream = await message.Content.ReadAsStreamAsync();
            await contentStream.CopyToAsync(context.Response.Body);
        }
    }
}