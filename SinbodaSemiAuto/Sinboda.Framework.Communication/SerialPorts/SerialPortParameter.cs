using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication.SerialPorts
{
    /// <summary>
    /// 串口通讯参数
    /// </summary>
    public class SerialPortParameter : ProtocolParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortParameter() : base()
        {
            PortName = "COM1";
            BaudRate = 115200;
            Parity = Parity.None;
            DataBits = 8;
            StopBits = StopBits.None;
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
    }
}
