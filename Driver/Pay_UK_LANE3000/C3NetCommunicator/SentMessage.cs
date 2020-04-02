namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// When a message is send threw the server socket 
    /// It's status will be updated when the corresponding confirmation message will be send threw the same socket server
    /// </summary>
    public class SentMessage
    {
        /// <summary>
        /// The string message
        /// </summary>
        public string MessageString { get; set; }
         
    }
}
