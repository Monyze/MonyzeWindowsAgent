using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Timers;

namespace MonyzeWindowsAgent
{
    public partial class MonyzeWindowsAgent : ServiceBase
    {
        private static Config config = new Config();

        private Timer timer = new Timer();

        private LoadGetter loadGetter = new LoadGetter(ref config);

        private bool started = false;

        public MonyzeWindowsAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Run();
        }

        public void Run()
        {
            Logger.InitLogger();

            Logger.Log.InfoFormat("Service is started, user id: {0}, device id: {1}", config.userId, config.deviceId);

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Log.Info("Service is stopped");
        }

        private void SendHWConfig()
        {
            var hwg = new HardwareGetter(ref config);

            var hwgOutput = hwg.GetComputerHardware();

            if (config.logJSONs)
            {
                Logger.Log.Info(hwgOutput);
            }

            try
            {
                WebRequest request = WebRequest.Create(config.url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(hwgOutput);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                Logger.Log.InfoFormat("Sending hw configuration result: {0}", ((HttpWebResponse)response).StatusDescription);

                response.Close();
            }
            catch (Exception exp)
            {
                Logger.Log.Error("SendHWConfig :: " + exp.Message);
            }
        }

        private void SendHWLoad()
        {
            var lgOutput = loadGetter.GetComputerLoad();

            if (config.logJSONs)
            {
                Logger.Log.Info(lgOutput);
            }

            try
            {
                WebRequest request = WebRequest.Create(config.url);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(lgOutput);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                Logger.Log.InfoFormat("Sending hw load result: {0}", ((HttpWebResponse)response).StatusDescription);

                response.Close();
            }
            catch (Exception exp)
            {
                Logger.Log.Error("SendHWLoad :: " + exp.Message);
            }
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            // Send the hardware configuration once after start
            if (!started)
            {
                started = true;

                SendHWConfig();
            }

            SendHWLoad();
        }
    }
}
