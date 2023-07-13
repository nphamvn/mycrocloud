using Jint;

namespace WebApplication.Domain.Interfaces;

public interface IExpressionTemplateWithScriptRenderer
{
    string Render(string source);
}
