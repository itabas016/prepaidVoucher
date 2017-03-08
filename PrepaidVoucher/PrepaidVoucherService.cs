using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using PayMedia.Framework.Integration.Contracts;
using PayMedia.Integration.CommunicationLog.ServiceContracts;
using PayMedia.Integration.CommunicationLog.ServiceContracts.DataContracts;
using PayMedia.Integration.FrameworkService.Interfaces.Common;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{

    /// <summary>
    /// The PrepaidVoucher Web Service handles PrepaidVoucher Related Calls and forwards
    /// them along to the configured commands.
    /// It is a singleton service that will return a response when called based on its contract.
	/// </summary>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    [ServiceContract(Namespace = "PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher")]
    public class PrepaidVoucherService : WcfListenerBase
    {
        #region Fields and Ctor

        static long commLogSequenceId = 0;
        public IComponentInitContext _context;
        public ListenerConfiguration _configuration;

        public PrepaidVoucherService(IComponentInitContext initContext)
        {
            this._context = initContext;
            //initialize listener configuration
            //this._configuration = _context.Config[]
        }

        #endregion

        #region WcfListenerBase overrides and implementation
        public override void Initialize(ListenerConfiguration configuration)
        {
            try
            {
                base.Initialize(configuration);

                //construct command

            }
            catch (Exception ex)
            {
                string error = string.Format("Error occured in {0}.{1}()", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                throw new IntegrationException(error, ex);
            }
        }

        /// <summary>
        /// Starts listening.
        /// </summary>
        public override void Start()
        {
            base.RequestStart(this);
        }

        #endregion

        #region PrepaidVoucherService Members

        [OperationContract(IsOneWay = false)]
        public PrepaidVoucherResponse ConsumeVoucher(PrepaidVoucherRequest request)
        {
            return (this.GetPrepaidVoucherResponse(request));
        }

        #endregion

        #region Protected methods

        protected virtual void WriteCommLogEntry(int customerId, long? historyId, string host, string messageString, CommunicationLogEntryMessageQualifier direction, string trackingId, string serialNumber)
        {
            var service = ServiceUtilities.GetService<ICommunicationLogService>(_context);
            var logEntry = new CommunicationLogEntry
            {
                CustomerId = customerId,
                HistoryId = historyId,
                Host = host,
                Message = messageString,
                MessageQualifier = direction,
                MessageTrackingId = (string.IsNullOrEmpty(trackingId)) ? historyId.GetValueOrDefault().ToString() : trackingId,
                SerialNumber = serialNumber

            };
            logEntry.TimeStamp = DateTime.Now;
            service.CreateCommunicationLogEntry(logEntry);
        }

        // This call is here so that our unit tests can mock up the creation of the voucher command.
        protected virtual PP_01_ConsumeVoucher GetVoucherCommand(List<Command> commands, IntegrationMailMessage mailMessage)
        {
            return commands[0] as PP_01_ConsumeVoucher;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>PrepaidVoucherResponse</returns>
        private PrepaidVoucherResponse GetPrepaidVoucherResponse(PrepaidVoucherRequest request)
        {
            IntegrationMailMessage baseMailMessage = new IntegrationMailMessage();
            string inputXml = string.Empty;
            string output = string.Empty;

            bool knownException = false;
            // Create the response object.
            PrepaidVoucherResponse response = new PrepaidVoucherResponse();

            ReturnCode returnCode;
            try
            {
                // Set the voucher to use in the response as the original request voucher.
                response.prepaidVoucher = request.prepaidVoucher;

                // Assume error unless otherwise specified. Set voucher amount = 0;
                // Per IDD, On error: The Web Method returns zero ("0") for "amount" and one of four possible codes for error: 
                response.prepaidVoucher.VoucherAmount = 0;

                // Serialize the Prepaid Voucher to pass to the worrker.
                inputXml = SerializationUtilities<PrepaidVoucher>.Xml.Serialize(request.prepaidVoucher);

                baseMailMessage.UseCase = "IntegrationEventId";
                //baseMailMessage.Dsn = authHeader.Dsn;
                baseMailMessage.CustomerId = request.prepaidVoucher.CustomerId;

                WriteCommLogEntry(
                                    baseMailMessage.CustomerId,
                                    null,
                                    this.Name,
                                    inputXml,
                                    CommunicationLogEntryMessageQualifier.Receive,
                                    System.Threading.Interlocked.Increment(ref commLogSequenceId).ToString(),
                                    request.prepaidVoucher.VoucherTicketNumber.ToString()
                                    );

                //Value for UseCaseData will be hardcoded and is the only code that needs to change if 
                // a different worker is to be used to handle this message.
                baseMailMessage.UseCaseData = "-100";
                baseMailMessage.XmlDoc = inputXml;

                //CommandFactory commandFactory = new CommandFactory();
                //List<Command> commands = commandFactory.CreateCommands(baseMailMessage, listenerName);

                var commands = new List<Command>();

                // Ensure that we only only ever get one command!
                if (commands.Count != 1)
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiSystemError;

                    knownException = true;

                    throw new IntegrationException(string.Format("There MUST be one listener defined for Event {0}. The Command Count is {1}.\r\n", baseMailMessage.UseCaseData, commands.Count));
                }

                // ensure that the command that we did get is the type we expected.
                PP_01_ConsumeVoucher voucherCommand = GetVoucherCommand(commands, baseMailMessage);
                if (voucherCommand == null)
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiSystemError;

                    knownException = true;

                    throw new IntegrationException(string.Format("The listener defined for Event {0} MUST be of type {1} . The listener configured is {2}.\r\n", baseMailMessage.UseCaseData, typeof(PP_01_ConsumeVoucher).Name, commands[0].GetType().Name));
                }

                // execute our worker and get the response.
                voucherCommand.GetResponse(out output);

                // Exception should only be thrown in a rare case. It is a System Error.
                // normal response will be ftId and return code combine with char | eg: 1234|VmsValidationSuccessful
                if (string.IsNullOrEmpty(output) || !output.Contains("|"))
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiSystemError;
                    Diagnostics.Error(output);
                    return response;
                }

                string[] outputs = output.Split(new char[] { '|' });
                if (outputs.Length > 1)
                {
                    int financialTransactionId = int.Parse(outputs[0]);
                    response.financialTransactionId = financialTransactionId;
                    output = outputs[1];
                }

                // Obtain the enum value from the response code returned from the response command.
                returnCode = (ReturnCode)Enum.Parse(typeof(ReturnCode), output);

                // Set the voucher amount on the Response voucher from the worker.
                response.prepaidVoucher.VoucherAmount = voucherCommand.VoucherAmount;

                // Set the return code on the response.
                response.returnCode = returnCode;
            }
            catch (Exception ex)
            {
                // Log the error.
                if (!knownException)
                {
                    response.returnCode = ReturnCode.IbsCiSystemError;
                }
                // Log the error.
                string error = string.Format("Error in {0}.{1}() processing received message.\r\n\r\n{2}\r\n\nMessage content:\r\n{3}.", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.ToString(), SerializationUtilities<PrepaidVoucher>.Xml.Serialize(request.prepaidVoucher));
                Diagnostics.Error(error);
            }
            finally
            {
                string outputXml = SerializationUtilities<PrepaidVoucherResponse>.Xml.Serialize(response);

                WriteCommLogEntry(
                                    baseMailMessage.CustomerId,
                                    null,
                                    this.Name,
                                    outputXml,
                                    CommunicationLogEntryMessageQualifier.Send,
                                    System.Threading.Interlocked.Increment(ref commLogSequenceId).ToString(),
                                    request.prepaidVoucher.VoucherTicketNumber.ToString());
            }

            return response;
        }

        #endregion

        #region Service Model

        [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
        public partial class PrepaidVoucherRequest
        {
            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 0)]
            public PrepaidVoucherService.PrepaidVoucher prepaidVoucher;

            public PrepaidVoucherRequest()
            {
            }

            public PrepaidVoucherRequest(PrepaidVoucher prepaidVoucher)
            {
                this.prepaidVoucher = prepaidVoucher;
            }
        }

        [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
        public partial class PrepaidVoucherResponse
        {
            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 0)]
            public PrepaidVoucherService.PrepaidVoucher prepaidVoucher;

            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 1)]
            public ReturnCode returnCode;

            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 2)]
            public int financialTransactionId;

            // Error code enum here.
            //[System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/logistics", Order = 0)]
            //public Integration.Library.VideoCon.ThirdPartyEndpoints.PrepaidVoucher.PrepaidVoucherService.PrepaidVoucher prepaidVoucher;

            public PrepaidVoucherResponse()
            {
            }

            public PrepaidVoucherResponse(PrepaidVoucher prepaidVoucher, ReturnCode returnCode, int financialTransactionId)
            {
                this.prepaidVoucher = prepaidVoucher;
                this.returnCode = returnCode;
                this.financialTransactionId = financialTransactionId;
            }
        }

        /// <remarks/>
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher")]
        public partial class PrepaidVoucher
        {
            private long voucherTicketNumber;

            private int financialAccountId;

            private int customerId;

            private decimal voucherAmount = 0;

            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("VoucherTicketNumber", Order = 0)]
            public long VoucherTicketNumber
            {
                get
                {
                    return this.voucherTicketNumber;
                }
                set
                {
                    this.voucherTicketNumber = value;
                }
            }
            [System.Xml.Serialization.XmlElementAttribute("FinancialAccountId", Order = 1)]
            public int FinancialAccountId
            {
                get
                {
                    return this.financialAccountId;
                }
                set
                {
                    this.financialAccountId = value;
                }
            }
            [System.Xml.Serialization.XmlElementAttribute("CustomerId", Order = 2)]
            public int CustomerId
            {
                get
                {
                    return this.customerId;
                }
                set
                {
                    this.customerId = value;
                }
            }
            [System.Xml.Serialization.XmlElementAttribute("VoucherAmount", Order = 3)]
            public decimal VoucherAmount
            {
                get
                {
                    return this.voucherAmount;
                }
                set
                {
                    this.voucherAmount = value;
                }
            }
        }

        #endregion
    }
}