using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.RestApi.Controllers;

namespace WebApp.RestApi;

[Route("apps/{appId:int}/[controller]")]
public class FilesController(AppDbContext appDbContext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List(int appId, int? folderId)
    {
        return Ok();
    }

    [HttpPost("folders")]
    public async Task<IActionResult> CreateFolder(int appId, CreateUpdateFolderModel createFolderModel)
    {
        return Created();
    }

    [HttpPut("folders/{id:int}")]
    public async Task<IActionResult> UpdateFolder(int appId, int id, CreateUpdateFolderModel createFolderModel)
    {
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int appId, int? fileId, int? folderId)
    {
        return NoContent();
    }
}

public class FileFolderItem {
    public string Type { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
}
public class CreateUpdateFolderModel
{
    public string? Name { get; set; }
}