using Acrelec.Mockingbird.Feather.Peripherals.Payment;
using Acrelec.Mockingbird.Feather.Peripherals.Payment.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Acrelec.Mockingbird.Feather.Peripherals.Enums;
using Acrelec.Mockingbird.Feather.Peripherals.Models;
using Acrelec.Mockingbird.Feather.Peripherals.Settings;
using Acrelec.Mockingbird.Interfaces.Peripherals;

namespace Acrelec.Mockingbird.Payment_UK_LANE3000
{
    [Export(typeof(IPayment))]
    public class Payment_UK_LANE3000 : IPaymentCard, ICommunicatorCallbacks
    {
        private const string PAYMENT_ID = "826018";

        private const string PAYMENT_LOG = "Payment_UK_LANE3000 ";

        private const string PAYMENT_NAME = "UK_LANE3000 ";

        private const string PAYMENT_APPLICATION_NAME = "PAY_UK_LANE3000 .exe";

        private const string PAYMENT_APPLICATION_PROCESS_NAME = "PAY_UK_LANE3000 ";

        private const string C3NET_CONFIG = "c3config";

        private const string DRIVER_FOLDER_NAME = "DriverExe";

        private const string TICKET_NAME = "ticket"; //ticket for the customer

        private const string MERCHANT_TICKET = "ticketmer"; //ticket for the merchant

        private const string VISA_RRP_DATA_VALUE = "DF6B10012E000100000BB90000000032204000DF6B0701022030004000DF6B1201AC000531026826200100000BB900000BB9DF6B1501AC0008310268261200000301000005DD000003E9DF6B1201AC0005310268261201000005DD000009C5DF6B1201AC0005310268260001000007D1000005DD";

        string DriverLocation;

        private const PeripheralType PAYMENT_TYPE = Feather.Peripherals.Enums.PeripheralType.card;

        /// <summary>
        /// Object used to record the specific error code and description
        /// </summary>
        private SpecificStatusDetails specificStatusDetails;

        /// <summary>
        /// Flag used by the Init method know when the Init response was received
        /// </summary>
        private bool IsInitFinished { get; set; }

        /// <summary>
        /// Flag used by the Test method know when the Test response was received
        /// </summary>
        private bool IsTestFinished { get; set; }

        /// <summary>
        /// Flag used by the private method to know when the Pay response was received
        /// </summary>
        private bool IsPayFinished { get; set; }

        /// <summary>
        /// Flag used to know if the Init Method was successful.
        /// The value of the flag is updated when the Init message response is received from the Payment Application
        /// </summary>
        private bool WasInitSuccessful { get; set; }

        /// <summary>
        /// Flag used to know if the Test Method was successful.
        /// The value of the flag is updated when the Test message response is received from the Payment Application
        /// </summary>
        private bool WasTestSuccessful { get; set; }

        /// <summary>
        /// Flag used to know if the Pay Method was successful.
        /// The value of the flag is updated when the Pay message response is received from the Payment Application
        /// </summary>
        private bool WasPaySuccessful { get; set; }

        /// <summary>
        /// The variable will be populated with the paid amount after the Pay method is called
        /// </summary>
        private PayDetails payDetails;

        /// <summary>
        /// The time interval when the payment driver will make a self check.
        /// (this variable will be passed in the init method)
        /// </summary>
        public int testTimeout;

        /// <summary>
        /// Object that will be used to communicate with the payment application
        /// </summary>
        private Communicator communicator;

        /// <summary>
        /// The factory details of the Payment including a list of settings
        /// </summary>
        private Payment currentPaymentInitConfig;

        /// <summary>
        /// The object that will be in charge with the Clear One Windows Service Management
        /// </summary>
        private C3NetManager c3NetManager;

        /// <summary>
        /// Used to set peripheral status
        /// </summary>
        public PeripheralStatus LastStatus => paymentStatus.ToPeripheralStatus();

        private PaymentStatus paymentStatus;


        public IPaymentCallbacks PaymentCallbacks { get; set; }

        public PaymentCapability Capability => new PaymentCapability()
        {
            AcceptsCash = false,
            CanRefund = false,
            ReceivePayProgressCalls = true,
            Type = PAYMENT_TYPE.ToString(),
            Name = PAYMENT_NAME
        };

        public string DriverId => PAYMENT_ID;

        /// <summary>
        /// Returns the name of the current payment
        /// </summary>
        public string PeripheralName => PAYMENT_NAME;

        /// <summary>
        /// Returns the type of the current payment
        /// </summary>
        public string PeripheralType => PAYMENT_TYPE.ToString();

        AdminPeripheralSetting comPort;

        AdminPeripheralSetting first_server;

        AdminPeripheralSetting second_server;

        AdminPeripheralSetting messageLineOne;

        AdminPeripheralSetting messageLineTwo;

        AdminPeripheralSetting cardApplications;

        AdminPeripheralSetting terminalID;

        AdminPeripheralSetting currency;

        /// <summary>
        /// Object in charge with log saving
        /// </summary>
        ILogger logger;

        [ImportingConstructor]
        public Payment_UK_LANE3000(ILogger logger = null)
        {
            this.logger = logger;

            paymentStatus = new PaymentStatus();

            communicator = new Communicator(logger);

            DriverLocation = $@"C:\Acrelec\Core\Peripherals\Payments\Drivers\{PAYMENT_NAME}\{DriverVersion}\Driver\{DRIVER_FOLDER_NAME}";

            //Create the object that is in charge with the sevice management
            c3NetManager = new C3NetManager(logger, DriverLocation);

            //Hook the payment callback to the current callback
            communicator.CommunicatorCallbacks = this;

            //COM PORT
            comPort = new AdminPeripheralSetting();
            comPort.ControlName = "COM Port";
            comPort.RealName = "L10_COM";
            //comPort.SettingFileName = C3NET_CONFIG;
            comPort.CurrentValue = "";
            comPort.ControlType = SettingDataType.SerialPortSelection;

            terminalID = new AdminPeripheralSetting();
            terminalID.ControlType = SettingDataType.String;
            terminalID.ControlName = "Terminal ID";
            terminalID.RealName = "QTPV";
            terminalID.CurrentValue = ""; //Set a default application name
            terminalID.ControlDescription = "The payment terminal ID.";

            //Init the Axis 1
            first_server = new AdminPeripheralSetting();
            first_server.ControlName = "Communication Server 1";
            first_server.RealName = "AXIS_COM";
            first_server.SettingFileName = C3NET_CONFIG;
            first_server.ControlDescription = "Ingenico communication Server. Example: 111.121.131.141 1234.";
            first_server.CurrentValue = "0.0.0.0 0000";
            first_server.SetttingSection = "";
            first_server.ControlType = SettingDataType.String;

            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("EUR", "Euro");
            dict.Add("GBP", "Pound");
            currency = new AdminPeripheralSetting();
            currency.ControlType = SettingDataType.SingleSelection;
            currency.ControlName = "Currency";
            currency.RealName = "Currency";
            currency.CurrentValue = "GBP";
            currency.PossibleValues = dict;
            currency.ControlDescription = "Flag that will control the currency of the payment.";

            //Init the Axis 2

            second_server = new AdminPeripheralSetting();
            second_server.ControlName = "Communication Server 2";
            second_server.RealName = "AXIS_COM2";
            second_server.SettingFileName = C3NET_CONFIG;
            second_server.ControlDescription = "Ingenico secondary communication Server. Example: 111.121.131.141 1234.";
            second_server.CurrentValue = "0.0.0.0 0000";
            second_server.SetttingSection = "";
            second_server.ControlType = SettingDataType.String;

            /// Idle Message line one
            messageLineOne = new AdminPeripheralSetting();
            messageLineOne.ControlName = "Welcome Message Line 1";
            messageLineOne.RealName = "REPOS_1";
            messageLineOne.SettingFileName = C3NET_CONFIG;
            messageLineOne.CurrentValue = IniFilesSimple.ReadString("REPOS_1", "", Path.Combine(DriverLocation, C3NET_CONFIG));
            messageLineOne.SetttingSection = "";
            messageLineOne.ControlType = SettingDataType.String;

            /// Idle Message line two
            messageLineTwo = new AdminPeripheralSetting();
            messageLineTwo.ControlName = "Welcome Message Line 2";
            messageLineTwo.RealName = "REPOS_2";
            messageLineTwo.SettingFileName = C3NET_CONFIG;
            messageLineTwo.CurrentValue = IniFilesSimple.ReadString("REPOS_2", "", Path.Combine(DriverLocation, C3NET_CONFIG));
            messageLineTwo.SetttingSection = "";
            messageLineTwo.ControlType = SettingDataType.String;

            //CARD APPLICATIONS
            cardApplications = new AdminPeripheralSetting();
            cardApplications.ControlName = "ACTIVE APPLICATIONS";
            cardApplications.RealName = "CARTES";
            cardApplications.SettingFileName = C3NET_CONFIG;
            cardApplications.CurrentValue = "ADM CPA";
            cardApplications.SetttingSection = "";
            cardApplications.ControlDescription = "Applications that will be active after the initialization of the terminal. Example: ADM EMV SSC";
            cardApplications.ControlType = SettingDataType.String;

            currentPaymentInitConfig = new Payment
            {
                PaymentName = PAYMENT_NAME,
                DriverFolderName = DRIVER_FOLDER_NAME,
                ConfigurationSettings = new List<AdminPeripheralSetting>(),
                Id = PAYMENT_ID,
                Type = PAYMENT_TYPE.ToString()
            };
        }

        /// <summary>
        /// Update the settings in all the configuration files with the ones stored in the Account
        /// This method will be called when the a device is Set from Admin or when a device is loaded at Core startup
        /// </summary>
        /// <param name="configJson"></param>
        /// <param name="overwrite"></param>
        /// <returns></returns>
        public bool UpdateSettings(string configJson, bool overwrite = false)
        {
            string exceptionMessage = "";
            try
            {
                //Deserialize the config object to get access to the driver settings
                Payment payment = JsonConvert.DeserializeObject<Payment>(configJson);

                //Check if the config object is empty
                if (currentPaymentInitConfig != payment)
                {

                    //Save the settings
                    foreach (AdminPeripheralSetting paymentSetting in payment.ConfigurationSettings)
                    {

                        switch (paymentSetting.RealName)
                        {
                            case "Currency":
                                currency = paymentSetting;
                                break;

                            case "L10_COM":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                comPort = paymentSetting;
                                break;
                            case "QTPV":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                terminalID = paymentSetting;
                                break;
                            case "AXIS_COM":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                first_server = paymentSetting;
                                break;
                            case "AXIS_COM2":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                second_server = paymentSetting;
                                break;
                            case "REPOS_1":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                messageLineOne = paymentSetting;
                                break;
                            case "REPOS_2":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                messageLineTwo = paymentSetting;
                                break;
                            case "CARTES":
                                //Update the c3config filed
                                if (!SetPaymentConfigSettings(paymentSetting, ref exceptionMessage))
                                {
                                    logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : {paymentSetting.ControlName}. {exceptionMessage}");
                                    return false;
                                }
                                cardApplications = paymentSetting;
                                break;
                        }
                    }

                    //Update the c3config "VISA_RRP_DATA" filed with this new Value. 
                    //This update it will not allow contact-less payments to be processed off-line(which we now can't do because of UK Law)
                    logger.Info(PAYMENT_LOG, "Updating c3config setting : VISA_RRP_DATA");

                    if (!IniFilesSimple.WriteValue("VISA_RRP_DATA", VISA_RRP_DATA_VALUE, Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage))
                    {
                        logger.Info(PAYMENT_LOG, $"Failed to update c3config setting : VISA_RRP_DATA. \r\n {exceptionMessage}");
                        return false;
                    }
                }
                //Return with failure if the payment config is empty
                else
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Failed to initialize payment settings.\r\n{0}", ex.ToString()));
            }
            return false;
        }

        private bool SetPaymentConfigSettings(AdminPeripheralSetting setting, ref string exceptionMessage)
        {
            try
            {
                logger.Info(PAYMENT_LOG, string.Format("Updating c3config setting : {0}", setting.ControlName));

                switch (setting.ControlType)
                {
                    case SettingDataType.String:

                        if (setting.RealName.Equals("AXIS_COM") || setting.RealName.Equals("AXIS_COM2"))
                        {
                            return IniFilesSimple.WriteValue(setting.RealName, string.Format("socket {0}", ((string)setting.CurrentValue).Trim()), Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);
                        }
                        //This is a treatment for the previous case where the COM was a string type not a SerialPortSelection type setting
                        else if (setting.RealName.Equals("L10_COM"))
                        {
                            //The user is only able to modify the com port, not the speed, parity and other properties of the COM
                            return IniFilesSimple.WriteValue(setting.RealName, string.Format("{0} 115200/8/1/0", ((string)setting.CurrentValue).Trim()), Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);
                        }
                        //For any other setting
                        else
                        {
                            return IniFilesSimple.WriteValue(setting.RealName, (string)setting.CurrentValue, Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);
                        }
                    case SettingDataType.SerialPortSelection:

                        //The user is only able to modify the com port, not the speed, parity and other properties of the COM
                        return IniFilesSimple.WriteValue(setting.RealName, string.Format("{0} 115200/8/1/0", ((string)setting.CurrentValue).Trim()), Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);

                    case SettingDataType.Int:
                        return IniFilesSimple.WriteValue(setting.RealName, setting.CurrentValue.ToString(), Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);

                    case SettingDataType.Bool:
                        return IniFilesSimple.WriteValue(setting.RealName, setting.CurrentValue.ToString(), Path.Combine(DriverLocation, C3NET_CONFIG), ref exceptionMessage);

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("SetPaymentConfigSettings : Failed to write payment setting.\r\n{0}", ex.ToString()));
                return false;
            }
        }

        public bool Init()
        {
            try
            {
                logger.Info(PAYMENT_LOG, "Init: Initializing payment.");

                if (!StartC3Net())
                    return false;

                //Start the pipe server so driver can wait for messages
                communicator.StartListening();

                //Start the driver Payment application (if it's already open try to close it before starting it)
                LaunchPaymentApplication();

                Thread.Sleep(2000);

                //Set the flag to false until a response is received from the payment application
                IsInitFinished = false;
                WasInitSuccessful = false;

                //If the message is not received by the payment application the method will fail
                if (!communicator.SendMessage(CommunicatorMethods.Init, new { TerminalID = terminalID.CurrentValue.ToString(), Currency = currency.CurrentValue.ToString(), TestTimeout = 3 }))
                    return false;

                //Wait until the payment application responds to the init message
                while (!IsInitFinished)
                {
                    Thread.Sleep(0);
                    Thread.Sleep(50);
                }

                logger.Info(PAYMENT_LOG, "Init: Finished Initializing payment.");

                return WasInitSuccessful;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Init: Failed to initialize payment.\r\n{0}", ex.ToString()));
            }
            return false;
        }

        public bool Test()
        {
            try
            {
                logger.Info(PAYMENT_LOG, "Test: Started testing payment.");

                //Set the flag to false until a response is received from the payment application
                IsTestFinished = false;
                WasTestSuccessful = false;

                //Check if the c3_rmp_net is started or if multiple instances of it are started.
                if (!c3NetManager.IsC3NetStarted() || c3NetManager.AreMultipleC3NetInstancesStarted())
                {
                    if (StartC3Net())
                    {
                        paymentStatus.CurrentStatus = PeripheralStatus.PeripheralOK().Status;
                        paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralOK().Description;
                        return true;
                    }
                    else
                    {
                        paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
                        paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
                        return false;
                    }
                }

                //If the message is not received by the payment application the method will fail
                if (!communicator.SendMessage(CommunicatorMethods.Test, new object()))
                {
                    paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
                    paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
                    return false;
                }

                //Wait until the payment application responds to the test message
                while (!IsTestFinished)
                {
                    Thread.Sleep(0);
                    Thread.Sleep(50);
                }

                logger.Info(PAYMENT_LOG, "Test: Finished testing payment.");

                if (WasTestSuccessful)
                {
                    paymentStatus.CurrentStatus = PeripheralStatus.PeripheralOK().Status;
                    paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralOK().Description;
                }
                else
                {
                    paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
                    paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
                }
                return WasTestSuccessful;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Test: Failed to test payment.\r\n{0}", ex.ToString()));
            }

            paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
            paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
            return false;
        }

        public bool Pay(PayRequest payRequest, ref PayDetails payDetails, ref SpecificStatusDetails specificStatusDetails, ref bool wasUncertainPaymentDetected)
        {
            try
            {
                logger.Info(PAYMENT_LOG, "Pay: Started payment.");

                //Init the payment details
                this.payDetails = new PayDetails();

                //Check if the c3_rmp_net is started or if multiple instances of it are started.
                if (!c3NetManager.IsC3NetStarted() || c3NetManager.AreMultipleC3NetInstancesStarted())
                {
                    logger.Info(PAYMENT_LOG, "   c3driver_net.exe is closed or has multiple instances opened.");
                    if (!StartC3Net())
                    {
                        logger.Info(PAYMENT_LOG, "   c3driver_net.exe restart failed.");
                        paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
                        paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
                        return false;
                    }
                }

                //Set the flag to false until a response is received from the payment application
                IsPayFinished = false;
                WasPaySuccessful = false;

                //Init the object that will be updated with the specific error code and description.
                this.specificStatusDetails = new SpecificStatusDetails();

                specificStatusDetails.StatusCode = PeripheralStatus.PeripheralGenericError().Status;
                specificStatusDetails.Description = PeripheralStatus.PeripheralGenericError().Description;

                //If the message is not received by the payment application the method will fail
                if (!communicator.SendMessage(CommunicatorMethods.Pay, (object)payRequest))
                {
                    paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
                    paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
                    return false;
                }

                //Wait until the payment application responds to the test message
                while (!IsPayFinished)
                {
                    Thread.Sleep(0);
                    Thread.Sleep(50);
                }

                logger.Info(PAYMENT_LOG, "Pay: Pay finished.");

                //Update the payment details reference
                payDetails = this.payDetails;

                if (WasPaySuccessful)
                {
                    paymentStatus.CurrentStatus = PeripheralStatus.PeripheralOK().Status;
                    paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralOK().Description;
                }

                specificStatusDetails = this.specificStatusDetails;

                return WasPaySuccessful;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Pay: Failed payment.\r\n{0}", ex.ToString()));
            }

            paymentStatus.CurrentStatus = PeripheralStatus.PeripheralGenericError().Status;
            paymentStatus.ErrorCodesDescription = PeripheralStatus.PeripheralGenericError().Description;
            return false;
        }


        public bool Unload()
        {
            logger.Info(PAYMENT_LOG, "Unload: started unloading payment.");
            try
            {
                communicator.Close();

                Process[] processesByName = Process.GetProcessesByName(PAYMENT_APPLICATION_PROCESS_NAME);
                if (processesByName.Length > 0)
                    processesByName[0].Kill();

                //Try to stop the payment windows service
                if (c3NetManager.StopC3Net())
                    logger.Info(PAYMENT_LOG, "  failed stop the payment windows service.");

                logger.Info(PAYMENT_LOG, "Unload: finished unloading payment.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Unload: \r\n{0}", ex.ToString()));
            }
            logger.Info(PAYMENT_LOG, "Unload: finished unloading payment.");
            return false;
        }

        public string GetPaymentFactoryDetails()
        {
            logger.Info(PAYMENT_LOG, "GetPaymentFactoryDetails: Started.");

            //Clear the payment configurations
            currentPaymentInitConfig.ConfigurationSettings.Clear();

            //Add the settings
            currentPaymentInitConfig.ConfigurationSettings.Add(comPort);
            currentPaymentInitConfig.ConfigurationSettings.Add(terminalID);
            currentPaymentInitConfig.ConfigurationSettings.Add(currency);
            currentPaymentInitConfig.ConfigurationSettings.Add(first_server);
            currentPaymentInitConfig.ConfigurationSettings.Add(second_server);
            currentPaymentInitConfig.ConfigurationSettings.Add(messageLineOne);
            currentPaymentInitConfig.ConfigurationSettings.Add(messageLineTwo);
            currentPaymentInitConfig.ConfigurationSettings.Add(cardApplications);

            JObject result = new JObject();
            JArray printersArray = new JArray();

            printersArray.Add(JObject.FromObject(currentPaymentInitConfig));

            result["Payment"] = printersArray;

            logger.Info(PAYMENT_LOG, "GetPaymentFactoryDetails: Finished.");

            return result.ToString();
        }

        /// <summary>
        /// Method will return the get minimum API version of the selected peripheral driver
        /// </summary>
        public int MinAPILevel => 3;

        /// <summary>
        /// Method will check the get version of the Payment driver application
        /// </summary>
        /// <returns>
        /// A string representing the version of of the Payment driver application '.exe'
        /// </returns>
        public string DriverVersion
        {
            get
            {
                try
                {
                    //var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(DriverLocation, PAYMENT_APPLICATION_NAME));
                    //return versionInfo.ProductVersion;
                    return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch (Exception ex)
                {
                    logger.Error(string.Format("Get Payment Driver Application Version: \r\n{0}", ex.Message));
                }
                return string.Empty;
            }
        }

        #region Unused Methods

        public bool ElectronicPay(PayRequest payRequest, ref PayDetails payDetails)
        {
            return true;
        }

        public bool StartPay()
        {
            return true;
        }

        public bool EndPay()
        {
            return true;
        }

        public bool StartAcceptingMoney()
        {
            return true;
        }

        public PeripheralStatus EndAcceptingMoney(AcceptMoneyRequest acceptMoneyRequest, ref AcceptMoneyDetails acceptMoneyDetails)
        {
            return null;
        }

        public List<int> AvailableCurrencies()
        {
            return new List<int>();
        }

        #endregion

        public void InitResponse(object parameters)
        {
            try
            {
                ResponseParameters responseParameters = JsonConvert.DeserializeObject<ResponseParameters>(parameters.ToString());

                //Check the status property of the parameters object to see if the Init was successful
                if (responseParameters.Status == 0)
                    WasInitSuccessful = true;
                else
                    WasInitSuccessful = false;

                //Notify the Init method that the message has stopped
                IsInitFinished = true;

            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Failed to validate the InitResponse. \r\n{0}", ex.ToString()));
            }
        }

        public void TestResponse(object parameters)
        {
            try
            {
                ResponseParameters responseParameters = JsonConvert.DeserializeObject<ResponseParameters>(parameters.ToString());

                //Check the status property of the parameters object to see if the Test was successful
                if (responseParameters.Status == 0)
                    WasTestSuccessful = true;
                else
                    WasTestSuccessful = false;

                //Notify the Init method that the message has stopped
                IsTestFinished = true;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Failed to validate the TestResponse. \r\n{0}", ex.ToString()));
            }
        }

        public void PayResponse(object parameters)
        {
            try
            {
                ResponseParameters responseParameters = JsonConvert.DeserializeObject<ResponseParameters>(parameters.ToString());

                //Update the payment details with the ones received from the payment terminal
                payDetails = responseParameters.PayDetails;

                //Check the status property of the parameters object to see if the Pay was successful
                if (responseParameters.Status == 0)
                {
                    WasPaySuccessful = true;
                }
                else
                {
                    WasPaySuccessful = false;
                }

                //Update the error code and error description
                specificStatusDetails.StatusCode = responseParameters.Status;
                specificStatusDetails.Description = responseParameters.Description;

                //Notify the Pay method that the message has stopped
                IsPayFinished = true;
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Failed to validate the PayResponse. \r\n{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Method that is triggered when money are been inserted in the CashMachine (a AmountUpdateResponse is received from the Cash Machine via pipes).
        /// </summary>
        public void ProgressMessageResponse(object parameters)
        {
            try
            {
                ResponseParameters responseParameters = JsonConvert.DeserializeObject<ResponseParameters>(parameters.ToString());

                logger.Info(PAYMENT_LOG, $"  Progress message:  {responseParameters.PayProgress.Message}");

                PayProgress payProgress = responseParameters.PayProgress;

                if (PaymentCallbacks != null)
                    PaymentCallbacks.PayProgressChangedCallback(payProgress);
            }
            catch (Exception ex)
            {
                logger.Error(PAYMENT_LOG, string.Format("Failed to send Progress Message Response. \r\n{0}", ex.ToString()));
            }
        }

        /// <summary>
        /// Check if the Payment Application is started and if it is stop it and start it again
        /// </summary>
        private void LaunchPaymentApplication()
        {
            try
            {
                //Stop the payment application
                Process[] processesByName = Process.GetProcessesByName(PAYMENT_APPLICATION_PROCESS_NAME);
                if (processesByName.Length > 0)
                {
                    processesByName[0].Kill();
                    logger.Info(PAYMENT_LOG, "Application was closed");
                    Thread.Sleep(3000);
                }
                else
                {
                    logger.Info(PAYMENT_LOG, string.Format("No '{0}' process was found.", PAYMENT_APPLICATION_PROCESS_NAME));
                }

                //Start the application
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = true;
                startInfo.FileName = PAYMENT_APPLICATION_NAME;
                startInfo.WorkingDirectory = DriverLocation;

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("StartPaymentApplication: \r\n{0}", ex.ToString()));
            }
        }

        private bool StartC3Net()
        {
            //Check if the c3driver_net.exe is started and if it is restart it
            if (c3NetManager.IsC3NetStarted())
            {
                logger.Info(PAYMENT_LOG, "  c3driver_net.exe is already installed.");

                //Try to stop the process
                if (!c3NetManager.StopC3Net())
                    logger.Info(PAYMENT_LOG, "  failed to stop c3driver_net.exe.");
                else
                    logger.Info(PAYMENT_LOG, "  successfully stopped c3driver_net.exe.");

                //check again if the process is stopped
                if (c3NetManager.IsC3NetStarted())
                {
                    logger.Info(PAYMENT_LOG, "  c3driver_net.exe is still opened.");
                    return false;
                }

                //Try to start service 
                if (!c3NetManager.StartC3Net())
                    logger.Info(PAYMENT_LOG, "  failed to start the payment process.");
                else
                    logger.Info(PAYMENT_LOG, "  successfully started the payment process.");

                //check again if the process was started
                if (!c3NetManager.IsC3NetStarted())
                {
                    logger.Info(PAYMENT_LOG, "  c3driver_net.exe failed to start.");
                    return false;
                }
                return true;
            }
            else
            {
                //Try to start service 
                if (!c3NetManager.StartC3Net())
                    logger.Info(PAYMENT_LOG, "  failed to start the payment windows service.");
                else
                    logger.Info(PAYMENT_LOG, "  successfully started the payment windows service.");

                //check again if the process was started
                if (!c3NetManager.IsC3NetStarted())
                {
                    logger.Info(PAYMENT_LOG, "  c3driver_net.exe failed to start.");
                    return false;
                }
                return true;
            }
        }
    }
}
