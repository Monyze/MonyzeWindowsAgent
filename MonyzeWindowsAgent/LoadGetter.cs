using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor.Hardware;
using System.Net.NetworkInformation;

namespace MonyzeWindowsAgent
{
    class LoadGetter
    {
        private Config config;

        private Entities.List cpuList = new Entities.List("cpu", "\t\t", Entities.BracketType.scCurly);
        private Entities.List hddList = new Entities.List("hdd", "\t\t", Entities.BracketType.scCurly);
        private Entities.List netList = new Entities.List("net", "\t\t", Entities.BracketType.scCurly);
        private Entities.Load.RAM ram;
        private Entities.Load.Widgets widgets = new Entities.Load.Widgets("\t");

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
                var temps = new Entities.List("temp", "\t\t\t\t", Entities.BracketType.scCurly);

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

                            if (name.Contains("CPU Total"))
                            {
                                widgets.cpuLoad = Convert.ToInt32(Convert.ToDouble(value));
                            }
                        }
                        else if (type.Contains("Temperature"))
                        {
                            string valueName = (name.Contains("Core") ? "core_" + (t++).ToString() : "total");
                            temps.Add(new Entities.Load.Core(valueName, Convert.ToInt32(Convert.ToDouble(value)), "\t\t\t\t\t"));

                            if (name.Contains("CPU Package"))
                            {
                                widgets.cpuTemp = Convert.ToInt32(Convert.ToDouble(value));
                            }
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
            widgets.disks.Clear();

            var searcherPhysicalDrives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            try
            {
                int x = 1;

                foreach (var physicalDrive in searcherPhysicalDrives.Get())
                {
                    Entities.List logicals = new Entities.List("ldisks", "\t\t\t\t");
                    Entities.List widgetLogicals = new Entities.List("ldisks", "\t\t\t\t");

                    var searcherDiskPartitions = new ManagementObjectSearcher("select * from Win32_DiskDriveToDiskPartition");
                    foreach (var diskPartition in searcherDiskPartitions.Get())
                    {
                        string deviceId = physicalDrive["deviceID"].ToString().TrimStart('\\');
                        deviceId = deviceId.TrimStart('.');
                        deviceId = deviceId.TrimStart('\\');

                        if (diskPartition["Antecedent"].ToString().Contains(deviceId))
                        {
                            string partitionId = diskPartition["Dependent"].ToString();

                            var searcherLogicalDisks = new ManagementObjectSearcher("select * from Win32_LogicalDiskToPartition");
                            foreach (var logicalDisk in searcherLogicalDisks.Get())
                            {
                                if (logicalDisk["Antecedent"].ToString().Contains(partitionId))
                                {
                                    string[] tokens = logicalDisk["Dependent"].ToString().Split('=');
                                    string ldisk = tokens[1].Trim('"');

                                    var searcherLogicalDiskParams = new ManagementObjectSearcher("select * from Win32_LogicalDisk");
                                    foreach (var logicalDiskParam in searcherLogicalDiskParams.Get())
                                    {
                                        if (logicalDiskParam["DeviceID"].ToString().Contains(ldisk))
                                        {
                                            double free = Convert.ToDouble(logicalDiskParam["FreeSpace"].ToString()) / 1000000000;
                                            double total = Convert.ToDouble(logicalDiskParam["Size"].ToString()) / 1000000000;
                                            double used = total - free;

                                            int load = 100 - (int)((free / total) * 100);

                                            logicals.Add(new Entities.Load.Logical(free, load, used, ldisk, "\t\t\t\t\t"));
                                            widgetLogicals.Add(new Entities.Load.WidgetLogical(load, ldisk, "\t\t\t\t\t"));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    hddList.Add(new Entities.Load.HDD(x, logicals, "\t\t\t"));
                    widgets.disks.Add(new Entities.Load.WidgetHDD(x, widgetLogicals, "\t\t\t"));
                    ++x;
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("HardwareGetter.GetHDDList :: " + exp.Message);
            }
        }

        private void GetNetLoad()
        {
            netList.Clear();

            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            int x = 1;

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //netList.Add(new Entities.Load.Net(x++, ni.GetIPv4Statistics().BytesReceived / 100000, "\t\t\t"));
                }
            }
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

            int ramLoad = 100 - (int)((ramAvailable / totalRamMb) * 100);

            ram = new Entities.Load.RAM(ramLoad, ramAvailable, "\t\t");

            widgets.ramLoad = ramLoad;
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
                ref ram,
                ref widgets);

            return load.Serialize();
        }
    }
}
