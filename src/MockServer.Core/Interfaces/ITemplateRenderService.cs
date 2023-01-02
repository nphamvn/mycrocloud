namespace MockServer.Core.Interfaces;

public interface ITemplateRenderService
{
    public List<string> Get(string text);
    public object EvaluateExpression(object source, string expression);
    public string Render(object data, string source);
}
