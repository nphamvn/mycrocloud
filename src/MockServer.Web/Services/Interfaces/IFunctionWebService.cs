using MockServer.Web.Models.Function;

namespace MockServer.Web.Services.Interfaces;

public interface IFunctionWebService
{
    Task<FunctionIndexViewModel> GetIndexViewModel();
    Task<FunctionSaveModel> GetCreateModel();
    Task Create(FunctionSaveModel function);
    Task<FunctionSaveModel> GetEditModel(int functionId);
    Task Edit(int functionId, FunctionSaveModel function);
    Task<FunctionViewModel> Get(int functionId);
    Task Delete(int functionId);
    Task<FunctionTestResult> Test(FunctionSaveModel function);
}
