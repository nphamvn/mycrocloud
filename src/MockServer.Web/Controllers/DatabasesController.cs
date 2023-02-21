using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.Database;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;

namespace MockServer.Web.Controllers;

public class DatabasesController: BaseController
{
    private readonly IDatabaseWebService _databaseWebService;

    public DatabasesController(IDatabaseWebService databaseWebService)
    {
        _databaseWebService = databaseWebService;
    }
    public async Task<IActionResult> Index() {
        var vm = await _databaseWebService.GetIndexViewModel();
        return View("/Views/Database/Index.cshtml", vm);
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> View(string Name) {
        var vm = await _databaseWebService.GetViewModel(Name);
        return View("/Views/Database/View.cshtml",vm);
    }
    
    [HttpGet("new")]
    public async Task<IActionResult> New()
    {
        var vm = new SaveDatabaseViewModel();
        return View("/Views/Database/Save.cshtml", vm);
    }
    [HttpPost("new")]
    public async Task<IActionResult> New(SaveDatabaseViewModel model)
    {
        await _databaseWebService.Create(model);
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _databaseWebService.GetViewModel(id);
        return View("/Views/Database/Save.cshtml", vm);
    }
    [HttpPost("edit/{id:int}")]
    public async Task<IActionResult> New(int id, SaveDatabaseViewModel model)
    {
        await _databaseWebService.Edit(id, model);
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }
}
