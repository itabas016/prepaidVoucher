using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayMedia.Integration.FrameworkService.Interfaces.Common;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public class ConfigureSetting
    {
        public const string VMS_WS_URL = "vmsWSUrl";
        public const string VMS_WS_USERCODE = "vmsUserCode";
        public const string VMS_WS_PASS = "vmsUserPass";
        public const string PREPAID_VOUCHER_LEDGER_ACCOUNT_ID = "prepaidVoucherLedgerAccountID";
        public const string PREPAID_VOUCHER_CURRENCY_ID = "prepaidVoucherCurrencyID";

        public string VmsWSUrl;
        public string VmsUserCode;
        public string VmsUserPass;
        public int PrepaidVoucherLedgerAccountId;
        public int PrepaidVoucherCurrencyId;

        public ConfigureSetting(IComponentInitContext context)
        {
            VmsWSUrl = context.Config[VMS_WS_URL];
            VmsUserCode = context.Config[VMS_WS_USERCODE];
            VmsUserPass = context.Config[VMS_WS_PASS];

            int.TryParse(context.Config[PREPAID_VOUCHER_LEDGER_ACCOUNT_ID], out PrepaidVoucherLedgerAccountId);
            int.TryParse(context.Config[PREPAID_VOUCHER_CURRENCY_ID], out PrepaidVoucherCurrencyId);
        }
    }
}
