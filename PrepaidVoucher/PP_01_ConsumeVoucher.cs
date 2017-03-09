using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Linq;
using PayMedia.ApplicationServices.Finance.ServiceContracts;
using PayMedia.ApplicationServices.Finance.ServiceContracts.DataContracts;
using PayMedia.Integration.FrameworkService.Interfaces.Common;
using PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    [Serializable]
    public class PP_01_ConsumeVoucher : Command
    {
        #region private fields

        private int _customerId;
        private int _financialAccountId;
        private long _voucherTicketNumber;
        private decimal _voucherAmount;
        private ConfigureSetting _setting;
        private IComponentInitContext _context;

        #endregion

        #region Ctor

        public PP_01_ConsumeVoucher(IComponentInitContext context)
        {
            try
            {
                _context = context;
                _setting = new ConfigureSetting(context);

            }
            catch (Exception ex)
            {
                throw new IntegrationException("Error while initializing componet configuration", ex);
            }
        }

        #endregion

        #region Public methods

        public decimal VoucherAmount
        {
            get { return _voucherAmount; }
        }

        public void GetResponse(out string response)
        {
            ReturnCode responseCode = ReturnCode.VmsValidationSuccessful;
            response = string.Empty;
            long financialTransactionId = 0;

            try
            {
                InitCommandParameters();
            }
            catch (Exception ex)
            {
                HandleError(string.Format("System Error occured while initializing 'PP_01_ConsumeVoucher' command.\r\n" +
                    "The error encountered was: '{0}'", ex.Message), ex);
                responseCode = ReturnCode.IbsCiSystemError;
                response = responseCode.ToString();
                return;
            }

            TraceInformation(string.Format("'PP_01_ConsumeVoucher' command for voucher ticket number '{0}' has been successfully initialized.", _voucherTicketNumber));

            string vmsResponseString = string.Empty;

            try
            {
                // Call the VMS WebService
                vmsResponseString = VmsConsumeVoucher();
            }
            catch (Exception ex)
            {
                HandleError(string.Format("Error Communicating with VMS web service occured while processing 'PP_01_ConsumeVoucher' message for voucher ticket number '{0}'.\r\n" + "The error encountered was: '{1}'", _voucherTicketNumber, ex.Message), ex);
                responseCode = ReturnCode.IbsCiVmsCommunicationFailure;
                response = responseCode.ToString();
                return;
            }

            VmsReturnCode vmsResponse = VmsReturnCode.Y;

            try
            {
                vmsResponse = (VmsReturnCode)Enum.Parse(typeof(VmsReturnCode), vmsResponseString, true);
            }
            catch (Exception ex)
            {
                HandleError(string.Format("VMS web service returned unexpected returncode '{0}' while processing 'PP_01_ConsumeVoucher' message for voucher ticket number '{1}'.\r\n" + "The error encountered was: '{2}'", vmsResponseString, _voucherTicketNumber, ex.Message), ex);
                responseCode = ReturnCode.IbsCiUnknownVMSResponse;
                response = responseCode.ToString();
                return;
            }

            responseCode = (ReturnCode)vmsResponse;

            if (responseCode != ReturnCode.VmsValidationSuccessful)
            {
                string message = string.Format("VMS web service returned error code '{0} ({1})' while processing 'PP_01_ConsumeVoucher' message for voucher ticket number '{2}'.\r\n",
                    vmsResponseString, responseCode.ToString(), _voucherTicketNumber);

                // 2010.12.01 IBSO 19840 - JCopus - Per this issue BBCL requests that we handle the returned 
                // error code 'C (VmsVoucherAlreadyConsumed)' as a Warning instead of an error.
                if (vmsResponseString == "C")
                    HandleWarning(message, null);
                else
                    HandleError(message, null);
                response = responseCode.ToString();
            }
            else if (_voucherAmount == 0m)
            {
                HandleError(string.Format("VMS web service returned unexpected returncode '{0}' while processing 'PP_01_ConsumeVoucher' message for voucher ticket number '{1}'.\r\n" +
                    "The error encountered was: '{2}'", vmsResponseString, _voucherTicketNumber, "Voucher Amount can not be 0."), null);
                responseCode = ReturnCode.IbsCiUnknownVMSResponse;
                response = responseCode.ToString();
            }
            else // VMS Communication Successfull so Proceed
            {
                TraceInformation(string.Format("'PP_01_ConsumeVoucher' worker for voucher ticket number '{0}' has successfully received '{1}' return code from VMS web service.", _voucherTicketNumber, vmsResponseString));
                TraceInformation(string.Format("'PP_01_ConsumeVoucher' worker for voucher ticket number '{0}' information:\r\n" + "Prepaid voucher number '{0}' successfuly consumed by VMS System.", _voucherTicketNumber));

                // Create manual (prepay) financial transaction in IBS
                try
                {
                    FinancialTransaction financialTransaction = CreatePrepayFinancialTransaction();
                    if (financialTransaction == null)
                        throw new IntegrationException("Unable to determine Financial Transaction ID from IBS CreatePaymentTransactions() call.");
                    financialTransactionId = financialTransaction.Id.Value;

                    TraceInformation(string.Format("'PP_01_ConsumeVoucher' worker for voucher ticket number '{0}' information:\r\n" + "Manual financial transaction for prepaid voucher number '{0}' has been created and successfully uploaded to IBS.", _voucherTicketNumber));
                    responseCode = ReturnCode.VmsValidationSuccessful;
                }
                catch (Exception ex)
                {
                    HandleError(string.Format("Error occured while creating or uploading a manual financial transaction for prepaid voucher number '{0}' while processing 'PP_01_ConsumeVoucher' message for voucher ticket number '{0}'.\r\n" +
                        "The error encountered was: '{1}'", _voucherTicketNumber, ex.Message), ex);
                    responseCode = ReturnCode.IbsCiIBSCommunicationFailure;
                }
                response = string.Format("{0}|{1}", financialTransactionId, responseCode);
            }
        }

        #endregion

        #region protected methods

        protected virtual void InitCommandParameters()
        {
            XmlDocument docMsg = new XmlDocument();
            docMsg.LoadXml(BaseMailMessage.XmlDoc);

            // Get values from mail message
            _customerId = BaseMailMessage.CustomerId;

            string namespacePrefix = "p";

            // Create namespace manager.
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(docMsg.NameTable);
            // Setting the prefix so there is no need to refer to the full namespace.
            namespaceManager.AddNamespace(namespacePrefix, "http://www.ibsinterprit.com/integration/prepaidvoucher");

            _financialAccountId = int.Parse(XmlUtilities.SafeSelectText(docMsg, "//p:FinancialAccountId", namespaceManager));
            _voucherTicketNumber = long.Parse(XmlUtilities.SafeSelectText(docMsg, "//p:VoucherTicketNumber", namespaceManager));
        }

        protected virtual string VmsConsumeVoucher()
        {
            float floatAmount = 0;

            using (IASC_CASH_VALID vmsWS = GetVMSAscCashValidService())
            {
                vmsWS.Url = _setting.VmsWSUrl;
                /* This parameter Amount is 0 for now. Per Jordi/Sergio this paramater is not used in current implementation. */
                string vmsResponseString = vmsWS.user_auth_amt(_voucherTicketNumber.ToString(), _setting.VmsUserCode, _setting.VmsUserPass, 0, out floatAmount);
                _voucherAmount = new decimal(floatAmount);

                return vmsResponseString;
            }
        }

        protected virtual IASC_CASH_VALID GetVMSAscCashValidService()
        {
            return new ASC_CASH_VALID();
        }

        protected virtual IFinanceService GetFinanceService()
        {
            return ServiceUtilities.GetService<IFinanceService>(_context);
        }

        private FinancialTransaction CreatePrepayFinancialTransaction()
        {
            FinancialTransactionCollection transactionsCollection = new FinancialTransactionCollection();
            FinancialTransaction transaction = new FinancialTransaction();
            transaction.CustomerId = _customerId;
            transaction.FinancialAccountId = _financialAccountId;
            transaction.BaseAmount = _voucherAmount;
            transaction.LedgerAccountId = _setting.PrepaidVoucherLedgerAccountId;
            transaction.CurrencyId = _setting.PrepaidVoucherCurrencyId;
            transaction.PaymentReferenceNumber = _voucherTicketNumber.ToString();

            transactionsCollection.Add(transaction);

            IFinanceService financeService = GetFinanceService();
            transactionsCollection = financeService.CreatePaymentTransactions(transactionsCollection, PaymentReceiptNumberingMethod.Automatic);
            if (transactionsCollection.Items.Any())
                return transactionsCollection.Items[0];
            return null;
        }

        // This method is not used until it is clear if combination of 
        // prepaidVoucherLedgerAccountID and prepaidVoucherCurrencyID should come from IBS ASM per DSN or IC Config.
        protected static int GetPrepaidVoucherCurrencyID(int prepaidVoucherLedgerAccountID)
        {
            throw new NotImplementedException();
        }

        protected virtual void TraceInformation(string message)
        {
            Diagnostics.Info(string.Format("\r\n\r\n{0}Message Content: {1}", message, baseMailMessage.ToString()));
        }

        protected virtual void HandleWarning(string message, Exception innerException)
        {
            string tmpMessage = (innerException == null) ? message : new IntegrationException(message, innerException).ToString();

            Diagnostics.Warning(string.Format("\r\n\r\n{0}Message Content: {1}", tmpMessage, baseMailMessage.ToString()));
        }

        protected virtual void HandleError(string message, Exception innerException)
        {
            string tmpMessage = (innerException == null) ? message : new IntegrationException(message, innerException).ToString();

            Diagnostics.Error(string.Format("\r\n\r\n{0}Message Content: {1}", tmpMessage, baseMailMessage.ToString()));
        }

        #endregion
    }
}
