using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class RAM : IEntity
    {
        public string indent = "";
        public int load;
        public UInt64 availPh;

        public RAM(int load_, UInt64 availPh_, string indent_ = "")
        {
            indent = indent_;
            load = load_;
            availPh = availPh_;
        }

        public string Serialize()
        {
            return indent + "\"ram\":{\r\n" +
                indent + "\t\"load\":" + load.ToString() + ",\r\n" +
                indent + "\t\"AvailPh\":" + availPh.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
