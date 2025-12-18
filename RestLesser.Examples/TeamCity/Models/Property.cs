using System.Xml.Serialization;

namespace RestLesser.Examples.TeamCity.Models
{
    public class Property
    {
        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string? Value { get; set; }

        [XmlAttribute(AttributeName = "inherited")]
        public bool Inherited { get; set; }
    }
}
