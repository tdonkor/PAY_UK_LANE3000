using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAY_UK_IPP350.Communicator;

namespace PAY_UK_IPP350
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreCommunicator coreCommunicator = new CoreCommunicator();

            PayIngenicoIpp350 payIngenicoIpp350 = new PayIngenicoIpp350(coreCommunicator);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Log.Info(PayIngenicoIpp350.PAY_INGENICO_IPP350_LOG, "Pay Conexflow started.");
        }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Info(PayIngenicoIpp350.PAY_INGENICO_IPP350_LOG, "Unhandled Exception: " + (e.ExceptionObject as Exception).Message);
    }
}
}
