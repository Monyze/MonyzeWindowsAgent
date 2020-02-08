using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonyzeWindowsAgent.Entities;

namespace MonyzeWindowsAgent.Entities.Load
{
    class Net : IEntity
    {
        public string indent = "";
        public int number;
        public Int64 rx;
        public Int64 tx;
        public int prx;
        public int ptx;

        public Net(int number_, Int64 rx_, Int64 tx_, int prx_, int ptx_, string indent_ = "")
        {
            indent = indent_;
            number = number_;
            rx = rx_;
            tx = tx_;
            prx = prx_;
            ptx = ptx_;
        }

        public string Serialize()
        {
            return indent + "\"net_" + number.ToString() + "\":{\r\n" +
                indent + "\t\"rx\":" + rx.ToString() + ",\r\n" +
                indent + "\t\"tx\":" + tx.ToString() + ",\r\n" +
                indent + "\t\"prx\":" + prx.ToString() + ",\r\n" +
                indent + "\t\"ptx\":" + ptx.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
