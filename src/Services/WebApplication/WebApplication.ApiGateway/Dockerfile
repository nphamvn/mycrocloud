#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockServer.ApiGateway/MockServer.ApiGateway.csproj", "MockServer.ApiGateway/"]
COPY ["MockServer.Core/MockServer.Core.csproj", "MockServer.Core/"]
COPY ["MockServer.Infrastructure/MockServer.Infrastructure.csproj", "MockServer.Infrastructure/"]
RUN dotnet restore "MockServer.ApiGateway/MockServer.ApiGateway.csproj"
COPY . .
WORKDIR "/src/MockServer.ApiGateway"
RUN dotnet build "MockServer.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockServer.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockServer.ApiGateway.dll"]