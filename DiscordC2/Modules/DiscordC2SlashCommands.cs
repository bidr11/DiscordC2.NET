using Discord.Interactions;

public class DiscordC2SlashCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    [SlashCommand("first-global-command", "sdfsdf")]
    public async Task FirstGlobalCommand()
    {
        await DeferAsync();
        await FollowupAsync($"first");
    }

    [SlashCommand("test", "sdfsdf")]
    public async Task Test()
    {
        await RespondAsync($"tsetsdfkshf");
    }
}