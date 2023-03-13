using System.CodeDom.Compiler;
using Microsoft.CSharp;
using MockServer.Core.Functions;
using MockServer.Web.Models.Function;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class FunctionWebService : BaseWebService, IFunctionWebService
{
    public FunctionWebService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
    }

    public Task Create(FunctionSaveModel function)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int functionId)
    {
        throw new NotImplementedException();
    }

    public Task Edit(int functionId, FunctionSaveModel function)
    {
        throw new NotImplementedException();
    }

    public Task<FunctionViewModel> Get(int functionId)
    {
        throw new NotImplementedException();
    }

    public Task<FunctionSaveModel> GetCreateModel()
    {
        throw new NotImplementedException();
    }

    public Task<FunctionSaveModel> GetEditModel(int functionId)
    {
        throw new NotImplementedException();
    }

    public Task<FunctionIndexViewModel> GetIndexViewModel()
    {
        throw new NotImplementedException();
    }

    public async Task<FunctionTestResult> Test(FunctionSaveModel function)
    {
        var result = new FunctionTestResult();
        var provider = new CSharpCodeProvider();
        var parameters = new CompilerParameters { GenerateInMemory = true };
        var results = provider.CompileAssemblyFromSource(parameters, function.Code);
        if (results.Errors.HasErrors)
        {
            var errorList = string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>());
            result.Message = $"Compilation errors:{Environment.NewLine}{errorList}";
            return result;
        }
        var assembly = results.CompiledAssembly;
        var types = assembly.GetTypes();

        var functionType = types.FirstOrDefault(x => typeof(IFunction).IsAssignableFrom(x));
        if (functionType == null)
        {
            result.Message = "Could not find a type that implements IFunction.";
            return result;
        }
        result.Passed = true;
        return result;
    }
}
