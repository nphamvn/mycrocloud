using Ocelot.Errors;

namespace Ocelot.DownstreamWebApplicationFinder.Finder
{
    public class UnableToFindApplicationError : Error
    {
        public UnableToFindApplicationError()
            : base($"UnableToFindDownstreamWebApplicationError", OcelotErrorCode.UnableToFindDownstreamWebApplicationError, 404)
        {
        }
    }
}