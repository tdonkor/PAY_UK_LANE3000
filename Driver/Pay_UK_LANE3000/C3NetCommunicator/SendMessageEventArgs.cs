using System;

namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// Object that represent the argument of the SendMessageEvent
    /// </summary>
    public class SendMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Flag that is True if a message was send correctly
        /// </summary>
        public bool IsSend => isSend;

        private bool isSend;
         
        public string Message => message;
        private string message;

        public SendMessageEventArgs(bool isSend, string messageType, string message)
        {
            this.isSend = isSend; 
            this.message = message;
        }
    }
}
