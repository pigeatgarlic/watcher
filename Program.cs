// See https://aka.ms/new-console-template for more information

using LibreMonitor;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

var manager = new ProcessManager.ProcessManager();
var displays = Screen.Display.Query();


var gpu = new Monitor.GPUInfo();
await gpu.Fetch();
Console.WriteLine($"{gpu}");

var inforam = new Monitor.RamInfo();
await inforam.Fetch();
Console.WriteLine($"{inforam}");

var networkinfo = new Monitor.NetworkInfo();
await networkinfo.Fetch();
Console.WriteLine($"{networkinfo}");

var disk = new Monitor.DiskInfo();
await disk.Fetch();
Console.WriteLine($"{disk}");

var proc = new Monitor.ProcessorInfo();
await proc.Fetch();
Console.WriteLine($"{proc}");
// var monitor = new HWMonitor.HWMonitor();

// await ProcessManager.ProcessManager.renderProcessesOnListView();


// while (true)
// {
//     Console.WriteLine(monitor.Report());
//     Thread.Sleep(TimeSpan.FromSeconds(1));
// }

// // monitor.Monitor();

