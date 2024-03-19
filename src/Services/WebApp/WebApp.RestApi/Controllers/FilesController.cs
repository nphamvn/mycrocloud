using System.Text;
using System.Text.Json;
using System.Transactions;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.RestApi.Controllers;
using WebApp.RestApi.Models.Files;

namespace WebApp.RestApi;

[Route("apps/{appId:int}/[controller]")]
public class FilesController(AppDbContext appDbContext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List(int appId, int? folderId)
    {
        var parentFolder = folderId != null ?
            await appDbContext.Folders.Where(f => f.AppId == appId && f.Id == folderId).SingleAsync()
            : await appDbContext.Folders.Where(f => f.AppId == appId && f.ParentId == null).SingleAsync();

        const string sql =
"""
SELECT 'Folder' AS "Type", f."Id", f."Name", f."CreatedAt", NULL AS "Size"
FROM "Folders" AS f
WHERE f."ParentId" = @FolderId
UNION
SELECT 'File' AS "Type", f0."Id", f0."Name", f0."CreatedAt", LENGTH(f0."Content") AS "Size"
FROM "Files" AS f0
WHERE f0."FolderId" = @FolderId
""";
        var items = await appDbContext.Database.GetDbConnection()
        .QueryAsync<FolderContent>(sql, new
        {
            FolderId = parentFolder.Id
        });

        const string sql2 =
"""
WITH RECURSIVE FolderPath AS (
  SELECT "Id", "ParentId", 1 AS depth
  FROM public."Folders"
  WHERE "Id" = @Id
  
  UNION ALL
  
  SELECT f."Id", f."ParentId", fp."depth" + 1
  FROM public."Folders" f
  INNER JOIN FolderPath fp ON f."Id" = fp."ParentId"
)
SELECT
     ROW_NUMBER() OVER (ORDER BY depth DESC) - 1 AS Depth,
     fp."Id",
     f."Name"
FROM FolderPath fp
INNER JOIN public."Folders" f ON f."Id" = fp."Id"
ORDER BY depth;
""";

        var pathItems = await appDbContext.Database.GetDbConnection()
        .QueryAsync<FolderPathItem>(sql2, new
        {
            Id = parentFolder.Id
        });
        Response.Headers.Append("Path-Items", JsonSerializer.Serialize(pathItems, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
        return Ok(items);
    }

    [HttpPost("folders")]
    public async Task<IActionResult> CreateFolder(int appId, int? folderId, CreateUpdateFolderModel createFolderModel)
    {
        var parentFolder = await appDbContext.Folders
                .Include(f => f.App)
                .Where(f => f.AppId == appId && ((folderId != null && f.Id == folderId) || (folderId == null && f.ParentId == null)))
                .SingleAsync();

        await appDbContext.Folders.AddAsync(new()
        {
            App = parentFolder.App,
            Name = createFolderModel.Name,
            Parent = parentFolder,
        });

        await appDbContext.SaveChangesAsync();
        return Created();
    }

    [HttpPatch("folders")]
    public async Task<IActionResult> RenameFolder(int appId, int folderId, CreateUpdateFolderModel renameFolderModel)
    {
        var folder = await appDbContext.Folders
                        .Where(f => f.AppId == appId && f.Id == folderId)
                        .SingleAsync();
        folder.Name = renameFolderModel.Name;

        await appDbContext.SaveChangesAsync();        
        return NoContent();
    }

    [HttpDelete("folders/{folderId:int}")]
    public async Task<IActionResult> Delete(int appId, int folderId)
    {
        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(int appId, int? folderId, IFormFile file)
    {
        var parentFolder = folderId != null ?
            await appDbContext.Folders.Where(f => f.AppId == appId && f.Id == folderId).SingleAsync()
            : await appDbContext.Folders.Where(f => f.AppId == appId && f.ParentId == null).SingleAsync();


        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);

            // Upload the file if less than 2 MB
            if (memoryStream.Length < 2097152)
            {
                await appDbContext.Files.AddAsync(new()
                {
                    Folder = parentFolder,
                    Name = file.FileName,
                    Content = memoryStream.ToArray()
                });

                await appDbContext.SaveChangesAsync();
            }
        }
        return NoContent();
    }
}

public class CreateUpdateFolderModel
{
    public string Name { get; set; }
}

public class FileUploadModel
{
    public IFormFile File { get; set; }
}