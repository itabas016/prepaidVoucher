using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public enum VmsReturnCode
    {
        Y, /* VMS Validation Successful */
        U, /* VMS Unauthorized Access */
        N, /* VMS Validation Unsuccessful */
        C, /* VMS Voucher Already Consumed */
        E  /* VMS System Error */
    }

    public enum ReturnCode
    {
        VmsValidationSuccessful,
        VmsUnauthorizedAccess,
        VmsValidationUnsuccessful,
        VmsVoucherAlreadyConsumed,
        VmsSystemError,

        IbsCiUnauthorized,
        IbsCiUnknownVMSResponse,
        IbsCiVmsCommunicationFailure,
        IbsCiIBSCommunicationFailure,
        IbsCiSystemError
    }
}
