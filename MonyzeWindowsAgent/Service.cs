using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MonyzeWindowsAgent
{
    public partial class MonyzeWindowsAgent : ServiceBase
    {
        Config config = new Config();

        Timer timer = new Timer();

        public MonyzeWindowsAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.InitLogger();

            Logger.Log.InfoFormat("Service is started, user id: {0}, device id: {1}", config.userId, config.deviceId);

            HardwareGetter hwg = new HardwareGetter();

            var hwgOutput = hwg.GetComputerHardware();

            if (config.logJSONs)
            {
                Logger.Log.Info(hwgOutput);
            }

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            Logger.Log.Info("Service is stopped");
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            //Logger.Log.Info("Service is recall");
        }
    }
}
