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
        engine.SetValue("ctx", ctx);
        engine.SetValue("source", source);
        engine.Execute("let hb = {};");
        engine.Execute(script);
        engine.Execute("const template = Handlebars.compile(source);");
        var result = engine.Execute("template(hb)").GetCompletionValue();
        return result.ToString();
    }
}
