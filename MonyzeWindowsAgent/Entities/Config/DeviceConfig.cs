using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Config
{
    class DeviceConfig : IEntity
    {
        public string userId;
        public string deviceId;
        public string deviceName;
        public string system;
        public string bits;
        public List cpu;
        public List hdd;
        public List net;
        public RAM ram;

        public DeviceConfig(string userId_,
            string deviceId_,
            string deviceName_,
            string system_,
            string bits_,
            ref List cpu_,
            ref List hdd_,
            ref List net_,
            ref RAM ram_)
        {
            userId = userId_;
            deviceId = deviceId_;
            deviceName = deviceName_;
            system = system_;
            bits = bits_;
            cpu = cpu_;
            hdd = hdd_;
            net = net_;
            ram = ram_;
    }

        public string Serialize()
        {
            return "{\r\n" +
                "\t\"id\": {\r\n" +
                "\t\t\"user_id\": \"" + userId + "\",\r\n" +
                "\t\t\"device_id\": \"" + deviceId + "\"\r\n\t},\r\n" +
                "\t\"state\": \"config\",\r\n" +
                "\t\"device_config\": {\r\n" +
                "\t\t\"device_name\": \"" + deviceName + "\",\r\n" +
                "\t\t\"system\": \"" + system + "\",\r\n" +
                "\t\t\"bits\": \"" + bits + "\",\r\n" +
                "\t\t\"icon\": \"f17a\",\r\n" +
                    cpu.Serialize() + ",\r\n" +
                    hdd.Serialize() + ",\r\n" +
                    net.Serialize() + ",\r\n" +
                    ram.Serialize() +
                "\r\n\t}\r\n}";
        }
    }
}
