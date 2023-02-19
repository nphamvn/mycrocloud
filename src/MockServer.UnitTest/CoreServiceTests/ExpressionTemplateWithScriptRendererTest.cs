using MockServer.Core.Interfaces;
using MockServer.Core.Services;

namespace MockServer.UnitTest.CoreServiceTests;

public class ExpressionTemplateWithScriptRendererTest
{
    private IExpressionTemplateWithScriptRenderer _expressionTemplateWithScriptRenderer;
    public ExpressionTemplateWithScriptRendererTest()
    {
        _expressionTemplateWithScriptRenderer = new ExpressionTemplateWithScriptRenderer();
    }
    [Fact]
    public void Test1()
    {
        var ctx = new
        {
            request = new
            {
                headers = new Dictionary<string, object>() {
                        {"number1", 1},
                        {"number2", 2},
                    }
            }
        };
        string template =
                """
                {
                    "message": "the sum of @{number1} and @{number2} is @{add(number1, number2)}"
                }
                """;
        string script =
                """
                const number1 = ctx.request.headers.number1;
                const number2 = ctx.request.headers.number2;
                const add = function(a, b) {
                    return a + b;
                }
                """;
        var result = _expressionTemplateWithScriptRenderer.Render(template);
        Console.WriteLine(result);
        Assert.True(true);
    }
}
