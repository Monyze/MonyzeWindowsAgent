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
        private Entities.Load.RAM ram = new Entities.Load.RAM(0, 0, "\t\t");
        private Entities.Load.Widgets widgets = new Entities.Load.Widgets("\t");

        private NetMeter netMeter = new NetMeter();
        private RAMMeter ramMeter = new RAMMeter();

        public LoadGetter(ref Config config_)
        {
            config = config_;

            widgets.nets = netList;
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

                        if (string.IsNullOrEmpty(value))
                        {
                            value = "0";
                        }
                        
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

                var driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive");
                foreach (ManagementObject d in driveQuery.Get())
                {
                    Entities.List logicals = new Entities.List("ldisks", "\t\t\t\t");
                    Entities.List widgetLogicals = new Entities.List("ldisks", "\t\t\t\t");

                    var partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                    var partitionQuery = new ManagementObjectSearcher(partitionQueryText);
                    foreach (ManagementObject p in partitionQuery.Get())
                    {
                        var logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                        var logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText);
                        foreach (ManagementObject ld in logicalDriveQuery.Get())
                        {
                            var driveName = Convert.ToString(ld.Properties["Name"].Value);

                            double free = Convert.ToDouble(ld.Properties["FreeSpace"].Value) / 1000000000;
                            double total = Convert.ToDouble(ld.Properties["Size"].Value) / 1000000000;
                            double used = total - free;

                            int load = 100 - (int)((free / total) * 100);

                            logicals.Add(new Entities.Load.Logical(free, load, used, driveName, "\t\t\t\t\t"));
                            widgetLogicals.Add(new Entities.Load.WidgetLogical(load, driveName, "\t\t\t\t\t"));
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

            netMeter.Do();

            int x = 1;

            foreach (var v in netMeter.values)
            {
                netList.Add(new Entities.Load.Net(x++, v.rx, v.tx, v.prx, v.ptx, "\t\t\t"));
            }
        }

        private void GetRAMLoad()
        {
            ramMeter.Do();

            ram.availPh = ramMeter.statEX.ullAvailPhys / 1000000;
            var totalMb = ramMeter.statEX.ullTotalPhys / 1000000;

            ram.load = 100 - (int)((Convert.ToDouble(ram.availPh) / Convert.ToDouble(totalMb)) * 100);

            widgets.ramLoad = ram.load;
        }

        public void GetUptime()
        {
            try
            {
                ManagementObject mo = new ManagementObject(@"\\.\root\cimv2:Win32_OperatingSystem=@");
                DateTime lastBootUp = ManagementDateTimeConverter.ToDateTime(mo["LastBootUpTime"].ToString());
                var uptime = DateTime.Now.ToUniversalTime() - lastBootUp.ToUniversalTime();

                widgets.uptime = Convert.ToInt64(uptime.TotalSeconds);
            }
            catch (Exception exp)
            {
                Logger.Log.Error("LoadGetter.GetUptime :: " + exp.Message);
            }
        }

        public string GetComputerLoad()
        {
            GetCPULoad();
            GetHDDLoad();
            GetNetLoad();
            GetRAMLoad();
            GetUptime();

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
