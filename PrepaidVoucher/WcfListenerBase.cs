using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public abstract class WcfListenerBase : IListener
    {
        protected WcfEndpoint wcfEndpoint;
        protected string listenerName;
        protected ServiceHost serviceHost;

        public string Name
        {
            get { return listenerName; }
        }

        public virtual void Initialize(WcfEndpoint wcfEndpoint)
        {
            this.wcfEndpoint = wcfEndpoint;
            this.listenerName = wcfEndpoint.Name;
        }

        ~WcfListenerBase()
        {
            ForceStop();
        }

        /// <summary>
        /// Starts up the service host.
        /// </summary>
        public abstract void Start();

        protected virtual void RequestStart(object contract)
        {
            // Start up the service host.
            serviceHost = ServiceUtilities.CreateServiceHost(wcfEndpoint, contract);
            serviceHost.Open();
            Diagnostics.Info(string.Format("{0} host is starting up.", Name));
        }

        /// <summary>
        /// Requests this listener to stop.
        /// </summary>
        /// <param name="secondsToWait">Number of seconds to wait for currently executing work to complete.</param>
        public virtual bool RequestStop(int secondsToWait)
        {
            bool successfullyStopped = true;

            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            if (successfullyStopped == true)
                GC.SuppressFinalize(this);

            return successfullyStopped;
        }

        public virtual void ForceStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
