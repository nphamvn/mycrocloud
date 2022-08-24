using MockServer.Entities;

namespace MockServer.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext dbContext)
    {
        if (!dbContext.Users.Any())
        {
            var user = new User();
            user.Username = "npham";
            user.Workspaces = new List<Workspace> {
                new Workspace {
                    Name = "app1",
                    FriendlyName = "App1",
                    AccessScope = 0,
                    ApiKey = Guid.NewGuid().ToString(),
                    Requests = new List<Request>() {
                        new Request {
                            Name = "Request1",
                            Method = "GET",
                            Path = "foo",
                            Response = new Response
                            {
                                StatusCode = 200,
                                Body = "bar"
                            }
                        }
                    }
                },
                new Workspace {
                    Name = "app2",
                    FriendlyName = "App2",
                    Requests = new List<Request>() {
                        new Request {
                            Name = "Request1",
                            Method = "GET",
                            Path = "foo",
                            Response = new Response
                            {
                                StatusCode = 200,
                                Body = "bar"
                            }
                        }
                    }
                }
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }
    }
}