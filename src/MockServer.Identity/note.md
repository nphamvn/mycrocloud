Doc
https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0

Run container command
docker run -it -dp 5001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="crypticpassword" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${HOME}/.aspnet/https:/https/ mock-server.identity-server

Deploy
1) Publish
dotnet publish -o publish

2) File transfer
scp -r publish/* nam@npham.me:workspaces/mock-server/identity-server

Create the service file

sudo nano /etc/systemd/system/mockserver-identity.service

[Unit]
Description=Example .NET Web API App running on Linux

[Service]
WorkingDirectory=/home/nam/workspaces/mock-server/identity-server
ExecStart=/usr/bin/dotnet /home/nam/workspaces/mock-server/identity-server/MockServer.Identity.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-example
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target

Save the file and enable the service.
sudo systemctl enable mockserver-identity.service

Start the service and verify that it's running.
sudo systemctl start mockserver-identity.service
sudo systemctl status mockserver-identity.service

View logs
sudo journalctl -fu mockserver-identity.service