using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAY_UK_LANE3000.Communicator;

namespace PAY_UK_LANE3000
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreCommunicator coreCommunicator = new CoreCommunicator();

            PayIngenicoLANE3000 payIngenicoLANE3000 = new PayIngenicoLANE3000(coreCommunicator);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Log.Info(PayIngenicoLANE3000.PAY_INGENICO_LANE3000_LOG, "Pay Conexflow started.");
        }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Info(PayIngenicoLANE3000.PAY_INGENICO_LANE3000_LOG, "Unhandled Exception: " + (e.ExceptionObject as Exception).Message);
    }
}
}
