namespace MockServer.Core.Interfaces;

public interface IExpressionTemplateWithScriptRenderer
{
    string Render(object ctx, string template, string script);
}
