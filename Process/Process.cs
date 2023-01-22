using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
