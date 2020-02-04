using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyzeWindowsAgent
{
    class Config
    {
        public const string url = "https://monyze.ru/api_dev";

        public readonly string userId;

        public readonly string deviceId;

        public readonly bool logJSONs;

        public Config()
        {
            var ini = new IniFile(AppDomain.CurrentDomain.BaseDirectory + "/monyze_config.ini");

            userId = ini.Read("user_id");

            deviceId = ini.Read("device_id");

            var logJSONsStr = ini.Read("log_json");

            logJSONs = logJSONsStr == "1" || logJSONsStr == "ON" || logJSONsStr == "On" || logJSONsStr == "on";
        }
    }
}
