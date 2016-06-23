using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class UserInfo
    {
        [DataMember(Name = "barName")]
        public string barName { get; set; }

        [DataMember(Name = "hostName")]
        public string hostName { get; set; }

        [DataMember(Name = "serverIp")]
        public string serverIp { get; set; }

        [DataMember(Name = "agentId")]
        public string agentId { get; set; }

        [DataMember(Name = "balance")]
        public int balance { get; set; }
    }
}
