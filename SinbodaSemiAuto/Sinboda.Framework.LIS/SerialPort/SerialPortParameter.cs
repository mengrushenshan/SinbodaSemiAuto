using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 串口通讯参数
    /// </summary>
    public class SerialPortParameter: LISProtocolParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortParameter() : base()
        {
            PortName = "COM2";
            BaudRate = 115200;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.None;
            SendTimeout = 1000;
            ReSendTime = 3;
            IsSplitSend = true;
            FrameLength = 200;
        }
        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// 奇偶校验
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits { get; set; }

        /// <summary>
        /// 串口使用：是否拆分发送，默认是true
        /// </summary>
        public bool IsSplitSend
        { get; set; }
        /// <summary>
        /// 串口使用：默认200,每一帧最多包含207个字符（包含帧开头和结尾字符），
        /// 超过200个字符的消息被分成两帧或多帧
        /// </summary>
        public int FrameLength
        { get; set; }
        /// <summary>
        /// 串口使用：超时重发次数
        /// </summary>
        public int ReSendTime
        { get; set; }
        /// <summary>
        /// 发送超时，默认1000毫秒
        /// </summary>
        public int SendTimeout
        { get; set; }
    }
}
