using Ocelot.Errors;

namespace Ocelot.DownstreamWebApplicationFinder.Finder
{
    public class ApplicationIsBlockedError : Error
    {
        public ApplicationIsBlockedError()
            : base($"WebApplicationIsBlockedError", OcelotErrorCode.WebApplicationIsBlockedError, 502)
        {
        }
    }
}