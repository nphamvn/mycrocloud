namespace Ocelot.PayloadValidation.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class PayloadValidationMiddlewareMiddlewareExtensions
    {
        public static IApplicationBuilder UsePayloadValidationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PayloadValidationMiddleware>();
        }
    }
}