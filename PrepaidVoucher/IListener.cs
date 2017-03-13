using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public interface IListener
    {
        string Name { get; }
        void Initialize(WcfEndpoint wcfEndpoint);
        void Start();
        bool RequestStop(int secondsToWait);
        void ForceStop();
    }
}
