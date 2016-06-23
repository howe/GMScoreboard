using System;
using System.IO;
using System.Xml.Serialization;

namespace GMScoreboardV6.Models
{
    [XmlRoot("ClientTip")]
    public class ClientTip
    {
        [XmlElement("errCode")]
        public int errCode { get; set; }

        [XmlElement("msg")]
        public string msg { get; set; }

        [XmlElement("createTime")]
        public long createTime { get; set; }

        [XmlElement("shopId")]
        public long shopId { get; set; }

        [XmlElement("hostName")]
        public String hostName { get; set; }

        [XmlElement("qq")]
        public String qq { get; set; }

        [XmlElement("title")]
        public String title { get; set; }

        [XmlElement("content")]
        public String content { get; set; }

        [XmlElement("headText")]
        public String headText { get; set; }

        [XmlElement("linkText")]
        public String linkText { get; set; }

        [XmlElement("browser")]
        public String browser { get; set; }

        [XmlElement("link")]
        public String link { get; set; }

        [XmlElement("taskType")]
        public int taskType { get; set; }

        public static ClientTip fromXML(string xml)
        {
            if (xml == null || xml.Trim().Equals(""))
                return null;

            using (StringReader rdr = new StringReader(xml))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ClientTip));
                ClientTip config = (ClientTip)serializer.Deserialize(rdr);
                return config;
            }
        }
    }
}
