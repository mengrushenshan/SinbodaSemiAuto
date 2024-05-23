using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Sinboda.Framework.LIS.SinHL7
{
    /// <summary>
    /// 接收数据事件
    /// </summary>
    public class InceptEventArgs : EventArgs
    {
        private readonly Stream datastream;
        private readonly Socket workSocket;
        private readonly HL7Message recvHL7Message;

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="DataStream">接收到的数据</param>
        /// <param name="WorkSocket">接收的插座</param>
        public InceptEventArgs(HL7Message RecvHL7Message, Stream DataStream, Socket WorkSocket)
        {
            datastream = DataStream;
            workSocket = WorkSocket;
            recvHL7Message = RecvHL7Message;
        }

        public HL7Message RecvHL7Message
        {
            get { return recvHL7Message; }
        }

        /// <summary>
        /// 接受的数据流
        /// </summary>
        public Stream DataStream
        {
            get { return datastream; }
        }
        /// <summary>
        /// 接收的插座
        /// </summary>
        public Socket WorkSocket
        {
            get { return workSocket; }
        }
    }


}
