using Acrelec.Library.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PAY_UK_LANE3000.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using PAY_UK_LANE3000.C3NetCommunicator;
using PAY_UK_LANE3000.Communicator;

namespace PAY_UK_LANE3000
{

    public class PayIngenicoLANE3000 : ICommunicatorCallbacks
    {
        public const string PAY_INGENICO_LANE3000_LOG = "Pay_Ingenico_LANE3000";

        /// <summary>
        /// Object that is used for the communication with the Core Payment Driver
        /// </summary>
        private CoreCommunicator coreCommunicator;

        private C3NetCommunicator.C3NetCommunicator c3NetCommunicator;

        /// <summary>
        /// Timer that will be used to check if the terminal is ready for payment.
        /// It will send an init command to the terminal.
        /// Timer will be reset if a payment is succesfull
        /// </summary>
        private System.Timers.Timer periodicalCheckTimer;

        /// <summary>
        /// Flag that will be updated by the periodic terminal test.
        /// Its value will reflect the current status of the terminal.
        /// True - terminal is ready
        /// False - terminal is NOT ready
        /// </summary>
        public bool IsTerminalReady;

        /// <summary>
        /// Object that will be updated when the EFT_END response message is received from the Payment terminal.
        /// It usually contains the result of an operation (Initialization, Sale).
        /// </summary>
        private EFTEndMessage eftEndResponse;

        /// <summary>
        /// When the EFT_BEGIN is sent an EFT_END response is received from the terminal.
        /// This flag is set to true when the EFT_BEGIN is sent and set to false when the EFT_END response is received.
        /// Based on it's value the WaitingEFTEndResponse will stop the waiting loop 
        /// </summary>
        private volatile bool waitingEFTEndResponse;

        /// <summary>
        /// When the EFT_BEGIN is sent an EFT_BEGIN CONFIRMATION response is received from the terminal.
        /// This flag is set to true when the EFT_BEGIN is sent and set to false when the EFT_BEGIN CONFIRMATION response is received.
        /// Based on it's value the WaitingEFTBeginConfirmationResponse will stop the waiting loop 
        /// </summary>
        private volatile bool waitingEFTBeginResponse;

        /// <summary>
        /// Object that will help with the variable locking
        /// </summary>
        //private object lockObject;

        /// <summary>
        /// The ID of the terminal
        /// </summary>
        private string TerminalID;

        /// <summary>
        /// Time periode when the status of the terminal will be checked
        /// </summary>
        private int TestTimeout;

        /// <summary>
        /// The currency that will be used to make a payment
        /// </summary>
        private string CurrencyValue;

        /// <summary>
        /// Property that will give access to the callback methods
        /// </summary>
        private ICommunicatorCallbacks CommunicatorCallbacks { get; set; }

        public PayIngenicoLANE3000(CoreCommunicator coreCommunicator)
        {
            this.coreCommunicator = coreCommunicator;

            //Hook the callback methods of the communicator to the ones of current class 
            coreCommunicator.CommunicatorCallbacks = this;

            //Try to start the communication with the Visual Plugin socket
            c3NetCommunicator = new C3NetCommunicator.C3NetCommunicator();
            c3NetCommunicator.OnReceivedMessageEventHandler += DoOnReceivedMessageEventHandler;
            c3NetCommunicator.OnSendMessageEventHandler += DoOnSendMessageEventHandler;

            //Set the Periodical Check Timer
            periodicalCheckTimer = new System.Timers.Timer();
            periodicalCheckTimer.Elapsed += ElapsedPeriodicalCheckTimer;
        }

        /// <summary>
        /// Flag that will be used to prevent 2 or more callback methods simultaneous execution
        /// </summary>
        public bool IsCallbackMethodExecuting;

        /// <summary>
        /// Method that is called when the Core requests the EFT initialization.
        /// </summary>
        /// <param name="parameters"></param>
        public void InitRequest(object parameters)
        {
            Task.Run(() => { Init(parameters); });
        }

        /// <summary>
        /// Method that is called when the Core requests the EFT initialization.
        /// This method will make an initialization request to the c3driver_net.exe.
        /// Response will be provided back to the Core based on the EFT initialization response.
        /// </summary>
        public void Init(object parameters)
        {
            Log.Info(PAY_INGENICO_LANE3000_LOG, "call Initialize");

            //Check if another method is executing
            if (IsCallbackMethodExecuting)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = -1 });

                Log.Info(PAY_INGENICO_LANE3000_LOG, "        another method is executing.");

                Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Initialize");

                return;
            }
            else
                IsCallbackMethodExecuting = true;

            periodicalCheckTimer.Stop();

            waitingEFTBeginResponse = false;

            //Get the initialization parameters
            if (!GetInitParameters(parameters.ToString()))
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = -1 });

                Log.Info(PAY_INGENICO_LANE3000_LOG, "        failed to deserialize the init parameters.");
            }

            //Connect to the C3Net 
            if (!ConnectToC3Net())
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = 332, Description = "the connection is down" });
            }
            else
            {
                //Reset the waiting for response flag
                waitingEFTBeginResponse = true;

                //Reset the waiting for response flag
                waitingEFTEndResponse = true;

                //Create the result object
                eftEndResponse = new EFTEndMessage();

                //Create the eft begin init request object
                EFTBeginMessage eftBeginRequestMessage = new EFTBeginMessage();

                //Send the eft begin init request
                string requestString = eftBeginRequestMessage.GetPinpadInitializationMessage(TerminalID);

                if (!c3NetCommunicator.Send(requestString))
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = -334, Description = "failed to send Initialization request" });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        failed to send initialization request.");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Initialize");
                    return;
                }

                //Wait the eft begin confirmationinit response
                if (!WaitingEFTBeginConfirmationResponse())
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = -335, Description = "No EFT Begin Confirmation was received" });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        No EFT Begin Confirmation was received");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Initialize");
                    return;
                }
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        received EFT Begin Confirmation.");

                //Wait eft end response
                if (!WaitingEFTEndResponse())
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = -336, Description = "No EFT End Response was received" });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        No EFT End Response was received");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Initialize");
                    return;
                }
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        received EFT End Confirmation.");


                if (eftEndResponse.Error == 0)
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = 0, Description = "Ready" });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        EFT succesfully initialized");

                    CheckPayment();

                    //Start the terminal periodic check
                    periodicalCheckTimer.Start();
                }
                else
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Code: {0}", eftEndResponse.Error);
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Code Response: {0}", eftEndResponse.ErrorResponseCode);
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Message:{0}", eftEndResponse.ErrorMessage);

                    coreCommunicator.SendMessage(CommunicatorMethods.Init, new { Status = eftEndResponse.Error, Description = eftEndResponse.ErrorMessage });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        EFT failed to initialized");
                }
            }

            c3NetCommunicator.Disconnect();

            IsCallbackMethodExecuting = false;

            Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Initialize");
        }

        public void PayRequest(object parameters)
        {
            Task.Run(() => { Pay(parameters); });
        }

        /// <summary>
        /// Method that is called when the Core requests a payment .
        /// This method will make a transaction request to the c3driver_net.exe.
        /// Response will be provided back to the Core based on the EFT initialization response.
        /// </summary>
        public void Pay(object parameters)
        {
            Log.Info(PAY_INGENICO_LANE3000_LOG, "call Pay");

            //Check if another method is executing
            if (IsCallbackMethodExecuting)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = 297, Description = "Another method is executing", PayDetails = new PayDetails() });

                Log.Info(PAY_INGENICO_LANE3000_LOG, "        another method is executing.");

                Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Pay");

                return;
            }
            else
                IsCallbackMethodExecuting = true;

            periodicalCheckTimer.Stop();

            //Get the pay request object that will be sent to the fiscal printer
            Log.Info(PAY_INGENICO_LANE3000_LOG, "        deserialize the pay request parameters.");
            PayRequest payRequest = GetPayRequest(parameters.ToString());

            //Check if the pay deserialization was successful
            if (payRequest == null)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -331, Description = "failed to deserialize the pay request parameters", PayDetails = new PayDetails() });
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        failed to deserialize the pay request parameters.");
                return;
            }
            Log.Info(PAY_INGENICO_LANE3000_LOG, "        deserialized pay request parameters.");

            if (payRequest.Amount == 0)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -299, Description = "amount can't be zero.", PayDetails = new PayDetails() });
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        amount can't be zero.");
                return;
            }

            if (string.IsNullOrEmpty(payRequest.TransactionReference))
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -298, Description = "transaction reference can't be empty.", PayDetails = new PayDetails() });
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        transaction reference can't be empty.");
                return;
            }

            //Connect to the C3Net 
            if (!ConnectToC3Net())
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -332, Description = "the connection is down", PayDetails = new PayDetails() });
            }
            else
            {
                //Reset the waiting for response flag
                waitingEFTBeginResponse = true;

                //Reset the waiting for response flag
                waitingEFTEndResponse = true;

                //Create the result object
                eftEndResponse = new EFTEndMessage();

                //Create the eft begin init request object
                EFTBeginMessage eftBeginRequestMessage = new EFTBeginMessage();

                //Based on the selected currency create the appropriate command
                string requestString = string.Empty;
                if (CurrencyValue == "EUR")
                    requestString = eftBeginRequestMessage.GetTransactionMessage(TerminalID, Currency.EUR.ToString(), payRequest.Amount.ToString(), payRequest.TransactionReference);
                if (CurrencyValue == "GBP")
                    requestString = eftBeginRequestMessage.GetTransactionMessage(TerminalID, Currency.GPB.ToString(), payRequest.Amount.ToString(), payRequest.TransactionReference);

                //Send the eft begin init request
                if (!c3NetCommunicator.Send(requestString))
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -334, Description = "failed to send Initialization request", PayDetails = new PayDetails() });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        failed to send initialization request.");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Pay");
                    return;
                }

                //Wait the eft begin confirmationinit response
                if (!WaitingEFTBeginConfirmationResponse())
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -335, Description = "No EFT Begin Confirmation was received", PayDetails = new PayDetails() });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        No EFT Begin Confirmation was received");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Pay");
                    return;
                }
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        received EFT Begin Confirmation.");

                //Wait eft end response
                if (!WaitingEFTEndResponse())
                {
                    coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = -336, Description = "No EFT End Response was received", PayDetails = new PayDetails() });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        No EFT End Response was received");
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Pay");
                    return;
                }
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        received EFT End Confirmation.");

                PayDetails payDetails = new PayDetails();

                if (eftEndResponse.Error == 0)
                {
                    //Update the paid amount
                    payDetails.PaidAmount = payRequest.Amount;

                    //Update the tender media id
                    payDetails.TenderMediaId = eftEndResponse.TenderMedia;

                    //Save the ticket
                    if (!string.IsNullOrEmpty(eftEndResponse.CustomerTicket))
                    {
                        SaveTicket(eftEndResponse.CustomerTicket, false);
                        payDetails.HasClientReceipt = true;
                    }
                    //Save the merchant ticket
                    if (!string.IsNullOrEmpty(eftEndResponse.MerchantTicket))
                    {
                        SaveTicket(eftEndResponse.MerchantTicket, true);
                        payDetails.HasMerchantReceipt = true;
                    }

                    //Reset the timer if a payment was successful
                    periodicalCheckTimer.Stop();
                    periodicalCheckTimer.Start();

                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Tender Media: {0}", eftEndResponse.TenderMedia);

                    coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = 0, Description = "Successful payment", PayDetails = payDetails });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        credit card payment succeeded.");
                }
                else
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Code: {0}", eftEndResponse.Error);
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Code Response: {0}", eftEndResponse.ErrorResponseCode);
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        Error Message:{0}", eftEndResponse.ErrorMessage);

                    //Save the ticket
                    if (!string.IsNullOrEmpty(eftEndResponse.CustomerTicket))
                    {
                        SaveTicket(eftEndResponse.CustomerTicket, false);
                        payDetails.HasClientReceipt = true;
                    }
                    //Save the merchant ticket
                    if (!string.IsNullOrEmpty(eftEndResponse.MerchantTicket))
                    {
                        SaveTicket(eftEndResponse.MerchantTicket, true);
                        payDetails.HasMerchantReceipt = true;
                    }

                    coreCommunicator.SendMessage(CommunicatorMethods.Pay, new { Status = eftEndResponse.Error, Description = eftEndResponse.ErrorMessage, PayDetails = payDetails });
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        credit card payment failed.");
                }
            }

            c3NetCommunicator.Disconnect();

            periodicalCheckTimer.Start();

            IsCallbackMethodExecuting = false;

            Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Pay");
        }

        /// <summary>
        /// Method that will save the ticket and the merchant ticket to the disk
        /// </summary>
        /// <param name="ticketContent"></param>
        /// <param name="isMerchantTicket"></param>
        private void SaveTicket(string ticketContent, bool isMerchantTicket)
        {
            try
            {
                string applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (isMerchantTicket)
                {
                    //Try to delete the previous ticket
                    if (File.Exists(Path.Combine(applicationPath, "ticketmer")))
                        File.Delete(Path.Combine(applicationPath, "ticketmer"));

                    //Save the ticket
                    File.WriteAllText(Path.Combine(applicationPath, "ticketmer"), ticketContent);
                }
                else
                {
                    //Try to delete the previous ticket
                    if (File.Exists(Path.Combine(applicationPath, "ticket")))
                        File.Delete(Path.Combine(applicationPath, "ticket"));

                    //Save the ticket
                    File.WriteAllText(Path.Combine(applicationPath, "ticket"), ticketContent);
                }
            }
            catch (Exception ex)
            {
                Log.Error(PAY_INGENICO_LANE3000_LOG, string.Format("        failed to save ticket.\r\n{0}", ex.ToString()));
            }
        }

        public void TestRequest(object parameters)
        {
            Task.Run(() => { Test(parameters); });
        }

        /// <summary>
        /// Method that is called when the Core requests the status of the EFT.
        /// This method will return the current status of the EFT.
        /// The method will NOT send a status request to the EFT, it will provide the last status 
        /// evaluated by the PeriodicalStatusCheck.
        /// </summary>
        public void Test(object parameters)
        {
            //Check if another method is executing
            if (IsCallbackMethodExecuting)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Test, new { Status = -1 });

                Log.Info(PAY_INGENICO_LANE3000_LOG, "        another method is executing (Stopped Test Method execution).");

                return;
            }
            else
                IsCallbackMethodExecuting = true;

            Log.Info(PAY_INGENICO_LANE3000_LOG, "call Test");

            if (IsTerminalReady)
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Test, new { Status = 0 });
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        success.");
            }
            else
            {
                coreCommunicator.SendMessage(CommunicatorMethods.Test, new { Status = -1 });
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        failed.");
            }

            IsCallbackMethodExecuting = false;

            Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall Test");
        }

        /// <summary>
        /// It will try send a command to the terminal to get the total number of transactions 
        /// (no ping method exists so this one was the only one that was available to test the terminal connection)
        /// </summary>
        private void CheckPayment()
        {
            Log.Info(PAY_INGENICO_LANE3000_LOG, "        checking payment status");

            periodicalCheckTimer.Stop();

            //Connect to the C3Net
            if (!ConnectToC3Net())
            {
                IsTerminalReady = false;
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        the connection is down.");
            }
            else
            {
                //Change the waiting for response flag
                waitingEFTBeginResponse = true;
                //Change the waiting for response flag
                waitingEFTEndResponse = true;

                eftEndResponse = new EFTEndMessage();

                //SEND THE EFT BEGIN INIT REQUEST
                EFTBeginMessage eftBeginRequestMessage = new EFTBeginMessage();

                string requestString = eftBeginRequestMessage.GetPinpadStatusMessage(TerminalID);
                if (!c3NetCommunicator.Send(requestString))
                {
                    UpdateAndRestartThePeriodicalCkeckStatus(false, "failed to send status request.");
                    return;
                }

                //WAIT THE EFT BEGIN CONFIRMATION STATUS RESPONSE
                if (!WaitingEFTBeginConfirmationResponse())
                {
                    UpdateAndRestartThePeriodicalCkeckStatus(false, "No EFT Begin Confirmation was received");
                    return;
                }
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        received EFT Begin Confirmation.");

                //WAIT EFT END RESPONSE
                if (!WaitingEFTEndResponse())
                {
                    UpdateAndRestartThePeriodicalCkeckStatus(false, "No EFT End Response was received");
                    return;
                }

                if (eftEndResponse.Error == 0)
                    UpdateAndRestartThePeriodicalCkeckStatus(true, "EFT ready.");
                else
                    UpdateAndRestartThePeriodicalCkeckStatus(false, "EFT NOT ready.");
            }

            //Start the terminal periodic check
            periodicalCheckTimer.Start();

            c3NetCommunicator.Disconnect();
        }

        /// <summary>
        /// Method that is called when the periodicalCheckTimer is elapsed.
        /// Method will call the CheckPayment() which will verify the terminal status by sending a Status request to the EFT.
        /// If the driver is currently executing another operation like Init or Pay or Test this method will not be executed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ElapsedPeriodicalCheckTimer(object sender, ElapsedEventArgs args)
        {
            try
            {
                //Only test the terminal if no other method is executing
                if (IsCallbackMethodExecuting)
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        periodical check postponed");
                }
                else
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "call PeriodicalCheck");
                    IsCallbackMethodExecuting = true;
                    CheckPayment();
                    IsCallbackMethodExecuting = false;
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall PeriodicalCheck");
                }
            }
            catch (Exception ex)
            {
                Log.Info(PAY_INGENICO_LANE3000_LOG, string.Format("        ElapsedPeriodicalCheckTimer: {0}", ex.ToString()));
                Log.Info(PAY_INGENICO_LANE3000_LOG, "endcall PeriodicalCheck");
            }
        }

        /// <summary>
        /// Deserialize the received json string into a PayRequest object
        /// </summary>
        /// <param name="jsonItems"></param>
        /// <returns></returns>
        private PayRequest GetPayRequest(string payRequestJSonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<PayRequest>(payRequestJSonString);
            }
            catch (Exception ex)
            {
                Log.Info(PAY_INGENICO_LANE3000_LOG, string.Format("        GetPayRequest: {0}", ex.ToString()));
            }

            return null;
        }

        /// <summary>
        /// Method that is executed when a message is received from the EFT via Socket.
        /// This method will analize the message and based on it's type change the appropriate flag 
        /// to notify the methods that are waiting for a message from the EFT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DoOnReceivedMessageEventHandler(object sender, ReceivedMessageEventArgs args)
        {
            //If the message is a EFT BEGIN Confirmation
            if (args.Message.IndexOf(C3MessageTypes.EFT_BEGIN) == 0)
            {
                //If the EFT BEGIN INIT request was sent change the flag to signal that the confirmation was received
                if (waitingEFTBeginResponse)
                {
                    waitingEFTBeginResponse = false;
                }

            }
            //If the message is a EFT END Confirmation
            else if (args.Message.IndexOf(C3MessageTypes.EFT_END) == 0)
            {
                //If the EFT END request was sent change the flag to signal that the confirmation was received
                if (waitingEFTEndResponse)
                {
                    eftEndResponse = new EFTEndMessage(args.Message);

                    waitingEFTEndResponse = false;
                }
            }

            //If the message is a DISPLAY MESSAGE send the DISPLAY MESSAGE confirmation 
            else if (args.Message.IndexOf(C3MessageTypes.POS_DISPLAY) == 0)
            {
                EFTPOSDispplayMessage eFTPOSDispplayMessage = new EFTPOSDispplayMessage(args.Message);

                //RESPOND_IMEDIATLY_ACTION
                if (eFTPOSDispplayMessage.ActionType == EFTPOSDisplayMessageActionTypes.RESPOND_IMEDIATLY_ACTION)
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        [DISPLAY POS MESSAGE] {0} [RIC]", eFTPOSDispplayMessage.Description);

                }
                //RESPOND_WITH_1SEC_DELAY
                else if (eFTPOSDispplayMessage.ActionType == EFTPOSDisplayMessageActionTypes.RESPOND_WITH_1SEC_DELAY)
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        [DISPLAY POS MESSAGE] {0} [RWD]", eFTPOSDispplayMessage.Description);

                    //Wait 1 second before sending the response message
                    Thread.Sleep(1000);
                }

                //RESPOND_AFTER USER CONFIRMATION
                else if (eFTPOSDispplayMessage.ActionType == EFTPOSDisplayMessageActionTypes.WAIT_FOR_AKNOLEDGEMENT_ACTION)
                {
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        [DISPLAY POS MESSAGE] {0} [AFC]", eFTPOSDispplayMessage.Description);
                }

                //Wait for the operator to press the "ENTER"
                Task.Run(() => { SendPosDisplayResponse(eFTPOSDispplayMessage.GetEFTPOSDispplayMessageCofirmationResponse()); });

                //Send progress message to Core
                SendProgressMessage(eFTPOSDispplayMessage.Description);
            }
            else if (args.Message.IndexOf(C3MessageTypes.PROGRESS_MESSAGE) == 0)
            {
                EFTProgressMessage eFTProgressMessage = new EFTProgressMessage(args.Message);

                Log.Info(PAY_INGENICO_LANE3000_LOG, "        [PROGRESS MESSAGE]");

                Task.Run(() => { SendProgressMessageResponse(eFTProgressMessage.GetEFTProgressMessageCofirmationResponse()); });
            }
        }

        private void DoOnSendMessageEventHandler(object sender, SendMessageEventArgs args)
        {
            //do nothing
        }

        /// <summary>
        /// Method is called when a Display message is received from the EFT.
        /// This method will foreward the message to the Core which will pass it to the application 
        /// that will display it on the screen
        /// </summary>
        private void SendProgressMessage(string message)
        {
            object payProgress = new
            {
                MessageClass = "CheckPINPADDisplay",
                Message = message
            };

            Log.Info(PAY_INGENICO_LANE3000_LOG, "        Sending Progress Message to Core");

            coreCommunicator.SendMessage(CommunicatorMethods.ProgressMessage, new { PayProgress = payProgress });
        }

        /// <summary>
        /// Method will send a response to the EFT to confirm that the Display message was received
        /// </summary>
        private void SendPosDisplayResponse(string message)
        {
            if (!c3NetCommunicator.Send(message))
            {
                //Make another attempt to sent the Display message confirmation 
                if (c3NetCommunicator.Send(message))
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        [DISPLAY POS MESSAGE] Send receival confirmation (second attept)");
            }
            else
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        [DISPLAY POS MESSAGE] Send receival confirmation");
        }

        /// <summary>
        /// Method will send a response to the EFT to confirm that the Display message was received
        /// </summary>
        private void SendProgressMessageResponse(string message)
        {
            if (!c3NetCommunicator.Send(message))
            {
                //Make another attempt to sent the Display message confirmation 
                if (c3NetCommunicator.Send(message))
                    Log.Info(PAY_INGENICO_LANE3000_LOG, "        [PROGRESS MESSAGE] Send receival confirmation (second attept)");
            }
            else
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        [PROGRESS MESSAGE] Send receival confirmation");
        }

        /// <summary>
        /// Method will try to establish a socket connection with the Visual plugin
        /// </summary>
        /// <returns>
        /// True - successfully to establish connection
        /// False - failed to establish connection
        /// </returns>
        private bool ConnectToC3Net()
        {
            //Connect to the visual plugin
            Log.Info(PAY_INGENICO_LANE3000_LOG, "        trying to establish the connection to the C3Net.");

            c3NetCommunicator.AcceptConnection(IPAddress.Parse("127.0.0.1"), 9518);
            //Wait for the connection to be established of for the timeout to be triggered
            while (c3NetCommunicator.WasConnected == false && c3NetCommunicator.WasDisconnected == false)
            {
                Thread.Sleep(50);
            }

            //Check the socket connection
            if (!c3NetCommunicator.WasConnected)
            {
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        connection to the C3Net is down.");
                return false;
            }
            else
            {
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        connection to the C3Net was established.");
                return true;
            }
        }

        /// <summary>
        /// Deserialize the received json string into a PayRequest object
        /// </summary>
        /// <param name="jsonItems"></param>
        /// <returns></returns>
        private bool GetInitParameters(string initJson)
        {
            try
            {
                JObject jObject = JObject.Parse(initJson);

                if (jObject == null)
                    return false;

                if (jObject["TerminalID"] == null || jObject["TestTimeout"] == null || jObject["Currency"] == null)
                    return false;

                CurrencyValue = jObject["Currency"].ToString();
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        Currency: {0}", CurrencyValue);

                TerminalID = jObject["TerminalID"].ToString();
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        Terminal ID: {0}", TerminalID);


                TestTimeout = Convert.ToInt32(jObject["TestTimeout"].ToString()) * 60 * 1000;
                Log.Info(PAY_INGENICO_LANE3000_LOG, "        Test Timeout: {0}", TestTimeout);

                //Update the periodical check timer with the value of the C3
                periodicalCheckTimer.Interval = TestTimeout;

                return true;
            }
            catch (Exception ex)
            {
                Log.Info(PAY_INGENICO_LANE3000_LOG, string.Format("        GetInitParameters: {0}", ex.ToString()));
            }
            return false;
        }

        /// <summary>
        /// Method that is called after the Initialization request was send to the C3Net to receive the response
        /// Timeout 30 seconds
        /// </summary>
        /// <returns></returns>
        private bool WaitingEFTBeginConfirmationResponse()
        {
            //Wait maxim 5 seconds for the response
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalSeconds < 30)
            {
                if (!waitingEFTBeginResponse)
                {
                    //Stop waiting because the response was received
                    break;
                }
                Thread.Sleep(0);
                Thread.Sleep(100);
            }

            if (waitingEFTBeginResponse)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Method that is called after the the EFT Begin confirmation was received
        /// Timeout 5 minutes
        /// </summary>
        /// <returns></returns>
        private bool WaitingEFTEndResponse()
        {
            //Wait maxim 5 minutes for the response
            DateTime startTime = DateTime.Now;
            while (DateTime.Now.Subtract(startTime).TotalMinutes < 5)
            {
                if (!waitingEFTEndResponse)
                {
                    break; //Stop waiting because the response was received
                }
                Thread.Sleep(0);
                Thread.Sleep(100);
            }

            if (waitingEFTEndResponse)
                return false;
            else
                return true;
        }

        public void UpdateAndRestartThePeriodicalCkeckStatus(bool result, string message)
        {
            //Log the message 
            Log.Info(PAY_INGENICO_LANE3000_LOG, "        {0}", message);

            //Change the terminal state
            IsTerminalReady = result;

            //restart the check counter 
            periodicalCheckTimer.Start();

            //disconnect from the c3Net Server
            c3NetCommunicator.Disconnect();
        }
    }
}
