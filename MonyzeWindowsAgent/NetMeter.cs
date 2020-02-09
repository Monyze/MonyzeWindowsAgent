using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace MonyzeWindowsAgent
{
    class NetMeterValues
    {
        public long prevBytesReceived = 0, prevBytesSent = 0;
        public long prevPacketsReceived = 0, prevPacketsSent = 0;

        public double rx = 0, tx = 0;
        public long prx = 0, ptx = 0;

        public NetMeterValues(long prevBytesReceived_, long prevBytesSent_, long prevPacketsReceived_, long prevPacketsSent_)
        {
            prevBytesReceived = prevBytesReceived_;
            prevBytesSent = prevBytesSent_;
            prevPacketsReceived = prevPacketsReceived_;
            prevPacketsSent = prevPacketsSent_;
        }
    }

    class NetMeter
    {
        public List<NetMeterValues> values = new List<NetMeterValues>();

        public int interval;

        public NetMeter(int interval_ = 5) // default interval in 5 second
        {
            interval = interval_;

            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    values.Add(new NetMeterValues ( ni.GetIPStatistics().BytesReceived, ni.GetIPStatistics().BytesSent, ni.GetIPStatistics().UnicastPacketsReceived, ni.GetIPStatistics().UnicastPacketsSent));
                }
            }
        }

        public void Do()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            int i = 0;

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && values.Count > i)
                {
                    values[i].rx = Convert.ToDouble(ni.GetIPStatistics().BytesReceived - values[i].prevBytesReceived) / (interval * 80000);
                    values[i].prevBytesReceived = ni.GetIPStatistics().BytesReceived;

                    values[i].prx = (ni.GetIPStatistics().UnicastPacketsReceived - values[i].prevPacketsReceived) / interval;
                    values[i].prevPacketsReceived = ni.GetIPStatistics().UnicastPacketsReceived;

                    values[i].tx = Convert.ToDouble(ni.GetIPStatistics().BytesSent - values[i].prevBytesSent) / (interval * 80000);
                    values[i].prevBytesSent = ni.GetIPStatistics().BytesSent;

                    values[i].ptx = (ni.GetIPStatistics().UnicastPacketsSent - values[i].prevPacketsSent) / interval;
                    values[i].prevPacketsSent = ni.GetIPStatistics().UnicastPacketsSent;

                    ++i;
                }
            }
        }
    }
}
