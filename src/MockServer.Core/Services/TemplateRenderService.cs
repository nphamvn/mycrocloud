using System.Text.RegularExpressions;
using Jint;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;

public class HandlebarsTemplateRenderService : ITemplateRenderService
{
    public object EvaluateExpression(object source, string expression)
    {
        var engine = new Engine();
        engine.SetValue("ctx", new
        {
            request = new
            {
                headers = new Dictionary<string, string> {
                    {"Name", "Nam"}
                }
            }
        });
        return engine.Execute(expression).GetCompletionValue();
    }

    public List<string> Get(string text)
    {
        string pattern = "@{(.*?)}";
        MatchCollection matches = Regex.Matches(text, pattern);
        var ret = new List<string>();
        foreach (Match match in matches)
        {
            if (match.Success)
            {
                string script = match.Groups[1].Value;
                ret.Add(script);
            }
        }
        return ret;
    }

    public string Render(object data, string source)
    {
        var handlebars = File.ReadAllText("handlebars.min-v4.7.7.js");
        var engine = new Engine();
        engine.Execute(handlebars);
        engine.SetValue("data", data);
        engine.SetValue("source", source);
        engine.Execute("const template = Handlebars.compile(source);");
        var result = engine.Execute("template(data)").GetCompletionValue();
        return result.ToString();
    }
}
