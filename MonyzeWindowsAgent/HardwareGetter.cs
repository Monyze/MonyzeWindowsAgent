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
        private Config config = new Config();

        private string GetCPUList()
        {
            string output = "\t\t\"cpu\":{";

            var searcherCPUs = new ManagementObjectSearcher("select * from Win32_Processor");
            try
            {
                int x = 1;

                foreach (var cpu in searcherCPUs.Get())
                {
                    output += "\r\n\t\t\t\"cpu_" + (x++).ToString() + "\":\"" + cpu["Name"] + "\",";
                }
            }
            catch (Exception)
            {
                //todo: logger => ("can't get data because of the followeing error \n" + exp.Message);
            }

            output = output.TrimEnd(',');

            return output + "\r\n\t\t}";
        }

        private string GetHDDList()
        {
            string output = "\t\t\"hdd\":[";

            var searcherPhysicalDrives = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            try
            {
                int x = 1;

                foreach (var physicalDrive in searcherPhysicalDrives.Get())
                {
                    output += "\r\n\t\t\t{\r\n\t\t\t\t\"hdd_" + (x++).ToString() + "\":{" +
                    "\r\n\t\t\t\t\t\"name\":\"" + physicalDrive["Model"] + "\"," +
                    "\r\n\t\t\t\t\t\"size\":" + (Convert.ToInt64(physicalDrive["Size"]) / 1000000000).ToString() + "," +
                    "\r\n\t\t\t\t\t\"LOGICAL\":[";

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

                                    output += "\r\n\t\t\t\t\t\t" + deviceLetter + ",";
                                }
                            }
                        }
                    }

                    output = output.TrimEnd(',');

                    output += "\r\n\t\t\t\t\t]" +
                        "\r\n\t\t\t\t}\r\n\t\t\t},";
                }
            }
            catch (Exception)
            {
                //todo: logger => ("can't get data because of the followeing error \n" + exp.Message);
            }

            output = output.TrimEnd(',');

            return output + "\r\n\t\t]";
        }

        private string GetNetList()
        {
            string output = "\t\t\"net\":[";

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
                            output += "\r\n\t\t\t{\r\n\t\t\t\t\"net_" + (x++).ToString() + "\":{" +
                            "\r\n\t\t\t\t\t\"name\":\"" + networkAdapter["NetConnectionID"] + "\"," +
                            "\r\n\t\t\t\t\t\"model\":\"" + networkAdapter["Name"] + "\"," +
                            "\r\n\t\t\t\t\t\"speed\":" + (Convert.ToInt64(networkAdapter["Speed"]) / 100000).ToString();

                            string adapterGUID = networkAdapter["GUID"].ToString();

                            var searcherNetworkAdapterConfigurations = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
                            foreach (var networkAdapterConfiguration in searcherNetworkAdapterConfigurations.Get())
                            {
                                if (networkAdapterConfiguration["SettingID"].ToString().Contains(adapterGUID))
                                {
                                    string[] addresses = (string[])networkAdapterConfiguration["IPAddress"];
                                    if (addresses.Count() != 0)
                                    {
                                        output += ",\r\n\t\t\t\t\t\"addr\":\"" + addresses[0] + "\"";
                                    }
                                }
                            }

                            output += "\r\n\t\t\t\t}\r\n\t\t\t},";
                        }
                    }
                }
            }
            catch (Exception)
            {
                //todo: logger => ("can't get data because of the followeing error \n" + exp.Message);
            }

            output = output.TrimEnd(',');

            return output + "\r\n\t\t]";
        }

        private string GetRAM()
        {
            string output = "\t\t\"ram\":{";

            var searcherComputerSystems = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            try
            {
                foreach (var computerSystem in searcherComputerSystems.Get())
                {
                    output += "\r\n\t\t\t\"TotalPh\":" + computerSystem["TotalPhysicalMemory"] + "";
                }
            }
            catch (Exception)
            {
                //todo: logger => ("can't get data because of the followeing error \n" + exp.Message);
            }

            return output + "\r\n\t\t}";
        }

        public string GetComputerHardware()
        {
            return "{\r\n" +
                "\t\"id\": {\r\n" +
                "\t\t\"user_id\": \"" + config.userId + "\",\r\n" +
                "\t\t\"device_id\": \"" + config.deviceId + "\"\r\n\t},\r\n" +
                "\t\"state\": \"config\",\r\n" +
                "\t\"device_config\": {\r\n" +
                "\t\t\"device_name\": \"" + Environment.MachineName + "\",\r\n" +
                "\t\t\"system\": \"" + Environment.OSVersion.ToString() + "\",\r\n" +
                "\t\t\"bits\": \"" + (Environment.Is64BitOperatingSystem ? "64bit" : "32bit") + "\",\r\n" +
                "\t\t\"icon\": \"f17a\",\r\n" +
                    GetCPUList() + ",\r\n" +
                    GetHDDList() + ",\r\n" +
                    GetNetList() + ",\r\n" +
                    GetRAM() +
                "\r\n\t}\r\n}";
        }
    }
}
