using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    public static class EFTPOSDisplayMessageActionTypes
    {
        /// <summary>
        /// Display the message and no wait.The Sales Application will respond to C3 immediately with a POS_DISPLAY_CONFIRM message
        /// </summary>
        public static char RESPOND_IMEDIATLY_ACTION = (char)0x00;

        /// <summary>
        /// Display the message and wait for operator acknowledgment.The Sales Application will respond to C3 after the operator presses the[ENTER] button
        /// </summary>
        public static char WAIT_FOR_AKNOLEDGEMENT_ACTION = (char)0x01;

        /// <summary>
        /// Display the message for 1 second.
        /// There is no operator interaction required, but the Sales Application either waits 1 second before responding to C3 with the POS_DISPLAY_CONFIRM message,
        /// or responds immediately to C3 with the POS_DISPLAY_CONFIRM message and ensures the message is displayed for 1 second.
        /// This implementation allows faster transaction processing but requires more logic on the Sales Application.
        /// </summary>
        public static char RESPOND_WITH_1SEC_DELAY = (char)0x02;
    }
}
