using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using Jint;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;

public class ExpressionTemplateWithScriptRenderer : IExpressionTemplateWithScriptRenderer
{
    public string Render(object ctx, string template, string script)
    {
        var engine = new Engine();
        engine.SetValue("ctx", ctx);
        engine.Execute(script);
        string pattern = @"@{(.*?)}";
        RegexOptions options = RegexOptions.Singleline;
        string output = Regex.Replace(template, pattern, (match) =>
        {
            if (match.Success)
            {
                return engine.Execute(match.Groups[1].Value).GetCompletionValue().ToString();
            }
            else
            {
                return "";
            }
        }, options);
        return output;
    }
}
