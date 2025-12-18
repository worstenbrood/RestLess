using System.Xml.Serialization;

namespace RestLesser.Examples.TeamCity.Models
{
    public class RunningInfo
    {
        [XmlAttribute(AttributeName = "percentageComplete")]
        public int PercentageComplete { get; set; }

        [XmlAttribute(AttributeName = "elapsedSeconds")]
        public int ElapsedSeconds { get; set; }

        [XmlAttribute(AttributeName = "estimatedTotalSeconds")]
        public int EstimatedTotalSeconds { get; set; }

        [XmlAttribute(AttributeName = "currentStageText")]
        public string? CurrentStageText { get; set; }
    }
}
