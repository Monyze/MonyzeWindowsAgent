using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Config
{
    class HDD : IEntity
    {
        public string indent = "";
        public int number;
        public string name;
        public Int64 size;
        public List<string> logicals;

        public HDD(int number_, string name_, Int64 size_, List<string> logicals_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            name = name_;
            size = size_;
            logicals = logicals_;
        }

        public string Serialize()
        {
            return indent + "{" + indent + "\r\n" + indent + "\t\"hdd_" + number.ToString() + "\":{\r\n" +
                indent + "\t\t\"name\":\"" + @name + "\",\r\n" +
                indent + "\t\t\"size\":" + size.ToString() + ",\r\n" +
                indent + "\t\t\"LOGICAL\":[" + string.Join(", ", logicals.ToArray()) + "]\r\n" +
                indent + "\t}\r\n" +
                indent + "}";
        }
    }
}
