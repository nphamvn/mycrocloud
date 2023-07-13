FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockServer.MockResponder/MockServer.MockResponder.csproj", "MockServer.MockResponder/"]
COPY ["MockServer.Core/MockServer.Core.csproj", "MockServer.Core/"]
COPY ["MockServer.Infrastructure/MockServer.Infrastructure.csproj", "MockServer.Infrastructure/"]
RUN dotnet restore "MockServer.MockResponder/MockServer.MockResponder.csproj"
COPY . .
WORKDIR "/src/MockServer.MockResponder"
RUN dotnet build "MockServer.MockResponder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockServer.MockResponder.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockServer.MockResponder.dll"]
