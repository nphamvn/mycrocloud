using Jint;

namespace MockServer.Domain.Interfaces;

public interface IExpressionTemplateWithScriptRenderer
{
    string Render(string source);
}
