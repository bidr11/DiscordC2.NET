// See https://aka.ms/new-console-template for more information


using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordC2.Common;
using DiscordC2.Init;
using DiscordC2.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program {
    private bool DEBUG = true;
    private DiscordShardedClient _client;
    private CommandService _textCommands;
    private InteractionService _commands;
    private readonly IConfigurationRoot _config;
    private ulong _testGuildId;

    public static Task Main(string[] args) => new Program().MainAsync();

    public Program() {
        var config = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json")
        .AddEnvironmentVariables()
        .Build();


        Bootstrapper.Init();

        var client = Bootstrapper.ServiceProvider.GetRequiredService<DiscordShardedClient>();
        var commands = Bootstrapper.ServiceProvider.GetRequiredService<InteractionService>();
        var textCommands = Bootstrapper.ServiceProvider.GetRequiredService<CommandService>();

        Bootstrapper.RegisterInstance(config);
        Bootstrapper.RegisterInstance(client);
        Bootstrapper.RegisterInstance(commands);
        Bootstrapper.RegisterInstance(textCommands);
        Bootstrapper.RegisterType<ICommandHandler, CommandHandler>();

        _commands = commands;
        _client = client;
        _textCommands = textCommands;
        _config = config;
        
    }

    public async Task MainAsync()
    {

        _client.ShardReady += async shard =>
        {
            await Logger.Log(LogSeverity.Info, "ShardReady", $"Shard Number {shard.ShardId} is connected and ready_!");
            if (DEBUG)
            {
                _testGuildId = ulong.Parse(_config.GetRequiredSection("Settings")["guildId"]);
                // this is where you put the id of the test discord guild
                Console.WriteLine($"In debug mode, adding commands to {_testGuildId}...");
                await _commands.RegisterCommandsToGuildAsync(_testGuildId);
            }
            else
            {
                // this method will add commands globally, but can take around an hour
                await _commands.RegisterCommandsGloballyAsync(true);
            }
        };


        var token = _config.GetRequiredSection("Settings")["DiscordBotToken"];
        if (string.IsNullOrWhiteSpace(token))
        {
            await Logger.Log(LogSeverity.Error, $"{nameof(Program)} | {nameof(MainAsync)}", "Token is null or empty.");
            return;
        }
            
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Bootstrapper.ServiceProvider.GetRequiredService<ICommandHandler>().InitializeAsync();
        
        await Task.Delay(Timeout.Infinite);
    }
}



