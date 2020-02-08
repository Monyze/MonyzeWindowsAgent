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
        public double rx;
        public double tx;
        public Int64 prx;
        public Int64 ptx;

        public Net(int number_, double rx_, double tx_, Int64 prx_, Int64 ptx_, string indent_ = "")
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
                indent + "\t\"rx\":" + rx.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) + ",\r\n" +
                indent + "\t\"tx\":" + tx.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) + ",\r\n" +
                indent + "\t\"prx\":" + prx.ToString() + ",\r\n" +
                indent + "\t\"ptx\":" + ptx.ToString() + "\r\n" +
                indent + "}";
        }
    }
}
