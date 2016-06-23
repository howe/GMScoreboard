using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class AwardConfig
    {
        [DataMember(Name = "playerName")]
        public string playerName { get; set; }

        [DataMember(Name = "serverName")]
        public string serverName { get; set; }

        [DataMember(Name = "userId")]
        public string userId { get; set; }

        [DataMember(Name = "tier")]
        public string tier { get; set; }

        [DataMember(Name = "hostName")]
        public string hostName { get; set; }

        [DataMember(Name = "ruleName")]
        public string ruleName { get; set; }

        [DataMember(Name = "prizeName")]
        public string prizeName { get; set; }

        [DataMember(Name = "createTime")]
        public string createTime { get; set; }

        [DataMember(Name = "avatar")]
        public string avatar { get; set; }

        [DataMember(Name = "rank")]
        public string rank { get; set; }

        [DataMember(Name = "ruleType")]
        public int ruleType { get; set; }

        [DataMember(Name = "awardImage")]
        public string awardImage { get; set; }

        [DataMember(Name = "ruleId")]
        public int ruleId { get; set; }

        [DataMember(Name = "status")]
        public int status { get; set; }

        [DataMember(Name = "qq")]
        public string qq { get; set; }

        [DataMember(Name = "lgjAutoCharge")]
        public int lgjAutoCharge { get; set; }

        [DataMember(Name = "myAward")]
        public bool myAward { get; set; }

        [DataMember(Name = "awardId")]
        public int awardId { get; set; }
    }
}