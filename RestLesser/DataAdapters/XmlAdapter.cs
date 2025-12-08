using System.IO;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// XML (de)serializer
    /// </summary>
    public class XmlAdapter : IDataAdapter
    {
        /// <inheritdoc/>
        public MediaTypeWithQualityHeaderValue MediaTypeHeader { get; } =
            new MediaTypeWithQualityHeaderValue("application/xml");

        /// <summary>
        /// Namespaces used for writing
        /// </summary>
        public readonly XmlSerializerNamespaces Namespaces =
            new ();
                
        /// <inheritdoc/>
        public T Deserialize<T>(string data)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(data))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// XML writer settings
        /// </summary>
        protected static readonly XmlWriterSettings XmlWriterSettings =
            new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = new string(' ', 2)
            };


        /// <inheritdoc/>
        public string Serialize<T>(T data)
        {
            using (var stream = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stream, XmlWriterSettings))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    if (Namespaces.Count == 0)
                    {
                       Namespaces.Add(string.Empty, string.Empty);
                    }
                    serializer.Serialize(writer, data, Namespaces);
                    return stream.ToString();
                }
            }
        }
    }
}
