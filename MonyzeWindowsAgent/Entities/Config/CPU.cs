using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Config
{
    class CPU : IEntity
    {
        public string indent = "";
        public int number;
        public string name;

        public CPU(int number_, string name_, string indent_= "")
        {
            indent = indent_;
            number = number_;
            name = name_;
        }

        public string Serialize()
        {
            return indent + "\"cpu_" + number.ToString() + "\":\"" + @name + "\"";
        }
    }
}
