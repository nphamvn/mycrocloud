using Jint;

namespace MockServer.Core.Interfaces;

public interface IExpressionTemplateWithScriptRenderer
{
    string Render(string source);
}
