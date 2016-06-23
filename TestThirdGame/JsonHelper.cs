using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TestThirdGame
{
    public static class JsonHelper
    {
        public static T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default(T);

            object result = null;
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                result = new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
            }
            return result == null ? default(T) : (T)result;
        }

        public static string ObjectToJson(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            return Encoding.UTF8.GetString(dataBytes);
        }
        // 从一个Json串生成对象信息
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }
    }
}