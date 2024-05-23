using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Communication.DataPackages;
using Sinboda.Framework.Communication.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sinboda.Framework.Communication.Utils.WriteLogs;

namespace Sinboda.Framework.Communication.Networks
{
    /// <summary>
    /// 网口客户端模式
    /// </summary>
    public class NetworkMutltiClientProtocol : MultiClientDeviceProtocol
    {
        #region 属性
        /// <summary>
        /// 网络连接
        /// </summary>
        protected Dictionary<string, NetworkTcpClient> m_TcpClientDictionary = new Dictionary<string, NetworkTcpClient>();
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public NetworkMutltiClientProtocol() : base()
        {
            _packageHandle = new NetworkPackageHeadHandle();
        }

        #region 对外接口
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="parameters"></param>
        public override void AddParameters(List<ProtocolParameter> parameters)
        {
            foreach (var item in parameters)
            {
                NetworkParameter netParam = (item as NetworkParameter);
                if (null == netParam)
                {
                    continue;
                }

                IPAddress ip;
                if (!IPAddress.TryParse(netParam.RemoteAddress, out ip))
                {
                    continue;
                }

                if (!m_TcpClientDictionary.ContainsKey(netParam.ID))
                {
                    NetworkTcpClient clientInfo = new NetworkTcpClient(netParam, null);
                    clientInfo.DoAnalysis += AnalysisReceiveData;
                    clientInfo.OnReceive += OnReceived;
                    clientInfo.ReceiveError += OnErrorReceived;
                    clientInfo.SendData += SendPackage;
                    m_TcpClientDictionary[netParam.ID] = clientInfo;
                }
            }
        }

        /// <summary>
        /// 删除参数
        /// </summary>
        /// <param name="parameters"></param>
        public override void RemoveParameters(List<ProtocolParameter> parameters)
        {
            foreach (var item in parameters)
            {
                if (m_TcpClientDictionary.ContainsKey(item.ID))
                {
                    m_TcpClientDictionary[item.ID].CloseClient();

                    m_TcpClientDictionary.Remove(item.ID);
                }
            }
        }

        /// <summary>
        /// 清空参数
        /// </summary>
        public override void ClearParameters()
        {
            //_tcpClientDictionary.Clear();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public override Dictionary<string, bool> Connect(string[] id = null)
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (var item in m_TcpClientDictionary)
            {
                result[item.Key] = false;
            }

            try
            {
                if (id == null)
                {
                    foreach (var tcpClient in m_TcpClientDictionary)
                    {
                        if (null != tcpClient.Value)
                        {
                            if (tcpClient.Value.ConnectService())
                            {
                                result[tcpClient.Key] = true;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var keyId in id)
                    {
                        if (m_TcpClientDictionary.ContainsKey(keyId))
                        {
                            if (m_TcpClientDictionary[keyId].ConnectService())
                            {
                                result[keyId] = true;
                            }
                        }
                    }
                }

                if (m_TcpClientDictionary.Values.Where(o => o.m_Connected).Count() == 0)
                {
                    Connected = false;
                    return result;
                }
                else
                {
                    Connected = true;
                    StartThreads();
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.Error("NetworkClient Connect error", ex);
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
                foreach (var tcpClient in m_TcpClientDictionary)
                {
                    tcpClient.Value.CloseClient();
                }
            }
            else
            {
                foreach (var item in id)
                {
                    if (m_TcpClientDictionary.ContainsKey(item) &&
                        null != m_TcpClientDictionary[item])
                    {
                        m_TcpClientDictionary[item].CloseClient();
                    }
                }
            }

            if (m_TcpClientDictionary.Values.Where(o => o.m_Connected).Count() == 0)
            {
                LogHelper.logCommunication.Info("------全部网络通信关闭-------");
                Connected = false;
            }
        }

        /// <summary>
        /// 是否连接
        /// </summary>
        /// <returns></returns>
        public override bool GetConnected(string id)
        {
            if (m_TcpClientDictionary.ContainsKey(id))
            {
                return m_TcpClientDictionary[id].m_Connected;
            }

            return false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public override bool SendPackage(DataPackage package)
        {
            try
            {
                if (null == package || null == package.PackageInfo)
                {
                    return false;
                }

                if (package.Data != null)
                {
                    const int nSplitUnit = 1024;
                    byte[] orinalData = package.Data;
                    int loop = (int)Math.Ceiling((double)package.Data.Length / (double)nSplitUnit);

                    for (int i = 0; i < loop; i++)
                    {
                        package.PackageInfo.PackNums = loop;
                        package.PackageInfo.PackNo = i;

                        byte[] dataTmp = null;
                        if (i != loop - 1)
                        {
                            dataTmp = new byte[nSplitUnit];
                            Array.Copy(orinalData, i * nSplitUnit, dataTmp, 0, nSplitUnit);
                        }
                        else
                        {
                            int nLen = orinalData.Length - i * nSplitUnit;
                            dataTmp = new byte[nLen];
                            Array.Copy(orinalData, i * nSplitUnit, dataTmp, 0, nLen);
                        }

                        package.Data = dataTmp;

                        byte[] data = null;
                        if (!_packageHandle.SetPackage(package, ref data))
                        {
                            return false;
                        }

                        if (!PostSend(package.PackageInfo.ID, data, 0, data.Length))
                        {
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    package.PackageInfo.PackNums = 1;
                    package.PackageInfo.PackNo = 0;

                    byte[] data = null;
                    if (!_packageHandle.SetPackage(package, ref data))
                    {
                        return false;
                    }

                    return PostSend(package.PackageInfo.ID, data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                LogHelper.logCommunication.InfoFormat("发送数据异常{0}", ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="package">数据包</param>
        /// <returns>发送是否成功</returns>
        public override bool SendPackage1(string id, ushort command, byte[] sendData, byte[] key)
        {
            if (!PostSend(id, sendData, 0, sendData.Length))
                return false;
            return true;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 开始线程
        /// </summary>
        protected void StartThreads()
        {
            if (_sendEventThread == null && _receiveEventThread == null)
            {
                _sendEventThread = new EventThread(SendThreadHandler);
                LogHelper.logCommunication.Debug("发送线程创建并启动");

                _receiveEventThread = new EventThread(ReceiveThreadHandler, ThreadPriority.Highest);
                LogHelper.logCommunication.Debug("接收线程创建并启动");
            }
        }

        protected override void DoDataReceived(ProtocolParameter param)
        {
            if (null == param || !m_TcpClientDictionary.ContainsKey(param.ID))
            {
                return;
            }

            DataPackage package = m_TcpClientDictionary[param.ID].GetDataPackage();

            if (null != package)
            {
                package.PackageInfo.ID = param.ID;
                OnDataReceived(package);
            }
        }

        /// <summary>
        /// 通讯类别
        /// </summary>
        /// <returns></returns>
        protected override CommunicateType GetCommunicateType()
        {
            return CommunicateType.Network;
        }

        protected override bool PostSend(string id, byte[] buffer, int offset, int count)
        {
            if (!m_TcpClientDictionary.ContainsKey(id) || null == _sendEventThread)
            {
                return false;
            }

            byte[] destBuffer = new byte[count];
            Array.Copy(buffer, offset, destBuffer, 0, count);
            List<byte> dataList = destBuffer.ToList();

            m_TcpClientDictionary[id].m_SendQueue.Enqueue(dataList);
            _sendEventThread.Enqueue(id, EventThread.ObjLevel.High);

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
                //NetworkParameter netParam = obj as NetworkParameter;
                DataPackage dataPackage = obj as DataPackage;
                //DoDataReceived(netParam);

                if (null != dataPackage)
                {
                    //package.PackageInfo.ID = param.ID;
                    OnDataReceived(dataPackage);
                }
            }
            catch (Exception e)
            {
            }
        }

        protected void doSendAction(object obj)
        {
            try
            {
                string clientId = (string)obj;
                if (!m_TcpClientDictionary.ContainsKey(clientId))
                {
                    return;
                }

                List<byte> dataList = new List<byte>();
                if (m_TcpClientDictionary[clientId].m_SendQueue.TryDequeue(out dataList))
                {
                    byte[] data = dataList.ToArray();
                    if (!m_TcpClientDictionary[clientId].WriteBuffer(data))
                    {
                        LogHelper.logCommunication.Debug("发送数据失败");
                    }
                    else
                    {
                        WriteLog(data, data.Length, communacationWay.SEND, GetCommunicateType(), m_TcpClientDictionary[clientId].IP.ToString());
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

        protected void OnReceived(object sender)
        {
            _receiveEventThread.Enqueue(sender, EventThread.ObjLevel.High);
        }

        /// <summary>
        /// 解析接收的数据
        /// </summary>
        /// <param name="recieveList"></param>
        /// <returns></returns>
        private DataPackage AnalysisReceiveData(ref List<byte> recieveList, string ipAddress)
        {
            if (null == _packageHandle)
            {
                return null;
            }

            DataPackage pkg = null;

            if (_packageHandle.GetPackage(recieveList, ref pkg, GetCommunicateType(), ipAddress))
            {
                return pkg;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnErrorReceived(object sender)
        {
            CommunicateError errorType = (CommunicateError)sender;
            OnCommunicateError(errorType, null);
        }

        protected override byte[] GetStandardKey(string id)
        {
            throw new NotImplementedException();
        }

        protected override bool IsUsingNewKey(string id)
        {
            throw new NotImplementedException();
        }

        protected override byte[] GetOrinalKey(string id)
        {
            throw new NotImplementedException();
        }

        protected override byte[] GetRealKey(string id)
        {
            throw new NotImplementedException();
        }

        protected override ushort GetShakeHandCommand(string id)
        {
            throw new NotImplementedException();
        }

        protected override ushort GetCommunicationEnsureCommand(string id)
        {
            throw new NotImplementedException();
        }

        protected override bool SetInfoForCom(string id, byte[] inputInfo)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
