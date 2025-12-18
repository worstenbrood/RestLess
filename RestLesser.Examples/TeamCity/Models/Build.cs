using System.Xml.Serialization;

namespace RestLesser.Examples.TeamCity.Models
{
    [XmlRoot(ElementName = "build")]
    public class Build
    {
        [XmlAttribute(AttributeName = "id")]
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "branchName")]
        public string? BranchName { get; set; }

        [XmlAttribute(AttributeName = "defaultBranch")]
        public bool DefaultBranch { get; set; }

        [XmlAttribute(AttributeName = "state")]
        public string? State { get; set; }

        [XmlAttribute(AttributeName = "status")]
        public string? Status { get; set; }

        [XmlAttribute(AttributeName = "webUrl")]
        public string? WebUrl { get; set; }

        [XmlElement(ElementName = "buildType")]
        public BuildType? BuildType { get; set; }

        [XmlElement(ElementName = "running-info")]
        public RunningInfo? RunningInfo { get; set; }

        [XmlElement(ElementName = "statusText")]
        public string? StatusText { get; set; }

        [XmlElement(ElementName = "properties")]
        public PropertyCollection? Properties { get; set; }
    }
}
