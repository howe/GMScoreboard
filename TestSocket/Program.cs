using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            // new IMClient("127.0.0.1", 9999).Start();
            new IMClient("119.29.56.137", 9999).Start();
        }
    }
}
