using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public abstract class WcfListenerBase : IListener
    {
        protected ListenerConfiguration _configuration;
        protected string _listenerName;

        private WcfEndpoint endpoint;
        private ServiceHost serviceHost;

        public string Name
        {
            get { return _listenerName; }
        }

        public virtual void Initialize(ListenerConfiguration configuration)
        {
            this._configuration = configuration;
            this.endpoint = (WcfEndpoint) configuration.Endpoint;
            this._listenerName = endpoint.Name;
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
            serviceHost = ServiceUtilities.CreateServiceHost(endpoint, contract);
            serviceHost.Open();
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
