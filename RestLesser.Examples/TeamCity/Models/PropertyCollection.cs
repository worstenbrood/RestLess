using System.Xml.Serialization;

namespace RestLesser.Examples.TeamCity.Models
{
    public class PropertyCollection
    {
        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        [XmlElement(ElementName = "property")]
        public Property[]? Properties { get; set; }
    }
}
