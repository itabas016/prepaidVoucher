using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    /// <summary>
	/// This class represents the structure of a WCF endpoint.
	/// </summary>
	[Serializable]
    public class WcfEndpoint
    {
        public string Name;
        public string Address;
        public string Binding;
        public string Contract;
        public string BindingConfiguration;
        public string WorkerConditionName;
        public string WorkerConditionValue;
    }
}
