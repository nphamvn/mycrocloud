Build Web image
docker build -t mock-server-web -f Dockerfile.Web .
Build Api image
docker build -t mock-server-api -f Dockerfile.Api .