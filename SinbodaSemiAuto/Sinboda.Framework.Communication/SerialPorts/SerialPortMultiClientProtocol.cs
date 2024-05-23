using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using Sinboda.Framework.Communication.Utils;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sinboda.Framework.Communication.Utils.WriteLogs;

namespace Sinboda.Framework.Communication.SerialPorts
{
    /// <summary>
    /// 串口客户端模式
    /// </summary>
    public class SerialPortMultiClientProtocol : MultiClientDeviceProtocol
    {
        #region 成员变量
        /// <summary>
        /// 串口通讯
        /// </summary>
        protected Dictionary<string, SerialPortClient> m_SerialDictionary = new Dictionary<string, SerialPortClient>();
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SerialPortMultiClientProtocol() : base()
        {
            _packageHandle = new SerialPortPackageHeadHandle();
        }

        #region 对外接口
        /// <summary>
        /// 添加串口参数
        /// </summary>
        /// <param name="parameters"></param>
        public override void AddParameters(List<ProtocolParameter> parameters)
        {
            foreach (var item in parameters)
            {
                if (null == item)
                {
                    continue;
                }

                SerialPortClient oneClient = new SerialPortClient((item as SerialPortParameter), null);
                oneClient.OnReceive += OnReceived;
                oneClient.ReceiveError += OnErrorReceived;
                oneClient.SendData += SendPackage;

                if (!m_SerialDictionary.ContainsKey(item.ID))
                {
                    LogHelper.logSoftWare.Info("ID not exist, ID is" + item.ID + ", original portName" + (item as SerialPortParameter).PortName);

                    if (ISNeedEncrypt == EncryptMode.ShakeHand)
                    {
                        AutoEventThread autoEventThread = new AutoEventThread(SendPackage1, 15, item.ID, item.ShakeHandKey, item.CommunicationHalfKey, item.ShakeHandCommand, item.CommunicationEnsureCommand);
                        oneClient.SetSecretParam(ref autoEventThread);
                    }
                }
                else
                {
                    LogHelper.logSoftWare.Info("ID exist, ID is" + item.ID + ", original portName" + m_SerialDictionary[item.ID].GetPortName() + ", new portName" + (item as SerialPortParameter).PortName);
                }

                m_SerialDictionary[item.ID] = oneClient;
            }
        }

        /// <summary>
        /// 移除串口参数
        /// </summary>
        /// <param name="parameters"></param>
        public override void RemoveParameters(List<ProtocolParameter> parameters)
        {
            if (m_SerialDictionary.Count > 0)
            {
                foreach (var item in parameters)
                {
                    if (m_SerialDictionary.ContainsKey(item.ID))
                    {
                        m_SerialDictionary[item.ID].Disconnect();
                        m_SerialDictionary.Remove(item.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 清除参数
        /// </summary>
        public override void ClearParameters()
        {
            //if (_serialPortDictionary.Count > 0)
            //    _serialPortDictionary.Clear();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, bool> Connect(string[] id = null)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (var item in m_SerialDictionary)
            {
                result[item.Key] = false;
            }

            try
            {
                if (id == null)
                {
                    foreach (var item in m_SerialDictionary)
                    {
                        var serialPort = item.Value;
                        LogHelper.logSoftWare.Info($"id is {id}, portName is {serialPort.GetPortName()}");

                        if (serialPort.Connect())
                        {
                            result[item.Key] = true;
                        }
                    }
                }
                else
                {
                    foreach (var item in id)
                    {
                        if (!m_SerialDictionary.ContainsKey(item))
                        {
                            continue;
                        }

                        var serialPort = m_SerialDictionary[item];
                        LogHelper.logSoftWare.Info($"id is {id}, portName is {serialPort.GetPortName()}");

                        if (serialPort.Connect())
                        {
                            result[item] = true;
                        }
                    }
                }

                if (m_SerialDictionary.Values.Where(o => o.IsOpen()).Count() == 0)
                {
                    Connected = false;
                    return result;
                }
                else
                {
                    Connected = true;
                    StartThreads();

                    if (ISNeedEncrypt == EncryptMode.ShakeHand)
                    {
                        foreach (var item in m_SerialDictionary)
                        {
                            if (null != item.Value)
                            {
                                item.Value.DoShakeHandSecret();
                            }
                        }

                        //调用握手及加密
                        int lastTick = System.Environment.TickCount;
                        while (System.Environment.TickCount - lastTick < 10000)
                        {
                            foreach (var item in m_SerialDictionary)
                            {
                                if (!item.Value.SecretSuccess())
                                    result[item.Key] = false;
                                Thread.Sleep(10);
                            }
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("SerialPort Connect error", ex);
                return result;
            }
        }

        /// <summary>
        /// 断开
        /// </summary>
        public override void Disconnect(string[] id = null)
        {
            if (id == null)
            {
                foreach (var item in m_SerialDictionary)
                {
                    var serialPort = item.Value;
                    if (null != serialPort)
                    {
                        serialPort.Disconnect();
                    }
                }
            }
            else
            {
                foreach (var item in id)
                {
                    if (m_SerialDictionary.ContainsKey(item) &&
                        null != m_SerialDictionary[item])
                    {
                        m_SerialDictionary[item].Disconnect();
                    }
                }
            }

            if (m_SerialDictionary.Values.Where(o => o.IsOpen()).Count() == 0)
            {
                Connected = false;
            }
        }
        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public override bool GetConnected(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].IsOpen();
            }

            return false;
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public override bool SendPackage(DataPackage package)
        {
            byte[] data = PackageSendData(package);

            if (null != data)
            {
                if (!PostSend(package.PackageInfo.ID, data, 0, data.Length))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 发送加密数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        public override bool SendPackage1(string id, ushort command, byte[] sendData, byte[] key)
        {
            byte[] data = null;
            if (_packageHandle.SetPackageForSendEncyptData(command, sendData, key, ref data))
            {
                if (!PostSend(id, data, 0, data.Length))
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 开始线程
        /// </summary>
        private void StartThreads()
        {
            if (_sendEventThread == null && _receiveEventThread == null)
            {
                _sendEventThread = new EventThread(SendThreadHandler);
                LogHelper.logCommunication.Debug("发送线程创建并启动");

                _receiveEventThread = new EventThread(ReceiveThreadHandler, ThreadPriority.Highest);
                LogHelper.logCommunication.Debug("接收线程创建并启动");
            }
        }

        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="portName"></param>
        protected override void DoDataReceived(ProtocolParameter param)
        {
            DataPackage pak = null;

            SerialPortParameter serialParam = param as SerialPortParameter;
            string id = serialParam.ID;
            string portName = serialParam.PortName;
            if (ISNeedEncrypt == EncryptMode.ShakeHand)
            {
                while (_packageHandle.GetPackageForReceiveEncyptData(_recieveList, IsUsingNewKey(id) ? GetRealKey(id) : GetOrinalKey(id), ref pak, GetCommunicateType(), portName))
                {
                    if (pak == null)
                    {
                        continue;
                    }

                    m_SerialDictionary[id].AnalysisKeepAlivePkg(pak.PackageInfo);
                    pak.PackageInfo.ID = id;

                    //握手命令
                    if (pak.PackageInfo.Command == GetShakeHandCommand(id))
                    {
                        SetInfoForCom(id, pak.Data);
                    }
                    //发送数据确认命令
                    else if (pak.PackageInfo.Command == GetCommunicationEnsureCommand(id))
                    {
                        SetInfoForCom(id, pak.Data);
                    }
                    else
                    {
                        OnDataReceived(pak);
                    }
                }
            }
            else if (ISNeedEncrypt == EncryptMode.None)
            {
                while (_packageHandle.GetPackage(_recieveList, ref pak, GetCommunicateType(), portName))
                {
                    if (pak == null)
                    {
                        continue;
                    }

                    m_SerialDictionary[id].AnalysisKeepAlivePkg(pak.PackageInfo);
                    pak.PackageInfo.ID = id;
                    OnDataReceived(pak);
                }
            }
            else
            {
                while (_packageHandle.GetPackageForReceiveEncyptData(_recieveList, GetStandardKey(id), ref pak, GetCommunicateType(), portName))
                {
                    if (pak == null)
                    {
                        continue;
                    }

                    m_SerialDictionary[id].AnalysisKeepAlivePkg(pak.PackageInfo);
                    pak.PackageInfo.ID = id;
                    OnDataReceived(pak);
                }
            }

            if (_recieveList.Count > 0)
            {
                _recieveList = new List<byte>();

                LogHelper.logCommunication.InfoFormat("_recieveList.Count = {0}", _recieveList.Count);
            }
        }

        /// <summary>
        /// 通讯类别
        /// </summary>
        /// <returns></returns>
        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.SerialPort;
        }

        protected override bool PostSend(string id, byte[] buffer, int offset, int count)
        {
            if (!m_SerialDictionary.ContainsKey(id))
            {
                return false;
            }


            byte[] destBuffer = new byte[count];
            Array.Copy(buffer, offset, destBuffer, 0, count);
            List<byte> dataList = destBuffer.ToList();

            m_SerialDictionary[id].m_SendQueue.Enqueue(dataList);
            _sendEventThread.Enqueue(id);

            return true;
        }

        protected override void SendThreadHandler()
        {
            object obj = null;
            while (Connected && EventThread.Dequeue(_sendEventThread, out obj))
            {
                if (obj == null)
                {
                    continue;
                }

                doSendAction(obj);
            }
        }

        protected override void ReceiveThreadHandler()
        {
            object obj = null;
            while (Connected && EventThread.Dequeue(_receiveEventThread, out obj))
            {
                if (obj == null)
                {
                    continue;
                }

                doAction(obj);
            }
        }

        protected void doAction(object obj)
        {
            try
            {
                SerialPortParameter serialParam = obj as SerialPortParameter;
                if (null == serialParam || !m_SerialDictionary.ContainsKey(serialParam.ID))
                {
                    return;
                }

                // 此处防止串口分包发送
                if (m_SerialDictionary[serialParam.ID].m_RecieveQueue.Count > 0 &&
                    m_SerialDictionary[serialParam.ID].m_RecieveQueue.Where(o => o == 0x0d).Count() > 0 &&
                    m_SerialDictionary[serialParam.ID].m_RecieveQueue.Where(o => o == 0x0a).Count() > 0)
                {
                    List<byte> listTmp = new List<byte>();
                    while (true)
                    {
                        if (listTmp.LastOrDefault() == 0x0a)
                            break;
                        byte dataTmp = new byte();
                        if (m_SerialDictionary[serialParam.ID].m_RecieveQueue.TryDequeue(out dataTmp))
                            listTmp.Add(dataTmp);
                    }

                    _recieveList.AddRange(listTmp.ToArray());
                    LogHelper.logCommunication.DebugFormat("****** OnDataReceived 平台处理一次数据，PortName：{0}", serialParam.PortName);
                    DoDataReceived(serialParam);
                }
            }
            catch (ThreadAbortException abortexcepion)
            {
                Disconnect();
            }
            catch (SocketException socketexception)
            {
                LogHelper.logCommunication.Error("ReceiveThread SocketException error", socketexception);
                OnCommunicateError(CommunicateError.CommunicationException, socketexception);
                Disconnect();
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.Error("接收处理出现：" + e.GetType().Name + "异常", e);
                OnCommunicateError(CommunicateError.UnknownError, e);
            }
        }

        protected void doSendAction(object obj)
        {
            try
            {
                string serialId = (string)obj;
                if (!m_SerialDictionary.ContainsKey(serialId))
                {
                    return;
                }

                List<byte> dataList = new List<byte>();
                if (m_SerialDictionary[serialId].m_SendQueue.TryDequeue(out dataList))
                {
                    byte[] data = dataList.ToArray();
                    if (!m_SerialDictionary[serialId].WriteBuffer(data, 0, data.Length))
                    {
                        LogHelper.logCommunication.Debug("发送数据失败");
                    }
                    else
                    {
                        string portName = m_SerialDictionary[serialId].GetPortName();
                        if (ISNeedEncrypt == EncryptMode.ShakeHand)
                        {
                            WriteLogForSendEncryptData(data, data.Length, m_SerialDictionary[serialId].IsUsingNewKey() ? m_SerialDictionary[serialId].GetRealKey() : m_SerialDictionary[serialId].GetOrinalKey(), communacationWay.SEND, GetCommunicateType(), portName);
                        }
                        else if (ISNeedEncrypt == EncryptMode.None)
                        {
                            WriteLog(data, data.Length, communacationWay.SEND, GetCommunicateType(), portName);
                        }
                        else
                        {
                            WriteLogForSendEncryptData(data, data.Length, m_SerialDictionary[serialId].GetStandardKey(), communacationWay.SEND, GetCommunicateType(), portName);
                        }
                    }
                }
            }
            catch (ThreadAbortException abortexception)
            {
                LogHelper.logCommunication.Error("线程异常：" + abortexception.Message);
            }
            catch (Exception e)
            {
                LogHelper.logCommunication.Error("发送处理出现：" + e.GetType().Name + "异常", e);
                OnCommunicateError(CommunicateError.UnknownError, e);
            }
        }

        /// <summary>
        /// 打包发送数据
        /// </summary>
        /// <param name="sendData"></param>
        /// <returns></returns>
        private byte[] PackageSendData(object sendData)
        {
            byte[] data = null;
            if (null == sendData || false == (sendData is DataPackage))
            {
                return data;
            }

            DataPackage package = sendData as DataPackage;

            if (ISNeedEncrypt == EncryptMode.ShakeHand)
            {
                _packageHandle.SetPackageForSendEncyptData((ushort)package.PackageInfo.Command, package.Data, IsUsingNewKey(package.PackageInfo.ID) ? GetRealKey(package.PackageInfo.ID) : GetOrinalKey(package.PackageInfo.ID), ref data);
            }
            else if (ISNeedEncrypt == EncryptMode.None)
            {
                _packageHandle.SetPackage(package, ref data);
            }
            else
            {
                _packageHandle.SetPackageForSendEncyptData((ushort)package.PackageInfo.Command, package.Data, GetStandardKey(package.PackageInfo.ID), ref data);
            }

            return data;
        }

        /// <summary>
        /// 获取加密秘钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override byte[] GetStandardKey(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].GetStandardKey();
            }

            return new byte[8];
        }

        /// <summary>
        /// 获取当前使用什么密钥（握手密钥或是通讯密钥）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override bool IsUsingNewKey(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].IsUsingNewKey();
            }

            return false;
        }
        /// <summary>
        /// 获取握手密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override byte[] GetOrinalKey(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].GetOrinalKey();
            }

            return new byte[8];
        }

        /// <summary>
        /// 获取通讯密钥
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override byte[] GetRealKey(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].GetRealKey();
            }

            return new byte[8];
        }

        /// <summary>
        /// 获取握手命令
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override ushort GetShakeHandCommand(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].GetShakeHandCommand();
            }

            return 0;
        }

        /// <summary>
        /// 获取通讯确认命令
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected override ushort GetCommunicationEnsureCommand(string id)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].GetCommunicationEnsureCommand();
            }

            return 0;
        }

        /// <summary>
        /// 下位机返回信息并设置信号量
        /// </summary>
        protected override bool SetInfoForCom(string id, byte[] inputInfo)
        {
            if (m_SerialDictionary.ContainsKey(id))
            {
                return m_SerialDictionary[id].SetInfoForCom(inputInfo);
            }

            return false;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnReceived(object sender)
        {
            _receiveEventThread.Enqueue(sender);
        }
        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnErrorReceived(object sender)
        {
            SerialError errorType = (SerialError)sender;
            switch (errorType)
            {
                case SerialError.RXParity:
                case SerialError.Frame:
                    OnCommunicateError(CommunicateError.CheckDataError, null);
                    break;
                case SerialError.Overrun:
                    OnCommunicateError(CommunicateError.UnknownError, null);
                    break;
                case SerialError.RXOver:
                    OnCommunicateError(CommunicateError.ReceiveFailed, null);
                    break;
                case SerialError.TXFull:
                    OnCommunicateError(CommunicateError.SendFailed, null);
                    break;
            }
        }
        #endregion
    }
}
