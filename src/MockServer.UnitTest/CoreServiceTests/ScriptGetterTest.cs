using MockServer.Core.Interfaces;
using MockServer.Core.Services;

namespace MockServer.UnitTest.CoreServiceTests;

public class ScriptGetterTest
{
    private IHandlebarsTemplateRenderer _scriptGetter;

    public ScriptGetterTest()
    {
        _scriptGetter = new HandlebarsTemplateRenderer();
    }

    // [Fact]
    // public void Should_Return_1_Script()
    // {
    //     string input = "The sum of 1 and 2 is @{1 + 2}";
    //     var scripts = _scriptGetter.Get(input);

    //     Assert.Equal(1, scripts.Count);
    // }

    // [Fact]
    // public void Should_Return_Script()
    // {
    //     string input = "The sum of 1 and 2 is @{1 + 2}";
    //     var scripts = _scriptGetter.Get(input);
    //     Assert.Equal("1 + 2", scripts[0]);
    // }

    // [Fact]
    // public void Should_Return_Evaluate_Result()
    // {
    //     var ret = _scriptGetter.EvaluateExpression("", "'mr.' + ctx.request.headers['Name']");
    //     Assert.Equal("mr.Nam", ret.ToString());
    // }

    [Fact]
    public void Should_Read_File_Text()
    {
        var ctx = new
        {
            request = new
            {
                headers = new Dictionary<string, string>()
                    {
                        {"name", "Nam"}
                    }
            }
        };
        var template = "Hi {{ctx.request.headers.name}}";
        var handlebars = _scriptGetter.Render(ctx, template, "");
        Assert.Equal("Hi Nam", handlebars);
    }
}
