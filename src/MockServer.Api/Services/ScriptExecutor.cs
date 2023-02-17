using Jint;

namespace MockServer.Api.Services;

public class ScriptExecutor : IScriptExecutor
{
    private readonly Engine _engine;
    public ScriptExecutor()
    {
        
    }
    public ScriptExecutor(Engine engine)
    {
        _engine = engine;
    }
    public void Execute(string script)
    {
        _engine.Execute(script);
    }
}
