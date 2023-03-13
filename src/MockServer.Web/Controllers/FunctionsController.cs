using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.Function;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Controllers;

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
        return View(model);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var model = await _functionWebService.GetCreateModel();
        return View(model);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(FunctionSaveModel function)
    {
        await _functionWebService.Create(function);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{FunctionId:int}")]
    public async Task<IActionResult> Edit(int FunctionId)
    {
        var model = await _functionWebService.GetEditModel(FunctionId);
        return View(model);
    }

    [HttpPost("edit/{FunctionId:int}")]
    public async Task<IActionResult> Edit(int FunctionId, FunctionSaveModel function)
    {
        await _functionWebService.Edit(FunctionId, function);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("test-function")]
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
