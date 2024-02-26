FROM mcr.microsoft.com/dotnet/sdk:8.0.201
WORKDIR /source

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY WebApp.Domain/WebApp.Domain.csproj WebApp.Domain/
COPY WebApp.Infrastructure/WebApp.Infrastructure.csproj WebApp.Infrastructure/
COPY WebApp.Migrations/WebApp.Migrations.csproj WebApp.Migrations/

RUN dotnet restore WebApp.Migrations/WebApp.Migrations.csproj

COPY WebApp.Infrastructure/ WebApp.Infrastructure/
COPY WebApp.Domain/ WebApp.Domain/
COPY WebApp.Migrations/ WebApp.Migrations/

CMD [ "dotnet", "ef", "database", "update", "--project", "WebApp.Migrations/WebApp.Migrations.csproj"]