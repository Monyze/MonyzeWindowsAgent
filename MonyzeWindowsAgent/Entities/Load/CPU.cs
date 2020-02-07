using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class CPU : IEntity
    {
        public string indent = "";
        public int number;
        public int load;
        public double temp;

        public CPU(int number_, int load_, double temp_, string indent_ = "")
        {
            indent = indent_;
            load = load_;
            temp = temp_;
        }

        public string Serialize()
        {
            return indent + "\"cpu_" + number.ToString() + "\":{\r\n" +
                indent + "\t\"load\":{\r\n" + indent + "\t\t\"total\":" + load.ToString() + "\r\n" + indent + "\t},\r\n" +
                indent + "\t\"temp\":" + temp.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
