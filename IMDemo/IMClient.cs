using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
namespace IMDemo
{
    public class IMClient
    {
        private String IP;
        private int port;
        private Socket clientSocket;
        private byte[] result = new byte[10240];
        private double lastSendTime = DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
        private int pingInterveal = 30000;
        public Queue messages = Queue.Synchronized(new Queue());

        public IMClient(string ip, int port)
        {
            this.IP = ip;
            this.port = port;
        }
        public bool Connect()
        {
            try
            {
                // 建立连接
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), port));

                // 新建心跳线程 在无消息发送情况下30秒发送一次心跳包
                new Thread(ping).Start();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("connect fail.");
            }
            return false;

        }
        public void Start()
        {
            //通过clientSocket接收数据
            while (true)
            {
                try
                {
                    int len = clientSocket.Receive(result);
                    if (len > 0)
                    {
                        string message = DecodeMessage(len);
                        if (!string.IsNullOrEmpty(message))
                        {
                            messages.Enqueue(message);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("receive error.");
                }

                Thread.Sleep(1000);
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                // 记录最后一次发送时间
                lastSendTime = DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;

                // 发送消息
                int ret = clientSocket.Send(EncodeMessage(message));
                Console.WriteLine("Send message : " + message + " ret:" + ret);

            } catch (Exception e)
            {
                Console.WriteLine("Error SendMessage.");
            }
        }

        private byte[] EncodeMessage(string messgae)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(messgae);
            byte[] len = new byte[4];
            len[0] = (byte)(bytes.Length >> 24);
            len[1] = (byte)(bytes.Length >> 16);
            len[2] = (byte)(bytes.Length >> 8);
            len[3] = (byte)bytes.Length;
            byte[] x = new byte[4 + bytes.Length];
            len.CopyTo(x, 0);
            bytes.CopyTo(x, 4);
            return x;
        
        }

        private string DecodeMessage(int len)
        {
            // 长度检查
            if (len < 6)
                return null;

            int jsonlen = (result[0] & 0xff) << 24 | (result[1] & 0xff) << 16 | (result[2] & 0xff) << 8 | result[3] & 0xff;

            // 头部长度检查
            if (jsonlen + 4 != len)
                return null;

            byte[] ret = new byte[jsonlen];
            for (int i = 0; i < jsonlen; i++)
            {
                ret[i] = result[i + 4];
            }

            return Encoding.UTF8.GetString(ret);
        }

        private void ping()
        {
            while (true)
            {
                Thread.Sleep(1000);

                // 30秒一次发送心跳
                if (DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds - lastSendTime > pingInterveal)
                {
                    SendMessage("{\"a\":\"pi\"}");
                }
            }
        }

        public void disconnect()
        {
            if (clientSocket != null)
            {
                try
                {
                    clientSocket.Disconnect(true);
                } catch (Exception e)
                {
                    Console.WriteLine("Error Disconnect.");
                }
            }
        }
    }
}
