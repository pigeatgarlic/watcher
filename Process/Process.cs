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
            Task.Run(() => { HandleWindowRect(); }); 
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

                    if (!found.Contains(process.Id)) {
                        found.Add(process.Id);
                    }

                    _processIDs[process.ProcessName] = found;
                }


                var rm_keys = new List<string>();
                foreach (var item in _processIDs) {
                    item.Value.RemoveAll(x=> !processList.Where(y => y.Id == x).Any() );
                    if (!processList.Where(x => x.ProcessName == item.Key).Any()) {
                        rm_keys.Add(item.Key);
                    }
                }

                rm_keys.ForEach(x=>{ _processIDs.Remove(x, out var discard); });
            }
        }


        private async Task HandleWindow()
        {
            while(true)
            {
                List<IntPtr> windows = new List<IntPtr>();
                EnumWindows(delegate(IntPtr wnd, IntPtr param) { windows.Add(wnd); return true; }, IntPtr.Zero);
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

                var rm_keys = new List<int>();
                foreach (var item in _processWindows) {
                    item.Value.RemoveAll(x=> !windows.Where(y => y == x).Any() );
                    if (!_processIDs.Where(x => x.Value.Where(y => y == item.Key).Any()).Any()) {
                        rm_keys.Add(item.Key);
                    }
                }

                rm_keys.ForEach(x=>{ _processWindows.Remove(x, out var discard); });
            }
        }


        private async Task HandleWindowRect()
        {
            while(true)
            {
                foreach (var item in _processWindows) {
                    foreach (var window in item.Value) {
                        _windows.TryGetValue(window, out var rect);
                        rect = GetWindowPosition(window);
                        _windows[window] = rect;
                    }
                }

                var rm_keys = new List<nint>();
                foreach (var item in _windows) {
                    if (!_processIDs.Where(x => x.Value.Where(y => y == item.Key).Any()).Any()) {
                        rm_keys.Add(item.Key);
                    }
                }

                rm_keys.ForEach(x=>{ _windows.Remove(x, out var discard); });
            }
        }

        public string FindWindowProcessname(nint windowID) {
            var id = _processWindows.Where(y => y.Value.Contains(windowID)).First().Key;
            return _processIDs.Where(x => x.Value.Contains(id)).First().Key;
        }

        public Dictionary<string,List<Rect>> FindProcessWindow() {
            var ret = new Dictionary<string, List<Rect>>();
            foreach (var item in _windows) {
                var name = FindWindowProcessname(item.Key);
                ret.TryGetValue(name, out var rects);
                if (rects == null) {
                    rects = new List<Rect>();
                }

                rects.Add(item.Value);
                ret[name] = rects;
            }

            return ret;
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
