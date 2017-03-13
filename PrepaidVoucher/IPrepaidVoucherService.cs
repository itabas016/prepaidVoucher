using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    [ServiceContract]
    public interface IPrepaidVoucherService
    {
        [OperationContract(IsOneWay = false)]
        PrepaidVoucherService.PrepaidVoucherResponse ConsumeVoucher(PrepaidVoucherService.PrepaidVoucherRequest request);
    }
}
