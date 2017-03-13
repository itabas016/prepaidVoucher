using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public class ConsumeVoucherData
    {
        public int CustomerId { get; private set; }
        public int FinancialAccountId { get; private set; }
        public long VoucherTicketNumber { get; private set; }

        public ConsumeVoucherData(PrepaidVoucherService.PrepaidVoucherRequest request)
        {
            CustomerId = request.prepaidVoucher.CustomerId;
            FinancialAccountId = request.prepaidVoucher.FinancialAccountId;
            VoucherTicketNumber = request.prepaidVoucher.VoucherTicketNumber;
        }

        public override string ToString()
        {
            return string.Format("\r\nCustomerId: {0}\r\nFinancialAccountId: {1}\r\nVoucherTicketNumber: {2}", CustomerId, FinancialAccountId, VoucherTicketNumber);
        }
    }
}
