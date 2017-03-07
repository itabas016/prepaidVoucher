using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public class ListenerConfiguration
    {
        public WcfEndpoint Endpoint { get; set; }
    }

    public interface IListener
    {
        string Name { get; }
        void Initialize(ListenerConfiguration configuration);
        void Start();
        bool RequestStop(int secondsToWait);
        void ForceStop();
    }
}
