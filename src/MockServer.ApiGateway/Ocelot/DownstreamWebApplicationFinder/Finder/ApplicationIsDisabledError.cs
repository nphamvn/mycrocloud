using Ocelot.Errors;

namespace Ocelot.DownstreamWebApplicationFinder.Finder
{
    public class ApplicationIsNotEnabledError : Error
    {
        public ApplicationIsNotEnabledError()
            : base($"ApplicationIsNotEnabledError", OcelotErrorCode.WebApplicationIsNotEnabledError, 502)
        {
        }
    }
}