using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Acrelec.Library.Logger;
using PAY_UK_LANE3000.Model;

namespace PAY_UK_LANE3000.C3NetCommunicator
{
    public class C3NetCommunicator
    {
        public const string C3NET_COMMUNICATOR_LOG = "C3NetCommunicatorLog";
         
        /// <summary>
        /// Socket representing the connection with the SCM
        /// </summary>
        private Socket connectedSocket;
          
        /// <summary>
        /// Callback that will be called each time a packet will be received from the SCM
        /// </summary>
        private AsyncCallback asyncReceiveCallBack;

        /// <summary>
        /// Callback that will be called when a connection attempt from the SCM will be received
        /// </summary>
        ///private AsyncCallback asyncAcceptConnectionCallBack;
        
        public bool WasDisconnected { get; private set; }

        public bool WasConnected { get; private set; }

        public bool MessageSendingFailed { get; private set; }

        public event ReceivedMessageEventHandler OnReceivedMessageEventHandler;

        public event SendMessageEventHandler OnSendMessageEventHandler;
         
        /// <summary>
        /// The timer is used to verify if the connection is up.
        /// Usually in 3 seconds the machine sends a message so if no message will be received in more than 6 seconds 
        /// the communication will be considered down
        /// </summary>
        public System.Timers.Timer waitForConnectionTimer;
        
        /// <summary>
        /// Object that will be used to create a safe thread update of the messageBufer
        /// </summary>
        private Object thisLock = new Object();

        /// <summary>
        /// Variable will retain all the messages received from the packets 
        /// as they come threw the socket
        /// </summary>
        private StringBuilder messageBufer;
        
        public C3NetCommunicator() {}

        /// <summary>
        /// Method that is called to open a socket listener to the given port and wait for connection to be established.
        /// </summary>        
        /// <param name="port"> The port that the socket will listen for connections</param>
        /// <param name="timeout">The time in seconds that the socket listener will wait for a connection before closing the port connection</param>
        /// <returns></returns>
        public bool AcceptConnection(IPAddress localIpAddress, int port)
        {
            try
            {                
                //Init and start the timer that is in charge with the connection status check
                waitForConnectionTimer = new System.Timers.Timer();
                waitForConnectionTimer.Interval = 10000;
                waitForConnectionTimer.Elapsed += OnConnectionTimerEvent;
                waitForConnectionTimer.Start();

                WasConnected = false;
                WasDisconnected = false;

                //Init the message buffer each time a new connection attempt is executed
                messageBufer = new StringBuilder();

                //Create the destination endpoint as Loop-back address
                IPEndPoint remoteEndPoint = new IPEndPoint(localIpAddress, port);
                 
                //Init the socket
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                clientSocket.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), clientSocket);
                
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("AcceptConnection : {0}", ex.ToString()));                
            }
            return false;
        }

        /// <summary>
        /// Method that is called when a message needs to be sent threw the socket to the SCM
        /// </summary>
        /// <param name="message">The message object</param>
        public bool Send(string message)
        {
            try
            {
                //Add the C3 message termination
                message = message + C3MessageTerminations.DLE + C3MessageTerminations.ETX;

                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(message);

                //Create the socket packet and assign a socket to it and ad the message type so when the callback is called we know what message was send
                SentSocketPacket sentSocketPacket = new SentSocketPacket();
                sentSocketPacket.Message = message;
                sentSocketPacket.workSocket = connectedSocket;

                // Begin sending the data to the remote device.
                connectedSocket.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), sentSocketPacket);

                Log.Info(C3NET_COMMUNICATOR_LOG, string.Format("OUT : {0}", message));

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("Send : {0}", ex.ToString()));
                return false;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (connectedSocket != null)
                {
                    connectedSocket.Shutdown(SocketShutdown.Both);
                    connectedSocket.Close();                    
                    asyncReceiveCallBack = null;
                    connectedSocket = null;
                }
                if (waitForConnectionTimer != null)
                    waitForConnectionTimer.Close();

                WasDisconnected = true;
            }
            catch (Exception ex)
            {
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("Disconnect : {0}", ex.ToString())); 
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            { 
                //Get the socket that handles the client request.            
                connectedSocket = (Socket)ar.AsyncState;
                
                //Start receiving messages from the connected client
                StartReceivingPackets();

                //Stop the connection 
                waitForConnectionTimer.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("AcceptCallback : {0}", ex.ToString())); 
            }
        }

        /// <summary>
        /// Callback Method that is called each time a packet form the connected socket is received
        /// </summary>
        /// <param name="asyn"></param>
        private void ReceiveCallback(IAsyncResult asyn)
        {
            try
            {
                //Get the current packet
                ReceivedSocketPacket receivedSocketPacket = (ReceivedSocketPacket)asyn.AsyncState;

                //Get the packet socket
                Socket client = receivedSocketPacket.workSocket;

                //Get the number of bytes received
                int bytesRead = receivedSocketPacket.workSocket.EndReceive(asyn);

                if (bytesRead > 0)
                {  
                    //Copy the read bytes from the packet buffer into the processing buffer                                        
                    lock (thisLock)
                    {
                        messageBufer.Append(Encoding.ASCII.GetString(receivedSocketPacket.buffer, 0, bytesRead));
                    }

                    //Process the message buffer tois  extract all the received messages
                    ProcessMessageBuffer();

                    StartReceivingPackets();                    
                }
                else
                {
                    StartReceivingPackets();
                }
            }
            catch (ObjectDisposedException)
            {
                //This is good error (when the socket closes an error that states that the deposed object can't be accesed is triggered)
                //Never foud a clean close solution
            }
            catch (SocketException ex)
            { 
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("ReceiveCallback : {0}", ex.ToString())); 
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                SentSocketPacket sentSocketPacket = (SentSocketPacket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = sentSocketPacket.workSocket.EndSend(ar);

                // Signal that all bytes have been sent.
                RaiseSendedMessageEventHandler(true, sentSocketPacket.MessageType, sentSocketPacket.Message);                            
            }
            catch (ObjectDisposedException)
            {
                //This is good error (when the socket closes an error that states that the deposed object can't be accesed is triggered)
                //Never foud a clean close solution
            }
            catch (Exception ex)
            { 
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("SendCallback : {0}", ex.ToString())); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool StartReceivingPackets()
        {
            try
            {
                //Create the socket packet and assign a socket to it 
                ReceivedSocketPacket receivedSocketPacket = new ReceivedSocketPacket();
                receivedSocketPacket.workSocket = connectedSocket;

                //Init the CallBack method that will wait for received messages
                if (asyncReceiveCallBack == null)
                    asyncReceiveCallBack = new AsyncCallback(ReceiveCallback);

                //Start accepting packets
                if(!WasDisconnected)
                    connectedSocket.BeginReceive(receivedSocketPacket.buffer, 0, ReceivedSocketPacket.BufferSize, SocketFlags.None, asyncReceiveCallBack, receivedSocketPacket);

                //Change the connected flag if the socket is connected to the EndPoint
                WasConnected = true;

                return true;
            }
            catch (ObjectDisposedException)
            {
                //This is good error (when the socket closes an error that states that the deposed object can't be accesed is triggered)
                //Never foud a clean close solution
            }
            catch (Exception ex)
            {
                //If the socket is already closed it's normal for the StartPacketReceiving to fail
                if (!WasDisconnected)
                {
                    //If there is an error when packet receival method is called the communication will be considered down
                    WasDisconnected = true;
                    waitForConnectionTimer.Stop();
                    Disconnect();

                    var win32Exception = ex as Win32Exception;
                    if (win32Exception == null)
                    {
                        win32Exception = ex.InnerException as Win32Exception;
                    }
                    if (win32Exception != null)
                    {
                        int code = win32Exception.ErrorCode;
                        if (code == 10057)
                            Log.Error(C3NET_COMMUNICATOR_LOG, "Socket was closed.");
                        else
                            Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("Start Receiving Packets Failed: {0}", ex.Message));
                    }
                    else
                    {
                        Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("Start Receiving Packets Failed: {0}", ex.Message));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Recursive method that will extract multiple messages from the messageBuffer StringBuilder that contains 
        /// all the received unprocessed information from the socket receiver callback method
        /// </summary>        
        private void ProcessMessageBuffer()
        {
            try
            {
                //If the messageBuffer is empty return
                if (string.IsNullOrEmpty(messageBufer.ToString().Trim()))
                    return;

                lock (thisLock)
                {
                    int endOfMessageMarkerIndex = messageBufer.ToString().IndexOf(C3MessageTerminations.ETX);
                    
                    //If the start or the end of an event is not found exit and wait for a new message packet
                    if (endOfMessageMarkerIndex == -1)
                        return;
                    
                    string message = messageBufer.ToString().Substring(0, endOfMessageMarkerIndex + 1);
  
                    //Raise event with the new message extracted
                    RaiseReceivedMessageEventHandler(message);

                    Log.Info(C3NET_COMMUNICATOR_LOG, string.Format("IN : {0}", message));

                    messageBufer.Remove(0, endOfMessageMarkerIndex + 1);
                }

                if (messageBufer.Length > 0)
                    ProcessMessageBuffer();
            }
            catch (Exception ex)
            { 
                Log.Error(C3NET_COMMUNICATOR_LOG, string.Format("IN : {0}", ex.ToString())); 
            }
        }
        
        /// <summary>
        /// Method is triggered when no packets are received in 6 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        private void OnConnectionTimerEvent(object sender, EventArgs ev)
        {
            waitForConnectionTimer.Stop();
            Disconnect();
            WasDisconnected = true;
        }

        protected virtual void RaiseReceivedMessageEventHandler(string message)
        {
            // Raise the event if it has subscribers.
            if (OnReceivedMessageEventHandler != null)
                OnReceivedMessageEventHandler(this, new ReceivedMessageEventArgs(message));
        }

        protected virtual void RaiseSendedMessageEventHandler(bool isSend, string messageType, string message)
        {
            // Raise the event if it has subscribers.
            if (OnSendMessageEventHandler != null)
                OnSendMessageEventHandler(this, new SendMessageEventArgs(isSend, messageType, message));
        }
    }
}
