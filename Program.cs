// See https://aka.ms/new-console-template for more information

using HWMonitor;



var monitor = new HWMonitor.HWMonitor();

while (true)
{
    Console.WriteLine(monitor.Report());
    Thread.Sleep(TimeSpan.FromSeconds(1));
}

// monitor.Monitor();

