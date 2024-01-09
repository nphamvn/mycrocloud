### Command
#### 1. Add migration
```
dotnet ef migrations add <MigrationName> -s ../WebApp.Api.Rest/WebApp.Api.Rest.csproj -o Repositories/EfCore/Migrations
```
Example:
```
dotnet ef migrations add EditFunctionExecutionDuration -s ../WebApp.Api.Rest/WebApp.Api.Rest.csproj -o Repositories/EfCore/Migrations

```
#### 2. Update database
```
dotnet ef database update -s ../WebApp.Api.Rest/WebApp.Api.Rest.csproj
```

# Others
#### 1. Remove migration
```
dotnet ef migrations remove -s ../WebApp.Api.Rest/WebApp.Api.Rest.csproj
```

