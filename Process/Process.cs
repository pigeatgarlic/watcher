using System.Diagnostics;
using System.Collections.Concurrent;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace ProcessManager
{
    public class ProcessManager {
        // group of process with the same name executable
        private readonly ConcurrentDictionary<string,List<int>> _processIDs = new ConcurrentDictionary<string, List<int>>();
        // map from ProcessID to WindowIDs
        private readonly ConcurrentDictionary<int,List<IntPtr>> _processWindows = new ConcurrentDictionary<int, List<IntPtr>>();


        // map from windowID to window position
        private readonly ConcurrentDictionary<IntPtr,Rect> _windows = new ConcurrentDictionary<IntPtr, Rect>();

        public ProcessManager() {
            Task.Run(() => { asyncProcess(); }); 
            Task.Run(() => { HandleWindow(); }); 
        }

        private async Task asyncProcess()
        {           
            while (true)
            {
                var processList = Process.GetProcesses().ToList();          
                foreach (var process in processList) {
                    _processIDs.TryGetValue(process.ProcessName, out var found);
                    if (found == null) {
                        found = new List<int>();
                    }

                    if (found.Contains(process.Id)) {
                        found.Add(process.Id);
                    }


                    _processIDs[process.ProcessName] = found;
                }


                foreach (var item in _processIDs) {
                    item.Value.RemoveAll(x=> !processList.Where(y => y.Id == x).Any() );

                    if (!processList.Where(x => x.ProcessName == item.Key).Any()) {
                        _processIDs.Remove(item.Key, out var discard);
                    }
                }
            }
        }


        private async Task HandleWindow()
        {
            while(true)
            {
                IntPtr found = IntPtr.Zero;
                List<IntPtr> windows = new List<IntPtr>();

                EnumWindows(delegate(IntPtr wnd, IntPtr param)
                {
                    windows.Add(wnd);

                    // but return true here so that we iterate all windows
                    return true;
                }, IntPtr.Zero);

                foreach (var item in windows) {
                    GetWindowThreadProcessId(item,out var procID);
                    _processWindows.TryGetValue((int)procID, out var IDs);
                    if (IDs == null) {
                        IDs = new List<IntPtr>();
                    }
                    if (!IDs.Contains(item)) {
                        IDs.Add(item);
                    }

                    _processWindows[(int)procID] = IDs;
                }
            }
        }


        public Rect GetWindowPosition(IntPtr id) {
            Rect rect = new Rect();
            GetWindowRect(id,ref rect);
            return rect;
        }

        // TODO
        // public Rect GetProcessInfo(int id) {
        // }


        [DllImport("user32.dll", SetLastError=true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    }


    public struct Rect {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
}
