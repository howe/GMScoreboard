using System.Runtime.Serialization;

namespace GMScoreboardV6.Models
{
    public class WebRetInfo<T>
    {
        [DataMember(Name = "errCode")]
        public int errCode { get; set; }

        [DataMember(Name = "retCode")]
        public int retCode { get; set; }

        [DataMember(Name = "msg")]
        public string msg { get; set; }

        [DataMember(Name = "dtag")]
        public string dtag { get; set; }

        [DataMember(Name = "v")]
        public int v { get; set; }

        [DataMember(Name = "body")]
        public T body { get; set; }
    }
}
