using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenHardwareMonitor;
using OpenHardwareMonitor.Hardware;

namespace MonyzeWindowsAgent
{
    class HardwareGetter
    {
        Computer computerHardware = new Computer() { CPUEnabled = true, MainboardEnabled = true, FanControllerEnabled = true, GPUEnabled = true, RAMEnabled = true, HDDEnabled = true };

        private string GetCPUList()
        {
            string output = "\"cpu\": [";

            /*foreach (var hardwareItem in thisComputer.Hardware)
            {
                if (hardwareItem.HardwareType == HardwareType.CPU)
                {
                    hardwareItem.Update();

                    output += "{\"" + hardwareItem.Identifier.ToString() + "\":{\"model\":\"" + "name" + "\"}},";
                    foreach (IHardware subHardware in hardwareItem.SubHardware)
                        subHardware.Update();

                    foreach (var sensor in hardwareItem.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                        {

                            temp += String.Format("{0} Temperature = {1}\r\n", sensor.Name, sensor.Value.HasValue ? sensor.Value.Value.ToString() : "no value");

                        }
                    }
                }
            }*/

            return output + "]";
        }

        private string UpdateOHM()
        {
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
            

            return UpdateOHM();
        }
    }
}
