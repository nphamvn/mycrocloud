Sync WebApp Protos from WebMvc To WebApp.Grpc

`cp Web/MycroCloud.WebMvc/Areas/Services/Services/Protos/* Services/WebApp/WebApp.Api.Grpc/Protos/`

**Build Web image**

`docker build -t mock-server-web -f Dockerfile.Web .`

**Build Api image**

`docker build -t mock-server-api -f Dockerfile.Api .`