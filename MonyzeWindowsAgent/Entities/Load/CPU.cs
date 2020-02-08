using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class Core : IEntity
    {
        public string indent = "";
        public string name;
        public int value;
        
        public Core(string name_, int value_, string indent_ = "")
        {
            indent = indent_;
            name = name_;
            value = value_;
        }

        public string Serialize()
        {
            return indent + "\"" + name + "\":" + value.ToString();
        }
    }

    class CPU : IEntity
    {
        public string indent = "";
        public int number;
        public List loads;
        public List temps;

        public CPU(int number_, List loads_, List temps_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            loads = loads_;
            temps = temps_;
        }

        public string Serialize()
        {
            return indent + "\"cpu_" + number.ToString() + "\":{\r\n" +
                loads.Serialize() + ",\r\n" +
                temps.Serialize() + "\r\n" +
                indent + "}";
        }
    }
}
