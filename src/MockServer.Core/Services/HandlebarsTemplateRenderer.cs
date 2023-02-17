using System.Dynamic;
using System.Text.RegularExpressions;
using Jint;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;

public class HandlebarsTemplateRenderer : IHandlebarsTemplateRenderer
{
    private readonly Engine _engine;
    public HandlebarsTemplateRenderer()
    {

    }
    public HandlebarsTemplateRenderer(Engine engine)
    {
        _engine = engine;
    }

    public string Render(string source)
    {
        var engine = _engine ?? new Engine();
        var handlebars = File.ReadAllText("handlebars.min-v4.7.7.js");
        engine.Execute(handlebars);
        engine.SetValue("source", source);
        engine.Execute("let hb = {};");
        engine.Execute("const template = Handlebars.compile(source);");
        var result = engine.Execute("template(hb)").GetCompletionValue();
        return result.ToString();
    }
}
