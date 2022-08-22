using MockServer.Models;

namespace MockServer.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.Users.Any())
        {
            var response = new Response();
            response.StatusCode = 200;
            response.Body = "Hi";

            var request = new Request();
            request.Name = "Request1";
            request.Method = "GET";
            request.Path = "foo";
            // request.Headers = new Dictionary<string, string>();
            // request.Headers["Content-Type"] = "application/json";
            // request.Headers["Accept"] = "*/*";
            request.Response = response;

            var workspace = new Workspace();
            workspace.Name = "testapi";
            workspace.FriendlyName = "Test Api";
            workspace.Requests = new List<Request>();
            workspace.Requests.Add(request);

            var user = new User();
            user.Username = "npham";
            user.Workspaces = new List<Workspace>();
            user.Workspaces.Add(workspace);

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}