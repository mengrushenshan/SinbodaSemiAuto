using System.Collections;
using System.IO;
using System.Net.Sockets;

namespace Sinboda.Framework.LIS.SinHL7
{
    class StateObj
    {
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="bufferSize">缓存</param>
        /// <param name="WorkSocket">工作的插座</param>
        public StateObj(int bufferSize, Socket WorkSocket)
        {
            buffer = new byte[bufferSize];
            workSocket = WorkSocket;
        }
        /// <summary>
        /// 缓存
        /// </summary>
        public byte[] buffer = null;
        /// <summary>
        /// 工作插座
        /// </summary>
        public Socket workSocket = null;
        /// <summary>
        /// 数据流
        /// </summary>
        public Stream Datastream = new MemoryStream();
        /// <summary>
        /// HL7组件
        /// </summary>
        //public HL7Message recvhl7Message = new HL7Message();
        ///// <summary>
        ///// 剩余大小
        ///// </summary>
        //public long residualSize = 0;
        ///// <summary>
        ///// 数据包大小
        ///// </summary>
        //public long packSize = 0;
        ///// <summary>
        ///// 计数器
        ///// </summary>
        //public int Cortrol = 0;
    }



}
