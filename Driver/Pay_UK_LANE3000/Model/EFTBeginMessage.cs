using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PAY_UK_IPP350.Model
{  
    /// <summary>
    /// The EFT_BEGIN message is the message type used by the Sales Application to initiate
    /// commands to C3.The EFT_BEGIN message is used to perform the following functions:
    /// · Get Pinpad status
    /// · Initialize C3 and the Pinpad
    /// · Perform C3 background maintenance functions
    /// · Perform a financial transaction
    /// · Perform various settlement functions
    /// · Retrieve track 2 data
    /// · Print duplicate receipt
    /// · Get last transaction
    /// · Generate report
    /// · Retrieve C3 version
    /// 
    /// The functions supported are selected by setting the relevant values in the fields cOperation
    ///  and cTenderType within the message.Refer to Section 3 Supported Transactions for details
    ///  on each transaction type.
    ///
    ///  Example : [‘B’] [EFT_BEGIN data] [DLE] [ETX]
    ///  
    /// </summary>
    public class EFTBeginMessage
    {
        private const string EFT_BEGIN_MESSAGE_LOG = "EFTBeginMessage";

        #region Variables

        /// <summary>
        /// Transaction amount in cents
        /// </summary>
        public char[] Amount;

        /// <summary>
        /// Transaction currency code
        /// </summary>
        public char[] CurrencyCode;

        /// <summary>
        /// Cash transaction component
        /// </summary>
        public char[] Amount2;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] CurrencyCode2;

        /// <summary>
        /// Function to be performed
        /// </summary>
        public char[] Operation;

        /// <summary>
        /// Sub-function
        /// </summary>
        public char[] TenderType;

        /// <summary>
        /// Data entry type
        /// </summary>
        public char[] ReaderType;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] CustomerPresent;

        /// <summary>
        /// Ingenico Point of Sale identifier
        /// </summary>
        public char[] TermNum;

        /// <summary>
        /// Point of Sale cashier number
        /// </summary>
        public char[] CashNum;

        /// <summary>
        /// Point of Sale transaction reference
        /// </summary>
        public char[] TrnsNum;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] Pan;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] CryptoVAD;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] DateInitValidite;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] DateFinValidite;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] Iso2;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] AutoType;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] NumContexte;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] NumAuto;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] CtrlCheque;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] IdentCheque;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] Cmc7;

        /// <summary>
        /// Sales Application data 1
        /// </summary>
        public char[] UserData1;

        /// <summary>
        /// Sales Application data 2
        /// </summary>
        public char[] UserData2;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] Option;

        /// <summary>
        /// Training mode flag
        /// </summary>
        public char[] TutorialMode;

        /// <summary>
        /// Sales Application
        /// </summary>
        public char[] NumTicket;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] NumDossier;

        /// <summary>
        /// Reserved
        /// </summary>
        public char[] TypeFacture;

        /// <summary>
        /// Extended data flag
        /// </summary>
        public char[] ExtendedFields;

        /// <summary>
        /// Extended data length
        /// </summary>
        public char[] LengthExtension;

        /// <summary>
        /// Card Issue Number
        /// </summary>
        public char[] IssueNumber;

        /// <summary>
        /// FFU interpretation flag
        /// </summary>
        public char[] FfuType;

        /// <summary>
        /// Data size in FFU
        /// </summary>
        public char[] TlvLength;

        /// <summary>
        /// Reserved / TLV buffer
        /// </summary>
        public char[] FFU;

        #endregion

        #region Constructor
        public EFTBeginMessage()
        {
            Amount = new char[12];
            InitializeCharArrayWithSpaceChar(Amount);

            CurrencyCode = new char[3];
            InitializeCharArrayWithSpaceChar(CurrencyCode);

            Amount2 = new char[12];
            InitializeCharArrayWithSpaceChar(Amount2);

            CurrencyCode2 = new char[3];
            InitializeCharArrayWithSpaceChar(CurrencyCode2);

            Operation = new char[1];
            InitializeCharArrayWithSpaceChar(Operation);

            TenderType = new char[2];
            InitializeCharArrayWithSpaceChar(TenderType);

            ReaderType = new char[2];
            InitializeCharArrayWithSpaceChar(ReaderType);

            CustomerPresent = new char[1];
            InitializeCharArrayWithSpaceChar(CustomerPresent);

            TermNum = new char[8];
            InitializeCharArrayWithSpaceChar(TermNum);

            CashNum = new char[8];
            InitializeCharArrayWithSpaceChar(CashNum);

            TrnsNum = new char[4];
            InitializeCharArrayWithSpaceChar(TrnsNum);

            Pan = new char[19];
            InitializeCharArrayWithSpaceChar(Pan);

            CryptoVAD = new char[3];
            InitializeCharArrayWithSpaceChar(CryptoVAD);

            DateInitValidite = new char[4];
            InitializeCharArrayWithSpaceChar(DateInitValidite);

            DateFinValidite = new char[4];
            InitializeCharArrayWithSpaceChar(DateFinValidite);

            Iso2 = new char[40];
            InitializeCharArrayWithSpaceChar(Iso2);

            AutoType = new char[1];
            InitializeCharArrayWithSpaceChar(AutoType);

            NumContexte = new char[8];
            InitializeCharArrayWithSpaceChar(NumContexte);

            NumAuto = new char[9];
            InitializeCharArrayWithSpaceChar(NumAuto);

            CtrlCheque = new char[1];
            InitializeCharArrayWithSpaceChar(CtrlCheque);

            IdentCheque = new char[1];
            InitializeCharArrayWithSpaceChar(IdentCheque);

            Cmc7 = new char[35];
            InitializeCharArrayWithSpaceChar(Cmc7);

            UserData1 = new char[32];
            InitializeCharArrayWithSpaceChar(UserData1);

            UserData2 = new char[32];
            InitializeCharArrayWithSpaceChar(UserData2);

            Option = new char[8];
            InitializeCharArrayWithSpaceChar(Option);

            TutorialMode = new char[1];
            InitializeCharArrayWithSpaceChar(TutorialMode);

            NumTicket = new char[8];
            InitializeCharArrayWithSpaceChar(NumTicket);

            NumDossier = new char[12];
            InitializeCharArrayWithSpaceChar(NumDossier);

            TypeFacture = new char[1];
            InitializeCharArrayWithSpaceChar(TypeFacture);

            ExtendedFields = new char[1];
            InitializeCharArrayWithSpaceChar(ExtendedFields);

            LengthExtension = new char[4];
            InitializeCharArrayWithSpaceChar(LengthExtension);

            IssueNumber = new char[2];
            InitializeCharArrayWithSpaceChar(IssueNumber);

            FfuType = new char[1];
            InitializeCharArrayWithSpaceChar(FfuType);

            TlvLength = new char[2];
            InitializeCharArrayWithSpaceChar(TlvLength);

            FFU = new char[99];
            InitializeCharArrayWithSpaceChar(FFU);
        }        
        #endregion

        #region Public Methods (Internal)
        
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal string GetPinpadInitializationMessage(string terminalID)
        {
            try
            {
                //Set the Operation 
                Operation[0] = OperationTypes.Initialize;

                //if the array is larger than the designated filed then trim it
                if (terminalID.Length > TermNum.Length)
                {
                    terminalID = terminalID.Substring(0, TermNum.Length);
                    TermNum = terminalID.ToArray();
                }
                else
                {
                    terminalID = terminalID.PadRight(8, ' ');
                    TermNum = terminalID.ToArray();
                }

                // DO NOT CHANAGE THE ORDER OF FIELDS
                string result = new string(Amount) +
                    new string(CurrencyCode) +
                    new string(Amount2) +
                    new string(CurrencyCode2) +
                    new string(Operation) +
                    new string(TenderType) +
                    new string(ReaderType) +
                    new string(CustomerPresent) +
                    new string(TermNum) +
                    new string(CashNum) +
                    new string(TrnsNum) +
                    new string(Pan) +
                    new string(CryptoVAD) +
                    new string(DateInitValidite) +
                    new string(DateFinValidite) +
                    new string(Iso2) +
                    new string(AutoType) +
                    new string(NumContexte) +
                    new string(NumAuto) +
                    new string(CtrlCheque) +
                    new string(IdentCheque) +
                    new string(Cmc7) +
                    new string(UserData1) +
                    new string(UserData2) +
                    new string(Option) +
                    new string(TutorialMode) +
                    new string(NumTicket) +
                    new string(NumDossier) +
                    new string(TypeFacture) +
                    new string(ExtendedFields) +
                    new string(LengthExtension) +
                    new string(IssueNumber) +
                    new string(FfuType) +
                    new string(TlvLength) +
                    new string(FFU);

                //Add the MessageType
                result = C3MessageTypes.EFT_BEGIN + result;

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(EFT_BEGIN_MESSAGE_LOG, ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        internal string GetPinpadStatusMessage(string terminalID)
        {
            try
            {
                //Set the Operation 
                Operation[0] = OperationTypes.Status;

                //Set the ternder type
                TenderType[0] = 'H';

                //if the array is larger than the designated filed then trim it
                if (terminalID.Length > TermNum.Length)
                {
                    terminalID = terminalID.Substring(0, TermNum.Length);
                    TermNum = terminalID.ToArray();
                }
                else
                {
                    terminalID = terminalID.PadRight(8, ' ');
                    TermNum = terminalID.ToArray();
                }

                // DO NOT CHANAGE THE ORDER OF FIELDS
                string result = new string(Amount) +
                    new string(CurrencyCode) +
                    new string(Amount2) +
                    new string(CurrencyCode2) +
                    new string(Operation) +
                    new string(TenderType) +
                    new string(ReaderType) +
                    new string(CustomerPresent) +
                    new string(TermNum) +
                    new string(CashNum) +
                    new string(TrnsNum) +
                    new string(Pan) +
                    new string(CryptoVAD) +
                    new string(DateInitValidite) +
                    new string(DateFinValidite) +
                    new string(Iso2) +
                    new string(AutoType) +
                    new string(NumContexte) +
                    new string(NumAuto) +
                    new string(CtrlCheque) +
                    new string(IdentCheque) +
                    new string(Cmc7) +
                    new string(UserData1) +
                    new string(UserData2) +
                    new string(Option) +
                    new string(TutorialMode) +
                    new string(NumTicket) +
                    new string(NumDossier) +
                    new string(TypeFacture) +
                    new string(ExtendedFields) +
                    new string(LengthExtension) +
                    new string(IssueNumber) +
                    new string(FfuType) +
                    new string(TlvLength) +
                    new string(FFU);

                //Add the MessageType
                result = C3MessageTypes.EFT_BEGIN + result;

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(EFT_BEGIN_MESSAGE_LOG, ex.Message);
            }
            return string.Empty;
        }

        internal string GetTransactionMessage(string terminalID, string currency, string amount, string transactionNumber)
        {
            try
            {
                CashNum = "00000001".ToArray();

                CustomerPresent[0] = '1';

                TrnsNum = "0001".ToArray();

                TenderType = "0 ".ToArray();

                //Set the Operation 
                Operation[0] = OperationTypes.Purchase;

                //Update the reader type
                ReaderType[0] = '0';

                //if the array is larger than the designated filed then trim it
                if (terminalID.Length > TermNum.Length)
                {
                    terminalID = terminalID.Substring(0, TermNum.Length);
                    TermNum = terminalID.ToArray();
                }
                else
                {
                    terminalID = terminalID.PadRight(8, ' ');
                    TermNum = terminalID.ToArray();
                }

                //If the amount is greater then the supported length then send it as '0'
                if (amount.Length > Amount.Length)
                {
                    Amount = "000000000000".ToArray();
                }
                else
                {
                    // 2,2 GBP  => '000000000220'
                    amount = amount.PadLeft(12, '0');
                    Amount = amount.ToArray();
                }

                //Set the transaction number
                if (transactionNumber.Length > NumTicket.Length)
                {
                    //Trun the last digits if the transaction number is larger than 8 chars
                    transactionNumber = transactionNumber.Substring(0, 8);
                    NumTicket = transactionNumber.ToArray();
                }
                else
                {
                    //Pad with spaces if the transction number is less than 8 characters
                    transactionNumber = transactionNumber.PadRight(8, ' ');
                    NumTicket = transactionNumber.ToArray();
                }


                //Update the currency code
                CurrencyCode = currency.ToArray();

                // DO NOT CHANAGE THE ORDER OF FIELDS
                string result = new string(Amount) +
                    new string(CurrencyCode) +
                    new string(Amount2) +
                    new string(CurrencyCode2) +
                    new string(Operation) +
                    new string(TenderType) +
                    new string(ReaderType) +
                    new string(CustomerPresent) +
                    new string(TermNum) +
                    new string(CashNum) +
                    new string(TrnsNum) +
                    new string(Pan) +
                    new string(CryptoVAD) +
                    new string(DateInitValidite) +
                    new string(DateFinValidite) +
                    new string(Iso2) +
                    new string(AutoType) +
                    new string(NumContexte) +
                    new string(NumAuto) +
                    new string(CtrlCheque) +
                    new string(IdentCheque) +
                    new string(Cmc7) +
                    new string(UserData1) +
                    new string(UserData2) +
                    new string(Option) +
                    new string(TutorialMode) +
                    new string(NumTicket) +
                    new string(NumDossier) +
                    new string(TypeFacture) +
                    new string(ExtendedFields) +
                    new string(LengthExtension) +
                    new string(IssueNumber) +
                    new string(FfuType) +
                    new string(TlvLength) +
                    new string(FFU);

                //Add the MessageType
                result = C3MessageTypes.EFT_BEGIN + result;

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(EFT_BEGIN_MESSAGE_LOG, ex.Message);
            }
            return string.Empty;
        }
         
        #endregion

        #region Private Methods
        /// <summary>
        /// Method will initialize all each item of the array with spaces ' ';
        /// </summary>
        /// <param name="array"></param>
        private void InitializeCharArrayWithSpaceChar(char[] array)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = (char)0x20;
        }
        #endregion
    }
}