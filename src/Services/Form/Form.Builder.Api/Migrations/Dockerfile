FROM mcr.microsoft.com/dotnet/sdk:8.0.202 AS build
WORKDIR /source

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY ["Form.Builder.Api.csproj", "./"]
RUN dotnet restore "Form.Builder.Api.csproj"
COPY . .

RUN dotnet ef migrations bundle -o /out/efbundle -v

FROM mcr.microsoft.com/dotnet/runtime:8.0.3
WORKDIR /db-migrator
COPY --from=build /out/efbundle .
COPY ["appsettings.json", "./"]
ENTRYPOINT [ "/db-migrator/efbundle" ]