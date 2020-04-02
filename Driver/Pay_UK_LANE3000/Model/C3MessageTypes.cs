using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAY_UK_IPP350.Model
{ 
    public static class C3MessageTypes
    {
        /// <summary>
        /// When sent: Start a transaction.
        /// When received: Acknowledge the receipt of the EFT_BEGIN message
        /// 
        /// 
        /// The EFT_BEGIN message is the message type used by the Sales Application to initiate
        ///  commands to C3.The EFT_BEGIN message is used to perform the following functions:
        ///  · Get Pinpad status
        ///  · Initialize C3 and the Pinpad
        ///  · Perform C3 background maintenance functions
        ///  · Perform a financial transaction
        ///  · Perform various settlement functions
        ///  · Retrieve track 2 data
        ///  · Print duplicate receipt
        ///  · Get last transaction
        ///  · Generate report
        ///  · Retrieve C3 version
        ///  
        ///    Example: [‘B’][EFT_BEGIN data][DLE][ETX]
        /// </summary>
        public const string EFT_BEGIN = "B";

        /// <summary>
        /// When received: End transaction. Contains details of the final status of a transaction
        /// </summary>
        public const string EFT_END = "E";

        /// <summary>
        /// When sent: Halt the program
        /// </summary>
        public const string EFT_STOP = "H";

        /// <summary>
        /// When sent: Keyboard Hit
        /// </summary>
        public const string GET_KEY = "K";

        /// <summary>
        ///  When sent: Display a message/an image for the operator
        ///  When received: Confirm a message was received
        /// </summary>
        public const string POS_DISPLAY = "D";
         
        /// <summary>
        ///  When received: PROGRESS message sent by the EFT
        ///  When sent: PROCESS message confirmation
        /// </summary>
        public const string PROGRESS_MESSAGE = "T";
        
        /// <summary>
        /// When received : Request a string from the Sales Application
        /// When sent: Respond with the requested string
        /// </summary>
        public const string GET_STRING_REQUEST = "S";

        /// <summary>
        /// When Received: Ask for permission
        /// When Sent: Respond with permission
        /// </summary>
        public const string GET_SECURITY_REQUEST = "R";

        /// <summary>
        /// When Received: Ask the POS application to print a receipt
        /// When Sent: Acknowledge the receipt is being printed
        /// </summary>
        public const string PRINT_REQUEST = "P";

        /// <summary>
        /// Error / ACK to halt request ‘H’
        /// </summary>
        public const string NAK = "N";

        /// <summary>
        /// Reserved – France
        /// </summary>
        //public const string FRENCH_SALES_CONFIRMATION = "C";

        /// <summary>
        /// Reserved – France
        /// </summary>
        //public const string FRENCH_GENERIC_CALLBACK = "G";
    }
    
}
