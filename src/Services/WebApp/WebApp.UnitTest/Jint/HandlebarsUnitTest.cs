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
        var name = "Nam";
        var body = new {
            Name = name
        };
        var bodyJson = JsonSerializer.Serialize(body);
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .Evaluate("""
                      const person = { name: 'Nam' };
                      const template = Handlebars.compile('My name is {{person.name}}');
                      template({ person });
                      """)
            .AsString();
        Assert.That(result, Is.EqualTo($"My name is {name}"));
    }
    [Test]
    public void Test2()
    {
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("person", new
            {
                name = "Nam",
                skills = new[] { "C#", "Javascript" }
            })
            .Evaluate("""
                      const template = Handlebars.compile('My name is {{person.name}}. I know {{person.skills.[0]}} and {{person.skills.[1]}}');
                      template({ person });
                      """)
            .AsString();
        
        Assert.That(result, Is.EqualTo("My name is Nam. I know C# and Javascript"));
    }
    [Test]
    public void Test3()
    {
        const string bodyString = """
                                  {
                                     "name": "Nam",
                                     "skills": ["C#", "Javascript"],
                                     "address": {
                                         "city": "HCM",
                                         "country": "VN"
                                     }
                                  }
                                  """;
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("bodyString", bodyString)
            .Evaluate("""
                      const person = JSON.parse(bodyString);
                      const template = Handlebars.compile('My name is {{person.name}}. I know {{person.skills.[0]}} and {{person.skills.[1]}}. I live in {{person.address.city}}, {{person.address.country}}');
                      template({ person });
                      """)
            .AsString();
        
        Assert.That(result, Is.EqualTo("My name is Nam. I know C# and Javascript. I live in HCM, VN"));
    }
    
    [Test]
    public void Test4()
    {
        var bodyString = JsonSerializer.Serialize(new
        {
            name = "Nam",
            skills =  new[] { "C#", "Javascript" },
            address = new {
                city = "HCM",
                country = "VN"
            }
        });
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("bodyString", bodyString)
            .Evaluate("""
                      const person = JSON.parse(bodyString);
                      const template = Handlebars.compile('My name is {{person.name}}. I know {{person.skills.[0]}} and {{person.skills.[1]}}. I live in {{person.address.city}}, {{person.address.country}}');
                      template({ person });
                      """)
            .AsString();
        
        Assert.That(result, Is.EqualTo("My name is Nam. I know C# and Javascript. I live in HCM, VN"));
    }
    
    [Test]
    public void Test5()
    {
        var bodyString = JsonSerializer.Serialize(new
        {
            name = "Nam",
            skills =  new[] { "C#", "Javascript" },
            address = new {
                city = "HCM",
                country = "VN"
            }
        });
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("bodyString", bodyString)
            .Evaluate("""
                      const person = JSON.parse(bodyString);
                      const template = Handlebars.compile('My name is {{person.name}}. I know {{person.skills.[0]}} and {{person.skills.[1]}}. I live in {{person.address.city}}, {{person.address.country}}');
                      template({ person });
                      """)
            .AsString();
        
        Assert.That(result, Is.EqualTo("My name is Nam. I know C# and Javascript. I live in HCM, VN"));
    }
    [Test]
    public void Test6()
    {
        var request = new
        {
            method = "GET",
            body = JsonSerializer.Deserialize<dynamic>("""
                                                       {
                                                            "name": "Nam",
                                                            "skills": ["C#", "Javascript"],
                                                            "address": {
                                                                "city": "HCM",
                                                                "country": "VN"
                                                            }
                                                       }
                                                       """)
        };
        var result = new Engine()
            .Execute(File.ReadAllText("Scripts/handlebars.min-v4.7.8.js"))
            .SetValue("requestJson", JsonSerializer.Serialize(request))
            .Evaluate("""
                      const request = JSON.parse(requestJson);
                      const template = Handlebars.compile('My name is {{request.body.name}}. I know {{request.body.skills.[0]}} and {{request.body.skills.[1]}}. I live in {{request.body.address.city}}, {{request.body.address.country}}');
                      template({ request });
                      """)
            .AsString();
        
        Assert.That(result, Is.EqualTo("My name is Nam. I know C# and Javascript. I live in HCM, VN"));
    }
}