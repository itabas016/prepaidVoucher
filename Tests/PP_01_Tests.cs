using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NMock2;
using NMock2.Actions;
using NUnit.Framework;
using PayMedia.ApplicationServices.Finance.ServiceContracts;
using PayMedia.ApplicationServices.Finance.ServiceContracts.DataContracts;
using PayMedia.Framework.Integration.Contracts;
using PayMedia.Integration.FrameworkService.Interfaces.Common;
using PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.AscCashValid;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher.Tests
{
    [TestFixture]
    public class PP_01_Tests
    {
        public IComponentInitContext _context;

        public PP_01_Tests()
        {
            
        }

        [Test]
        [Category("MockTests")]
        public void PP_01_Mock_Success()
        {
            XmlNode settings = XmlUtilities.GetNode("/TestMessages/MessageGroup[@name='PrepaidVoucher']/Message[@name='PP_01_ConsumeVoucher']/MessageContent/CustomContent/Settings");
            int customerID = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "CustomerId"));
            int financialAccountId = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "FinancialAccountId"));
            long voucherTicketNumber = long.Parse(XmlUtilities.SafeSelectText(settings, "VoucherTicketNumber"));
            string asmUsername = XmlUtilities.SafeSelectText(settings, "ASMUsername");
            string asmPassword = XmlUtilities.SafeSelectText(settings, "ASMPassword");
            string asmDsn = XmlUtilities.SafeSelectText(settings, "ASMDsn");

            PrepaidVoucherService.PrepaidVoucher voucher = new PrepaidVoucherService.PrepaidVoucher();
            voucher.FinancialAccountId = financialAccountId;
            voucher.CustomerId = customerID;
            voucher.VoucherTicketNumber = voucherTicketNumber;

            PrepaidVoucherService.AuthenticationHeader header = new PrepaidVoucherService.AuthenticationHeader();
            header.Password = asmPassword;
            header.Username = asmUsername;
            header.Dsn = asmDsn;

            PrepaidVoucherService.PrepaidVoucherRequest request = new PrepaidVoucherService.PrepaidVoucherRequest(voucher, header);

            PrepaidVoucherServiceMock service = new PrepaidVoucherServiceMock();
            PrepaidVoucherService.PrepaidVoucherResponse response = service.ConsumeVoucher(request);
            service.VerifyExpectations();

            Console.WriteLine("Return code: {0}; Voucher Amount: {1}; FT ID: {2}", response.returnCode, response.prepaidVoucher.VoucherAmount, response.financialTransactionId);
            Assert.AreEqual(ReturnCode.VmsValidationSuccessful, response.returnCode, "Return code was not successful from web service call.");
        }

        [Test]
        [Category("MockTests")]
        public void PP_01_Mock_Auth_Failure()
        {
            PrepaidVoucherService.PrepaidVoucher voucher = new PrepaidVoucherService.PrepaidVoucher();
            voucher.FinancialAccountId = 123;
            voucher.CustomerId = 456;
            voucher.VoucherTicketNumber = 123456;

            PrepaidVoucherService.PrepaidVoucherRequest request = new PrepaidVoucherService.PrepaidVoucherRequest(voucher, null);

            List<ListenerConfiguration> listenerConfigurations = ListenerUtilities.GetListenerConfigurations();
            PrepaidVoucherService service = new PrepaidVoucherService();
            service.Initialize(listenerConfigurations.Find(x => x.Endpoint.Name == "InboundPrepaidVoucherListener"));
            PrepaidVoucherService.PrepaidVoucherResponse response = service.ConsumeVoucher(request);

            Console.WriteLine("Return code: {0}", response.returnCode.ToString());
            Assert.AreEqual(ReturnCode.IbsCiUnauthorized, response.returnCode, "Return code was not unauthorized from web service call.");
        }

        [Test]
        [Category("MockTests")]
        public void PP_01_Mock_Exception()
        {
            PrepaidVoucherService.PrepaidVoucher voucher = new PrepaidVoucherService.PrepaidVoucher();
            voucher.FinancialAccountId = 123;
            voucher.CustomerId = 456;
            voucher.VoucherTicketNumber = 123456;

            PrepaidVoucherService.AuthenticationHeader header = new PrepaidVoucherService.AuthenticationHeader();
            header.Password = "entriqeng";
            header.Username = "entriqeng";
            header.Dsn = "UnitTest";

            PrepaidVoucherService.PrepaidVoucherRequest request = new PrepaidVoucherService.PrepaidVoucherRequest(voucher, header);


            List<ListenerConfiguration> listenerConfigurations = ListenerUtilities.GetListenerConfigurations();
            PrepaidVoucherServiceExceptionMock service = new PrepaidVoucherServiceExceptionMock();
            service.Initialize(listenerConfigurations.Find(x => x.Endpoint.Name == "InboundPrepaidVoucherListener"));

            //PrepaidVoucherServiceExceptionMock service = new PrepaidVoucherServiceExceptionMock();
            service.exceptionText = "A custom error occurred.  This is the error text.";
            PrepaidVoucherService.PrepaidVoucherResponse response = service.ConsumeVoucher(request);
            Assert.AreEqual(ReturnCode.IbsCiSystemError, response.returnCode, "Exception did not return expected error code.");
        }

        [Test]
        [Category("MockTests")]
        public void PP_01_Mock_AlreadyConsumedVoucher()
        {
            PP_01_AlreadConsumed_Mock alreadConsumed_Mock = new PP_01_AlreadConsumed_Mock(new IntegrationMailMessage());

            string response;
            alreadConsumed_Mock.GetResponse(out response);
            Assert.AreEqual(response, "VmsVoucherAlreadyConsumed", "Expected response was NOT returned!");
        }

        [Test]
        [Explicit]
        public void RealFTTest()
        {
            XmlNode settings = TestConfiguration.GetNode("/TestMessages/MessageGroup[@name='PrepaidVoucher']/Message[@name='RealFTTest']/MessageContent/CustomContent/Settings");
            int customerId = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "CustomerId"));
            int financialAccountId = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "FinancialAccountId"));
            decimal baseAmount = ValidationUtilities.ParseDecimal(XmlUtilities.SafeSelectText(settings, "BaseAmount"));
            int ledgerAccountId = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "LedgerAccountId"));
            int currencyId = ValidationUtilities.ParseInt(XmlUtilities.SafeSelectText(settings, "CurrencyId"));
            string paymentReferenceNumber = XmlUtilities.SafeSelectText(settings, "PaymentReferenceNumber");
            string asmDsn = XmlUtilities.SafeSelectText(settings, "ASMDsn");


            FinancialTransactionCollection transactionsCollection = new FinancialTransactionCollection();
            FinancialTransaction transaction = new FinancialTransaction();
            transaction.CustomerId = customerId;
            transaction.FinancialAccountId = financialAccountId;
            transaction.BaseAmount = baseAmount;
            transaction.LedgerAccountId = ledgerAccountId;
            transaction.CurrencyId = currencyId;
            transaction.PaymentReferenceNumber = paymentReferenceNumber;

            transactionsCollection.Add(transaction);

            IFinanceService financeService = ServiceUtilities.GetService<IFinanceService>(asmDsn);

            financeService.CreatePaymentTransactions(transactionsCollection, PaymentReceiptNumberingMethod.Automatic);
        }

        #region Helper Classes

        private class PP_01_Mock : PP_01_ConsumeVoucher
        {
            private Mockery m_mock = null;

            public PP_01_Mock(IntegrationMailMessage args)
                : base(args)
            {
                m_mock = new Mockery();
            }

            protected override IASC_CASH_VALID GetVMSAscCashValidService()
            {
                IASC_CASH_VALID svcCashValid = m_mock.NewMock<IASC_CASH_VALID>();

                Expect.Once.On(svcCashValid)
                    .SetProperty("Url");

                Expect.Once.On(svcCashValid)
                    .Method("user_auth_amt")
                    .WithAnyArguments()
                    .Will(new SetNamedParameterAction("amount", (float)1.0), Return.Value(VmsReturnCode.Y.ToString()));

                Expect.Once.On(svcCashValid)
                      .Method("Dispose")
                      .WithAnyArguments();

                return svcCashValid;
            }

            protected override IFinanceService GetIBSFinanceService()
            {
                IFinanceService svcFinance = m_mock.NewMock<IFinanceService>();
                FinancialTransaction financialTransaction = new FinancialTransaction();
                financialTransaction.Id = 123;
                FinancialTransactionCollection col = new FinancialTransactionCollection(new FinancialTransaction[] { financialTransaction });
                col.TotalCount = 1;

                Expect.Once.On(svcFinance)
                    .Method("CreatePaymentTransactions")
                    .WithAnyArguments()
                    .Will(Return.Value(col));

                return svcFinance;
            }

            public void VerifyExpectations()
            {
                if (!(m_mock == null))
                    m_mock.VerifyAllExpectationsHaveBeenMet();
            }
        }

        private class PrepaidVoucherServiceMock : PrepaidVoucherService
        {
            private PP_01_Mock pp_01;

            public PrepaidVoucherServiceMock()
            {
            }

            protected override void DecrementActiveConnectionsPerfCounter()
            {
            }
            protected override void IncrementActiveConnectionsPerfCounter()
            {
            }
            protected override void IncrementErrorPerfCounter()
            {
            }
            protected override void IncrementItemRecNAvgExecPerfCounter(long start, long end)
            {
            }
            protected override void WriteCommLogEntry(string dsn, int customerId, long? historyId, string host, string messageString, PayMedia.Integration.CommunicationLog.ServiceContracts.CommunicationLogEntryMessageQualifier direction, string trackingId, string serialNumber)
            {
            }
            protected override void Process(Integration.Main.Framework.ResponseCommand voucherCommand, out string output)
            {
                voucherCommand.GetResponse(out output);
            }
            protected override PP_01_ConsumeVoucher GetVoucherCommand(List<BaseCommand> commands, IntegrationMailMessage mailMessage)
            {
                pp_01 = new PP_01_Mock(mailMessage);
                return pp_01;
            }

            protected override void LogonToASM(string p, string p_2, string p_3)
            {
            }

            public void VerifyExpectations()
            {
                pp_01.VerifyExpectations();
            }
        }

        private class PrepaidVoucherServiceExceptionMock : PrepaidVoucherService
        {
            public string exceptionText;

            private PP_01_Mock pp_01;

            public PrepaidVoucherServiceExceptionMock()
            {
            }

            protected override PP_01_ConsumeVoucher GetVoucherCommand(List<BaseCommand> commands, IntegrationMailMessage mailMessage)
            {
                throw new Exception(exceptionText);
            }

            protected override void LogonToASM(string p, string p_2, string p_3)
            {
            }

            public void VerifyExpectations()
            {
                pp_01.VerifyExpectations();
            }
        }

        private class PP_01_AlreadConsumed_Mock : PP_01_ConsumeVoucher
        {
            private Mockery m_mock = null;

            public PP_01_AlreadConsumed_Mock(IntegrationMailMessage args)
                : base(args)
            {
                m_mock = new Mockery();
            }

            protected override void InitWorker()
            {
            }

            protected override string VmsConsumeVoucher()
            {
                return "C";
            }

            public void VerifyExpectations()
            {
                if (!(m_mock == null))
                    m_mock.VerifyAllExpectationsHaveBeenMet();
            }
        }

        #endregion
    }
}
