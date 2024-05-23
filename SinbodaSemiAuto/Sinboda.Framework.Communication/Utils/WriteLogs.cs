using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.Utils
{
    /// <summary>
    /// 记录日志
    /// </summary>
    internal static class WriteLogs
    {
        /// <summary>
        /// 书写通讯日志
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <param name="way">收或发</param>
        public static void WriteLog(byte[] data, int length, communacationWay way, CommunicateType types, string portName = "")
        {
            if (data.Length == 0 || length == 0 || length > data.Length) return;

            StringBuilder sb = new StringBuilder();
            if (types == CommunicateType.SerialPort)
            {
                sb.Append(Convert.ToString(data[0], 16).PadLeft(2, '0'));
                sb.Append(" ");

                byte[] dataBytes = new byte[length - 3];
                Array.Copy(data, 1, dataBytes, 0, dataBytes.Length);
                byte[] tmp = SerialPortPackageHeadHandle.TransferToShortValue(dataBytes);

                for (int i = 0; i < tmp.Length; i++)
                {
                    sb.Append(Convert.ToString(tmp[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }

                sb.Append(Convert.ToString(data[length - 2], 16).PadLeft(2, '0'));
                sb.Append(" ");
                sb.Append(Convert.ToString(data[length - 1], 16).PadLeft(2, '0'));
                sb.Append(" ");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }
            }

            string infoHead = portName + "\t";
            if (way == communacationWay.SEND)
                infoHead += ">>>>>>      ";
            if (way == communacationWay.RECIEVE)
                infoHead += "<<<<<<      ";
            string logInfo = sb.ToString() + " Length(byte):" + length.ToString();
            if (!string.IsNullOrEmpty(logInfo))
                LogHelper.logCommunication.Info(infoHead + logInfo);
        }

        /// <summary>
        /// 书写通讯日志
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <param name="way">收或发</param>
        public static void WriteLogForSendEncryptData(byte[] data, int length, byte[] key, communacationWay way, CommunicateType types, string portName = "")
        {
            if (data.Length == 0 || length == 0 || length > data.Length) return;

            StringBuilder sb = new StringBuilder();
            if (types == CommunicateType.SerialPort)
            {
                sb.Append(Convert.ToString(data[0], 16).PadLeft(2, '0'));
                sb.Append(" ");

                byte[] dataBytes = new byte[length - 3];
                Array.Copy(data, 1, dataBytes, 0, dataBytes.Length);
                byte[] tmp = Helper.DecryptDES(SerialPortPackageHeadHandle.TransferToShortValue(dataBytes), key);
                for (int i = 0; i < tmp.Length; i++)
                {
                    sb.Append(Convert.ToString(tmp[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }

                sb.Append(Convert.ToString(data[length - 2], 16).PadLeft(2, '0'));
                sb.Append(" ");
                sb.Append(Convert.ToString(data[length - 1], 16).PadLeft(2, '0'));
                sb.Append(" ");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }
            }

            string infoHead = portName + "\t";
            if (way == communacationWay.SEND)
                infoHead += ">>>>>>      ";
            if (way == communacationWay.RECIEVE)
                infoHead += "<<<<<<      ";
            string logInfo = sb.ToString();
            if (!string.IsNullOrEmpty(logInfo))
                LogHelper.logCommunication.Info(infoHead + logInfo);
        }

        /// <summary>
        /// 书写通讯日志
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">长度</param>
        /// <param name="way">收或发</param>
        public static void WriteLogForReceiveEncryptData(byte[] data, int length, communacationWay way, CommunicateType types, string portName = "")
        {
            if (data.Length == 0 || length == 0 || length > data.Length) return;

            StringBuilder sb = new StringBuilder();
            if (types == CommunicateType.SerialPort)
            {
                sb.Append(Convert.ToString(data[0], 16).PadLeft(2, '0'));
                sb.Append(" ");

                byte[] dataBytes = new byte[length - 3];
                Array.Copy(data, 1, dataBytes, 0, dataBytes.Length);
                byte[] tmp = dataBytes;
                for (int i = 0; i < tmp.Length; i++)
                {
                    sb.Append(Convert.ToString(tmp[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }

                sb.Append(Convert.ToString(data[length - 2], 16).PadLeft(2, '0'));
                sb.Append(" ");
                sb.Append(Convert.ToString(data[length - 1], 16).PadLeft(2, '0'));
                sb.Append(" ");
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    sb.Append(Convert.ToString(data[i], 16).PadLeft(2, '0'));
                    sb.Append(" ");
                }
            }

            string infoHead = portName + "\t";
            if (way == communacationWay.SEND)
                infoHead += ">>>>>>      ";
            if (way == communacationWay.RECIEVE)
                infoHead += "<<<<<<      ";
            string logInfo = sb.ToString();
            if (!string.IsNullOrEmpty(logInfo))
                LogHelper.logCommunication.Info(infoHead + logInfo);
        }

        public static void WriteDebugLog(string prefix, byte[] data)
        {
            string info = prefix + " : ";
            foreach (var item in data)
                info += " " + Convert.ToString(item, 16).PadLeft(2, '0');
            LogHelper.logCommunication.Debug(info);
        }

        /// <summary>
        /// 书写通讯日志枚举
        /// </summary>
        internal enum communacationWay
        {
            SEND,
            RECIEVE
        }
    }
}
