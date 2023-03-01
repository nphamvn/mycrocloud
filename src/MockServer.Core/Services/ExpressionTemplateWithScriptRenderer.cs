using System.Text.RegularExpressions;
using Jint;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;

public class ExpressionTemplateWithScriptRenderer : IExpressionTemplateWithScriptRenderer
{
    private readonly Engine _engine;
    public ExpressionTemplateWithScriptRenderer(Engine engine)
    {
        _engine = engine;
    }

    public string Render(string source)
    {
        string pattern = @"@{(.*?)}";
        RegexOptions options = RegexOptions.Singleline;
        string output = Regex.Replace(source, pattern, (match) =>
        {
            if (match.Success)
            {
                return _engine.Execute(match.Groups[1].Value).ToString();
            }
            else
            {
                return "";
            }
        }, options);
        return output;
    }
}
