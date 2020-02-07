using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Config
{
    class Net : IEntity
    {
        public string indent = "";
        public int number;
        public string name;
        public string model;
        public Int64 speed;
        string address;

        public Net(int number_, string name_, string model_, Int64 speed_, string address_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            name = name_;
            model = model_;
            speed = speed_;
            address = address_;
        }

        public string Serialize()
        {
            return indent + "\"net_" + number.ToString() + "\":{\r\n" +
                indent + "\t\"addr\":\"" + @address + "\",\r\n" +
                indent + "\t\"name\":\"" + @name + "\",\r\n" +
                indent + "\t\"model\":\"" + @model + "\",\r\n" +
                indent + "\t\"speed\":" + speed.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
