using System.Text.Json;
using Jint;

namespace WebApp.UnitTest.Jint;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        const string name = "Nam";
        var requestQueryJson = JsonSerializer.Serialize(new
        {
            Name = name
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        var requestHeadersJson = JsonSerializer.Serialize(new
        {
            ContentType = name
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("log", new Action<object>(Console.WriteLine))
            .SetValue("template", "Hi {{request.query.name}}")
            .SetValue("request_query_string", requestQueryJson)
            .SetValue("request_header_string", requestHeadersJson)
            .Evaluate("""
                      log(request_query_string);
                      const data = {
                        request: {
                            headers: JSON.parse(request_header_string),
                            query: JSON.parse(request_query_string)
                        }
                      };
                      Handlebars.compile(template)(data);
                      """)
            .AsString();
        Assert.That(result, Is.EqualTo($"Hi {name}"));
    }
    
    [Test]
    public void Test2()
    {
        var engine = new Engine();
        
        const string userCode = """
                            function handle(number) {
                                return number;
                            }
                            """;
        
        engine.Execute(userCode);
        
        const int number = 1;
        engine.SetValue("number", number);
        
        
        const string code = """
                            (() => {
                                return handle(number);
                            })();
                            """;
        
        var resultNumber = engine.Evaluate(code).AsNumber();
        
        Assert.That(resultNumber, Is.EqualTo(number));
    }
}