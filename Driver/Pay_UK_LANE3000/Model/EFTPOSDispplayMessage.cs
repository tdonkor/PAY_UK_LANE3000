using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    /// <summary>
    /// This message is used by C3 to send a text message to the Sales Application to display on the
    /// Point of Sale device.The displayed message may or may not require the operator to respond.
    ///
    /// Example:  
    /// [‘D’] [Action] [Message] [DLE] [ETX]
    ///
    /// </summary>
    public class EFTPOSDispplayMessage
    {
        public const string EFT_POS_DISPPLAY_MESSAGE_LOG = "EFTPOSDispplayMessage";

        #region Properties

        public char MessageType { get; private set; }

        public char ActionType { get; private set; }

        public string Description { get; private set; }

        #endregion

        #region Constructor
        public EFTPOSDispplayMessage(string message)
        {
            try
            {
                //Check the length of message
                if (message.Length < 4)
                {
                    Log.Error(EFT_POS_DISPPLAY_MESSAGE_LOG, "Invalid POS Display Message Received. Length is less the 4 characters");
                }
                else
                {
                    MessageType = message[0];
                    ActionType = message[1];
                    Description = message.Substring(2, message.IndexOf(C3MessageTerminations.DLE) - 2).Trim();
                }
            }
            catch (Exception ex)
            {
                Log.Error(EFT_POS_DISPPLAY_MESSAGE_LOG, ex.Message);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create the response string that must be send to the EFT terminal after a DISPLAY MESSAGE is received
        /// </summary>
        /// <returns></returns>
        public string GetEFTPOSDispplayMessageCofirmationResponse()
        {
            return string.Format("{0}{1}{2}", C3MessageTypes.POS_DISPLAY, C3MessageTerminations.DLE, C3MessageTerminations.ETX);
        }

        #endregion
    }
}
