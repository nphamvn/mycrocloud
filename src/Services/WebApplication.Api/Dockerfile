FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/WebApplication.Api/WebApplication.Api.csproj", "Services/WebApplication.Api/"]
RUN dotnet restore "Services/WebApplication.Api/WebApplication.Api.csproj"
COPY . .
WORKDIR "/src/Services/WebApplication.Api"
RUN dotnet build "WebApplication.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApplication.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApplication.Api.dll"]
