using MicroCloud.Web.Models.Function;

namespace MicroCloud.Web.Services.Interfaces;

public interface IFunctionWebService
{
    Task<FunctionIndexViewModel> GetIndexViewModel();
    Task<FunctionSaveModel> GetCreateModel();
    Task Create(FunctionSaveModel function);
    Task<FunctionSaveModel> GetEditModel(int functionId);
    Task Edit(int functionId, FunctionSaveModel function);
    Task Delete(int functionId);
    Task<FunctionTestResult> Test(FunctionSaveModel function);
}
