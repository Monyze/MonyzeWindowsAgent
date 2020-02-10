using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Config
{
    class RAM : IEntity
    {
        public string indent = "";
        public UInt64 totalPh;
        
        public RAM(UInt64 totalPh_, string indent_ = "")
        {
            indent = indent_;
            totalPh = totalPh_;
        }

        public string Serialize()
        {
            return indent + "\"ram\":{\r\n" +
                indent + "\t\"TotalPh\":" + totalPh.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
