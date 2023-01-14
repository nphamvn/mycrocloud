namespace MockServer.Core.Interfaces;

public interface IHandlebarsTemplateRenderer
{
    public string Render(object ctx, string template, string script);
}
