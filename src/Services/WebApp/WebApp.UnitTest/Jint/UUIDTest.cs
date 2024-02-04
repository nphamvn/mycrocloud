using Jint;

namespace WebApp.UnitTest;

public class UUIDTest
{

    [Test]
    public void ShouldGenerateUUIDUseRequire()
    {
        var engine = new Engine();
        var result = engine.Evaluate("const uuid = require('uuid'); uuid.v4();");
        Assert.IsTrue(Guid.TryParse(result.AsString(), out _));
    }

    [Test]
    public void ShouldGenerateUUIDUseImport()
    {
        var engine = new Engine();
        engine.Execute(File.ReadAllText("Scripts/uuid.js"));
        var result = engine.Evaluate("import { v4 as uuidv4 } from 'uuid'; uuidv4();");
        Assert.IsTrue(Guid.TryParse(result.AsString(), out _));
    }
}
