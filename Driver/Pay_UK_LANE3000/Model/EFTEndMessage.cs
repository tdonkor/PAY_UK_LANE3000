using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{

    /// <summary>
    /// The EFT_END message is the completion for the EFT_BEGIN and provides the final details or status of the transaction.
    /// For financial transactions, the EFT_END message may also contain images of the merchant and customer receipts.
    /// </summary>
    public class EFTEndMessage
    {
        private const string EFTENDMESSAGE_LOG = "EFTEndMessage";
        
        public static C3ErrorsAndResponses C3ErrorsAndResponses;

        #region Properties

        public int Error { get; set; }

        public int ErrorResponseCode { get; set; }

        public string ErrorMessage { get; set; }

        public string CustomerTicket { get; set; }

        public string MerchantTicket {get; set;}

        public string TenderMedia {get; set;}

        public string PaidAmount {get; set;} 

    #endregion

        #region Variables

    /// <summary>
    /// Message type.This field contains ‘E’
    /// </summary>
    string MessageType;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string Amount;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string CurrencyCode;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string Amount2;

        /// <summary>
        /// Reserved
        /// </summary>
        string CurrencyCode2;

        /// <summary>
        /// Host response code
        /// </summary>
        string ResponseCode;

        /// <summary>
        /// See below
        /// </summary>
        string C3Error;

        /// <summary>
        /// See below
        /// </summary>
        string TicketAvailable;

        /// <summary>
        /// Reserved
        /// </summary>
        string Pan;

        /// <summary>
        /// Authorisation number for the transaction
        /// </summary>
        string NumAuto;

        /// <summary>
        /// Reserved
        /// </summary>
        string SignatureDemande;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string TermNum;

        /// <summary>
        /// Reserved
        /// </summary>
        string TypeMedia;

        /// <summary>
        /// Masked PAN may be returned in this field
        /// </summary>
        string Iso2;

        /// <summary>
        /// Reserved
        /// </summary>
        string Cmc7;

        /// <summary>
        /// Reserved
        /// </summary>
        string CardType;

        /// <summary>
        /// Reserved
        /// </summary>
        string SSCarte;

        /// <summary>
        /// Reserved
        /// </summary>
        string TimeLoc;

        /// <summary>
        /// Reserved for Expiry date
        /// </summary>
        string DateFinValidate;

        /// <summary>
        /// Reserved
        /// </summary>
        string CodeService;

        /// <summary>
        /// Reserved
        /// </summary>
        string TypeForcage;

        /// <summary>
        /// Reserved
        /// </summary>
        string TenderIdent;

        /// <summary>
        /// Reserved
        /// </summary>
        string CertificatVA;

        /// <summary>
        /// See below
        /// </summary>
        string DateTrns;

        /// <summary>
        /// See below
        /// </summary>
        string HeureTrns;

        /// <summary>
        /// Reserved
        /// </summary>
        string PisteK;

        /// <summary>
        /// Reserved
        /// </summary>
        string DepassPuce;

        /// <summary>
        /// Reserved
        /// </summary>
        string IncidentCam;

        /// <summary>
        /// POS Entry Mode
        /// </summary>
        string CondSaisie;

        /// <summary>
        /// Reserved
        /// </summary>
        string OptionChoisie;

        /// <summary>
        /// Reserved
        /// </summary>
        string OptionLibelle;

        /// <summary>
        /// Reserved
        /// </summary>
        string NumContexte;

        /// <summary>
        /// Reserved
        /// </summary>
        string CodeRejet;

        /// <summary>
        /// Reserved
        /// </summary>
        string CAI_Emetteur;

        /// <summary>
        /// Reserved
        /// </summary>
        string CAI_Auto;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string TrnsNum;

        /// <summary>
        /// Reserved
        /// </summary>
        string NumFile;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string UserData1;

        /// <summary>
        /// Unchanged from request
        /// </summary>
        string UserData2;

        /// <summary>
        /// Reserved
        /// </summary>
        string NumDossier;

        /// <summary>
        /// Reserved
        /// </summary>
        string TypeFacture;

        /// <summary>
        /// Axis server identifier
        /// </summary>
        string Axis;

        /// <summary>
        /// Reserved
        /// </summary>
        string NumFileV5;

        /// <summary>
        /// Reserved
        /// </summary>
        string OtherAmount;

        /// <summary>
        /// See below
        /// </summary>
        string OfflineTrnsStored;

        /// <summary>
        /// Flag for extension fields
        /// </summary>
        string ExtendedFields;

        /// <summary>
        /// Size of extension fields. I.e. “0512”.
        /// </summary>
        string ExtendedLength;

        /// <summary>
        /// Reserved
        /// </summary>
        string FFU;

        /// <summary>
        /// Reserved
        /// </summary>
        string Padding;

        /// <summary>
        /// Type of extension
        /// </summary>
        string ExtensionType;

        /// <summary>
        /// Extension length
        /// </summary>
        string ExtensionLength;

        /// <summary>
        /// Extension buffer
        /// </summary>
        string Extension;

        /// <summary>
        /// Length of customer data
        /// </summary>
        string CustLen;

        /// <summary>
        /// Customer receipt data
        /// </summary>
        string CustData;

        /// <summary>
        /// Length of merchant data
        /// </summary>
        string MerchLen;

        /// <summary>
        /// Merchant receipt data
        /// </summary>
        string MerchData;

        #endregion

        #region Constructor
        public EFTEndMessage()
        {
            C3ErrorsAndResponses = new C3ErrorsAndResponses();
        }

        public EFTEndMessage(string message)
        {
            Error = -1;
            ErrorResponseCode = -1;
            ErrorMessage = "Undefined";

            try
            {
                if (message.Length < 514)
                {
                    Error = -2;

                    ErrorMessage = "Message length is less than 514";
                }
                else
                {
                    //The index at which each parameter substring will start.
                    //This value will be incremented with the field length
                    int currentIndex = 0;

                    //Char 1
                    MessageType = message.Substring(0, 1);
                    currentIndex += 1;

                    //Char 12
                    Amount = message.Substring(currentIndex, 12);
                    currentIndex += 12;

                    //Char 3
                    CurrencyCode = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char 12
                    Amount2 = message.Substring(currentIndex, 12);
                    currentIndex += 12;

                    //Char 3
                    CurrencyCode2 = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char 4
                    ResponseCode = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Char 4
                    C3Error = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Char 1
                    TicketAvailable = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 19
                    Pan = message.Substring(currentIndex, 19);
                    currentIndex += 19;

                    //Char 9
                    NumAuto = message.Substring(currentIndex, 9);
                    currentIndex += 9;

                    //Char 1
                    SignatureDemande = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 8
                    TermNum = message.Substring(currentIndex, 8);
                    currentIndex += 8;

                    //Char 1
                    TypeMedia = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 38
                    Iso2 = message.Substring(currentIndex, 38);
                    currentIndex += 38;

                    //Char 35
                    Cmc7 = message.Substring(currentIndex, 35);
                    currentIndex += 35;

                    //Char 1
                    CardType = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 1
                    SSCarte = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 6
                    TimeLoc = message.Substring(currentIndex, 6);
                    currentIndex += 6;

                    //Char 4
                    DateFinValidate = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Char 3 
                    CodeService = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char 1
                    TypeForcage = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 2
                    TenderIdent = message.Substring(currentIndex, 2);
                    currentIndex += 2;

                    //Char 13
                    CertificatVA = message.Substring(currentIndex, 13);
                    currentIndex += 13;

                    // Char 6
                    DateTrns = message.Substring(currentIndex, 6);
                    currentIndex += 6;

                    //Char 6
                    HeureTrns = message.Substring(currentIndex, 6);
                    currentIndex += 6;

                    //Char 1
                    PisteK = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 1
                    DepassPuce = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 3
                    IncidentCam = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char 3
                    CondSaisie = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char 8
                    OptionChoisie = message.Substring(currentIndex, 8);
                    currentIndex += 8;

                    //Char 16
                    OptionLibelle = message.Substring(currentIndex, 16);
                    currentIndex += 16;

                    //Char 8
                    NumContexte = message.Substring(currentIndex, 8);
                    currentIndex += 8;

                    //Char 36
                    CodeRejet = message.Substring(currentIndex, 36);
                    currentIndex += 36;

                    //Char 28
                    CAI_Emetteur = message.Substring(currentIndex, 28);
                    currentIndex += 28;

                    //Char 52
                    CAI_Auto = message.Substring(currentIndex, 52);
                    currentIndex += 52;

                    //Char 4
                    TrnsNum = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Char 2
                    NumFile = message.Substring(currentIndex, 2);
                    currentIndex += 2;

                    //Char 32
                    UserData1 = message.Substring(currentIndex, 32);
                    currentIndex += 32;

                    //Char 32
                    UserData2 = message.Substring(currentIndex, 32);
                    currentIndex += 32;

                    //Char 12
                    NumDossier = message.Substring(currentIndex, 12);
                    currentIndex += 12;

                    //Char 1
                    TypeFacture = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 1
                    Axis = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 6
                    NumFileV5 = message.Substring(currentIndex, 6);
                    currentIndex += 6;

                    //Char 12
                    OtherAmount = message.Substring(currentIndex, 12);
                    currentIndex += 12;

                    //Char 1
                    OfflineTrnsStored = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 1
                    ExtendedFields = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char 4
                    ExtendedLength = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Char 55
                    FFU = message.Substring(currentIndex, 55);
                    currentIndex += 55;

                    //The "Padding" filed exists only if the "ExtendedFields" is not equal to "1"
                    if (ExtendedFields != "1")
                    {
                        //Char 512
                        Padding = message.Substring(currentIndex, 512);
                        currentIndex += 512;
                    }
                    //The "ExtensionType", "ExtensionLength", "Extension" fields exist only if the "ExtendedFields" is equal to "1"
                    else if (ExtendedFields == "1")
                    {
                        //Char 1
                        ExtensionType = message.Substring(currentIndex, 1);
                        currentIndex += 1;

                        //Char 3 
                        ExtensionLength = message.Substring(currentIndex, 3);
                        currentIndex += 3;

                        //Char 508 
                        Extension = message.Substring(currentIndex, 508);
                        currentIndex += 508;
                    }

                    //Char 4
                    CustLen = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Customer receipt data. "CustData" is present only if the only if "CustLen" is greater than zero 
                    if (CustLen != "0000" && !string.IsNullOrEmpty(CustLen.Trim().Trim('\0')))
                    {
                        int custLen = int.Parse(CustLen);

                        CustData = message.Substring(currentIndex, custLen);
                        currentIndex += custLen;
                    }

                    //Char 4
                    MerchLen = message.Substring(currentIndex, 4);
                    currentIndex += 4;

                    //Merchant receipt data. "CustData" is present only if the only if "CustLen" is greater than zero 
                    if (MerchLen != "0000" && !string.IsNullOrEmpty(MerchLen.Trim().Trim('\0')))
                    {
                        int merchLen = int.Parse(MerchLen);

                        MerchData = message.Substring(currentIndex, merchLen);
                        currentIndex += merchLen;
                    }

                    //Get the ERROR CODE
                    int tempErrorCode = -1;
                    int.TryParse(C3Error, out tempErrorCode);
                    Error = tempErrorCode;

                    

                    //Get the ERROR RESPONSE
                    int tempErrorResponseCode = 0;
                    int.TryParse(ResponseCode, out tempErrorResponseCode);
                    ErrorResponseCode = tempErrorResponseCode;

                    //Get the ERROR MESSAGE
                    string c3ErrorDescription = "Unknown";
                    try
                    {
                        c3ErrorDescription = C3ErrorsAndResponses.C3ErrorDescriptionDictionary[C3Error];
                    } catch (Exception){}

                    string c3ResponseDescription = "";
                    try
                    {
                        c3ResponseDescription = C3ErrorsAndResponses.C3ResponseDescriptionDictionary[ResponseCode];
                    }
                    catch (Exception) { }

                    ErrorMessage = string.Format(" {0}. {1}", c3ErrorDescription, c3ResponseDescription);

                    //Amount
                    int tempPaidAmount = 0;
                    int.TryParse(Amount, out tempPaidAmount);
                    PaidAmount = tempPaidAmount.ToString();

                    CustomerTicket = CustData;
                    MerchantTicket = MerchData;
                    TenderMedia = CardType;
                }
            }
            catch (Exception ex)
            {
                Log.Error(EFTENDMESSAGE_LOG, ex.Message);
            }
        }
        #endregion
    }

}
