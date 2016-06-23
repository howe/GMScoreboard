using System;
using System.Collections.Generic;
using System.Text;

namespace GMScoreboard.Util
{
    /// <summary>
    /// Json类型，与常量表“JSON类型”开关的常量对应（如“JSON类型_普通”）
    /// </summary>
    public enum en_JsonType
    {
        /// <summary>
        /// JSON类型_普通
        /// </summary>
        Common = 0,
        /// <summary>
        /// JSON类型_对象
        /// </summary>
        Object,
        /// <summary>
        /// JSON类型_数组
        /// </summary>
        Array
    }

    /// <summary>
    /// 战绩常量数据，与常量表“战绩”开头的常量做对应（如“战绩_胜”、“战绩_负”等）
    /// </summary>
    public class cls_Result
    {
        /// <summary>
        /// 战绩_胜
        /// </summary>
        public static readonly string WIN = "WIN";
        /// <summary>
        /// 战绩_负
        /// </summary>
        public static readonly string LOSE = "LOSE";
        /// <summary>
        /// 战绩_击杀
        /// </summary>
        public static readonly string CHAMPIONS_KILLED = "CHAMPIONS_KILLED";
        /// <summary>
        /// 战绩_死亡
        /// </summary>
        public static readonly string NUM_DEATHS = "NUM_DEATHS";
        /// <summary>
        /// 战绩_助攻
        /// </summary>
        public static readonly string ASSISTS = "ASSISTS";
        /// <summary>
        /// 战绩_补刀
        /// </summary>
        public static readonly string MINIONS_KILLED = "MINIONS_KILLED";
        /// <summary>
        /// 战绩_打钱
        /// </summary>
        public static readonly string GOLD_EARNED = "GOLD_EARNED";
        /// <summary>
        /// 战绩_最高多杀
        /// </summary>
        public static readonly string LARGEST_MULTI_KILL = "LARGEST_MULTI_KILL";
        /// <summary>
        /// 战绩_最高连杀
        /// </summary>
        public static readonly string LARGEST_KILLING_SPREE = "LARGEST_KILLING_SPREE";
        /// <summary>
        /// 战绩_英雄等级
        /// </summary>
        public static readonly string LEVEL = "LEVEL";
    }

}
