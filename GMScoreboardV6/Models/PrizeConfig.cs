using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class PrizeConfig
    {
        [DataMember(Name = "activies")]
        public ActivieConfig[] activies { get; set; }

        [DataMember(Name = "awards")]
        public AwardConfig[] awards { get; set; }
    }
}