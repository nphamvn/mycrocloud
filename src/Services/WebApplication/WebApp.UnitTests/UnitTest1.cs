using WebApp.Domain.Shared;

namespace WebApp.UnitTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var intValue = 1;
        var routeAuthorizationType = (RouteAuthorizationType)intValue;
    }
}