import docker

client = docker.from_env()

client.images.build(
    path = "/Users/nampham/workspaces/dotnet/mock-server/src/MockServer.CallbackExecutor",
    tag = "img-nampham-05:01",
    buildargs = {"CALLBACK_FILE":"/Users/nampham/workspaces/dotnet/mock-server/src/MockServer.CallbackExecutor/Callbacks/ExpectionCallback.cs"},
    nocache = False,
    rm = True,
    forcerm = True,
    squash = True
    )

# client.containers.run(
#     image = "builder:01",
#     detach = True,
#     name = "builder-nampham-01",
#     volumes = {'/Users/nampham/workspaces/dotnet/mock-server/src/MockServer.CallbackExecutor':
#         {'bind': '/source', 'mode': 'rw'}},
#     remove=True
#     )