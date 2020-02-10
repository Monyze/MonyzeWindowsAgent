using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Net.NetworkInformation;

namespace MonyzeWindowsAgent
{
    class HardwareGetter
    {
        private Config config;

        private Entities.List cpuList = new Entities.List("cpu", "\t\t", Entities.BracketType.scCurly);
        private Entities.List hddList = new Entities.List("hdd", "\t\t");
        private Entities.List netList = new Entities.List("net", "\t\t", Entities.BracketType.scCurly);
        private Entities.Config.RAM ram = new Entities.Config.RAM(0, "\t\t");

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

                var driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                foreach (ManagementObject d in driveQuery.Get())
                {
                    var model = Convert.ToString(d.Properties["Model"].Value);
                    var size = Convert.ToInt64(d.Properties["Size"].Value) / 1000000000;
                    var logicals = new List<string>();

                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        var logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                        var logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
                        foreach (ManagementObject ld in logicalDriveQuery.Get())
                        {
                            var driveName = Convert.ToString(ld.Properties["Name"].Value);
                            logicals.Add(driveName);
                        }
                    }

                    hddList.Add(new Entities.Config.HDD(x++, model, size, logicals, "\t\t\t"));
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

            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            int x = 1;

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    string addr = "";
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            addr = ip.Address.ToString() ;
                            break; // get only the first address...
                        }
                    }

                    netList.Add(new Entities.Config.Net(x++, ni.Name, ni.Description, ni.Speed / 100000, addr, "\t\t\t"));
                }
            }
        }

        private void GetRAM()
        {
            var ramMeter = new RAMMeter();
            ramMeter.Do();

            ram.totalPh = ramMeter.statEX.ullTotalPhys;
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
