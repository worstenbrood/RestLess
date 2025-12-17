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

        /// <inheritdoc/>
        public T Deserialize<T>(string data)
        {
            return Xml<T>.Deserialize(data);
        }

        /// <summary>
        /// Namespaces used for writing
        /// </summary>
        public readonly XmlSerializerNamespaces Namespaces =
            new([new XmlQualifiedName(string.Empty, string.Empty)]);

        /// <summary>
        /// XML writer settings
        /// </summary>
        protected static readonly XmlWriterSettings XmlWriterSettings =
            new ()
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = new string(' ', 2)
            };

        /// <inheritdoc/>
        public string Serialize<T>(T data)
        {
            return Xml<T>.Serialize(data, XmlWriterSettings, Namespaces);
        }
    }
}
