using System.Linq;

namespace Monitor
{
    class GPUInfo : RawCounter{
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\GPU Engine(*)\\*'");
            this.Timestamp = result.Timestamp;
            this.CounterSamples = result.CounterSamples;

            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\GPU Adapter Memory(*)\\*'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\GPU Process Memory(*)\\*'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            return;
        }
    }

    class RamInfo : RawCounter{
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Memory\\*'");
            this.Timestamp = result.Timestamp;
            this.CounterSamples = result.CounterSamples;
            return;
        }
    }
    class ProcessInfo : RawCounter{
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\% Processor Time'");
            this.CounterSamples = result.CounterSamples;
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\% User Time'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\% Privileged Time'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\IO Read Bytes/sec'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\IO Write Bytes/sec'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Process(*)\\Thread Count'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            this.Timestamp = result.Timestamp;
            return;
        }
    }
    class ProcessorInfo : RawCounter{
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Processor(*)\\% Processor Time'");
            this.CounterSamples = result.CounterSamples;
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Processor(*)\\% User Time'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Processor(*)\\% Privileged Time'");
            result.CounterSamples.ForEach(s => this.CounterSamples.Add(s));
            this.Timestamp = result.Timestamp;
            return;
        }
    }
    class DiskInfo : RawCounter{
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\PhysicalDisk(*)\\*'");
            this.Timestamp = result.Timestamp;
            this.CounterSamples = result.CounterSamples;
            return;
        }
    }

    class NetworkInfo : RawCounter {
        public async Task Fetch() {
            var result = await Utils.Task.RunTaskAndDecodeJson<RawCounter>("Get-Counter '\\Network Adapter(*)\\*'");
            this.Timestamp = result.Timestamp;
            this.CounterSamples = result.CounterSamples;
            return;
        }
    }


    class RawCounter {
        public DateTime Timestamp {get;set;}
        public List<CounterSamples> CounterSamples {get;set;}
    }

    public class CounterSamples
    {
        public string Path {get;set;}
        public string InstanceName {get;set;}
        public Int64 CookedValue {get;set;}
        public Int64 RawValue {get;set;}
        public Int64 SecondValue {get;set;}
        public Int64 MultipleCount {get;set;}
        public Int64 CounterType {get;set;}
        public Int64 Timestamp100NSec {get;set;}
        public Int64 Status {get;set;}
        public Int64 DefaultScale {get;set;}
        public Int64 TimeBase {get;set;}

        public DateTime Timestamp {get;set;}
    }
    
    
    
    
    
//     async Task<object> GetCPUInfo() {
//     var proc = new Process{
//         StartInfo = new ProcessStartInfo {
//         FileName = "powershell.exe",
//         Arguments = "(Get-WMIObject Win32_ComputerSystem) | ConvertTo-Json",
//         UseShellExecute = false,
//         RedirectStandardOutput = true,
//         CreateNoWindow = true
//     }};

//     var result = "";
//     proc.Start();
//     while (!proc.StandardOutput.EndOfStream)
//     {
//         string line = proc.StandardOutput.ReadLine();
//         result += line;
//     }

//     return JsonConvert.DeserializeObject<object>(result);
// }

// async Task<object> GetDiskInfo() {
//     var proc = new Process{
//         StartInfo = new ProcessStartInfo {
//         FileName = "powershell.exe",
//         Arguments = "(get-wmiobject -class win32_logicaldisk) | ConvertTo-Json",
//         UseShellExecute = false,
//         RedirectStandardOutput = true,
//         CreateNoWindow = true
//     }};

//     var result = "";
//     proc.Start();
//     while (!proc.StandardOutput.EndOfStream)
//     {
//         string line = proc.StandardOutput.ReadLine();
//         result += line;
//     }

//     return JsonConvert.DeserializeObject<object>(result);
// }


}

