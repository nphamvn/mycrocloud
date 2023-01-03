using MockServer.Core.Services;

namespace MockServer.UnitTest.CoreServiceTests;

public class TemplateUtilityTest
{
    private TemplateUtility _templateUtility;
    public TemplateUtilityTest()
    {
        _templateUtility = new TemplateUtility();
    }
    [Fact]
    public void Should_Return_Expression()
    {
        string template =
                """
                {
                    "message": "the sum of @{ctx.request.headers.number1} and @{ctx.request.headers.number2} is
                    @{add(ctx.request.headers.number1, ctx.request.headers.number2)}"
                }
                """;
        string script =
                """
                const add = function(a, b) {
                    return a + b;
                }
                """;
        var expressions = _templateUtility.GetExpressions(template);
        Assert.Equal(3, expressions.Count);
    }

    [Fact]
    public void Print_Expressions()
    {
        string template =
                """
                {
                    "message": "the sum of @{ctx.request.headers.number1} and @{ctx.request.headers.number2} is
                    @{add(ctx.request.headers.number1, ctx.request.headers.number2)}"
                }
                """;
        var expressions = _templateUtility.GetExpressions(template);
        foreach (var exp in expressions)
        {
            Console.WriteLine($"{exp.Key}: {exp.Value}");
        }
        Assert.Equal(3, expressions.Count);
    }
}
