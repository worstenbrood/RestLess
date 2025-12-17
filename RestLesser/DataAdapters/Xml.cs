using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// Generic static <see cref="XmlSerializer"/> cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Xml<T>
    {
        private static readonly XmlSerializer _xmlSerializer = new(typeof(T));

        /// <summary>
        /// Return static instance
        /// </summary>
        public static XmlSerializer Serializer => _xmlSerializer;

        /// <summary>
        /// Serialize an object to a string
        /// </summary>
        /// <param name="data"></param>
        /// <param name="writerSettings"></param>
        /// <param name="serializerNamespaces"></param>
        /// <returns></returns>
        public static string Serialize(
            T data,
            XmlWriterSettings writerSettings = null,
            XmlSerializerNamespaces serializerNamespaces = null)
        {
            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, writerSettings);
            _xmlSerializer.Serialize(writer, data, serializerNamespaces);
            return stream.ToString();
        }

        /// <summary>
        /// Deserialize a string to an object
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize(string data)
        {
            using var reader = new StringReader(data);
            return (T)_xmlSerializer.Deserialize(reader);
        }
    }
}
