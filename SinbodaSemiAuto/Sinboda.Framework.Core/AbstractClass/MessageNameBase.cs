using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.AbstractClass
{
    /// <summary>
    /// 消息基类
    /// </summary>
    public class MessageNameBase
    {
        /// <summary>
        /// 设置仪器状态信息
        /// </summary>
        public const string SetAnalyzerCurrentStateInfo = "SetAnalyzerCurrentStateInfo";

        /// <summary>
        /// 系统消息
        /// </summary>
        public const string SetMessageInfo = "SetMessageInfo";
    }

    /// <summary>
    /// 仪器信息
    /// </summary>
    public class AnalyzerCurrentStateInfo
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public string CurrentState { get; set; }
        /// <summary>
        /// 剩余秒数
        /// </summary>
        public int RemainSeconds { get; set; }
    }
    /// <summary>
    /// 计时器时间处理类
    /// </summary>
    public class RemainTimer
    {
        private Int32 _TotalSecond;
        /// <summary>
        /// 总数
        /// </summary>
        public Int32 TotalSecond
        {
            get { return _TotalSecond; }
            set { _TotalSecond = value; }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public RemainTimer(Int32 totalSecond)
        {
            this._TotalSecond = totalSecond;
        }


        /// <summary>
        /// 减秒
        /// </summary>
        /// <returns></returns>
        public bool ProcessRemainTimerDown()
        {
            if (_TotalSecond == 0)
                return false;
            else
            {
                _TotalSecond--;
                return true;
            }
        }


        /// <summary>
        /// 获取小时显示值
        /// </summary>
        /// <returns></returns>
        public string GetHour()
        {
            return String.Format("{0:D2}", (_TotalSecond / 3600));
        }


        /// <summary>
        /// 获取分钟显示值
        /// </summary>
        /// <returns></returns>
        public string GetMinute()
        {
            return String.Format("{0:D2}", (_TotalSecond % 3600) / 60);
        }


        /// <summary>
        /// 获取秒显示值
        /// </summary>
        /// <returns></returns>
        public string GetSecond()
        {
            return String.Format("{0:D2}", _TotalSecond % 60);
        }
    }
}
