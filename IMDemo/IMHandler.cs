using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace IMDemo
{
    public class IMHandler
    {
        private string message;
        private IMClient client;
        public IMHandler(IMClient client, String message)
        {
            this.message = message;
        }
        public void Handle()
        {
            Console.WriteLine("Received message : " + message);
        }
    }
}