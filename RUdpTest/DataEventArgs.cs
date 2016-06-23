using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace RUdpTest
{
    class DataEventArgs : EventArgs
    {
        public IPEndPoint RemoteEP { get; set; }

        public byte[] Data { get; set; }
    }
}