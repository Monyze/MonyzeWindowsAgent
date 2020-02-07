using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace MonyzeWindowsAgent
{
    class HardwareGetter
    {
        private Config config;

        private Entities.List cpuList = new Entities.List("cpu", "\t\t", Entities.BracketType.scCurly);
        private Entities.List hddList = new Entities.List("hdd", "\t\t");
        private Entities.List netList = new Entities.List("net", "\t\t", Entities.BracketType.scCurly);
        private Entities.Config.RAM ram;

        public HardwareGetter(ref Config config_)
        {
            config = config_;
        }

        private string GetWindowsVersion()
        {
            var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            return name != null ? name.ToString() : "Unknown";
        }

        private void GetCPUList()
        {
            cpuList.Clear();

            var searcherCPUs = new ManagementObjectSearcher("select * from Win32_Processor");
            try
            {
                int x = 1;

                foreach (var cpu in searcherCPUs.Get())
                {
                    cpuList.Add(new Entities.Config.CPU(x++, cpu["Name"].ToString(), "\t\t\t"));
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("HardwareGetter.GetCPUList :: " + exp.Message);
            }
        }

        private void GetHDDList()
        {
            hddList.Clear();

            var searcherPhysicalDrives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            try
            {
                int x = 1;

                foreach (var physicalDrive in searcherPhysicalDrives.Get())
                {
                    List<string> logicals = new List<string>();

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

                                    string deviceLetter = tokens[1];

                                    logicals.Add(deviceLetter);
                                }
                            }
                        }
                    }

                    hddList.Add(new Entities.Config.HDD(x++, physicalDrive["Model"].ToString(), Convert.ToInt64(physicalDrive["Size"].ToString()) / 1000000000, logicals, "\t\t\t"));
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("HardwareGetter.GetHDDList :: " + exp.Message);
            }
        }

        private void GetNetList()
        {
            netList.Clear();

            var searcherNetworkAdapters = new ManagementObjectSearcher("select * from Win32_NetworkAdapter");
            try
            {
                int x = 1;

                foreach (var networkAdapter in searcherNetworkAdapters.Get())
                {
                    foreach (var prop in networkAdapter.Properties)
                    {
                        if (prop.Name == "MACAddress" && prop.Value != null)
                        {
                            string address = "";

                            string adapterGUID = networkAdapter["GUID"].ToString();

                            var searcherNetworkAdapterConfigurations = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
                            foreach (var networkAdapterConfiguration in searcherNetworkAdapterConfigurations.Get())
                            {
                                if (networkAdapterConfiguration["SettingID"].ToString().Contains(adapterGUID))
                                {
                                    string[] addresses = (string[])networkAdapterConfiguration["IPAddress"];
                                    if (addresses.Count() != 0)
                                    {
                                        address = addresses[0];
                                    }
                                }
                            }

                            netList.Add(new Entities.Config.Net(x++, networkAdapter["NetConnectionID"].ToString(), networkAdapter["Name"].ToString(), (Convert.ToInt64(networkAdapter["Speed"]) / 100000), address, "\t\t\t"));
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("HardwareGetter.GetNetList :: " + exp.Message);
            }
        }

        private void GetRAM()
        {
            Int64 ramSize = 0;

            var searcherComputerSystems = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            try
            {
                foreach (var computerSystem in searcherComputerSystems.Get())
                {
                    ramSize = Convert.ToInt64(computerSystem["TotalPhysicalMemory"].ToString());
                }
            }
            catch (Exception exp)
            {
                Logger.Log.Error("HardwareGetter.GetRAM :: " + exp.Message);
            }

            ram = new Entities.Config.RAM(ramSize, "\t\t");
        }

        public string GetComputerHardware()
        {
            GetCPUList();
            GetHDDList();
            GetNetList();
            GetRAM();

            var deviceConfig = new Entities.Config.ConfigSummary(config.userId,
                config.deviceId,
                Environment.MachineName,
                GetWindowsVersion(),
                (Environment.Is64BitOperatingSystem ? "64bit" : "32bit"),
                ref cpuList,
                ref hddList,
                ref netList,
                ref ram);

            return deviceConfig.Serialize();
        }
    }
}
