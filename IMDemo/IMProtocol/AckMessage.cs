using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IMDemo.IMProtocol
{
    [DataContract]
    public class AckMessage : BaseMessage
    {
        [DataMember(Name = "re")]
        private String re = "";

        [DataMember(Name = "ec")]
        private int ec;

        [DataMember(Name = "em")]
        private String em;
    }
}
