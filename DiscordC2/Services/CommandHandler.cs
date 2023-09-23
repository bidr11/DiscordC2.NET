using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordC2.Common;
using DiscordC2.Init;

namespace DiscordC2.Services;

public class CommandHandler : ICommandHandler
{
    private readonly DiscordShardedClient _client;
    private readonly InteractionService _commands;
    private readonly CommandService _textCommands;

    public CommandHandler(
        DiscordShardedClient client, 
        InteractionService commands,
        CommandService textCommands)
    {
        _client = client;
        _commands = commands;
        _textCommands = textCommands;
    }

    public async Task InitializeAsync()
    {
        // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);
        await _textCommands.AddModulesAsync(Assembly.GetExecutingAssembly(), Bootstrapper.ServiceProvider);
        
        // Subscribe a handler to see if a message invokes a command.
        _client.InteractionCreated += HandleInteraction;
        _client.MessageReceived += HandleCommandAsync;

        _commands.SlashCommandExecuted += SlashCommandExecuted;

        _textCommands.CommandExecuted += async (optional, context, result) =>
        {
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            {
                // the command failed, let's notify the user that something happened.
                await context.Channel.SendMessageAsync($"error: {result}");
            }
        };

        _commands.SlashCommandExecuted += async (optional, context, result) =>
        {
            if (!result.IsSuccess && result.Error != InteractionCommandError.UnknownCommand)
            {
                // the command failed, let's notify the user that something happened.
                await context.Channel.SendMessageAsync($"error: {result}");
            }
        };

        foreach (var module in _commands.Modules)
        {
            await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | Commands", $"Module '{module.Name}' initialized.");
        }
        foreach (var module in _textCommands.Modules)
        {
            await Logger.Log(LogSeverity.Info, $"{nameof(CommandHandler)} | Commands", $"Module '{module.Name}' initialized.");
        }
    }
    
    private async Task HandleCommandAsync(SocketMessage arg)
    {
        // Bail out if it's a System Message.
        if (arg is not SocketUserMessage msg) 
            return;

        // We don't want the bot to respond to itself or other bots.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) 
            return;

        // Create a Command Context.
        var context = new ShardedCommandContext(_client, msg);
        
        var markPos = 0;
        if (msg.HasCharPrefix('!', ref markPos))
        {
            var result = await _textCommands.ExecuteAsync(context, markPos, Bootstrapper.ServiceProvider);
        }
    }

    private async Task HandleInteraction (SocketInteraction arg)
        {
            try
            {
                var ctx = new ShardedInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, Bootstrapper.ServiceProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    
    private Task SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                case InteractionCommandError.UnknownCommand:
                    // implement
                    break;
                case InteractionCommandError.BadArgs:
                    // implement
                    break;
                case InteractionCommandError.Exception:
                    // implement
                    break;
                case InteractionCommandError.Unsuccessful:
                    // implement
                    break;
                default:
                    break;
            }
        }

        return Task.CompletedTask;
    }
}

    