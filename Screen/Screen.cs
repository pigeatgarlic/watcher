using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Screen
{
    class Display
    {
        public Rectangle _bounds;
        public Rectangle _workingArea;
        public string _name,_deviceId;


        public static List<Display> Query()
        {
            List<Display> list = new List<Display>();
            WinApi.MonitorEnumDelegate MonitorEnumProc = new WinApi.MonitorEnumDelegate((IntPtr hMonitor, IntPtr hdcMonitor, ref WinApi.RECT lprcMonitor, IntPtr dwData) => {
                WinApi.MONITORINFOEX mi = new WinApi.MONITORINFOEX() { Size = Marshal.SizeOf(typeof(WinApi.MONITORINFOEX)) };

                if (WinApi.GetMonitorInfo(hMonitor, ref mi))
                {
                    WinApi.DISPLAY_DEVICE device = new WinApi.DISPLAY_DEVICE();
                    device.Initialize();

                    if (WinApi.EnumDisplayDevices(mi.DeviceName.ToLPTStr(), 0, ref device, 0))
                    {
                        Display display = new Display()
                        {
                            _name = device.DeviceString,
                            _deviceId = mi.DeviceName,
                            _bounds=new Rectangle(mi.Monitor.Left,mi.Monitor.Top,mi.Monitor.Right-mi.Monitor.Left,mi.Monitor.Bottom-mi.Monitor.Top),
                            _workingArea = new Rectangle(mi.WorkArea.Left, mi.WorkArea.Top, mi.WorkArea.Right - mi.WorkArea.Left, mi.WorkArea.Bottom - mi.WorkArea.Top),
                        };
                        list.Add(display);
                    }
                }
                
                return true;
            });

            WinApi.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProc, IntPtr.Zero);
            return list;
        }
    }
    
    static class WinApi
    {
    #region DISPLAY_DEVICE struct
        [StructLayout(LayoutKind.Sequential)]
        internal struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public DisplayDeviceStateFlags StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;

            public void Initialize()
            {
                cb = 0;
                DeviceName = new string((char)32, 32);
                DeviceString = new string((char)32, 128);
                DeviceID = new string((char)32, 128);
                DeviceKey = new string((char)32, 128);
                cb = Marshal.SizeOf(this);
            }
        }
    #endregion

    #region RECT struct
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    #endregion

    #region MONITORINFOEX struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MONITORINFOEX
        {
            public int Size;
            public RECT Monitor;
            public RECT WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
        }
    #endregion

    #region DisplayDeviceStateFlags enum
        [Flags()]
        public enum DisplayDeviceStateFlags : int
        {
            /// <summary>The device is part of the desktop.</summary>
            AttachedToDesktop = 0x1,
            MultiDriver = 0x2,
            /// <summary>The device is part of the desktop.</summary>
            PrimaryDevice = 0x4,
            /// <summary>Represents a pseudo device used to mirror application drawing for remoting or other purposes.</summary>
            MirroringDriver = 0x8,
            /// <summary>The device is VGA compatible.</summary>
            VGACompatible = 0x10,
            /// <summary>The device is removable; it cannot be the primary display.</summary>
            Removable = 0x20,
            /// <summary>The device has more display modes than its output devices support.</summary>
            ModesPruned = 0x8000000,
            Remote = 0x4000000,
            Disconnect = 0x2000000,
        }
    #endregion

        public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("User32.dll")]
        internal static extern bool EnumDisplayDevices(byte[] lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);

        public static byte[] ToLPTStr(this string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
            lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }
    }
}