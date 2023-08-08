namespace MycroCloud.WeMvc.Services;
public interface IUserAgentService
{
    Platform GetPlatform();
}

public class UserAgentService(IHttpContextAccessor httpContextAccessor) : IUserAgentService
{
    private readonly string _userAgent = httpContextAccessor.HttpContext.Request.Headers.UserAgent.ToString();

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