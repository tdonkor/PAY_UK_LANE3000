using System.Net.Sockets;

namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// Object that represents a packet that is sent threw the socket
    /// </summary>
    public class SentSocketPacket
    {
        // Client socket.
        public Socket workSocket = null;

        // Message Sent Type
        public string MessageType  { get; set; }

        // Message Sent content.
        public string Message { get; set; }
    }
}
