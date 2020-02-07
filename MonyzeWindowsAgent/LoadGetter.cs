using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

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
            cpuList.Clear();

        }

        private void GetHDDLoad()
        {
            hddList.Clear();
        }

        private void GetNetLoad()
        {
            netList.Clear();
        }

        private void GetRAMLoad()
        {
            double totalRamMb = 0;

            var searcherComputerSystems = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            try
            {
                foreach (var computerSystem in searcherComputerSystems.Get())
                {
                    totalRamMb = Convert.ToInt64(computerSystem["TotalPhysicalMemory"].ToString()) / 1000000;
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("LoadGetter.GetRAMLoad :: " + exp.Message);
            }

            var ramAvailableGetter = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");
            double ramAvailable = ramAvailableGetter.NextValue();

            ram = new Entities.Load.RAM(100 - (int)((ramAvailable / totalRamMb) * 100), ramAvailable, "\t\t");
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
