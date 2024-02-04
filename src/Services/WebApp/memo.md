### Docker Commands

Build an image from a Dockerfile
```bash
docker build -t <image_name> <path_to_dockerfile> <build_context>
```

example:
Build WebApp.RestApi image
```bash
docker build -t webapp-restapi -f WebApp.RestApi/Dockerfile .
```
Build WebApp.ApiGateway image
```bash
docker build -t webapp-apigateway -f WebApp.MiniApiGateway/Dockerfile .
```

Tag an image
```bash
docker tag <image_id> <image_name>:<tag>
```

example:
```bash
docker tag 1234567890 my_image:latest
```