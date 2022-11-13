#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MockServer.CallbackExecutor/MockServer.CallbackExecutor.csproj", "MockServer.CallbackExecutor/"]
RUN dotnet restore "MockServer.CallbackExecutor/MockServer.CallbackExecutor.csproj"
COPY . .
WORKDIR "/src/MockServer.CallbackExecutor"
RUN dotnet build "MockServer.CallbackExecutor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MockServer.CallbackExecutor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MockServer.CallbackExecutor.dll"]
