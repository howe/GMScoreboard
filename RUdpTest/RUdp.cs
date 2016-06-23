using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RUdpTest
{
    static class RUdp
    {
        public static event EventHandler<DataEventArgs> Data;   //数据接收事件

        static SortedDictionary<int, Msg> s_sMsgs = new SortedDictionary<int, Msg>();
        static SortedDictionary<int, Msg> s_rMsgs = new SortedDictionary<int, Msg>();
        static object s_SyncRoot = new object();
        static UdpClient s_UdpClient;
        static volatile int s_rID = 1;
        static volatile int s_sID = 1;

        public static void Bind(IPEndPoint endPoint)
        {
            s_UdpClient = new UdpClient(endPoint);
            BeginReceive();
        }

        static void Confirm(int id, IPEndPoint remoteEP)
        {
            s_UdpClient.Send(BitConverter.GetBytes(id), 4, remoteEP);
        }

        public static void Send(byte[] data, IPEndPoint remoteEP)
        {
            var id = s_sID;
            var buffer = new byte[4 + data.Length];

            buffer[3] = (byte)(id >> 24);
            buffer[2] = (byte)(id >> 16);
            buffer[1] = (byte)(id >> 8);
            buffer[0] = (byte)(id & 0x000000FF);

            Buffer.BlockCopy(data, 0, buffer, 4, data.Length);
            s_UdpClient.Send(buffer, buffer.Length, remoteEP);

            s_sMsgs.Add(id, new Msg()
            {
                ID = id,
                Buffer = buffer,
                Tick = DateTime.Now.Ticks
            });

            s_sID = (id == int.MaxValue ? 1 : id + 1);
        }

        static void BeginReceive()
        {
            s_UdpClient.BeginReceive(EndReceive, null);
        }

        static void EndReceive(IAsyncResult ar)
        {
            IPEndPoint endPoint = null;

            var buffer = s_UdpClient.EndReceive(ar,
                ref endPoint);
            
            if (buffer.Length > 0)
            {
                if (buffer.Length == 4)                 //确认发送
                {
                    var id = BitConverter.ToInt32(buffer, 0);

                    if (s_sMsgs.ContainsKey(Math.Abs(id)))
                    {
                        if (id > 0)
                        {
                            s_sMsgs.Remove(id);
                        }
                        else
                        {
                            var msg = s_sMsgs[-id];

                            s_UdpClient.Send(msg.Buffer, msg.Buffer.Length, endPoint);
                        }
                    }
                }
                else
                {
                    var id = BitConverter.ToInt32(buffer, 0);
                    var data = new byte[buffer.Length - 4];

                    Buffer.BlockCopy(buffer, 4, data, 0, data.Length);
                    Confirm(id, endPoint);                  //确认接受

                    if (id == s_rID)
                    {
                        id++;

                        if (Data != null)
                        {
                            Data(s_UdpClient, new DataEventArgs() { RemoteEP = endPoint, Data = data });
                        }

                        while (s_rMsgs.ContainsKey(id))
                        {
                            var msg = s_rMsgs[id++];

                            if (Data != null)
                                Data(s_UdpClient, new DataEventArgs() { Data = msg.Buffer });
                        }

                        s_rID = id;
                    }
                    else if (id > s_rID)                    //提前收到
                    {
                        Confirm(-s_rID, endPoint);          //要求重发

                        s_rMsgs.Add(id, new Msg()
                        {
                            ID = id,
                            Buffer = data,
                            Tick = DateTime.Now.Ticks
                        });
                    }
                }

                BeginReceive();
            }
            else
            {
                //程序关闭
            }
        }

    }
}