These are the steps and commands I did to develop this demo.
Sorry for the undetailed instructions.

```powershell
dotnet new worker -o MinecraftServerBot
cd .\MinecraftServerBot\
```

```powershell
dotnet add package DSharpPlus
# docs for DSharpPlus https://dsharpplus.github.io/
dotnet add package DSharpPlus.SlashCommands --version 4.2.0
# docs for ShashCommands https://github.com/DSharpPlus/DSharpPlus/tree/master/DSharpPlus.SlashCommands  
```

Create Discord app at https://discord.com/developers/applications   
Enable as bot, generate OAuth2 link with bot scope and send message permissions and slash commands   
https://discord.com/api/oauth2/authorize?client_id=1018587971328409700&permissions=2147485696&scope=bot    

Enable Read Message Content intent    
https://support-dev.discord.com/hc/en-us/articles/4404772028055-Message-Content-Privileged-Intent-FAQ   

```powershell
dotnet add package Microsoft.Extensions.Azure
dotnet add package Microsoft.Extensions.Hosting
dotnet add package Azure.ResourceManager.Compute
```

```powershell
# create service principal, 
az ad sp create-for-rbac --name minecraft_developer
```

Configure SP credentials in user-secrets.

```powershell
# give principal permission to read resource group and manage VM

az acr create --name swimburgerdemossacr --resource-group demos `
              --sku Basic `
              --location eastus

az acr login --name swimburgerdemossacr

az acr build -r swimburgerdemossacr -t discord-server-bot-image:latest .


$AcrRegistryId=$(az acr show --name swimburgerdemossacr --query id --output tsv)

$SpPassword=$(az ad sp create-for-rbac --name acr-service-principal --scopes $AcrRegistryId --role acrpull --query password --output tsv)
$SpAppid=$(az ad sp list --display-name acr-service-principal --query [0].appId -o tsv)

az container create --resource-group demos `
                    --name discord-bot-container `
                    --image swimburgerdemossacr.azurecr.io/discord-server-bot-image:latest `
                    --registry-username $SpAppId `
                    --registry-password $SpPassword `
                    --secure-environment-variables DiscordBot__Token=$DiscordBotToken `
                    --environment-variables Minecraft__VirtualMachineName=minecraft `
                    --environment-variables Minecraft__ResourceGroupName=minecraft `
                    --location eastus
```