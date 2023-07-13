FROM builder-base:latest AS build
#WORKDIR /source
# copy csproj and restore as distinct layers
COPY ${CALLBACK_FILE} /ExpectionCallback.cs
RUN dotnet restore
# copy everything else and build app
RUN dotnet publish -c release -o app --no-restore
# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0.11-alpine3.16-amd64
WORKDIR /app
COPY --from=build /source/app .
EXPOSE 80
ENTRYPOINT ["dotnet", "MockServer.CallbackExecutor.dll"]