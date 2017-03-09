using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    public static class XmlUtilities
    {
        public static string SafeSelectText(XmlNode parent, string xpath)
        {
            return SafeSelectText(parent, xpath, string.Empty);
        }

        public static string SafeSelectText(XmlNode parent, string xpath, string alternateValue)
        {
            XmlNode node = parent.SelectSingleNode(xpath);

            // if the node does NOT exist OR if the value of the node is NULL or empty, then return the alternate value
            string value = node == null ? alternateValue : string.IsNullOrEmpty(node.InnerText) ? alternateValue : node.InnerText;

            return value;
        }

        public static string SafeSelectText(XmlNode parent, string xpath, XmlNamespaceManager namespaceManager)
        {
            return SafeSelectText(parent, xpath, string.Empty, namespaceManager);
        }

        public static string SafeSelectText(XmlNode parent, string xpath, string alternateValue, XmlNamespaceManager namespaceManager)
        {
            XmlNode node = parent.SelectSingleNode(xpath, namespaceManager);

            // if the node does NOT exist OR if the value of the node is NULL or empty, then return the alternate value
            string value = node == null ? alternateValue : string.IsNullOrEmpty(node.InnerText) ? alternateValue : node.InnerText;

            return value;
        }
    }
}
