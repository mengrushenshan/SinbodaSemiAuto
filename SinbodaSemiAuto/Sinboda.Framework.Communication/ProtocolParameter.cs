using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    /// <summary>
    /// 通讯参数基类
    /// </summary>
    public abstract class ProtocolParameter
    {
        /// <summary>
        /// ID信息
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 串口包头
        /// </summary>
        public byte SerialPortHead { get; set; } = 0x02;
        /// <summary>
        /// 串口包尾
        /// </summary>
        public byte[] SerialPortEnd { get; set; } = new byte[2] { 0x0D, 0x0A };
        /// <summary>
        /// 网口包头
        /// </summary>
        public byte NetworkHead { get; set; } = 0x0A;
        /// <summary>
        /// 网口协议原始板号
        /// </summary>
        public byte NetworkOriginalID { get; set; } = 0x01;
        /// <summary>
        /// 接收缓冲器大小
        /// </summary>
        public int ReceiveBufferSize { get; set; }
        /// <summary>
        /// 发送数据大小
        /// </summary>
        public int SendBufferSize { get; set; }
        /// <summary>
        /// 心跳频率
        /// </summary>
        public int KeepAliveInterval { get; set; }
        ///// <summary>
        ///// 连接超时时间
        ///// </summary>
        //public int ConnectTimeout { get; set; }
        ///// <summary>
        ///// 接收超时时间
        ///// </summary>
        //public int ReceiveDataTimeout { get; set; }
        ///// <summary>
        ///// 发送超时时间
        ///// </summary>
        //public int SendDataTimeout { get; set; }

        /// <summary>
        /// 握手密钥
        /// </summary>
        public byte[] ShakeHandKey { get; set; }
        /// <summary>
        /// 通讯前半密钥
        /// </summary>
        public byte[] CommunicationHalfKey { get; set; }
        /// <summary>
        /// 握手命令
        /// </summary>
        public ushort ShakeHandCommand { get; set; }
        /// <summary>
        /// 通讯加密后确认命令
        /// </summary>
        public ushort CommunicationEnsureCommand { get; set; }

        /// <summary>
        /// 标准密钥
        /// </summary>
        public byte[] StandardKey { get; set; }

        private int reconnectionTime = 3;
        /// <summary>
        /// 重连次数
        /// </summary>
        public int ReconnectionTime
        {
            get { return reconnectionTime; }
            set
            {
                if (value < 1)
                    return;

                reconnectionTime = value;
            }
        }

        private int timeout = 5000;
        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProtocolParameter()
        {
            ReceiveBufferSize = 10240;
            SendBufferSize = 10240;
            KeepAliveInterval = 15000;
            //ConnectTimeout = 5000;
            //SendDataTimeout = 5000;
            //ReceiveDataTimeout = 20000;
        }
    }
}
