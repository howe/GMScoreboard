using System;
using System.Collections.Generic;
using System.Text;

namespace TestThirdGame
{
    public class Auth
    {
        public string a { get; set; }
        public string sid { get; set; }
        public string hn { get; set; }
        public string qq { get; set; }
        public string icon { get; set; }
        public string nn { get; set; }
    }

    public class Result
    {
        public string a { get; set; }
        public string ec { get; set; }
        public string re { get; set; }
    }

    public class heart
    {
        public string a { get; set; }
    }

    public class Message
    {
        public string a { get; set; }
        public string msg { get; set; }
    }

    public enum clientType
    {
        auth,
        pi,
        scr,
        ack,

    }
}
