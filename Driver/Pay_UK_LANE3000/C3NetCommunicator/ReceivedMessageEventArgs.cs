using System;

namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// Object that represent the argument of the ReceivedMessageEvent
    /// </summary>
    public class ReceivedMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The content of the received message
        /// </summary>
        public string Message => this.message;

        private string message;

        public ReceivedMessageEventArgs(string message)
        {
            this.message = message;
        }
    }
}
