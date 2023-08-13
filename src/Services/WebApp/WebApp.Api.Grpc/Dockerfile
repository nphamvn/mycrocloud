FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Services/WebApplication/WebApp.Api.Grpc/WebApp.Api.Grpc.csproj", "Services/WebApplication/WebApp.Api.Grpc/"]
RUN dotnet restore "Services/WebApplication/WebApp.Api.Grpc/WebApp.Api.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/WebApplication/WebApp.Api.Grpc"
RUN dotnet build "WebApp.Api.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApp.Api.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApp.Api.Grpc.dll"]
