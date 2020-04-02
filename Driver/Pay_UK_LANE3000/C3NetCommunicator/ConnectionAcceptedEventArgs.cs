using System;

namespace PAY_UK_IPP350.C3NetCommunicator
{
    public class  ConnectionAcceptedEventArgs : EventArgs
    {
        public bool IsConnected => isConnected;
        private bool isConnected;

        public ConnectionAcceptedEventArgs(bool isConnected)
        {
            this.isConnected = isConnected;
        }
    }
}
