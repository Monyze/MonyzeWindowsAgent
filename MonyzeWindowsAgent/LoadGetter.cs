using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor.Hardware;

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

            Computer computerHardware = new Computer() { CPUEnabled = true };

            computerHardware.Open();
            var hardwareCount = computerHardware.Hardware.Count();
            for (var i = 0; i != hardwareCount; ++i)
            {
                var loads = new Entities.List("load", "\t\t\t\t", Entities.BracketType.scCurly);
                var temps = new Entities.List("temp", "\t\t\t\t");

                var subcount = computerHardware.Hardware[i].SubHardware.Count();

                // NEED Update to view Subhardware
                for (var j = 0; j != subcount; ++j)
                {
                    computerHardware.Hardware[i].SubHardware[j].Update();
                }
                
                var sensorcount = computerHardware.Hardware[i].Sensors.Count();

                int l = 1, t = 1;

                if (sensorcount > 0)
                {
                    for (var z = 0; z != sensorcount; ++z)
                    {
                        var name = computerHardware.Hardware[i].Sensors[z].Name;
                        var type = computerHardware.Hardware[i].Sensors[z].SensorType.ToString();
                        var value = computerHardware.Hardware[i].Sensors[z].Value.ToString();
                        
                        if (type.Contains("Load"))
                        {
                            string valueName = (name.Contains("Core") ? "core_" + (l++).ToString() : "total");
                            loads.Add(new Entities.Load.Core(valueName, Convert.ToInt32(Convert.ToDouble(value)), "\t\t\t\t\t"));
                        }
                        else if (type.Contains("Temperature"))
                        {
                            string valueName = (name.Contains("Core") ? "core_" + (t++).ToString() : "total");
                            temps.Add(new Entities.Load.Core(valueName, Convert.ToInt32(Convert.ToDouble(value)), "\t\t\t\t\t"));
                        }
                    }
                }

                cpuList.Add(new Entities.Load.CPU(i + 1, loads, temps, "\t\t\t"));
            }
            computerHardware.Close();
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
            var ramAvailable = Convert.ToInt64(ramAvailableGetter.NextValue());

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
