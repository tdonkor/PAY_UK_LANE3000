using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    public class C3ErrorsAndResponses
    {
        public static Dictionary<string, string> C3ErrorDescriptionDictionary;
        public static Dictionary<string, string> C3ResponseDescriptionDictionary;
        
        public C3ErrorsAndResponses()
        {
            C3ErrorDescriptionDictionary = new Dictionary<string, string>();
            C3ErrorDescriptionDictionary.Add("0000", "No error Approved");
            C3ErrorDescriptionDictionary.Add("0100", "Pinpad timeout");
            C3ErrorDescriptionDictionary.Add("0101", "Fail to open connection to Pinpad");
            C3ErrorDescriptionDictionary.Add("0102", "Fail to read message from Pinpad");
            C3ErrorDescriptionDictionary.Add("0103", "Fail to write message to Pinpad");
            C3ErrorDescriptionDictionary.Add("0110", "Magstripe unreadable");
            C3ErrorDescriptionDictionary.Add("0111", "Card blocked");
            C3ErrorDescriptionDictionary.Add("0112", "Check unreadable");
            C3ErrorDescriptionDictionary.Add("0113", "Mute ICC");
            C3ErrorDescriptionDictionary.Add("0120", "Check reader unknown");
            C3ErrorDescriptionDictionary.Add("0121", "Check reader not present");
            C3ErrorDescriptionDictionary.Add("0122", "Wrong check reading");
            C3ErrorDescriptionDictionary.Add("0123", "Wrong check writing");
            C3ErrorDescriptionDictionary.Add("0124", "Unattended firm");
            C3ErrorDescriptionDictionary.Add("0125", "PCD not initialize");
            C3ErrorDescriptionDictionary.Add("0126", "PCD unreadable");
            C3ErrorDescriptionDictionary.Add("0127", "PCD entry code");
            C3ErrorDescriptionDictionary.Add("0128", "Invalid input message");
            C3ErrorDescriptionDictionary.Add("0129", "Terminal not authenticated");
            C3ErrorDescriptionDictionary.Add("0130", "Bad serial number of terminal");
            C3ErrorDescriptionDictionary.Add("0131", "Bad application");
            C3ErrorDescriptionDictionary.Add("0132", "Initialisation is required");
            C3ErrorDescriptionDictionary.Add("0199", "Invalid c3config parameter");
            C3ErrorDescriptionDictionary.Add("0201", "Fail to connect to server");
            C3ErrorDescriptionDictionary.Add("0202", "Fail to read message from server");
            C3ErrorDescriptionDictionary.Add("0203", "Fail to write message to server");
            C3ErrorDescriptionDictionary.Add("0204", "In progress");
            C3ErrorDescriptionDictionary.Add("0205", "Unexpected server message");
            C3ErrorDescriptionDictionary.Add("0206", "NACK received from server");
            C3ErrorDescriptionDictionary.Add("0207", "Server time-out");
            C3ErrorDescriptionDictionary.Add("0208", "Axis not started");
            C3ErrorDescriptionDictionary.Add("0209", "Bad axis parameters");
            C3ErrorDescriptionDictionary.Add("0300", "Init pos context impossible");
            C3ErrorDescriptionDictionary.Add("0301", "Get pos context impossible");
            C3ErrorDescriptionDictionary.Add("0302", "Set pos context impossible");
            C3ErrorDescriptionDictionary.Add("0303", "Get Bin table impossible"); 
            C3ErrorDescriptionDictionary.Add("0304", "Set bin table impossible");
            C3ErrorDescriptionDictionary.Add("0310", "Cashier cancel");
            C3ErrorDescriptionDictionary.Add("0311", "Transaction refused / Pinpad not ready");
            C3ErrorDescriptionDictionary.Add("0312", "Transaction suspended");
            C3ErrorDescriptionDictionary.Add("0313", "Card no tender");
            C3ErrorDescriptionDictionary.Add("0314", "Bad ISO2 format");
            C3ErrorDescriptionDictionary.Add("0315", "Exceed floor limit amount for foreign card");
            C3ErrorDescriptionDictionary.Add("0316", "Downloading failed");
            C3ErrorDescriptionDictionary.Add("0400", "Exceed amount authorization");
            C3ErrorDescriptionDictionary.Add("0401", "DCC unavailable");
            C3ErrorDescriptionDictionary.Add("0900", "Cannot find c3config");

            C3ResponseDescriptionDictionary = new Dictionary<string, string>();
            C3ResponseDescriptionDictionary.Add("0000", "Accepted by host");
            C3ResponseDescriptionDictionary.Add("0001", "Referral");
            C3ResponseDescriptionDictionary.Add("0002", "Referral");
            C3ResponseDescriptionDictionary.Add("0003", "Referral");
            C3ResponseDescriptionDictionary.Add("0004", "Refused");
            C3ResponseDescriptionDictionary.Add("0005", "Refused");
            C3ResponseDescriptionDictionary.Add("0006", "Comms Failure");
            C3ResponseDescriptionDictionary.Add("0007", "Comms Failure");
            C3ResponseDescriptionDictionary.Add("0013", "BAD AMOUNT");
            C3ResponseDescriptionDictionary.Add("0014", "Operation not authorized");
            C3ResponseDescriptionDictionary.Add("0015", "Unknown BIN");
            C3ResponseDescriptionDictionary.Add("0016", "Unknown POS Number");
            C3ResponseDescriptionDictionary.Add("0017", "Aborted");
            C3ResponseDescriptionDictionary.Add("0018", "POS not initialized");
            C3ResponseDescriptionDictionary.Add("0020", "Axis application not activated");
            C3ResponseDescriptionDictionary.Add("0021", "Transaction File Full");
            C3ResponseDescriptionDictionary.Add("0023", "Cheque session already running.");
            C3ResponseDescriptionDictionary.Add("0024", "Currency not supported");
            C3ResponseDescriptionDictionary.Add("0030", "Format error");
            C3ResponseDescriptionDictionary.Add("0031", "Declined");
            C3ResponseDescriptionDictionary.Add("0033", "Card expired");
            C3ResponseDescriptionDictionary.Add("0051", "Not enough funds");
            C3ResponseDescriptionDictionary.Add("0070", "Comms Failure, Axis not reachable");
            C3ResponseDescriptionDictionary.Add("0076", "Failed Reversal, Initial Transaction not found");
            C3ResponseDescriptionDictionary.Add("1000", "End - to - End Encryption Error");
            C3ResponseDescriptionDictionary.Add("1005", "Incorrect Card number read on the card(invalid format) CHIP_CARD_SWIPED ");
            C3ResponseDescriptionDictionary.Add("1006", "Chip card read by swipe card reader and fallback not allowed");
            C3ResponseDescriptionDictionary.Add("1007", "Currency request is not supported by the application");
            C3ResponseDescriptionDictionary.Add("1008", "No transaction, associated to The requested User Data, was found (for transactions against User Data)");
            C3ResponseDescriptionDictionary.Add("1030", "Expired Card");
            C3ResponseDescriptionDictionary.Add("2006", "Amount too low");
            C3ResponseDescriptionDictionary.Add("2008", "Amount too high");
            C3ResponseDescriptionDictionary.Add("2010", "Card present in reader");
            C3ResponseDescriptionDictionary.Add("2011", "Error while generating / printing the customer receipt");
            C3ResponseDescriptionDictionary.Add("2012", "Error while generating / printing the merchant receipt");
        }
    }
}
