using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class Logical : IEntity
    {
        public string indent = "";
        public double free;
        public int load;
        public double used;
        string ldisk;

        public Logical(double free_, int load_, double used_, string ldisk_, string indent_ = "")
        {
            indent = indent_;
            free = free_;
            load = load_;
            used = used_;
            ldisk = ldisk_;
        }

        public string Serialize()
        {
            return indent + "{\r\n" +
                indent + "\t\"free\":" + free.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",\r\n" +
                indent + "\t\"load\":" + load.ToString() + ",\r\n" +
                indent + "\t\"used\":" + used.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",\r\n" +
                indent + "\t\"ldisk\":\"" + @ldisk + "\"\r\n" +
                indent + "}";
        }
    }

    class HDD : IEntity
    {
        public string indent = "";
        public int number;
        public List logicals;

        public HDD(int number_, List logicals_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            logicals = logicals_;
        }

        public string Serialize()
        {
            return indent + "\"hdd_" + number.ToString() + "\":{\r\n" +
                logicals.Serialize() + "\r\n" +
                indent + "}";
        }
    }
}
