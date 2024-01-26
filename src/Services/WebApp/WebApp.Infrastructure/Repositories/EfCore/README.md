### Command
#### 1. Add migration
```
dotnet ef migrations add <MigrationName> -s ../WebApp.RestApi/WebApp.RestApi.csproj -o Repositories/EfCore/Migrations
```
Example:
```
dotnet ef migrations add AddAppSettings -s ../WebApp.RestApi/WebApp.RestApi.csproj

```
#### 2. Update database
```
dotnet ef database update -s ../WebApp.RestApi/WebApp.RestApi.csproj
```

# Others
#### 1. Remove migration
```
dotnet ef migrations remove -s ../WebApp.RestApi/WebApp.RestApi.csproj
```

