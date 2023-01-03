using Microsoft.AspNetCore.Http;

namespace MNB.Web.Services
{
    public interface IUserAgentService
    {
        Platform GetPlatform();
    }

    public class UserAgentService : IUserAgentService
    {
        private readonly string _userAgent;

        public UserAgentService(IHttpContextAccessor httpContextAccessor)
        {
            _userAgent = httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();
        }

        public Platform GetPlatform()
        {
            if (_userAgent.Contains("Mac OS"))
            {
                return Platform.Mac;
            }

            if (_userAgent.Contains("Windows NT"))
            {
                return Platform.Windows;
            }

            return Platform.Others;
        }
    }

    public enum Platform
    {
        Windows,
        Mac,
        iOS,
        iPadOS,
        Android,
        Others
    }
}