using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class ActivieConfig
    {
        [DataMember(Name = "prizeName")]
        public string prizeName { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }

        [DataMember(Name = "condition")]
        public string condition { get; set; }

        [DataMember(Name = "awardImage")]
        public string awardImage { get; set; }

        [DataMember(Name = "color")]
        public string color { get; set; }

        [DataMember(Name = "ruleType")]
        public string ruleType { get; set; }

        [DataMember(Name = "ruleId")]
        public string ruleId { get; set; }
    }
}