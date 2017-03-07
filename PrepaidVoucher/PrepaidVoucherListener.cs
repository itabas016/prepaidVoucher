using PayMedia.Integration.FrameworkService.Interfaces;
using PayMedia.Integration.FrameworkService.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public class PrepaidVoucherListener : IFComponent, IDisposable
    {
        private PrepaidVoucherService _service;

        public PrepaidVoucherListener(IComponentInitContext initContext)
        {
            try
            {
                _service = new PrepaidVoucherService(initContext);

                _service.Initialize(_service._configuration);

                _service.Start();
            }
            catch (Exception)
            {
                
                throw;
            }

        }

        ~PrepaidVoucherListener()
        {
            //listener force stop
            _service.ForceStop();
        }

        public void Dispose()
        {
            //listener request stop
            _service.RequestStop(1000);
        }

        IMessageAction IFComponent.Process(IMsgContext msgContext)
        {
            //This is a listner
            throw new NotImplementedException();
        }
    }
}
