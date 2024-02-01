using Jint;

namespace WebApp.UnitTest.Jint;

public class LimitUnitTest
{
    [SetUp]
    public void SetUp()
    {
        
    }
    [Test]
    public void ShouldThrowTimeoutException()
    {
        try
        {
            var engine = new Engine(options => { options.TimeoutInterval(TimeSpan.FromSeconds(1)); });

            engine.Execute("""
                            while(true) {
                            }
                           """);
        }
        catch (TimeoutException e)
        {
            Assert.Pass();
        }
    }
}