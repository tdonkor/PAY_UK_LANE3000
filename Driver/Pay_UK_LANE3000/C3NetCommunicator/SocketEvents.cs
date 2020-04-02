namespace PAY_UK_IPP350.C3NetCommunicator
{
    /// <summary>
    /// Event that will be raised when a message is received threw the socket
    /// </summary>
    public delegate void ReceivedMessageEventHandler(object sender, ReceivedMessageEventArgs args);

    /// <summary>
    /// Event that will be raised when a message was send successfully to the destination threw the socket
    /// </summary>    
    public delegate void SendMessageEventHandler(object sender, SendMessageEventArgs args);

    /// <summary>
    /// Event that will be raised when a connection with the Cash machine is successfully made
    /// </summary>    
    public delegate void ConnectionAcceptedEventHandler(object sender, ConnectionAcceptedEventArgs args);
}
