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
    /// <summary>
    /// 
    /// </summary>
    public abstract class ClientLISProtocol : ILISProtocol
    {
        #region 属性
        protected readonly object lockObj = new object();
        /// <summary>
        /// 自动重连信号
        /// </summary>
        //public ManualResetEvent autoResetEvent;
        /// <summary>
        /// 通讯参数
        /// </summary>
        protected LISProtocolParameter _parameter = null;

        /// <summary>
        /// 判断是否联机
        /// </summary>
        public bool Connected
        {
            get { return GetConnected(); }
        }
        /// <summary>
        /// 是否联机
        /// </summary>
        /// <returns></returns>
        protected abstract bool GetConnected();
        /// <summary>
        /// 连接LIS的通讯参数
        /// </summary>
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
        /// 创建通讯参数
        /// </summary>
        /// <returns></returns>
        protected abstract LISProtocolParameter CreateParameterInstance();

        /// <summary>
        /// 设备通讯方式
        /// </summary>
        public CommunicateType CommunicateType
        {
            get { return GetCommunicateType(); }
        }
        /// <summary>
        /// 获取通讯方式
        /// </summary>
        /// <returns></returns>
        protected abstract CommunicateType GetCommunicateType();
        ///// <summary>
        ///// LIS客户端对象
        ///// </summary>
        //protected Component _lisClient = null;
        ///// <summary>
        ///// LIS客户端对象(网口，串口)
        ///// </summary>
        //public Component LisClient
        //{
        //    get
        //    {
        //        if (null == _lisClient)
        //            _lisClient = CreateLISClientInstance();
        //        return _lisClient;
        //    }
        //    set
        //    {
        //        _lisClient = value;
        //    }
        //}

        /// <summary>
        /// 创建LIS客户端对象
        /// </summary>
        /// <returns></returns>
        protected abstract Component CreateLISClientInstance();

        /// <summary>
        /// 发送lis绑定数据
        /// </summary>
        protected Component _sendData = null;
        /// <summary>
        /// 发送lis绑定数据
        /// </summary>
        public Component SendData
        {
            get
            {
                if (null == _sendData)
                    _sendData = CreateLISSendDataInstance();
                return _sendData;
            }

            set
            {
                _sendData = value;
            }
        }
        /// <summary>
        /// 创建LIS客户端对象
        /// </summary>
        /// <returns></returns>
        protected abstract Component CreateLISSendDataInstance();

        /// <summary>
        /// 发送超时默认1000毫秒
        /// </summary>
        private int sendTimeout = 1000;
        /// <summary>
        /// 发送超时默认1000毫秒
        /// </summary>
        protected int SendTimeout
        {
            get
            {
                return sendTimeout;
            }

            set
            {
                sendTimeout = value;
            }
        }
        /// <summary>
        /// 网口：连接超时，默认3秒
        /// </summary>
        private int timeout = 3000;
        /// <summary>
        /// 网口：连接超时
        /// </summary>
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
        /// <summary>
        /// 发送次数默认是3
        /// </summary>
        protected int reSendTime = 3;
        /// <summary>
        /// 串口使用：超时重发次数
        /// </summary>
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
        /// 帧长度
        /// </summary>
        protected int frameLength = 200;
        /// <summary>
        /// 串口使用：默认200,每一帧最多包含207个字符（包含帧开头和结尾字符），
        /// 超过200个字符的消息被分成两帧或多帧
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        protected bool isSplitSend = true;
        /// <summary>
        /// 串口使用：是否拆分发送，默认是true
        /// </summary>
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
        Common.Encoding _encoding = Common.Encoding.Default;
        /// <summary>
        /// 字符编码
        /// </summary>
        public Common.Encoding Encoding
        {
            set
            {
                _encoding = value;
            }
            get
            {
                return _encoding;
            }
        }
        protected int autoConnectTime = 30000;
        /// <summary>
        /// 网口：断网重连的时间，单位毫秒，默认30秒
        /// </summary>
        public int AutoConnectTime
        {
            set
            {
                autoConnectTime = value;
            }
            get
            {
                return autoConnectTime;
            }
        }

        protected bool isAutoConnect = false;
        /// <summary>
        /// 网口：断网后是否自动重连
        /// </summary>
        public bool IsAutoConnect
        {
            set
            {
                isAutoConnect = value;
            }
            get
            {
                return isAutoConnect;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 连接
        /// </summary>
        public abstract bool Connect();
        /// <summary>
        /// 断开
        /// </summary>
        public abstract void Disconnect();
        /// <summary>
        /// 断开
        /// </summary>
        public abstract void Disconnect(bool isThrow);
        /// <summary>
        /// 
        /// </summary>
        public abstract string MessageToString();
        /// <summary>
        /// 发送一个流数据，目前网口使用
        /// </summary>
        /// <param name="stream">数据流</param>
        public abstract void SendStream(Stream stream);

        /// <summary>
        /// 发送Message数据
        /// </summary>
        public abstract void SendMessage();

        //public abstract void SendMessage(Component message);
        /// <summary>
        /// 发送命令,串口使用
        /// </summary>
        public virtual void SendCommand(string command) { }

        public virtual void SendMessage(string socketIP, Component sendData) { }

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
        /// <summary>
        ///  超时事件
        /// </summary>
        public virtual event TimeoutEvent OnTimeout;
        #endregion
    }
}
