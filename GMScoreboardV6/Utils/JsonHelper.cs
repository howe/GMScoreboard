using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace GMScoreboardV6.Utils
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
    }
}