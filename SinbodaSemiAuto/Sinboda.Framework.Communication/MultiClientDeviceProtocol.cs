using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using Sinboda.Framework.Communication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    public abstract class MultiClientDeviceProtocol : IMultiDeviceProtocol
    {
        #region 属性
        /// <summary>
        /// 通讯参数
        /// </summary>
        protected ProtocolParameter _parameters = null;
        /// <summary>
        /// 封包处理
        /// </summary>
        protected IPackageHandle _packageHandle = null;
        /// <summary>
        /// 发送线程
        /// </summary>
        protected EventThread _sendEventThread = null;
        /// <summary>
        /// 接收线程
        /// </summary>
        protected EventThread _receiveEventThread = null;
        /// <summary>
        /// 接收集合
        /// </summary>
        protected List<byte> _recieveList = new List<byte>();

        /// <summary>
        /// 设备通讯方式
        /// </summary>
        public CommunicateType CommunicateType
        {
            get { return GetCommunicateType(); }
        }

        /// <summary>
        /// 通讯是否加密
        /// </summary>
        public EncryptMode ISNeedEncrypt { get; set; }
        #endregion

        #region 变量
        /// <summary>
        /// 是否有连接（多组连接有一个连接存在即为true）
        /// </summary>
        protected bool Connected { get; set; }
        #endregion

        #region 事件
        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event ClientDataReceivedEventHandler DataReceivedEvent;
        /// <summary>
        /// 错误事件
        /// </summary>
        public event CommunicateErrorEventHandler CommunicateErrorEvent;
        #endregion

        #region 抽象方法
        /// <summary>
        /// 添加通讯参数
        /// </summary>
        public abstract void AddParameters(List<ProtocolParameter> parameters);
        /// <summary>
        /// 移除通讯参数
        /// </summary>
        public abstract void RemoveParameters(List<ProtocolParameter> parameters);
        /// <summary>
        /// 清空通讯参数
        /// </summary>
        public abstract void ClearParameters();

        /// <summary>
        /// 联机
        /// </summary>
        public abstract Dictionary<string, bool> Connect(string[] id = null);
        /// <summary>
        /// 脱机
        /// </summary>
        public abstract void Disconnect(string[] id = null);

        /// <summary>
        /// 是否联机
        /// </summary>
        /// <returns></returns>
        public abstract bool GetConnected(string id);

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        public abstract bool SendPackage(DataPackage package);

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        public abstract bool SendPackage1(string id, ushort command, byte[] sendData, byte[] key);

        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="portName"></param>
        protected abstract void DoDataReceived(ProtocolParameter param);

        /// <summary>
        /// 获取通讯方式
        /// </summary>
        /// <returns></returns>
        protected abstract CommunicateType GetCommunicateType();

        /// <summary>
        /// 将发送数据添加到发送队列
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected abstract bool PostSend(string id, byte[] buffer, int offset, int count);

        protected abstract void SendThreadHandler();

        protected abstract void ReceiveThreadHandler();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract byte[] GetStandardKey(string id);

        /// <summary>
        /// 判断当前使用什么密钥（握手密钥或是通讯密钥）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract bool IsUsingNewKey(string id);

        /// <summary>
        /// 获取握手密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract byte[] GetOrinalKey(string id);

        /// <summary>
        /// 获取通讯密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract byte[] GetRealKey(string id);

        /// <summary>
        /// 获取握手密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract ushort GetShakeHandCommand(string id);

        /// <summary>
        /// 获取通讯确认命令
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected abstract ushort GetCommunicationEnsureCommand(string id);

        /// <summary>
        /// 对执行握手流程的串口进行返回数据设置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputInfo"></param>
        /// <returns></returns>
        protected abstract bool SetInfoForCom(string id, byte[] inputInfo);
        #endregion

        #region 共通方法
        /// <summary>
        /// 创建不同通讯模式对应的封包处理
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void PackageHandler(CommunicateType type)
        {
            //switch (type)
            //{
            //    case CommunicateType.Network:
            //        _parameters = new NetworkParameter();
            //        _packageHandle = new NetworkPackageHeadHandle();
            //        LogHelper.logCommunication.Debug("创建网口解析方式");
            //        break;
            //    case CommunicateType.SerialPort:
            //        _parameters = new SerialPortParameter();
            //        _packageHandle = new SerialPortPackageHeadHandle();
            //        LogHelper.logCommunication.Debug("创建串口解析方式");
            //        break;
            //    default:
            //        _packageHandle = null;
            //        break;
            //}
        }

        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="package"></param>
        protected void OnDataReceived(DataPackage package)
        {
            if (DataReceivedEvent != null)
            {
                if (GetCommunicateType() == CommunicateType.SerialPort)
                {
                    if (package != null)
                    {
                        int modelId = (package.PackageInfo != null) ? (package.PackageInfo.Command >> 8) & 0xff : int.MinValue;
                        int command = (package.PackageInfo != null) ? (package.PackageInfo.Command & 0xff) : int.MinValue;

                        StringBuilder packageData = new StringBuilder();
                        if (package.Data != null)
                            foreach (var byteData in package.Data)
                            {
                                packageData.Append(Convert.ToString(byteData, 16).PadLeft(2, '0'));
                                packageData.Append(" ");
                            }

                        LogHelper.logCommunication.DebugFormat("****** OnDataReceived 平台向外发送数据，模块：{0}，指令码：{1:X2}, 数据：{2}",
                            modelId,
                            command,
                            packageData.ToString());
                    }
                    else
                        LogHelper.logCommunication.DebugFormat("****** OnDataReceived 平台向外发送数据，数据：package == null");
                }

                ReceivedEventArgs e = new ReceivedEventArgs(package);
                DataReceivedEvent(this, e);

                LogHelper.logCommunication.DebugFormat("****** OnDataReceived 平台向外发送数据完成");
            }
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="error"></param>
        /// <param name="e"></param>
        protected void OnCommunicateError(CommunicateError error, Exception e)
        {
            if (CommunicateErrorEvent != null)
            {
                CommunicateErrorEventArgs args = new CommunicateErrorEventArgs(error, e);
                CommunicateErrorEvent(this, args);
            }
        }

        #endregion
    }
}
