using Ocelot.Logging;
using Ocelot.Middleware;

namespace Ocelot.PayloadValidation.Middleware {
    public class PayloadValidationMiddleware : OcelotMiddleware
    {
        public PayloadValidationMiddleware(IOcelotLogger logger) : base(logger)
        {
        }
    }
}