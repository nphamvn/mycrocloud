using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Extentions;
using MockServer.Core.Services;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Database;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;

namespace MockServer.Web.Controllers;

[GetAuthUserDatabaseId(RouteName.DatabaseName, RouteName.DatabaseId)]
public class DatabasesController: BaseController
{
    public const string Name = "Databases";
    private readonly IDatabaseWebService _databaseWebService;

    public DatabasesController(IDatabaseWebService databaseWebService)
    {
        _databaseWebService = databaseWebService;
    }
    public async Task<IActionResult> Index() {
        var vm = await _databaseWebService.GetIndexViewModel();
        return View("/Views/Database/Index.cshtml", vm);
    }

    [HttpGet("{DatabaseName}")]
    public async Task<IActionResult> View(int DatabaseId) {
        var vm = await _databaseWebService.GetViewModel(DatabaseId);
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
        if (!ModelState.IsValid)
        {
            return View("/Views/Database/Save.cshtml", model);
        }
        await _databaseWebService.Create(model);
        return RedirectToAction(nameof(View), Request.RouteValues.With(RouteName.DatabaseName, model.Name));
    }

    [HttpGet("{DatabaseName}/edit")]
    public async Task<IActionResult> Edit(int DatabaseId)
    {
        var vm = await _databaseWebService.GetViewModel(DatabaseId);
        return View("/Views/Database/Save.cshtml", vm);
    }
    
    [HttpPost("{DatabaseName}/edit")]
    public async Task<IActionResult> Edit(int DatabaseId, SaveDatabaseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var vm = await _databaseWebService.GetViewModel(DatabaseId);
            return View("/Views/Database/View.cshtml", vm);
        }
        await _databaseWebService.Edit(DatabaseId, model);
        return RedirectToAction(nameof(View), Request.RouteValues.With(RouteName.DatabaseName, model.Name));
    }

    [AjaxOnly]
    [HttpGet("{DatabaseName}/data")]
    public async Task<IActionResult> Data(int DatabaseId)
    {
        string data = await _databaseWebService.GetData(DatabaseId);
        return Ok(data);
    }

    [AjaxOnly]
    [HttpPost("{DatabaseName}/data")]
    public async Task<IActionResult> Data(int DatabaseId, string Data)
    {
        await _databaseWebService.SaveData(DatabaseId, Data);
        return Ok();
    }

    [HttpGet("{DatabaseName}/data/export")]
    public async Task<IActionResult> Download(string DatabaseName, int DatabaseId)
    {
        var data = await _databaseWebService.GetDataBinary(DatabaseId);
        return File(data, "application/json", DatabaseName + ".json");
    }

    [AjaxOnly]
    [HttpPost("{DatabaseName}/applications")]
    public async Task<IActionResult> ConfigureApplication(int DatabaseId, Service service, bool allowed) {
        await _databaseWebService.ConfigureApplication(DatabaseId, service, allowed);
        return Ok();
    }

    [HttpPost("{DatabaseName}/delete")]
    public async Task<IActionResult> Delete(int DatabaseId)
    {
        await _databaseWebService.Delete(DatabaseId);
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }
}
