using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyzeWindowsAgent
{
    class LoadGetter
    {
        private Config config;

        private Entities.List cpuList = new Entities.List("cpu", "\t\t", Entities.BracketType.scCurly);
        private Entities.List hddList = new Entities.List("hdd", "\t\t", Entities.BracketType.scCurly);
        private Entities.List netList = new Entities.List("net", "\t\t", Entities.BracketType.scCurly);
        private Entities.Load.RAM ram;

        public LoadGetter(ref Config config_)
        {
            config = config_;
        }

        private void GetCPULoad()
        {
            
        }

        private void GetHDDLoad()
        {
            
        }

        private void GetNetLoad()
        {
            
        }

        private void GetRAMLoad()
        {
            
        }

        private void GetWidgets()
        {
            
        }

        public string GetComputerLoad()
        {
            GetCPULoad();
            GetHDDLoad();
            GetNetLoad();
            GetRAMLoad();

            var load = new Entities.Load.LoadSummary(config.userId,
                config.deviceId,
                ref cpuList,
                ref hddList,
                ref netList,
                ref ram);

            return load.Serialize();
        }
    }
}
