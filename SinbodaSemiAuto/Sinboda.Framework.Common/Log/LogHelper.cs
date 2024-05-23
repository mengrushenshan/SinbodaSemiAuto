using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.Log
{
    /// <summary>
    /// 文件日志帮助类
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        ///上位机日志
        /// </summary>   
        public static readonly log4net.ILog logSoftWare = log4net.LogManager.GetLogger("logSoftWare");

        /// <summary>
        /// 通讯日志
        /// </summary>
        public static readonly log4net.ILog logCommunication = log4net.LogManager.GetLogger("logCommunication");

        /// <summary>
        ///上位机数据库语句日志
        /// </summary>   
        public static readonly log4net.ILog logSoftWareSQL = log4net.LogManager.GetLogger("logSoftWareSQL");
        /// <summary>
        /// 与Lis通讯日志
        /// </summary>
        public static readonly log4net.ILog logLisComm = log4net.LogManager.GetLogger("logLisComm");
        /// <summary>
        /// 下位机日志
        /// </summary>
        public static readonly log4net.ILog logAnalyzer = log4net.LogManager.GetLogger("logAnalyzer");

        /// <summary>
        /// 调试级别报警日志
        /// </summary>
        public static readonly log4net.ILog debugAlarm = log4net.LogManager.GetLogger("debugAlarm");
    }
}
