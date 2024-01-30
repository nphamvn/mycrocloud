using System.Text.Json;
using Jint;

namespace TestProject2;

public class JintTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        const string content = "[{\"foo\":\"bar\"}]";
        var obj = JsonSerializer.Deserialize<object>(content);
        // var result = new Engine()
        //     .Evaluate("""
        //               const person = { name: 'Nam' };
        //               const template = Handlebars.compile('My name is {{person.name}}');
        //               template({ person });
        //               """)
        //     .AsString();
        Assert.Pass();
    }
    
    [Test]
    public void Test2()
    {
        const string content = "[{\"foo\":\"bar\"}]";
        var obj = JsonSerializer.Deserialize<dynamic>(content);
        // var result = new Engine()
        //     .Evaluate("""
        //               const person = { name: 'Nam' };
        //               const template = Handlebars.compile('My name is {{person.name}}');
        //               template({ person });
        //               """)
        //     .AsString();
        Assert.Pass();
    }
    [Test]
    public void Test3()
    {
        const string content = "[{\"foo\":\"bar\"}]";
        var obj = JsonSerializer.Deserialize<Dictionary<string, object>>("""
                                                                         {
                                                                         "docs": [
                                                                            {
                                                                                "foo": "bar"
                                                                            }
                                                                         ]
                                                                         }
                                                                         """);
        Assert.Pass();
    }
}