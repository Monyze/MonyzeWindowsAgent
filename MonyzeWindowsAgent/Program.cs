using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;

namespace MonyzeWindowsAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default: /// Debug
                        MonyzeWindowsAgent mwa = new MonyzeWindowsAgent();
                        mwa.Run();
                        while (true)
                        {
                            System.Threading.Thread.Sleep(5000);
                        }
                        break;
                }
            }
            else
            {
                ServiceBase[] servicesToRun = {
                    new MonyzeWindowsAgent()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
