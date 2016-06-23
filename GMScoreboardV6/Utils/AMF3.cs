using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GMScoreboardV6.Utils
{
    public class AMF3
    {
        /// <summary>
        /// 封包数据(C#数组的长度需要在初始化时决定，不能动态追加成员)
        /// </summary>
        List<byte> m_packageData;
        /// <summary>
        /// 当前位置，注意设置EF设置为零的地方C#需要设置为-1，因为EF的数组索引从1开始，C#数组索引从零开始！
        /// 此索引位移时有个隐含的方法：数据读取完后，索引指向所读数据的末位，任何方法读取数据时先读取下一位
        /// </summary>
        int m_currentPosition;
        List<object> m_ReadObjRefTab;
        List<string> m_ReadStrRefTab;
        List<Trait> m_ReadTraitsRefTab;
        bool m_isFlexStringId;
        const int INFINITE = -1;

        public void 置数据(List<byte> buff)
        {
            m_packageData = buff;
            m_currentPosition = -1;//注意索引的转化
            m_ReadObjRefTab = new List<object>();
            m_ReadStrRefTab = new List<string>();
            m_ReadTraitsRefTab = new List<Trait>();
        }
        public void 清除数据()
        {
            m_packageData = new List<byte>();
            m_currentPosition = -1;//注意索引的转化
            m_isFlexStringId = false;
            m_ReadObjRefTab = new List<object>();
            m_ReadStrRefTab = new List<string>();
            m_ReadTraitsRefTab = new List<Trait>();
        }
        /// <summary>
        /// 读取并返回元素对象
        /// 返回的数据类型有：
        /// null、bool、byte、int、double、string、DateTime、Dictionary(string,object)
        /// 以及这些类型的数组
        /// </summary>
        /// <returns></returns>
        public object read_elem()
        {
            object ret = null;
            en_AMFDataType type;

            if (m_currentPosition >= m_packageData.Count)
                return ret;

            int b = read_byte();

            if (Enum.IsDefined(typeof(en_AMFDataType), b))
            {
                type = (en_AMFDataType)b;
            }
            else
            {
                Debug.WriteLine("暂不支持的AMF数据类型" + b.ToString() + "！");
                return ret;
            }

            switch (type)
            {
                case en_AMFDataType.Undefined:
                    ret = null;
                    break;

                case en_AMFDataType.Null:
                    ret = null;
                    break;

                case en_AMFDataType.False:
                    ret = false;
                    break;

                case en_AMFDataType.True:
                    ret = true;
                    break;

                case en_AMFDataType.Integer:
                    ret = read_int();
                    break;

                case en_AMFDataType.Number:
                    ret = read_double();
                    break;

                case en_AMFDataType.String:
                    ret = read_string();
                    break;

                case en_AMFDataType.XML:
                    ret = read_XML();
                    break;

                case en_AMFDataType.Date:
                    ret = read_date();
                    break;

                case en_AMFDataType.Array:
                    ret = read_array();
                    break;

                case en_AMFDataType.Object:
                    ret = read_obj();
                    break;

                case en_AMFDataType.XMLDocument:
                    ret = read_XML();
                    break;

                case en_AMFDataType.ByteArray:
                    ret = read_XML();
#warning 是否应该调用read_ByteArray()
                    break;

                default:
                    break;
            }

            //Debug.WriteLine(变体型转换文本(ret));
            return ret;
        }
        public byte read_byte()
        {
            if (m_currentPosition + 1 >= m_packageData.Count)
            {
                m_currentPosition = m_packageData.Count;
                return 0;
            }

            m_currentPosition += 1;
            return m_packageData[m_currentPosition]; ;
        }
        public int read_Uint29()
        {
            int iValue, iByte;
            iByte = read_byte();

            if ((iByte & 128) == 0)
            {
                return iByte;
            }

            iValue = (iByte & 127) << 7;
            iByte = read_byte();

            if ((iByte & 128) == 0)
            {
                return (iValue | iByte);
            }

            iValue = (iValue | (iByte & 127)) << 7;
            iByte = read_byte();

            if ((iByte & 128) == 0)
            {
                return (iValue | iByte);
            }

            iValue = (iValue | (iByte & 127)) << 8;
            iByte = read_byte();
            iValue = (iValue | iByte);
            return iValue;
        }
        public int read_int()
        {
            int ret = read_Uint29();
            ret = ret << 3;
            ret = ret >> 3;
            return ret;
        }
        public double read_double()
        {
            var data = read_bytes(8);
            return BitConverter.ToDouble(data, 0);
        }
        public byte[] read_bytes(int len)
        {
            byte[] bytes = new byte[len];

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                bytes[i] = read_byte();
            }

            return bytes;
        }
        public string read_string()
        {
            if (m_currentPosition + 1 >= m_packageData.Count)
            {
                return string.Empty;
            }

            int head = read_Uint29();
            string ret = string.Empty;
            int len;

            if ((head & 1) == 0)
            {
                ret = GetStrRefTab((head >> 1));
                return ret;
            }

            len = head >> 1;

            if (len > 0)
            {
                ret = read_UTFBytes(len);
                m_ReadStrRefTab.Add(ret);
            }

            return ret;
        }
        public object read_date()
        {
            object ret;
            int head = read_Uint29();

            if ((head & 1) == 0)
            {
                var data = GetObjRefTab((head >> 1));
                ret = Convert.ToDateTime(data);
                return ret;
            }

            var val = read_double();
            ret = 时间戳_数值型到日期时间型(val);
            addObjRefTab(ret);
            return ret;
        }
        public object read_array()
        {
            int head;
            int position;// 变量：位置
            object btysj1 = null, btysj2 = null, btysj3 = null;
            object[] btysz1;
            object[] btysz2;
            string name;
            Dictionary<string, object> dic = new Dictionary<string, object>(); //变量：字典
            bool A = false, B = false;
            head = read_Uint29();

            if ((head & 1) == 0)
            {
                return GetObjRefTab((head >> 1));
            }

            position = addObjRefTab(btysj1) - 1; //新元素索引=数组元素总数-1
            name = read_string();

            if (!string.IsNullOrEmpty(name))
            {
                A = true;

                while (!string.IsNullOrEmpty(name))
                {
                    dic[name] = read_elem();
                    name = read_string();
                }

                btysj2 = dic;
            }

            head = head >> 1;

            if (head >= 1)
            {
                B = true;
                btysz2 = new object[head];

                for (int i = 0; i < head; i++)
                {
                    btysz2[i] = read_elem();

                    if (m_currentPosition >= m_packageData.Count)
                    {
                        break;
                    }
                }

                btysj3 = btysz2;
            }

            if (A && B)
            {
                btysz1 = new object[head];
                btysz1[0] = btysj2;
                btysz1[1] = btysj3;
                btysj1 = btysz1;
            }

            if (A && !B)
            {
                btysj1 = btysj2;
            }

            if (!A && B)
            {
                btysj1 = btysj3;
            }

            if (!A && !B)
            {
                btysz1 = new object[] { };
                btysj1 = btysz1;
            }

            addObjRefTab(btysj1, position);
            return btysj1;
        }
        public object read_obj()
        {
            object ret = null;
            int objref;
            Trait trait = new Trait();
            bool boolA, boolB;
            string wb1;
            int a;
            List<string> wbsz1;
            Dictionary<string, object> zd1 = new Dictionary<string, object>();
            object btx1;
            string wb2;
            List<string> wbsz2;
            string wb3;
            int position = -1;
            int position1 = -1;
            string wb4;
            object btx2;
            objref = read_Uint29();

            if ((objref & 1) == 0)
            {
                ret = GetObjRefTab((objref >> 1));
                return ret;
            }

            if ((objref & 3) == 1)
            {
                trait = GetTraitRefTab((objref >> 2));
            }
            else
            {
                boolA = ((objref & 4) == 4);
                boolB = ((objref & 8) == 8);
                wb1 = read_string();
                a = objref >> 4;
                wbsz1 = new List<string>();

                for (int i = 0; i < a; i++)
                {
                    wbsz1.Add(read_string());

                    if (m_currentPosition + 1 >= m_packageData.Count)
                    {
                        break;
                    }
                }

                trait.a = wb1;
                trait.b = wbsz1.ToArray();
                trait.c = a;
                trait.d = boolA;
                trait.e = boolB;
                position1 = addTraitRefTab(trait) - 1;//易语言的数组索引从1开始，但C#是从零开始，这里新元素的位置应该是元素总数减一
            }

            trait.a = _getClassAliasRegistry(trait.a);
            position = addObjRefTab(ret) - 1;

            if (trait.d)
            {
                //易语言的数组索引从1开始，但C#是从零开始
                if (trait.a.StartsWith("flex.", StringComparison.OrdinalIgnoreCase))
                {
                    wbsz2 = new List<string>();
                    wbsz2.AddRange(trait.a.Split(new char[] { '.' }));
                    wb3 = wbsz2[wbsz2.Count - 1];
                    ret = _readExternalClass(wb3);
                }
                else
                {
                    wb4 = "错误：无法读取数据类型:" + trait.a;
                    ret = wb4;
                    m_currentPosition = m_packageData.Count;
                }
            }
            else
            {
                for (int i = 0; i < trait.b.Length; i++)
                {
                    wb2 = trait.b[i];
                    btx1 = read_elem();
                    zd1[wb2] = btx1;

                    if (m_currentPosition + 1 >= m_packageData.Count)
                    {
                        break;
                    }
                }

                if (trait.e)
                {
                    wb2 = read_string();

                    while (!string.IsNullOrEmpty(wb2) && m_currentPosition + 1 < m_packageData.Count)
                    {
                        btx1 = read_elem();
                        zd1[wb2] = btx1;
                        wb2 = read_string();
                    }
                }

                ret = zd1;
            }

            btx2 = ret;
            addObjRefTab(btx2, position);

            if (position1 >= 0)
            {
                addTraitRefTab(trait, position1);
            }

            return btx2;
        }
        public object read_XML()
        {
            object ret;
            int b, a;
            a = read_Uint29();

            if ((a & 1) == 0)
            {
                return GetObjRefTab((a >> 1));
            }

            b = a >> 1;
            ret = read_UTFBytes(b);
            addObjRefTab(ret);
            return ret;
        }
        public object read_ByteArray()
        {
            object ret;
            int a, b;
            a = read_Uint29();

            if ((a & 1) == 0)
            {
                ret = GetObjRefTab((a >> 1));
                return ret;
            }

            b = a >> 1;
            ret = read_bin(b);
            addObjRefTab(ret);
            return ret;
        }
        public List<byte> read_bin(int len)
        {
            List<byte> bin = new List<byte>();

            if (len > 0)
            {
                bin = 取字节集中间(m_packageData, m_currentPosition + 1, len);// m_packageData.GetRange(m_currentPosition + 1, len);
                m_currentPosition = m_currentPosition + bin.Count;
            }

            return bin;
        }
        public string GetStrRefTab(int idx)
        {
            int i = idx;//注意索引，易语言按idx+1作索引取数，C#需要改变为idx

            if (i < 0 || i >= m_ReadStrRefTab.Count)
            {
                Debug.WriteLine("GetStrRefTab 上下文对象索引超出数组范围！");
                return string.Empty;
            }

            return m_ReadStrRefTab[i];
        }
        public object GetObjRefTab(int idx)
        {
            int i = idx;//注意索引，易语言按idx+1作索引取数，C#需要改变为idx

            if (i < 0 || i >= m_ReadObjRefTab.Count)
            {
                Debug.WriteLine("GetObjRefTab 上下文对象索引超出数组范围！");
                return null;
            }

            return m_ReadObjRefTab[i];
        }
        public Trait GetTraitRefTab(int idx)
        {
            int i = idx;//注意索引，易语言按idx+1作索引取数，C#需要改变为idx
            Trait ret = new Trait();

            if (i < 0 || i >= m_ReadTraitsRefTab.Count)
            {
                Debug.WriteLine("GetTraitRefTab 上下文对象索引超出数组范围！");
                return ret;
            }

            return m_ReadTraitsRefTab[i];
        }
        /// <summary>
        /// 更新列表m_ReadObjRefTab指定索引数据或向列表添加元素，并返回元素总数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int addObjRefTab(object obj, int? idx = null)
        {
            int ret;

            if (!idx.HasValue)
            {
                m_ReadObjRefTab.Add(obj);
            }
            else
            {
                if (idx >= 0 && idx < m_ReadObjRefTab.Count)
                {
                    m_ReadObjRefTab[idx.Value] = obj;
                }
                else
                {
                    Debug.WriteLine("addObjRefTab 上下文对象索引超出数组范围！");
                }
            }

            ret = m_ReadObjRefTab.Count;
            return ret;
        }
        /// <summary>
        /// 更新列表m_ReadTraitsRefTab指定索引数据或向列表添加元素，并返回元素总数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        public int addTraitRefTab(Trait obj, int? idx = null)
        {
            int ret;

            if (!idx.HasValue)
            {
                m_ReadTraitsRefTab.Add(obj);
            }
            else
            {
                if (idx >= 0 && idx < m_ReadTraitsRefTab.Count)
                {
                    m_ReadTraitsRefTab[idx.Value] = obj;
                }
                else
                {
                    Debug.WriteLine("addTraitRefTab 上下文对象索引超出数组范围！");
                }
            }

            ret = m_ReadTraitsRefTab.Count;
            return ret;
        }
        public string read_UTFBytes(int len)
        {
            if (len > 0)
            {
                return 解码UTF8(read_bin(len));
            }

            return string.Empty;
        }
        public string _getClassAliasRegistry(string p1)
        {
            string str = string.IsNullOrEmpty(p1) ? "" : p1.ToUpper();

            if (str == "DSK")
            {
                return cls_flex_messaging_messages.AcknowledgeMessageExt;
            }

            if (str == "DSA")
            {
                return cls_flex_messaging_messages.AsyncMessageExt;
            }

            if (str == "DSC")
            {
                return cls_flex_messaging_messages.CommandMessageExt;
            }

            return p1;
        }
        public object _readExternalClass(string p1)
        {
            object ret = null;

            if (!string.IsNullOrEmpty(p1))
            {
                if (string.Equals(p1, "AbstractMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AbstractMessage();
                }

                if (string.Equals(p1, "AsyncMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AsyncMessage();
                }

                if (string.Equals(p1, "AsyncMessageExt", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AsyncMessage();
                }

                if (string.Equals(p1, "AcknowledgeMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AcknowledgeMessage();
                }

                if (string.Equals(p1, "AcknowledgeMessageExt", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AcknowledgeMessage();
                }

                if (string.Equals(p1, "CommandMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_CommandMessage();
                }

                if (string.Equals(p1, "CommandMessageExt", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_CommandMessage();
                }

                if (string.Equals(p1, "ErrorMessage", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_AcknowledgeMessage();
                }

                if (string.Equals(p1, "ArrayCollection", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_ArrayCollection();
                }

                if (string.Equals(p1, "ArrayList", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_ArrayCollection();
                }

                if (string.Equals(p1, "ObjectProxy", StringComparison.OrdinalIgnoreCase))
                {
                    return read_elem();
                }

                if (string.Equals(p1, "ManagedObjectProxy", StringComparison.OrdinalIgnoreCase))
                {
                    return read_elem();
                }

                if (string.Equals(p1, "SerializationProxy", StringComparison.OrdinalIgnoreCase))
                {
                    return _readExternal_SerializationProxy();
                }
            }

            return ret;
        }
        public int _readByte()
        {
            int b = read_byte();

            if (b > 127)
            {
                return b - 256;
            }

            return b;
        }
        public List<int> _readFlags()
        {
            List<int> data = new List<int>();
            int b;

            while (true)
            {
                b = _readByte();
                data.Add(b);

                if ((b & 128) == 0)
                {
                    break;
                }
            }

            return data;
        }
        public Dictionary<string, object> _readExternal_AbstractMessage()
        {
            int a, b, c;
            object btx1, btx2;
            Dictionary<string, object> zd = new Dictionary<string, object>();
            m_isFlexStringId = false;
            List<int> tab = _readFlags();

            for (int i = 0; i < tab.Count; i++)
            {
                a = tab[i];
                b = 0;

                if (i == 0)//注意索引变化
                {
                    if ((a & 1) != 0)
                    {
                        zd["body"] = read_elem();
                    }

                    if ((a & 2) != 0)
                    {
                        zd["clientId"] = read_elem();
                        m_isFlexStringId = true;
                    }

                    if ((a & 4) != 0)
                    {
                        zd["destination"] = read_elem();
                    }

                    if ((a & 8) != 0)
                    {
                        zd["headers"] = read_elem();
                    }

                    if ((a & 16) != 0)
                    {
                        zd["messageId"] = read_elem();
                        m_isFlexStringId = true;
                    }

                    if ((a & 32) != 0)
                    {
                        zd["timestamp"] = read_elem();
                    }

                    if ((a & 64) != 0)
                    {
                        zd["timeToLive"] = read_elem();
                    }

                    b = 7;
                }
                else if (i == 1)//注意索引变化
                {
                    if ((a & 1) != 0)
                    {
                        btx1 = _fromByteArray(read_elem());
                        zd["clientId"] = btx1;
                        m_isFlexStringId = false;
                    }

                    if ((a & 2) != 0)
                    {
                        btx2 = _fromByteArray(read_elem());
                        zd["messageId"] = btx2;
                        m_isFlexStringId = false;
                    }

                    b = 2;
                }

                if ((a >> b) != 0)
                {
                    c = b;

                    while (c < 6)
                    {
                        if (((a >> c) & 1) != 0)
                        {
                            read_elem();
                        }

                        c += 1;
                    }
                }
            }

            return zd;
        }
        public Dictionary<string, object> _readExternal_AsyncMessage()
        {
            var ret = _readExternal_AbstractMessage();
            var Flags = _readFlags();

            for (int i = 0; i < Flags.Count; i++)
            {
                var a = Flags[i];
                var b = 0;

                if (i == 0) //注意索引
                {
                    if ((a & 1) != 0)
                    {
                        ret["correlationId"] = read_elem();
                        m_isFlexStringId = true;
                    }

                    if ((a & 2) != 0)
                    {
                        var bty = _fromByteArray(read_elem());
                        ret["correlationId"] = bty;
                        m_isFlexStringId = false;
                    }

                    b = 2;
                }

                if ((a >> b) != 0)
                {
                    var c = b;

                    while (c < 6)
                    {
                        if (((a >> c) | 1) != 0)
                        {
                            read_elem();
                        }

                        c += 1;
                    }
                }
            }

            return ret;
        }
        public Dictionary<string, object> _readExternal_AcknowledgeMessage()
        {
            var ret = _readExternal_AsyncMessage();
            var Flags = _readFlags();

            for (int i = 0; i < Flags.Count; i++)
            {
                var a = Flags[i];
                var b = 0;

                if ((a >> b) != 0)
                {
                    var c = b;

                    while (c < 6)
                    {
                        if (((a >> c) & 1) != 0)
                        {
                            read_elem();
                        }

                        c += 1;
                    }
                }
            }

            return ret;
        }
        public Dictionary<string, object> _readExternal_CommandMessage()
        {
            var ret = _readExternal_AsyncMessage();
            var Flags = _readFlags();

            for (int i = 0; i < Flags.Count; i++)
            {
                var a = Flags[i];
                var b = 0;

                if (i == 0)
                {
                    if ((a & 1) != 0)
                    {
                        var btx = read_elem();
                        ret["operation"] = btx;
                    }

                    b = 1;
                }

                if ((a >> b) != 0)
                {
                    var c = b;

                    while (c < 6)
                    {
                        if (((a >> c) & 1) != 0)
                        {
                            read_elem();
                        }

                        c += 1;
                    }
                }
            }

            return ret;
        }
        public Dictionary<string, object> _readExternal_ArrayCollection()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["source"] = read_elem();
            return ret;
        }
        public Dictionary<string, object> _readExternal_SerializationProxy()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["defaultInstance"] = read_elem();
            return ret;
        }
        public object _fromByteArray(object p1)
        {
            var bytes = _variantToBytes(p1);
            string str = string.Empty;//变量：文本

            if (bytes.Count == 16)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (i == 4 || i == 6 || i == 8 || i == 10)//C#的索引从零开始
                    {
                        str += "-";
                    }

                    str += 字节到十六(bytes[i]);
                }
            }
            else
            {
                str = 字节集到十六(bytes);
            }

            return str;
        }
        public List<byte> _variantToBytes(object p1)
        {
            List<byte> bytes = new List<byte>();
            Array arr = p1 as Array;

            if (arr == null)
            {
                return bytes;
            }

            var cnt = arr.Length;//变量：数量

            if (cnt > 0)
            {
                bytes = new List<byte>(cnt);

                for (int i = 0; i < arr.Length; i++)
                {
                    bytes[i] = Convert.ToByte(arr.GetValue(i));
                }
            }

            return bytes;
        }
        public string 字节到十六(byte b)
        {
            string str = Convert.ToString(b, 16);

            if (b < 16)
            {
                //自动补零以对齐数据
                return "0" + str;
            }

            return str;
        }
        public string 字节集到十六(List<byte> bytes, string p1 = "")
        {
            if (string.IsNullOrEmpty(p1))
            {
                p1 = " ";
            }

            var len = bytes.Count;
            string ret = "";

            for (int i = 0; i < len; i++)
            {
                if (i + 1 < len) //注意索引的不同
                {
                    ret = ret + 字节到十六(bytes[i]) + p1;
                }
                else
                {
                    ret = ret + 字节到十六(bytes[i]);
                }
            }

            return ret;
        }
        public DateTime 时间戳_数值型到日期时间型(double millisecond)
        {
            const int millisecondPerDay = 86400000;//一天多少86400000毫秒
            int days = (int)((long)(millisecond / millisecondPerDay));//为兼容易语言，需要先转换为long再取整数部分 //int days = (int)(millisecond / millisecondPerDay);
#warning 需要考虑易语言的“四舍五入”是否“银行家算法”，易语言中的代码“b ＝ 四舍五入 ((a ％ 86400000) ÷ 1000, )”取秒数语意是错误的！
            int seconds = (int)Math.Round((millisecond % millisecondPerDay) / 1000, MidpointRounding.AwayFromZero);//int seconds = (int)Math.Round((decimal)(days % millisecondPerDay ) / 1000, MidpointRounding.AwayFromZero);//中国式的四舍五入需要MidpointRounding.AwayFromZero，并非银行家算法
            DateTime dt = new DateTime(1970, 1, 1);

            try
            {
                dt.AddDays(days);
            }
            catch
            {
                dt = (days > 0) ? DateTime.MaxValue : DateTime.MinValue;
            }

            try
            {
                dt.AddSeconds(seconds);//四舍五入，“五入”时可能会令秒数总数超过一天（DateTime.MaxValue.Date再添加也会引发异常）
            }
            catch
            {
                dt = (seconds > 0) ? DateTime.MaxValue : DateTime.MinValue;
            }

            return dt;
        }
        public string 变体型转换文本(object p1)
        {
            string str = "null";

            //read_elem()读取出的数据类型有：null、bool、byte、int、double、string、DateTime、Dictionary<string,object>，以及这些类型的数组
            if (p1 != null)
            {
                Type t = p1.GetType();

                if (p1 is Array)
                {
                    str = 数组处理(p1 as Array);
                }
                else if (t == typeof(bool))
                {
                    str = p1.ToString().ToLower();
                }
                else if (t == typeof(byte) || t == typeof(int) || t == typeof(double))
                {
                    str = p1.ToString();
                }
                else if (t == typeof(string))
                {
                    str = "\"" + p1.ToString() + "\"";
                }
                else if (t == typeof(DateTime))
                {
                    str = string.Format("\"{0:yyyy年M月d日H时m分s秒}\"", p1);
                }
                else if (t == typeof(Dictionary<string, object>))
                {
                    str = DicToJsonString(p1 as Dictionary<string, object>);
                }
            }

            return str;
        }
        public string 数组处理(Array p1)
        {
            var cnt = p1.Length;

            if (cnt < 1)
            {
                return "[ ]";
            }

            StringBuilder a = new StringBuilder();
            a.Append("[");

            for (int i = 0; i < cnt; i++)
            {
                var obj = p1.GetValue(i);
#if !StandardJsonFormat
#warning 数组未初始化的元素以数值0表示（是为了使数据与易语言解析出来的结果一致！标准值应该是null）

                //测试发布易语言的变体数组没有初始化的元素，其变体类型均是数值，且值是零，这里稍作兼容修正
                if (obj == null)
                {
                    obj = 0;
                }

#endif

                if (i + 1 < cnt)
                {
                    a.Append(变体型转换文本(obj) + ",");
                }
                else
                {
                    a.Append(变体型转换文本(obj) + "]");
                }
            }

            return a.ToString();
        }
        public List<byte> 封包处理(List<byte> buf)
        {
            List<byte> ret = new List<byte>();
            var len = buf.Count;
            var position = IndexOfSubArray(buf, new byte[] { 17, 10 });

            //索引在这里比易语言的小一
            if (position >= 0)
            {
                ret = 取字节集中间(buf, position + 1, len - position - 2);// ret = buf.GetRange(position + 1, len - position - 2);
                ret = 替换C3(ret);
            }

            return ret;
        }
        public List<byte> 替换C3(List<byte> buf)
        {
            var position = buf.IndexOf(195);
            var ret = 取字节集中间(buf, 0, buf.Count);// var ret = buf.GetRange(0, buf.Count);
            int times = 0;

            for (int i = position; i < buf.Count; i = i + 129)
            {
                if (i == -1)
                {
                    return ret;
                }

                if (buf[i] == 195)
                {
                    ret.RemoveRange(i - times, 1);//将子集替换为空集也即移除该子集
                    times += 1;
                }
            }

            return ret;
        }
        public string 解码UTF8(List<byte> buf)
        {
            var encoding = Encoding.GetEncoding("GB2312");
            var b = Encoding.Convert(Encoding.UTF8, encoding, buf.ToArray());
            var s = Encoding.Default.GetString(b);
            return s;
        }
        public string 淼引号(string str)
        {
            return str.Replace("''", "\"");
        }

        /// <summary>
        /// 构造字典的Json字符串
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public string DicToJsonString(Dictionary<string, object> dic)
        {
            if (dic == null || dic.Count == 0)
            {
                return "{ }";
            }

            StringBuilder s = new StringBuilder();
            s.Append("{");

            foreach (var item in dic)
            {
                s.AppendFormat("\"{0}\":{1},", item.Key, 变体型转换文本(item.Value));
            }

            s.Remove(s.Length - 1, 1);//移除最后一个逗号
            s.Append("}");
            return s.ToString();
        }
        /// <summary>
        /// 查找集合第一个匹配的索引，找不到则返回-1
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="sub"></param>
        /// <returns></returns>
        public int IndexOfSubArray(List<byte> buf, byte[] sub)
        {
            for (int i = 0; i < buf.Count - sub.Length + 1; i++)
            {
                int iSubIdx = 0;

                while (iSubIdx < sub.Length)
                {
                    if (buf[i + iSubIdx] != sub[iSubIdx])
                    {
                        break;
                    }

                    iSubIdx++;
                }

                if (iSubIdx == sub.Length)//如果iSubIdx与sub.Length一致，则表示子集元素完全匹配
                {
                    return i;
                }
            }

            return -1;
        }

        public List<byte> 取字节集中间(List<byte> buf, int start, int len)
        {
            if (buf.Count <= start)
            {
                return new List<byte>();
            }

            var maxLen = buf.Count - start;
            len = len > maxLen ? maxLen : len;
            return buf.GetRange(start, len);
        }
    }

    /// <summary>
    /// 处理结果类型，与常量表“PROCESS_”开头的常量对应（如“PROCESS_CREATE_THREAD”、“PROCESS_VM_OPERATION”等）
    /// </summary>
    public enum en_PROCESS
    {
        PROCESS_CREATE_THREAD = 2,
        PROCESS_VM_OPERATION = 8,
        PROCESS_VM_READ = 16,
        PROCESS_DUP_HANDLE = 64,
        PROCESS_QUERY_INFORMATION = 1024
    }

    /// <summary>
    /// 数据类型，与常量表下划线开头的常量对应（如“_Undefined”、“_Null”、“_False”等）
    /// </summary>
    public enum en_AMFDataType
    {
        Undefined = 0,
        Null,
        False,
        True,
        Integer,
        Number,
        String,
        XML,
        Date,
        Array,
        Object,
        XMLDocument,
        ByteArray
    }

    /// <summary>
    /// 与常量表flex_messaging_messages开头的常量对应（如“cls_flex_messaging_messages_AcknowledgeMessageExt”等）
    /// </summary>
    public class cls_flex_messaging_messages
    {
        public static readonly string AcknowledgeMessageExt = "flex.messaging.messages.AcknowledgeMessageExt";
        public static readonly string AsyncMessageExt = "flex.messaging.messages.AsyncMessageExt";
        public static readonly string CommandMessageExt = "flex.messaging.messages.CommandMessageExt";
    }

    /// <summary>
    /// 与自定义数据类型Trait对应
    /// </summary>
    public class Trait
    {
        public string a = "";
        public string[] b = new string[] { };
        public int c = 0;
        public bool d = false;
        public bool e = false;
    }
}