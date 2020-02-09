using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class WidgetLogical : IEntity
    {
        public string indent = "";
        public int load;
        string ldisk;

        public WidgetLogical(int load_, string ldisk_, string indent_ = "")
        {
            indent = indent_;
            load = load_;
            ldisk = ldisk_;
        }

        public string Serialize()
        {
            return indent + "{\r\n" +
                indent + "\t\"load\":" + load.ToString() + ",\r\n" +
                indent + "\t\"ldisk\":\"" + @ldisk + "\"\r\n" +
                indent + "}";
        }
    }

    class WidgetHDD : IEntity
    {
        public string indent = "";
        public int number;
        List logicals;

        public WidgetHDD(int number_, List logicals_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            logicals = logicals_;
        }

        public string Serialize()
        {
            return indent + "\"hdd_" + number.ToString() + "\":{\r\n"+
                logicals.Serialize() + "\r\n" +
                indent + "}";
        }
    }

    class Widgets : IEntity
    {
        public string indent = "";
        public int cpuLoad;
        public int cpuTemp;
        public int ramLoad;
        public List disks = new List("hdd_load", "\t\t", BracketType.scCurly);

        public Widgets(string indent_ = "")
        {
            indent = indent_;
        }

        public string Serialize()
        {
            return indent + "\"widgets\":{\r\n" +
                indent + "\t\"cpu_load\":" + cpuLoad.ToString() + ",\r\n" +
                indent + "\t\"cpu_temp\":" + (cpuTemp != 0 ? cpuTemp.ToString() : "\"n/a\"") + ",\r\n" +
                indent + "\t\"ram_load\":" + ramLoad.ToString() + ",\r\n" +
                disks.Serialize() + "\r\n" +
                indent + "}";
        }
    }
}
