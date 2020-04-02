using Acrelec.Library.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PAY_UK_IPP350.Model
{
    public class EFTProgressMessage
    {
        private const string EFT_PROGRESS_MESSAGE_LOG = "EFTEndMessage";
        
        #region Properties

        /// <summary>
        /// Message type.This field contains ‘T’
        /// </summary>
        string MessageType;

        /// <summary>
        /// Progress Message Type 
        /// 01 : Card Read
        /// </summary>
        string ProgressType;

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
        ///  Masked PAN may be returned in this field
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
        /// Reserved
        /// </summary>
        string DateTrns;
        
        /// <summary>
        /// Reserved
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
        /// Size of extension fields.I.e. “0512”.
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

        #endregion

        public EFTProgressMessage(string message)
        {
            try
            {
                //The index at which each parameter substring will start.
                //This value will be incremented with the field length
                int currentIndex = 1;

                //Char 1
                MessageType = message.Substring(1, 1);
                currentIndex += 1;

                //Char
                ProgressType = message.Substring(currentIndex, 2);
                currentIndex += 2;

                //Char
                Amount = message.Substring(12);
                currentIndex += 2;

                //Char
                CurrencyCode = message.Substring(currentIndex, 3);
                currentIndex += 3;

                //Char
                Amount2 = message.Substring(currentIndex, 12);
                currentIndex += 12;

                //Char
                CurrencyCode2 = message.Substring(currentIndex, 3);
                currentIndex += 3;

                //Char
                ResponseCode = message.Substring(currentIndex, 4);
                currentIndex += 4;

                //Char
                C3Error = message.Substring(currentIndex, 4);
                currentIndex += 4;

                //Char
                TicketAvailable = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                Pan = message.Substring(currentIndex, 19);
                currentIndex += 19;

                //Char
                NumAuto = message.Substring(currentIndex, 9);
                currentIndex += 9;

                //Char
                SignatureDemande = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                TermNum = message.Substring(currentIndex, 8);
                currentIndex += 8;

                //Char
                TypeMedia = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                Iso2 = message.Substring(currentIndex, 38);
                currentIndex += 38;

                //Char
                Cmc7 = message.Substring(currentIndex, 35);
                currentIndex += 35;

                //Char
                CardType = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                SSCarte = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                TimeLoc = message.Substring(currentIndex, 6);
                currentIndex += 6;

                //Char
                DateFinValidate = message.Substring(currentIndex, 4);
                currentIndex += 4;

                //Char
                CodeService = message.Substring(currentIndex, 3);
                currentIndex += 3;

                //Char
                TypeForcage = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                TenderIdent = message.Substring(currentIndex, 2);
                currentIndex += 2;

                //Char
                CertificatVA = message.Substring(currentIndex, 13);
                currentIndex += 13;

                //Char
                DateTrns = message.Substring(currentIndex, 6);
                currentIndex += 6;

                //Char
                HeureTrns = message.Substring(currentIndex, 6);
                currentIndex += 6;

                //Char
                PisteK = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                DepassPuce = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                IncidentCam = message.Substring(currentIndex, 3);
                currentIndex += 3;

                //Char
                CondSaisie = message.Substring(currentIndex, 3);
                currentIndex += 3;

                //Char
                OptionChoisie = message.Substring(currentIndex, 8);
                currentIndex += 8;

                //Char
                OptionLibelle = message.Substring(currentIndex, 16);
                currentIndex += 16;

                //Char
                NumContexte = message.Substring(currentIndex, 8);
                currentIndex += 8;

                //Char
                CodeRejet = message.Substring(currentIndex, 36);
                currentIndex += 36;

                //Char
                CAI_Emetteur = message.Substring(currentIndex, 28);
                currentIndex += 28;

                //Char
                CAI_Auto = message.Substring(currentIndex, 52);
                currentIndex += 52;

                //Char
                TrnsNum = message.Substring(currentIndex, 4);
                currentIndex += 4;

                //Char
                NumFile = message.Substring(currentIndex, 2);
                currentIndex += 2;

                //Char
                UserData1 = message.Substring(currentIndex, 32);
                currentIndex += 32;

                //Char
                UserData2 = message.Substring(currentIndex, 32);
                currentIndex += 32;

                //Char
                NumDossier = message.Substring(currentIndex, 12);
                currentIndex += 12;

                //Char
                TypeFacture = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                Axis = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                NumFileV5 = message.Substring(currentIndex, 6);
                currentIndex += 6;

                //Char
                OtherAmount = message.Substring(currentIndex, 12);
                currentIndex += 12;

                //Char
                OfflineTrnsStored = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                ExtendedFields = message.Substring(currentIndex, 1);
                currentIndex += 1;

                //Char
                ExtendedLength = message.Substring(currentIndex, 4);
                currentIndex += 4;

                //Char
                FFU = message.Substring(currentIndex, 55);
                currentIndex += 55;


                if (ExtendedFields == "1")
                {
                    //Char
                    ExtensionType = message.Substring(currentIndex, 1);
                    currentIndex += 1;

                    //Char
                    ExtensionLength = message.Substring(currentIndex, 3);
                    currentIndex += 3;

                    //Char
                    Extension = message.Substring(currentIndex, 508);
                    currentIndex += 508;
                }
                else
                {
                    //Char
                    Padding = message.Substring(currentIndex, 512);
                    currentIndex += 512;
                }
            }
            catch (Exception ex)
            {
                Log.Error(EFT_PROGRESS_MESSAGE_LOG, ex.Message);
            }
        }

        /// <summary>
        /// Create the response string that must be send to the EFT terminal after a DISPLAY MESSAGE is received
        /// </summary>
        /// <returns></returns>
        public string GetEFTProgressMessageCofirmationResponse()
        {
            //Used to change the C3 behavior.
            // “0000” : OK
            // “0310” : Void
            string Status = "0000";
            
            return string.Format("{0}{1}{2}{3}{4}", C3MessageTypes.PROGRESS_MESSAGE, ProgressType, Status, C3MessageTerminations.DLE, C3MessageTerminations.ETX);
        }
    }
}
