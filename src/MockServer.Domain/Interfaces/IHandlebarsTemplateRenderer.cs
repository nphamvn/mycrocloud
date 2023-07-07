using Jint;
namespace MockServer.Domain.Interfaces;

public interface IHandlebarsTemplateRenderer
{
    string Render(string source);
}
