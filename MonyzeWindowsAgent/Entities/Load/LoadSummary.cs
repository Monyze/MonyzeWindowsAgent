using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class LoadSummary : IEntity
    {
        public string userId;
        public string deviceId;
        public List cpu;
        public List hdd;
        public List net;
        public RAM ram;
        public Widgets widgets;

        public LoadSummary(string userId_,
            string deviceId_,
            ref List cpu_,
            ref List hdd_,
            ref List net_,
            ref RAM ram_,
            ref Widgets widgets_)
        {
            userId = userId_;
            deviceId = deviceId_;
            cpu = cpu_;
            hdd = hdd_;
            net = net_;
            ram = ram_;
            widgets = widgets_;
        }

        public string Serialize()
        {
            return "{\r\n" +
                 "\t\"id\": {\r\n" +
                 "\t\t\"user_id\": \"" + userId + "\",\r\n" +
                 "\t\t\"device_id\": \"" + deviceId + "\"\r\n\t},\r\n" +
                 "\t\"state\": \"load\",\r\n" +
                 "\t\"load\": {\r\n" +
                     cpu.Serialize() + ",\r\n" +
                     hdd.Serialize() + ",\r\n" +
                     net.Serialize() + ",\r\n" +
                     ram.Serialize() + "\r\n\t},\r\n" +
                 widgets.Serialize() + "\r\n}";
        }
    }
}
