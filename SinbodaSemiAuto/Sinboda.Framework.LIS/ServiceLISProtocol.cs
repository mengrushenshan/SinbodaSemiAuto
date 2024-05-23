using Sinboda.Framework.LIS.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS
{
    public abstract class ServiceLISProtocol : ILISProtocol
    {
        #region 属性
        protected readonly object lockObj = new object();
        public CommunicateType CommunicateType
        {
            get
            {
                return GetCommunicateType();
            }
        }

        public bool Connected
        {
            get;
        }

        Common.Encoding _encoding = Common.Encoding.Default;
        public Common.Encoding Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = value;
            }
        }

        protected int frameLength = 200;
        public int FrameLength
        {
            get
            {
                return frameLength;
            }
            set
            {
                frameLength = value;
            }
        }

        protected bool isSplitSend = true;
        public bool IsSplitSend
        {
            get
            {
                return isSplitSend;
            }
            set
            {
                isSplitSend = value;
            }
        }

        /// <summary>
        /// 通讯参数
        /// </summary>
        protected LISProtocolParameter _parameter = null;
        public LISProtocolParameter Parameter
        {
            get
            {
                if (null == _parameter)
                    _parameter = CreateParameterInstance();
                return _parameter;
            }
            set
            {
                _parameter = value;
            }
        }

        /// <summary>
        /// 发送次数默认是3
        /// </summary>
        protected int reSendTime = 3;
        public int ReSendTime
        {
            get
            {
                return reSendTime;
            }
            set
            {
                reSendTime = value;
            }
        }

        /// <summary>
        /// 网口：连接超时，默认3秒
        /// </summary>
        private int timeout = 3000;
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        Component ILISProtocol.SendData
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取通讯方式
        /// </summary>
        /// <returns></returns>
        protected abstract CommunicateType GetCommunicateType();
        /// <summary>
        /// 创建通讯参数
        /// </summary>
        /// <returns></returns>
        protected abstract LISProtocolParameter CreateParameterInstance();

        /// <summary>
        /// 连接
        /// </summary>
        public abstract bool Connect();
        /// <summary>
        /// 断开
        /// </summary>
        public abstract void Disconnect();
        /// <summary>
        /// 
        /// </summary>
        public abstract string MessageToString();
        /// <summary>
        /// 发送一个流数据，目前网口使用
        /// </summary>
        /// <param name="stream">数据流</param>
        public virtual void SendStream(Stream stream) { }

        /// <summary>
        /// 发送Message数据
        /// </summary>
        public virtual void SendMessage() { }

        //public abstract void SendMessage(Component message);
        /// <summary>
        /// 发送命令,串口使用
        /// </summary>
        public virtual void SendCommand(string command) { }

        public abstract void SendMessage(string socketIP, Component sendData);
        #endregion

        #region 事件
        /// <summary>
        /// 接收到数据引发的事件
        /// </summary>
        public virtual event InceptEvent OnAccept;
        /// <summary>
        /// 发生错误引发的事件
        /// </summary>
        public virtual event ErrorEvent OnError;
        /// <summary>
        /// 连接事件
        /// </summary>
        public virtual event ConnectEvent OnConnect;
        /// <summary>
        /// 断开连接事件
        /// </summary>
        public virtual event DisconnectEvent OnDisconnect;
        #endregion
    }
}
