

using System.Diagnostics;
using Newtonsoft.Json;

namespace Utils
{
    class Task {
        public static async Task<T?> RunTaskAndDecodeJson<T>(string powershell) {
            var proc = new Process{
                StartInfo = new ProcessStartInfo {
                FileName = "powershell.exe",
                Arguments = $"{powershell} | ConvertTo-Json",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }};

            var result = "";
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                string? line = proc.StandardOutput.ReadLine();
                result += line != null ? line : "";
            }

            return JsonConvert.DeserializeObject<T>(result);
        }
    }
    
}
