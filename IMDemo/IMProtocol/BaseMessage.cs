using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace IMDemo.IMProtocol
{
    [DataContract]
    public class BaseMessage
    {

        [DataMember(Name = "a")]
        public string a;

        [DataMember(Name = "v")]
        public int v;
        
        [DataMember(Name = "u")]
        public int u;
    }
}
