using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using PayMedia.ApplicationServices.Authentication.ServiceContracts;
using PayMedia.ApplicationServices.Authentication.ServiceContracts.DataContracts;
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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple,
         UseSynchronizationContext = false)]
    [ServiceContract(Namespace = "PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher")]
    public class PrepaidVoucherService : WcfListenerBase, IPrepaidVoucherService
    {
        #region Fields and Ctor

        public IComponentInitContext context;
        public WcfEndpoint wcfEndpoint;
        private int port;
        private static long commLogSequenceId = 0;

        public PrepaidVoucherService(IComponentInitContext initContext)
        {
            this.context = initContext;

            Int32.TryParse(context.Config["endpoint_port"], out port);
            wcfEndpoint = CreateWcfEndpoint(context.Config["endpoint_name"], port);
        }

        #endregion

        #region overrides and implementation

        /// <summary>
        /// Starts listening.
        /// </summary>
        public override void Start()
        {
            try
            {
                base.RequestStart(this);
            }
            catch (Exception ex)
            {
                string error = string.Format("Error occured in {0}.{1}()", this.GetType().Name,
                    MethodBase.GetCurrentMethod().Name);
                throw new IntegrationException(error, ex);
            }
        }

        public PrepaidVoucherResponse ConsumeVoucher(PrepaidVoucherRequest request)
        {
            return (this.GetPrepaidVoucherResponse(request));
        }

        #endregion

        #region Protected methods

        protected virtual void WriteCommLogEntry(int customerId, long? historyId, string host, string messageString,
            CommunicationLogEntryMessageQualifier direction, string trackingId, string serialNumber)
        {
            var service = ServiceUtilities.GetService<ICommunicationLogService>(context);
            var logEntry = new CommunicationLogEntry
            {
                CustomerId = customerId,
                HistoryId = historyId,
                Host = host,
                Message = messageString,
                MessageQualifier = direction,
                MessageTrackingId =
                    (string.IsNullOrEmpty(trackingId)) ? historyId.GetValueOrDefault().ToString() : trackingId,
                SerialNumber = serialNumber

            };
            logEntry.TimeStamp = DateTime.Now;
            service.CreateCommunicationLogEntry(logEntry);
        }

        // This call is here so that our unit tests can mock up the creation of the voucher command.
        protected virtual PP_01_ConsumeVoucher CreateVoucherCommand(IComponentInitContext context,
            ConsumeVoucherData data)
        {
            return new PP_01_ConsumeVoucher(context, data);
        }

        protected virtual IAuthenticationService GetAuthenticationService()
        {
            return ServiceUtilities.GetService<IAuthenticationService>(context);
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
            string output = string.Empty;
            bool knownException = false;

            // Initilize consume voucher.
            var consumeVoucherData = new ConsumeVoucherData(request);

            // Create the response object.
            PrepaidVoucherResponse response = new PrepaidVoucherResponse();

            try
            {
                // Get the Authentication Header.
                if (request.header == null)
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiUnauthorized;
                    knownException = true;
                    throw new IntegrationException("No Authentication Header was Provided.\r\n");
                }

                AuthenticationHeader authHeader = request.header;

                // Set the voucher to use in the response as the original request voucher.
                response.prepaidVoucher = request.prepaidVoucher;

                // Assume error unless otherwise specified. Set voucher amount = 0;
                // Per IDD, On error: The Web Method returns zero ("0") for "amount" and one of four possible codes for error: 
                response.prepaidVoucher.VoucherAmount = 0;

                if (string.IsNullOrEmpty(authHeader.Dsn))
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiUnauthorized;
                    knownException = true;
                    throw new IntegrationException("Authentication credentials are invalid. No Dsn was provided.\r\n");
                }

                try
                {
                    var authService = GetAuthenticationService();
                    var result = authService.AuthenticateByProof(new UserIdentity
                    {
                        UserName = authHeader.Username,
                        Dsns = new DsnCollection { new Dsn { Name = authHeader.Dsn } }
                    }, authHeader.Password, string.Empty);

                    if (result == null || !result.Authenticated.Value)
                    {
                        response.returnCode = ReturnCode.IbsCiUnauthorized;
                        knownException = true;
                        throw new IntegrationException("Authentication credentials are invalid. No Dsn was provided.\r\n");
                    }
                }
                catch (Exception ex)
                {
                    // Set the correct return code of the on the response.
                    response.returnCode = ReturnCode.IbsCiUnauthorized;

                    knownException = true;

                    // Just throw the exception.
                    throw ex;
                }

                // Serialize the Prepaid Voucher to pass to the worrker.
                var inputXml = SerializationUtilities<PrepaidVoucher>.Xml.Serialize(request.prepaidVoucher);

                WriteCommLogEntry(
                                    consumeVoucherData.CustomerId,
                                    null,
                                    this.Name,
                                    inputXml,
                                    CommunicationLogEntryMessageQualifier.Receive,
                                    System.Threading.Interlocked.Increment(ref commLogSequenceId).ToString(),
                                    consumeVoucherData.VoucherTicketNumber.ToString()
                                    );

                // ensure that the command that we did get is the type we expected.
                PP_01_ConsumeVoucher voucherCommand = CreateVoucherCommand(context, consumeVoucherData);

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
                var returnCode = (ReturnCode)Enum.Parse(typeof(ReturnCode), output);

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
                                    consumeVoucherData.CustomerId,
                                    null,
                                    this.Name,
                                    outputXml,
                                    CommunicationLogEntryMessageQualifier.Send,
                                    System.Threading.Interlocked.Increment(ref commLogSequenceId).ToString(),
                                    consumeVoucherData.VoucherTicketNumber.ToString());
            }

            return response;
        }

        private WcfEndpoint CreateWcfEndpoint(string name, int port)
        {
            var wcfEndpoint = new WcfEndpoint();

            wcfEndpoint.Name = name;
            wcfEndpoint.Address = string.Format("http://localhost:{0}/ibs.interprit.com/{1}", port, name);
            wcfEndpoint.Binding = "basicHttpBinding";
            wcfEndpoint.Contract = "PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher";
            wcfEndpoint.BindingConfiguration = "nonCertBinding";
            return wcfEndpoint;
        }

        #endregion

        #region Service Model

        [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
        public partial class PrepaidVoucherRequest
        {
            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 0)]
            public PrepaidVoucherService.AuthenticationHeader header;

            [System.ServiceModel.MessageBodyMemberAttribute(Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher", Order = 1)]
            public PrepaidVoucherService.PrepaidVoucher prepaidVoucher;

            public PrepaidVoucherRequest()
            {
            }

            public PrepaidVoucherRequest(PrepaidVoucher prepaidVoucher, AuthenticationHeader header)
            {
                this.header = header;
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

        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.ibsinterprit.com/integration/prepaidvoucher")]
        public partial class AuthenticationHeader
        {
            private string dsn;

            private string username;

            private string password;

            [System.Xml.Serialization.XmlElementAttribute("Username", Order = 0)]
            public string Username
            {
                get
                {
                    return this.username;
                }
                set
                {
                    this.username = value;
                }
            }
            [System.Xml.Serialization.XmlElementAttribute("Password", Order = 1)]
            public string Password
            {
                get
                {
                    return this.password;
                }
                set
                {
                    this.password = value;
                }
            }
            /// <remarks/>
            [System.Xml.Serialization.XmlElementAttribute("Dsn", Order = 2)]
            public string Dsn
            {
                get
                {
                    return this.dsn;
                }
                set
                {
                    this.dsn = value;
                }
            }
        }

        #endregion
    }
}