using System;
using System.Collections.Generic;
using System.Text;

namespace RUdpTest
{
    class Msg
    {
        public int ID { get; set; }

        public byte[] Buffer { get; set; }

        public long Tick { get; set; }
    }
}