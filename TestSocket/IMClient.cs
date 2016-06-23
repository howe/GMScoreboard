using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestSocket
{
    public class IMClient
    {
        private String IP;
        private int port;
        private Socket clientSocket;
        private byte[] result = new byte[10240];
        private long lastSendTime = DateTime.Now.Ticks;
        private int pingInterveal = 10000;

        public IMClient(string ip, int port)
        {
            this.IP = ip;
            this.port = port;
        }

        public void Start()
        {
            try
            {
                // 建立连接
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(new IPEndPoint(IPAddress.Parse(IP), port));
                Console.WriteLine("connect success.");

                // 发送登录请求
                SendMessage("{\"a\":\"auth\", \"sid\":\"23702\", \"hn\":\"Jacob0\", \"qq\":\"28539789\", \"icon\":\"12\", \"nn\":\"帅哥\"}");

                // TODO:失败重连
                int len = clientSocket.Receive(result);
                string message = DecodeMessage(len);
                Console.WriteLine("auth ret:" + message);

                // 发送聊天室消息
                SendMessage("{\"a\":\"scr\", \"msg\":\"帅哥\"}");
                len = clientSocket.Receive(result);
                message = DecodeMessage(len);
                Console.WriteLine("src ret:" + message);

                // 新建心跳线程
                new Thread(ping).Start();

            }
            catch (Exception e)
            {
                Console.WriteLine("connect fail.");
                return;
            }


            //通过clientSocket接收数据
            while(true)
            {
                try
                {
                    int len = clientSocket.Receive(result);
                    if (len > 0)
                    {
                        string message = DecodeMessage(len);
                        new Thread(new IMHandler(this, message).Handle).Start();
                    }
                }
                catch
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    Console.WriteLine("Error shutdown.");
                    break;
                }
            }
            
        }

        public void SendMessage(string message)
        {
            lastSendTime = DateTime.Now.Ticks;
            clientSocket.Send(EncodeMessage(message));
            Console.WriteLine("Send message : " + message);
        }

        private byte[] EncodeMessage(string messgae)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(messgae);

            byte[] len = new byte[4];
            len[0] = (byte)(messgae.Length >> 24);
            len[1] = (byte)(messgae.Length >> 16);
            len[2] = (byte)(messgae.Length >> 8);
            len[3] = (byte) messgae.Length;

            byte[] x = new byte[4 + bytes.Length];
            len.CopyTo(x, 0);
            bytes.CopyTo(x, 4);
            return x;
        }

        private string DecodeMessage(int len)
        {
            int jsonlen = (result[0] & 0xff) << 24 | (result[1] & 0xff) << 16 | (result[2] & 0xff) << 8 | result[3] & 0xff;
            byte[] ret = new byte[jsonlen];
            for (int i = 0; i < jsonlen; i++)
            {
                ret[i] = result[i + 4];
            }
            string retstring =  Encoding.UTF8.GetString(ret);
            return retstring;
        }

        private void ping()
        {
            while(true)
            {
                Thread.Sleep(pingInterveal);

                // 30秒一次发送心跳
                if (DateTime.Now.Ticks - lastSendTime > pingInterveal)
                {
                    SendMessage("{\"a\":\"pi\"}");
                }

            }
        }
    }
}
