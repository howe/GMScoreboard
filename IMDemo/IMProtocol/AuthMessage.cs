using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IMDemo.IMProtocol
{
    [DataContract]
    public class AuthMessage : BaseMessage
    {
        [DataMember(Name = "sid")]
        public long sid;

        [DataMember(Name = "hn")]
        public String hn = "";

        [DataMember(Name = "qq")]
        public String qq = "";

        [DataMember(Name = "icon")]
        public String icon = "";

        [DataMember(Name = "nickname")]
        public String nickname = "";

    }
}
