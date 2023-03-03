using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp;
using MockServer.Core.Functions;

namespace MockServer.Web.Controllers;

public class FunctionsController: BaseController
{
    public const string Name = "Functions";
    [HttpPost("test-function")]
    public async Task<IActionResult> TestFunction(IFormFile codeFile)
    {
        using (var reader = new StreamReader(codeFile.OpenReadStream()))
        {
            var code = await reader.ReadToEndAsync();
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters { GenerateInMemory = true };
            var results = provider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.HasErrors)
            {
                var errorList = string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>());
                return BadRequest($"Compilation errors:{Environment.NewLine}{errorList}");
            }
            var assembly = results.CompiledAssembly;
            var types = assembly.GetTypes();

            var functionType = types.FirstOrDefault(x => typeof(IFunction).IsAssignableFrom(x));
            if (functionType == null)
            {
                return BadRequest("Could not find a type that implements IFunction.");
            }
            return Ok();
        }
    }
}
