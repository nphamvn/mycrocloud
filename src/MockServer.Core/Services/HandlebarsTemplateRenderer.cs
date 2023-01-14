using System.Dynamic;
using System.Text.RegularExpressions;
using Jint;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;

public class HandlebarsTemplateRenderer : IHandlebarsTemplateRenderer
{
    public string Render(object ctx, string source, string script)
    {
        var handlebars = File.ReadAllText("handlebars.min-v4.7.7.js");
        var engine = new Engine();
        engine.Execute(handlebars);
        dynamic data = new ExpandoObject();
        data.ctx = ctx;
        engine.SetValue("data", data);
        engine.SetValue("source", source);
        engine.Execute("const template = Handlebars.compile(source);");
        engine.Execute(script);
        var result = engine.Execute("template(data)").GetCompletionValue();
        return result.ToString();
    }
}
