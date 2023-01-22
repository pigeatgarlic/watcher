using System.Diagnostics;
using System.Linq;

namespace ProcessManager
{
    public class ProcessManager {

        public async Task<List<ProcessModel>> renderProcessesOnListView()
        {           
            var ret = new List<ProcessModel>();

            var processList = Process.GetProcesses().ToList();          
            processList.ForEach(proc => {

            });




            return ret;
        }
    }
}
