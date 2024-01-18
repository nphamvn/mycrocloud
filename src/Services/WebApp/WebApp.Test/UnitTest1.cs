using Jint;

namespace WebApp.Test;

public class Tests
{
    private Engine _engine;
    [SetUp]
    public void Setup()
    {
        _engine = new Engine();
    }

    [Test]
    public void Test1()
    {
        var result = _engine.Evaluate("1 + 1");
        Assert.AreEqual(2, result.AsNumber());
    }
    
    [Test]
    public void Test2()
    {
        
        Assert.AreEqual(2, result.AsNumber());
    }
}