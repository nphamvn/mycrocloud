using Jint;
namespace WebApplication.Domain.Interfaces;

public interface IHandlebarsTemplateRenderer
{
    string Render(string source);
}
