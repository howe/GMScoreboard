using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    [DataContract]
    public class MenuConfig
    {
        [DataMember(Name = "index")]
        public int index { get; set; }

        [DataMember(Name = "name")]
        public string name { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "type")]
        public int type { get; set; }

        [DataMember(Name = "content")]
        public string content { get; set; }
    }
}