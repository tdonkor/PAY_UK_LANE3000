using System;
using System.Diagnostics;
using Acrelec.Mockingbird.Interfaces;
using Acrelec.Mockingbird.Interfaces.Peripherals;

namespace Acrelec.Mockingbird.Payment_UK_IPP350
{
    public class C3NetManager
    {
        private const string C3NET_MANAGER_LOG = "C3NetServiceManager";

        private const string C3_NET_SERVICE_LOCATION_REGISTRY = @"SYSTEM\CurrentControlSet\Services\C3Net";
        
        private const string C3NET_PROCESS_NAME = "c3driver_net";

        private string c3netApplicationLocation;

        private ILogger logger;

        public C3NetManager(ILogger logger, string driverLocation)
        {
            c3netApplicationLocation = driverLocation;

            this.logger = logger;
        }

        /// <summary>
        /// Method that will check if the received service name is installed
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public bool IsC3NetStarted()
        {
            try
            {
               if (Process.GetProcessesByName(C3NET_PROCESS_NAME).Length > 0)
                    return true;
               else
                    return false;           
            }
            catch (Exception ex)
            {
                logger.Error(C3NET_MANAGER_LOG, string.Format("IsC3NetStarted: {0}", ex.Message));
            }
            return false;
        }

        /// <summary>
        /// Method that will check if multiple c3driver_net.exe are opened
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public bool AreMultipleC3NetInstancesStarted()
        {
            try
            {
                if (Process.GetProcessesByName(C3NET_PROCESS_NAME).Length > 1)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                logger.Error(C3NET_MANAGER_LOG, string.Format("AreMultipleC3NetInstancesStarted: {0}", ex.Message));
            }
            return false;
        }



        /// <summary>
        /// Method will check if the given service is started and if not it will try to start the service.
        /// </summary>
        /// <returns>
        /// True - if the service was successfully started
        /// False - if the service start failed
        /// </returns>
        public bool StopC3Net()
        {
            try
            {
                //Stop the payment application
                Process[] processesByName = Process.GetProcessesByName(C3NET_PROCESS_NAME);
                if (processesByName.Length > 0)
                {
                    foreach (Process process in processesByName)
                    {
                        process.Kill();
                        logger.Info(C3NET_MANAGER_LOG, "Application was closed");
                    }
                }
                else
                {
                    logger.Info(C3NET_MANAGER_LOG, string.Format("No '{0}' process was found.", C3NET_PROCESS_NAME));
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(C3NET_MANAGER_LOG, string.Format("{0} stop failed. {1}", C3NET_PROCESS_NAME, ex.Message));
            }
            return false;
        }

        public bool StartC3Net()
        {
            try
            {
                //Start the application
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = true;
                startInfo.FileName = string.Format("{0}.exe", C3NET_PROCESS_NAME);
                startInfo.WorkingDirectory = c3netApplicationLocation;

                Process.Start(startInfo);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(C3NET_MANAGER_LOG, string.Format("{0} start failed. {1}", C3NET_PROCESS_NAME, ex.Message));
            }
            
            return false;
        }
        
        /// <summary>
        /// Method will execute a Command prompt command and will return the output of that command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //private string RunCmdCommand(string command)
        //{
        //    try
        //    {
        //        Process process = new Process();
        //        ProcessStartInfo startInfo = new ProcessStartInfo();
        //        startInfo.FileName = "cmd.exe";
        //        startInfo.Arguments = string.Format("/c start {0}", command);
        //        startInfo.UseShellExecute = false;
        //        startInfo.RedirectStandardOutput = true;
        //        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        startInfo.CreateNoWindow = true;

        //        process.StartInfo = startInfo;
        //        process.Start();
        //        process.WaitForExit();

        //        return process.StandardOutput.ReadToEnd();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(C3NET_SERVICE_MANAGER_LOG, ex.ToString());
        //    }
        //    return string.Empty;
        //}

        /// <summary>
        /// Method will periodically check if the process has been stopped for <paramref name="timeout"/> seconds.
        /// after which it will return anyway
        /// </summary>
        /// <param name="timeout"></param>
        //private void WaitForServiceToStop(int timeout)
        //{
        //    try
        //    { 
        //        DateTime start = DateTime.Now;
        //        while (DateTime.Now.Subtract(start).TotalSeconds <= timeout)
        //        {
        //            Process[] processesByName = Process.GetProcessesByName(C3NET_PROCESS_NAME);
        //            if (processesByName.Length == 0)
        //            {
        //                logger.Info(C3NET_SERVICE_MANAGER_LOG, "Process was already close.");
        //                return;
        //            }
        //            Thread.Sleep(200);
        //        }
        //        logger.Info(C3NET_SERVICE_MANAGER_LOG, "Process is still opened after 30 seconds.");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(C3NET_SERVICE_MANAGER_LOG, string.Format("Wait For Service To Stop failed. {0}", ex.Message));
        //    }
        //}
    }
}
