using System.Net.Sockets;
using System.Text;

namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// Object that represents a packet that is received threw the socket
    /// </summary>
    public class ReceivedSocketPacket
    {
        // Size of receive buffer.
        public const int BufferSize = 8192;

        // Client socket.
        public Socket workSocket = null;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}
