using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace RUdpTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Console.ReadLine().Equals("A"))
            {
                RUdp.Data += RUdp_DataA;

                RUdp.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8999));
            }
            else
            {
                RUdp.Data += RUdp_DataB;

                RUdp.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8998));

                RUdp.Send(Encoding.UTF8.GetBytes("NI HAO" + Environment.TickCount)
                    , new IPEndPoint(IPAddress.Parse ("127.0.0.1"), 8999));
            }

            Console.Read();
        }

        private static void RUdp_DataA(object sender, DataEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Data));

            RUdp.Send(Encoding.UTF8.GetBytes("OK" + Environment.TickCount), e.RemoteEP);
        }

        private static void RUdp_DataB(object sender, DataEventArgs e)
        {
            Console.WriteLine(Encoding.UTF8.GetString(e.Data));

            RUdp.Send(Encoding.UTF8.GetBytes("NI HAO" + Environment.TickCount), e.RemoteEP);
        }
    }
}