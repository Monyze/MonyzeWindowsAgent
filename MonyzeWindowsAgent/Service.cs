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
        Timer timer = new Timer();

        public MonyzeWindowsAgent()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger.InitLogger();

            Logger.Log.Info("Service is started");
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
            Logger.Log.Info("Service is recall");
        }
    }
}
