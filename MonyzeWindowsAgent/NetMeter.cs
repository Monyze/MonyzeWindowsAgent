using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace MonyzeWindowsAgent
{
    class NetMeterValues
    {
        public long prevBytesReceived = 0, prevBytesSent = 0;
        public long prevPacketsReceived = 0, prevPacketsSent = 0;

        public double rx = 0, tx = 0;
        public long prx = 0, ptx = 0;

        public string name, description;
        public long speed;
        public string addr;

        public NetMeterValues(long prevBytesReceived_, long prevBytesSent_, long prevPacketsReceived_, long prevPacketsSent_,
            string name_, string description_, long speed_, string addr_)
        {
            prevBytesReceived = prevBytesReceived_;
            prevBytesSent = prevBytesSent_;
            prevPacketsReceived = prevPacketsReceived_;
            prevPacketsSent = prevPacketsSent_;
            name = name_;
            description = description_;
            speed = speed_;
            addr = addr_;
        }
    }

    class NetMeter
    {
        public List<NetMeterValues> values = new List<NetMeterValues>();

        public int interval;

        private bool IsPhysicalDevice(string deviceId)
        {
            if (deviceId != null)
            {
                string key = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\" + deviceId + "\\Connection";
                return Registry.GetValue(key, "PnPInstanceId", "").ToString().Contains("PCI");
            }
            return false;
        }

        public NetMeter(int interval_ = 5) // default interval in 5 second
        {
            interval = interval_;

            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface ni in interfaces)
            {
                if ((ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) && IsPhysicalDevice(ni.Id))
                {
                    string addr = "";
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            addr = ip.Address.ToString();
                            break; // get only the first address...
                        }
                    }

                    values.Add(new NetMeterValues ( ni.GetIPStatistics().BytesReceived,
                        ni.GetIPStatistics().BytesSent,
                        ni.GetIPStatistics().UnicastPacketsReceived,
                        ni.GetIPStatistics().UnicastPacketsSent,
                        ni.Name,
                        ni.Description,
                        ni.Speed / 100000,
                        addr));
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
