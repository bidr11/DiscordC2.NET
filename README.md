# DiscordC2.NET
The DiscordC2.NET is a bot-based C2 (Command and Control) tool made using C# that is designed to provide remote access and control over computer connections. This tool allows for you can perform various tasks and manage computers completely through Discord.

## Features
- Taking screenshots
- Remote code execution
- Downloading files
- Uploading files
- Machine identifiers to designate a specific machine (might need more work to avoid collisions)
- Pinging for alive machines
- Passing voice through discord VC
## Todo
- [ ] Smaller file size
- [ ] Keylogging
- [ ] Screen Recording
- [ ] Webcam

## Installation
### Setting up the server and the bot
To use DiscordGo, you need to create a Discord bot and a Discord server. After that, invite the bot to your server.

Click [here](https://support.discord.com/hc/en-us/articles/204849977-How-do-I-create-a-server-) to learn how to create a server and [here](https://discordjs.guide/preparations/setting-up-a-bot-application.html#creating-your-bot) to create a bot and retrieve your token. And finally, [here](https://discordjs.guide/preparations/adding-your-bot-to-servers.html#bot-invite-links) to invite the bot to your server.
Put your Bot Token, Main usage channel ID, and testing server ID into `appsettings.conf`.

When creating the bot, you need it give it some permission. For testing, I gave the bot full administrative permission.

### Building the project
Use this command to compile the entire project into a standalone executable (`appsettings.conf` variables are not hardcoded in).
```powershell
dotnet.exe publish DiscordC2.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
```
