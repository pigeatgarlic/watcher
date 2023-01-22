using System;
using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json;

namespace HWMonitor {
    public class UpdateLibreVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
        }
        public void VisitSensor(ISensor sensor) { 
        }
        public void VisitParameter(IParameter parameter) { 
        }
    }




    public class HWMonitor {
        private Computer computer;
        public HWMonitor() {
            this.computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true,
                IsBatteryEnabled = true,
                IsPsuEnabled = true,
            };
            computer.Open();
            computer.Accept(new UpdateLibreVisitor());
        }

        ~HWMonitor(){
            computer.Close();
        }

        public string Report()
        {
            computer.Reset();
            return computer.GetReport();
        }
    }
}