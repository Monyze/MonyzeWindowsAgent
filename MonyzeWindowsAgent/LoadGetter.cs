using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyzeWindowsAgent
{
    class LoadGetter
    {
        private Config config = new Config();

        private string GetCPULoad()
        {
            string output = "";

            return output;
        }

        private string GetHDDLoad()
        {
            string output = "";

            return output;
        }

        private string GetNetLoad()
        {
            string output = "";

            return output;
        }

        private string GetRAMLoad()
        {
            string output = "";

            return output;
        }

        private string GetWidgets()
        {
            string output = "";

            return output;
        }

        public string GetComputerLoad()
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
                    GetCPULoad() + ",\r\n" +
                    GetHDDLoad() + ",\r\n" +
                    GetNetLoad() + ",\r\n" +
                    GetRAMLoad() + ",\r\n" +
                    GetWidgets() +
                "\r\n\t}\r\n}";
        }
    }
}
