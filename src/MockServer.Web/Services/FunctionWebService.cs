using System.CodeDom.Compiler;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CSharp;
using MockServer.Core.Functions;
using MockServer.Core.Repositories;
using MockServer.Web.Models.Function;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class FunctionWebService : BaseWebService, IFunctionWebService
{
    private readonly IFunctionRepository _functionRepository;
    private readonly IMapper _mapper;

    public FunctionWebService
                    (IHttpContextAccessor contextAccessor,
                    IFunctionRepository functionRepository,
                    IMapper mapper
                    ) : base(contextAccessor)
    {
        _functionRepository = functionRepository;
        _mapper = mapper;
    }

    public Task Create(FunctionSaveModel function)
    {
        var coreFunction = _mapper.Map<Core.Functions.Function>(function);
        return _functionRepository.Add(AuthUser.Id, coreFunction);
    }

    public Task Delete(int functionId)
    {
        return _functionRepository.Delete(functionId);
    }

    public Task Edit(int functionId, FunctionSaveModel function)
    {
        var coreFunction = _mapper.Map<Core.Functions.Function>(function);
        return _functionRepository.Update(functionId, coreFunction);
    }

    public async Task<FunctionSaveModel> GetCreateModel()
    {
        var model = new FunctionSaveModel();
        var runtimes = await _functionRepository.GetAvailableRuntimes();
        model.RuntimeSelectListItem = runtimes.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.Name
        });
        return model;
    }

    public async Task<FunctionSaveModel> GetEditModel(int functionId)
    {
        var function = await _functionRepository.Get(functionId);
        var model = _mapper.Map<FunctionSaveModel>(function);
        var runtimes = await _functionRepository.GetAvailableRuntimes();
        model.RuntimeSelectListItem = runtimes.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = r.Name
        });
        return model;
    }

    public async Task<FunctionIndexViewModel> GetIndexViewModel()
    {
        var model = new FunctionIndexViewModel();
        var functions = await _functionRepository.GetAll(AuthUser.Id);
        model.Functions = _mapper.Map<IEnumerable<FunctionIndexItem>>(functions);
        return model;
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

    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Core.Functions.Function, FunctionSaveModel>().ReverseMap();
            CreateMap<Core.Functions.Function, FunctionIndexItem>();
        }
    }
}
