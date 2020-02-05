using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace MonyzeWindowsAgent
{
    class HardwareGetter
    {
        private Config config = new Config();

        private string GetCPUList()
        {
            string output = "\t\t\"cpu\":{";

            Computer computerHardware = new Computer() { CPUEnabled = true };

            computerHardware.Open();
            var hardwareCount = computerHardware.Hardware.Count();
            for (int x = 0; x != hardwareCount; ++x)
            {
                var name = computerHardware.Hardware[x].Name;
                
                output += "\r\n\t\t\t\"cpu_" + (x + 1).ToString() + "\":\"" + name + "\",";
            }

            computerHardware.Close();

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
            catch (Exception exp)
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
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //todo: logger => ("can't get data because of the followeing error \n" + exp.Message);
            }

            output = output.TrimEnd(',');

            return output + "\r\n\t\t]";
        }

        private string UpdateOHM()
        {
            // CPUEnabled = true, MainboardEnabled = true, FanControllerEnabled = true, GPUEnabled = true, RAMEnabled = true, HDDEnabled = true

            Computer computerHardware = new Computer() { HDDEnabled = true };

            string output = "";

            string name = string.Empty;
            string type = string.Empty;
            string value = string.Empty;
            int x, y, z, n;
            int hardwareCount;
            int subcount;
            int sensorcount;

            computerHardware.Open();
            hardwareCount = computerHardware.Hardware.Count();
            for (x = 0; x < hardwareCount; x++)
            {
                name = computerHardware.Hardware[x].Name;
                type = computerHardware.Hardware[x].HardwareType.ToString();
                value = ""; // no value for non-sensors;
                //AddReportItem(name, type, value);

                output += name + ", " + type + ": " + value + ";\n";

                subcount = computerHardware.Hardware[x].SubHardware.Count();

                // ADDED 07-28-2016
                // NEED Update to view Subhardware
                for (y = 0; y < subcount; y++)
                {
                    computerHardware.Hardware[x].SubHardware[y].Update();
                }
                //

                if (subcount > 0)
                {
                    for (y = 0; y < subcount; y++)
                    {
                        sensorcount = computerHardware.Hardware[x].
                        SubHardware[y].Sensors.Count();
                        name = computerHardware.Hardware[x].SubHardware[y].Name;
                        type = computerHardware.Hardware[x].SubHardware[y].
                        HardwareType.ToString();
                        value = "";
                        output += name + ", " + type + ": " + value + ";\n";

                        if (sensorcount > 0)
                        {

                            for (z = 0; z < sensorcount; z++)
                            {

                                name = computerHardware.Hardware[x].
                                SubHardware[y].Sensors[z].Name;
                                type = computerHardware.Hardware[x].
                                SubHardware[y].Sensors[z].SensorType.ToString();
                                value = computerHardware.Hardware[x].
                                SubHardware[y].Sensors[z].Value.ToString();
                                output += name + ", " + type + ": " + value + ";\n";

                            }
                        }
                    }
                }
                sensorcount = computerHardware.Hardware[x].Sensors.Count();

                if (sensorcount > 0)
                {
                    for (z = 0; z < sensorcount; z++)
                    {
                        name = computerHardware.Hardware[x].Sensors[z].Name;
                        type = computerHardware.Hardware[x].Sensors[z].SensorType.ToString();
                        value = computerHardware.Hardware[x].Sensors[z].Value.ToString();
                        output += name + ", " + type + ": " + value + ";\n";

                    }
                }
            }
            computerHardware.Close();

            return output;
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
                    GetNetList() +
                "\r\n\t}\r\n}";
        }
    }
}
