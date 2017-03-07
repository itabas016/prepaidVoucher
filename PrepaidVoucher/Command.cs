using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using PayMedia.Framework.Integration.Contracts;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    [Serializable]
    public class Command
    {
        [NonSerialized]
        [XmlIgnore]
        protected IntegrationMailMessage baseMailMessage;

        public IntegrationMailMessage BaseMailMessage
        {
            get { return baseMailMessage; }
        }
    }
}
