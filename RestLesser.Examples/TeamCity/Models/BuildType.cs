using System.Xml.Serialization;

namespace RestLesser.Examples.TeamCity.Models
{
    public class BuildType
    {
        [XmlAttribute(AttributeName = "id")]
        public string? Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string? Name { get; set; }
    }
}
