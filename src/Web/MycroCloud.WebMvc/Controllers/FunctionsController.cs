using MicroCloud.Web.Models.Function;
using MicroCloud.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MicroCloud.Web.Controllers;

public class FunctionsController: BaseController
{
    private readonly IFunctionWebService _functionWebService;
    public FunctionsController(IFunctionWebService functionWebService)
    {
        _functionWebService = functionWebService;
    }

    [HttpGet]
    public async Task<IActionResult> Index() {
        var model = await _functionWebService.GetIndexViewModel();
        return View("/Views/Functions/Index.cshtml", model);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var model = await _functionWebService.GetCreateModel();
        return View("/Views/Functions/Save.cshtml", model);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(FunctionSaveModel function)
    {
        if (!ModelState.IsValid)
        {
            return View("/Views/Functions/Save.cshtml", function);;
        }
        await _functionWebService.Create(function);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{FunctionId:int}")]
    public async Task<IActionResult> Edit(int FunctionId)
    {
        var model = await _functionWebService.GetEditModel(FunctionId);
        return View("/Views/Functions/Save.cshtml", model);
    }

    [HttpPost("edit/{FunctionId:int}")]
    public async Task<IActionResult> Edit(int FunctionId, FunctionSaveModel function)
    {
        await _functionWebService.Edit(FunctionId, function);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("test")]
    public async Task<IActionResult> TestFunction(IFormFile codeFile)
    {
        using (var reader = new StreamReader(codeFile.OpenReadStream()))
        {
            var code = await reader.ReadToEndAsync();
            var function = new FunctionSaveModel
            {
                Code = code
            };
            var result = await _functionWebService.Test(function);
            return Ok(result);
        }
    }
}
