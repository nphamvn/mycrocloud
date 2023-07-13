using Jint;
using WebApplication.Domain.Interfaces;

namespace WebApplication.Domain.Services;

public class JintHandlebarsTemplateRenderer : IHandlebarsTemplateRenderer
{
    private readonly Engine _engine;
    private readonly string _handlebarsCode;
    public JintHandlebarsTemplateRenderer(Engine engine, string handlebarsCode)
    {
        _engine = engine;
        _handlebarsCode = handlebarsCode;
    }

    public string Render(string source)
    {
        _engine.Execute(_handlebarsCode);
        _engine.SetValue("source", source);
        _engine.Execute("let hb = {};");
        _engine.Execute("const template = Handlebars.compile(source);");
        var result = _engine.Execute("template(hb)");
        return result.ToString();
    }
}
