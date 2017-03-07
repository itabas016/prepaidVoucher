using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PayMedia.Integration.IFComponents.BBCL.PrepaidVoucher
{
    /// <summary>
	/// Provides serialization utilities.
	/// </summary>
	/// <typeparam name="T">This is the type to serialize or deserialize and must be a class with a parameterless constructor.</typeparam>
    public static class SerializationUtilities<T> where T : class, new()
    {
        /// <summary>
		/// Provides serialization methods using the XmlSerializer.
		/// </summary>
		public static class Xml
        {
            /// <summary>
            /// Serializes an object to XML via the XmlSerializer.
            /// Note that the XmlSerializer does not support the serialization of interface data members.
            /// </summary>
            /// <param name="input">The object to serialze.</param>
            /// <returns></returns>
            public static string Serialize(T input)
            {
                return Xml.Serialize(input, false);
            }

            /// <summary>
            /// Serializes an object to XML via the XmlSerializer.
            /// Note that the XmlSerializer does not support the serialization of interface data members.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="omitXmlDeclaration">when true, the XML returned will NOT contain an XML declaration</param>
            /// <returns></returns>
            public static string Serialize(T input, bool omitXmlDeclaration)
            {
                return Xml.Serialize(input, null, omitXmlDeclaration);
            }

            /// <summary>
            /// Serializes an object to XML via the XmlSerializer.
            /// Note that the XmlSerializer does not support the serialization of interface data members.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="omitXmlDeclaration">when true, the XML returned will NOT contain an XML declaration</param>
            /// <param name="defaultNamespace">The default namespace to put on the root element of the serialized object</param>
            /// <returns></returns>
            public static string Serialize(T input, string defaultNamespace, bool omitXmlDeclaration)
            {
                string output;
                StringBuilder builder = new StringBuilder();
                XmlSerializer formatter;

                using (XmlWriter writer = XmlWriter.Create(builder))
                {
                    // if the caller wants the xml declartion....
                    if (omitXmlDeclaration == false)
                    {
                        // NOTE: The below serialization always outputs UTF-16 encoding,
                        // in the XML declaration.
                        // So if the caller wants the declaration, we prefer utf-8 for now, so .....
                        // Set the encoding.
                        writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    }

                    if (defaultNamespace == null)
                    {
                        // Set up an XmlSerializer to write to a stream.
                        formatter = new XmlSerializer(typeof(T));
                    }
                    else
                    {
                        // Set up an XmlSerializer to write to a stream.
                        formatter = new XmlSerializer(typeof(T), defaultNamespace);
                    }

                    // Serialize the object into the stream.
                    formatter.Serialize(writer, input);

                    // Return the text of the stream in UTF8 encoding.
                    output = builder.ToString();

                    // if the caller does NOT want an XML declaration in the xml returned....
                    if (omitXmlDeclaration)
                    {
                        // Okay, so this is lazy code to strip out any XML header information.
                        // I searched through the documentation on XmlWriter and XmlSerializer and could NOT
                        // find any way to stop the XmlSerializer from emitting a declaration.
                        // if you find a way to get these objects to not emit a XML Declaration please 
                        // update this code, and let me know how you did it.  2007.11.28 - JCopus
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(output);
                        output = xmlDoc.DocumentElement.OuterXml;
                    }
                }

                return output;
            }

            /// <summary>
            /// Deserializes an object of type T from XML via the XmlSerializer.
            /// </summary>
            /// <param name="content">The XML to deserialize.</param>
            /// <returns>The deserialized object of type T.</returns>
            public static T Deserialize(string content)
            {
                T output;

                using (MemoryStream stream = new MemoryStream())
                {
                    // Funnel the serialized content into a stream.
                    byte[] bytes = Encoding.UTF8.GetBytes(content);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Seek(0, 0);

                    // Deserialize the object from the stream.
                    XmlSerializer formatter = new XmlSerializer(typeof(T));
                    output = formatter.Deserialize(stream) as T;
                }

                return output;
            }

            /// <summary>
            /// Deserializes an object of type T from XML via the XmlSerializer.
            /// </summary>
            /// <param name="content">The XML to deserialize.</param>
            /// <param name="rootNamespace">The namespace of the root element.</param>
            /// <returns>The deserialized object of type T.</returns>
            public static T Deserialize(string content, string rootNamespace)
            {
                // 2010.02.19 - JCopus - Question: Why are we using a MemoryStream and 
                // explicit UTF8 encoding in he non-root namespace version of this method?
                // The XmlReader should interpret string encoding correctly.
                // I'm afraid to change the existing method for fear of breaking something
                // so I will leave it alone for now.
                using (XmlReader xr = XmlReader.Create(new StringReader(content)))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T), rootNamespace);
                    T output = (T)xs.Deserialize(xr);
                    return output;
                }
            }
        }
    }
}
