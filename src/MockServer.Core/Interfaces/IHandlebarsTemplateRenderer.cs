using Jint;
namespace MockServer.Core.Interfaces;

public interface IHandlebarsTemplateRenderer
{
    string Render(string source);
}
