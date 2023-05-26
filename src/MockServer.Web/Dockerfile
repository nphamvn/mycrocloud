#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockServer.Web/MockServer.Web.csproj", "MockServer.Web/"]
COPY ["MockServer.Infrastructure/MockServer.Infrastructure.csproj", "MockServer.Infrastructure/"]
COPY ["MockServer.Core/MockServer.Core.csproj", "MockServer.Core/"]
RUN dotnet restore "MockServer.Web/MockServer.Web.csproj"
COPY . .
WORKDIR "/src/MockServer.Web"
RUN dotnet build "MockServer.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockServer.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockServer.Web.dll"]