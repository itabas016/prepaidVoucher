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

        public override string ToString()
        {
            if (baseMailMessage == null)
            {
                return "IntegrationMailMessage is null";
            }
            else
            {
                return string.Format("\r\nHistoryID: {0}\r\nUseCase: {1}\r\nUseCaseData: {2}\r\nCustomerID: {3}\r\nUserID: {4}\r\nUsername: {5}\r\nDsn: {6}",
                        baseMailMessage.Id, baseMailMessage.UseCase, baseMailMessage.UseCaseData, baseMailMessage.CustomerId, baseMailMessage.HistoryUserId, baseMailMessage.HistoryUserName, baseMailMessage.Dsn);
            }
        }
    }
}
