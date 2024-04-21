using System.Data;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Api.Auth0;

public class Auth0UserService(
    IConfiguration configuration,
    IServiceProvider serviceProvider,
    ILogger<Auth0UserService> logger)
    : IHostedService, IDisposable
{
    private ManagementApiClient? _client;
    private Timer? _timer;
    
    private async Task<GetAccessTokenResponse> GetAccessToken()
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri($"{configuration["Auth0ManagementAPIM2M:Domain"]}/oauth/token");
        request.Method = HttpMethod.Post;

        var bodyString = JsonSerializer.Serialize(new GetAccessTokenRequest
        {
            client_id = configuration["Auth0ManagementAPIM2M:ClientId"]!,
            client_secret = configuration["Auth0ManagementAPIM2M:ClientSecret"]!,
            audience = configuration["Auth0ManagementAPIM2M:Audience"]!
        });
        request.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GetAccessTokenResponse>(result)!;
    }

    private async Task<Job> CreateExportUsersJob()
    {
        var job = await _client!.Jobs.ExportUsersAsync(new UsersExportsJobRequest
        {
            Format = UsersExportsJobFormat.CSV,
            Fields = new List<UsersExportsJobField>
            {
                new() { Name = "user_id" },
                new() { Name = "name" },
                new() { Name = "picture" },
            }
        });

        while (true)
        {
            job = await _client.Jobs.GetAsync(job.Id);
            
            if (job.Status == "completed")
            {
                break;
            }
            
            await Task.Delay(1000);
        }
        
        return job;
    }

    private async void DoWork(object? state)
    {
        var accessToken = (await GetAccessToken()).access_token;
        
        _client = new ManagementApiClient(accessToken, new Uri($"{configuration["Auth0ManagementAPIM2M:Domain"]}/api/v2")); 
        
        var job = await CreateExportUsersJob();
        
        var fileName = $"users_{job.Id}.csv.gz";
        var path = await DownloadAndSave(job.Location, "temp", fileName);
        var users = await ReadUsersFromFile(path);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var connection = dbContext.Database.GetDbConnection();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            const string createTempTableSql =
                """
                CREATE TEMP TABLE temp_users
                (
                  id TEXT PRIMARY KEY,
                  full_name TEXT,
                  picture TEXT
                );
                """;
            await connection.ExecuteAsync(createTempTableSql);

            //
            var npgsqlConnection = (NpgsqlConnection)connection;
            const string copySql = "COPY temp_users (id, full_name, picture) FROM STDIN (FORMAT BINARY)";
            await using (var writer = await npgsqlConnection.BeginBinaryImportAsync(copySql))
            {
                for (var i = 1; i < users.Rows.Count; i++)
                {
                    var row = users.Rows[i];
                    await writer.StartRowAsync();
                    await writer.WriteAsync(row["id"], NpgsqlTypes.NpgsqlDbType.Text);
                    await writer.WriteAsync(row["full_name"], NpgsqlTypes.NpgsqlDbType.Text);
                    await writer.WriteAsync(row["picture"], NpgsqlTypes.NpgsqlDbType.Text);
                }

                await writer.CompleteAsync();
            }
            //

            const string upsertSql =
                """
                INSERT INTO "Users" ("Id", "FullName", "Picture")
                SELECT id, full_name, picture
                FROM temp_users
                ON CONFLICT ("Id") DO UPDATE
                    SET "FullName" = EXCLUDED."FullName", "Picture" = EXCLUDED."Picture";
                """;
            await connection.ExecuteAsync(upsertSql);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<DataTable> ReadUsersFromFile(string filePath)
    {
        var extractedFilePath = await ExtractFile(filePath);
        
        // Create a DataTable
        var dt = new DataTable();
        dt.Columns.Add("id", typeof(string));
        dt.Columns.Add("full_name", typeof(string));
        dt.Columns.Add("picture", typeof(string));
        
        // Read the file
        var lines = await File.ReadAllLinesAsync(extractedFilePath);

        foreach (var line in lines)
        {
            var values = line.Split(',')
                .Select(x => x.Trim('\"').TrimStart('\'').Replace("\"", string.Empty))
                .ToArray();

            // ReSharper disable once CoVariantArrayConversion
            dt.Rows.Add(values);
        }
        
        return dt;
    }

    private async Task<string> ExtractFile(string filePath)
    {
        // Extract the file
        var extractedFilePath = Path.Combine("temp", Path.GetFileNameWithoutExtension(filePath));
        await using var fileStream = File.OpenRead(filePath);
        await using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
        await using var extractedFileStream = File.Create(extractedFilePath);
        await gzipStream.CopyToAsync(extractedFileStream);

        return extractedFilePath;
    }

    private async Task<Stream> GetFileStream(Uri fileUrl)
    {
        using var httpClient = new HttpClient();
        var fileStream = await httpClient.GetStreamAsync(fileUrl);
        return fileStream;
    }

    private async Task<string> DownloadAndSave(Uri sourceFile, string destinationFolder, string destinationFileName)
    {
        var fileStream = await GetFileStream(sourceFile);
        return await SaveStream(fileStream, destinationFolder, destinationFileName);
    }

    private async Task<string> SaveStream(Stream fileStream, string destinationFolder, string destinationFileName)
    {
        if (!Directory.Exists(destinationFolder))
            Directory.CreateDirectory(destinationFolder);

        var path = Path.Combine(destinationFolder, destinationFileName);

        await using var outputFileStream = new FileStream(path, FileMode.CreateNew);
        await fileStream.CopyToAsync(outputFileStream);

        return path;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Auth0UserService is starting.");
        
        _timer = new Timer(DoWork, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(5));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Auth0UserService is stopping.");
        
        _timer?.Change(Timeout.Infinite, 0);
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _client?.Dispose();
        _timer?.Dispose();
    }
}