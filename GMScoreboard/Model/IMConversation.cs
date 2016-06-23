using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GMScoreboard.Model
{
    /// <summary>
    /// IM对话实体  包含实体PO和静态方法用于存取信息
    /// </summary>
    public class IMConversation:IComparable
    {

        public IMConversation()
        {
        }

        public int unread { get; set; }

        public string userId { get; set; }

        public string serverName { get; set; }

        public string playerName { get; set; }

        public string avatar { get; set; }

        public string lastMessage { get; set;}

        public DateTime updateTime { get; set; }

        public IMPrivateForm dialog { get; set; }

        public List<object> messages { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return 1;
            }
            IMConversation conv = (IMConversation) obj;
            return this.updateTime.ToUniversalTime().Ticks < conv.updateTime.ToUniversalTime().Ticks ? 1 : -1;
        }

        public string toJSONString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"userId\":\"").Append(userId).Append("\",");
            sb.Append("\"unread\":").Append(unread).Append(",");
            sb.Append("\"serverName\":\"").Append(serverName).Append("\",");
            sb.Append("\"playerName\":\"").Append(playerName).Append("\",");
            sb.Append("\"avatar\":\"").Append(avatar).Append("\",");
            sb.Append("\"updateTime\":\"").Append(updateTime).Append("\"");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// IM对话实体静态工具方法
        /// </summary>
        private static Hashtable conversations = new Hashtable();
        private static ScoreboardForm mainForm = null;
        public static Hashtable globalData = new Hashtable();

        /// <summary>
        /// 初始化主窗体 并清理所有缓存数据
        /// </summary>
        public static void initialMainForm(ScoreboardForm form)
        {
            mainForm = form;
            conversations.Clear();
            globalData.Clear();
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        public static IMPrivateForm receiveIMMessage(string userId, string serverName, string playerName, string avatar, string sumary, object message)
        {
            IMConversation conv = (IMConversation) conversations[userId];
            if (conv == null)
            {
                conv = new IMConversation();
                conv.dialog = null;
                conv.userId = userId;
                conversations.Add(userId, conv);
                conv.messages = new List<object>();
            }

            conv.updateTime = DateTime.Now;
            conv.lastMessage = sumary;
            conv.avatar = avatar;
            conv.playerName = playerName;
            conv.serverName = serverName;
            conv.messages.Add(message);

            // 如果窗体存在
            if (conv.dialog != null)
            {
                // 私聊窗口接收消息  并  闪动任务栏
                conv.dialog.invokeScript("runJS", new object[] { "ExtUtil.receiveMsg", message });
                conv.dialog.doNotifyNewMessage();
            }
            else
            {
                // 更新主窗体的未读消息数
                conv.unread++;
                mainForm.invokeScript("runJS", new object[] { "ExtUtil.hasUnread", IMConversation.getUnreadCount() });
            }


            return conv.dialog;
        }

        /// <summary>
        /// 获取对话列表 对象列表返回
        /// </summary>
        public static List<IMConversation> getConversationList()
        {
            List<IMConversation> cons = new List<IMConversation>();
            foreach (DictionaryEntry de in conversations)
            {
                IMConversation conv = (IMConversation) de.Value;
                if (conv.updateTime != DateTime.MinValue)
                {
                    cons.Add(conv);
                }
            }
            cons.Sort();
            return cons;
        }

        /// <summary>
        /// 获取对话列表 JSON返回
        /// </summary>
        public static string getConversationListJSON()
        {
            StringBuilder sb = new StringBuilder("[");
            List<IMConversation> convs = getConversationList();
            if (convs == null || convs.Count == 0)
            {
                return sb.Append("]").ToString();
            }

            foreach (IMConversation conv in convs)
            {
                sb.Append(conv.toJSONString()).Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            string ret = sb.Append("]").ToString();
            return (ret);
        }

        /// <summary>
        /// 获取未读消息总数
        /// </summary>
        public static int getUnreadCount()
        {
            int totalUnread = 0;
            foreach (DictionaryEntry con in conversations)
            {
                totalUnread += ((IMConversation)con.Value).unread;
            }
            return totalUnread;
        }

        /// <summary>
        /// 关闭对话窗
        /// </summary>
        public static void closeDialog(string userId)
        {
            if (userId != null && conversations[userId] != null)
            {
                IMConversation conv = (IMConversation)conversations[userId];
                if (conv.dialog != null) 
                {
                    conv.dialog.Dispose();
                    conv.dialog = null;
                }
            }
        }

        /// <summary>
        /// 打开对话窗
        /// </summary>
        public static IMPrivateForm openDialog(string toUserId, string serverName, string playerName, string avatar)
        {
            IMConversation conv = (IMConversation)conversations[toUserId];
            if (conv == null)
            {
                conv = new IMConversation();
                conv.dialog = null;
                conv.userId = toUserId;
                conversations.Add(toUserId, conv);
                conv.lastMessage = "";
                conv.avatar = avatar;
                conv.playerName = playerName;
                conv.serverName = serverName;
                conv.updateTime = DateTime.MinValue;
                conv.messages = new List<object>();
            }

            if (conv.dialog == null)
            {
                conv.dialog = new IMPrivateForm(toUserId, serverName, playerName, avatar);
            }
            
            // 更新主窗体的未读消息数
            conv.unread = 0;
            mainForm.invokeScript("runJS", new object[] { "ExtUtil.hasUnread", IMConversation.getUnreadCount() });

            return conv.dialog;
        }
        
        /// <summary>
        /// 发送即时消息
        /// </summary>
        public static void sendIMMessage(string toUserId, string sumary, object message)
        {
            if (toUserId == null || sumary == null || message == null || conversations[toUserId] == null)
            {
                return;
            }
            IMConversation conv = (IMConversation)conversations[toUserId];
            conv.updateTime = DateTime.Now;
            conv.lastMessage = sumary;
            conv.messages.Add(message);

            mainForm.invokeScript("runJS", new object[] { "ExtUtil.sendMsg", message });
        }

        /// <summary>
        /// 获取历史消息
        /// </summary>
        public static List<object> getHistoryMessages(string toUserId)
        {
            IMConversation conv = (IMConversation)conversations[toUserId];
            if (conv == null || conv.messages == null)
            {
                return new List<object>();
            }

            return conv.messages;
        }
    }
}
