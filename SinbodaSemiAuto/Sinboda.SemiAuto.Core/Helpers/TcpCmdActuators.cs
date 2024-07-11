using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Interfaces;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Sinboda.SemiAuto.Core.Helpers
{
    /// <summary>
    /// 通讯指令执行器基类
    /// </summary>
    public abstract class ActuatorsBase : ISubSystem
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        protected readonly object _lockObj = new object();

        /// <summary>
        /// 连接器
        /// </summary>
        protected ICommDriver commDriver;

        /// <summary>
        /// 等待发送帧
        /// </summary>
        protected Dictionary<int, IDataFrame> FramesForSend = new Dictionary<int, IDataFrame>();

        /// <summary>
        /// 已确认帧
        /// </summary>
        protected Dictionary<int, IDataFrame> FramesConfirmd = new Dictionary<int, IDataFrame>();

        /// <summary>
        /// 发送定时器
        /// </summary>
        protected System.Timers.Timer _timerSend;

        /// <summary>
        /// 接收定时器
        /// </summary>
        protected Task _timerRev;

        /// <summary>
        /// 接收器控制
        /// </summary>
        bool revEnabled = false;

        //检查超时指令
        const int TimeToCheckTimeOut = 1000;

        /// <summary>
        /// 当前执行次数
        /// </summary>
        int TimeElapsed = 0;

        public bool IsConnected()
        {
            if (commDriver == null)
            {
                return false;
            }

            return commDriver.IsConnected();
        }

        public void Init(ICommDriver driver)
        {
            lock (_lockObj)
            {
                commDriver = driver;
            }
        }

        public bool Connect()
        {
            lock (_lockObj)
            {
                return commDriver.Connect();
            }
        }

        public void Dispose()
        {
            lock (_lockObj)
            {
                _timerSend.Dispose();
                _timerRev.Dispose();
                commDriver?.Dispose();
            }
        }

        public abstract void ReStart(int time);

        public bool SendAsync(IDataFrame frame)
        {
            lock (_lockObj)
            {
                if (frame.IsNull())
                    return false;
                FramesForSend[frame.FrameID()] = frame;
                return true;
            }
        }

        public void StartSequence()
        {
            lock (_lockObj)
            {
                // 创建并初始化定时器
                _timerSend = new System.Timers.Timer(100); // 设置间隔时间
                _timerSend.Elapsed += Timer_Elapsed_Send;
                _timerSend.AutoReset = true; // 设置为自动重置
                _timerSend.Start();

                // 创建并初始化定时器
                revEnabled = true;
                _timerRev = new Task(Timer_Elapsed_Rev); // 设置间隔时间
                _timerRev.Start();
            }
        }

        private void Timer_Elapsed_Rev()
        {
            while (revEnabled)
            {
                TimeElapsed++;
                if (TimeElapsed >= Int16.MaxValue)
                    TimeElapsed = 0;

                //接收数据帧
                IDataFrame frameRcv = Receive();
                if (!frameRcv.IsNull())
                {
                    var kpFrame = FramesConfirmd.FirstOrDefault(x => x.Value.FrameID() == frameRcv.FrameID());
                    if ((!kpFrame.IsNull()) && !kpFrame.Value.IsNull())
                    {
                        //确认帧
                        if (frameRcv.GetCmd() == CmdType.Confirm)
                        {
                            //TODO：确认帧返回错误 无需等待结果帧 具体处理暂缺
                            if (frameRcv.GetError() != ErrType.EC_NoError)
                            {
                                kpFrame.Value.UnPacking(frameRcv.GetUPData());
                                FramesConfirmd.Remove(frameRcv.FrameID());
                                LogHelper.logCommunication.Error($"Confirm Frame Error:[{frameRcv.GetUPData()}]");
                            }
                            else
                            {
                                
                            }
                        }
                        //结果帧
                        else if (frameRcv.GetCmd() == CmdType.Result)
                        {
                            //解包数据
                            kpFrame.Value.UnPacking(frameRcv.GetUPData());
                            FramesConfirmd.Remove(frameRcv.FrameID());
                            //TODO:结果帧返回错误  具体处理暂缺
                            if (frameRcv.GetError() != ErrType.EC_NoError)
                            {
                                LogHelper.logCommunication.Error($"Result Frame Error:[{frameRcv.GetUPData()}]");
                            }
                            else
                            {
                            }
                        }
                    }
                    else
                    {
                        //消息帧 下位机主推
                        if (frameRcv.GetCmd() == CmdType.Message)
                        {
                            LogHelper.logCommunication.Info($"message frame receive, FrameID:[{frameRcv.FrameID()}] cmd:[{frameRcv.GetCmd()}]");
                            //消息帧推送
                            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                Messenger.Default.Send<IResponse>(frameRcv.CreateResponse(), GlobalData.TokenTcpMsg);
                            }));
                        }
                    }
                }

                //处理超时帧
                if (TimeElapsed % TimeToCheckTimeOut == 0)
                {
                    var frames = FramesConfirmd.Where(x => x.Value.GetSendTime().AddSeconds(GlobalData.TimeOut) <= DateTime.Now).ToList();
                    foreach (var item in frames)
                    {
                        item.Value.SetError(ErrType.TimeOut);
                        FramesConfirmd.Remove(item.Key);
                        LogHelper.logCommunication.Error($"Frame Tiemout,FrameID:[{item.Value.FrameID()}] cmd:[{item.Value.GetCmd()}] error:[{item.Value.GetError()}]");
                    }
                }

            }
        }

        /// <summary>
        /// 发送定时器回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void Timer_Elapsed_Send(object sender, ElapsedEventArgs e)
        {
            lock (_lockObj)
            {
                //发送数据帧
                var frame = FramesForSend.FirstOrDefault();
                if ((!frame.IsNull()) && (!frame.Value.IsNull()))
                {
                    IDataFrame dataFrame = frame.Value;
                    if (!dataFrame.IsNull())
                    {
                        if (!Send(dataFrame))
                        {
                            PauseSequence();
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将数据帧加入列表
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public virtual bool Send(IDataFrame frame)
        {
            if (FramesForSend.ContainsKey(frame.FrameID()))
                FramesForSend.Remove(frame.FrameID());
            FramesConfirmd[frame.FrameID()] = frame;
            return true;
        }

        public abstract IDataFrame Receive();

        public void PauseSequence()
        {
            lock (_lockObj)
            {
                _timerSend.Stop();
                _timerRev.Dispose();
                revEnabled = false;
            }
        }
    }

    /// <summary>
    /// TCP通讯指令执行器
    /// </summary>
    public class TcpCmdActuators : ActuatorsBase
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            ICommDriver driver = new TcpDriver(GlobalData.IpAddress, GlobalData.Port);
            base.Init(driver);
        }

        /// <summary>
        /// 当前已经读取到的帧数据
        /// </summary>
        private List<char> sbRev = new List<char>();

        /// <summary>
        /// 获取完整帧
        /// </summary>
        /// <returns></returns>
        private string GetDataFrameString()
        {
            //包尾位置
            int last = -1;
            //包头位置
            int First = -1;
            //包完整标记
            int numParenthesesLeft = 0;
            for (int i = 0; i < sbRev.Count; i++)
            {
                char c = sbRev[i];
                //包头
                if (c == '{')
                {
                    numParenthesesLeft++;
                    First = i;
                }
                //包尾
                else if (c == '}')
                    numParenthesesLeft--;
                //获取到完整包
                if (First >= 0 && numParenthesesLeft == 0)
                {
                    last = i;
                    break;
                }
            }
            if (First < 0 || last <= First)
                return string.Empty;
            List<char> chars = sbRev.GetRange(First, last - First + 1);
            //去除解析过的数据 以及不完整的碎片帧
            sbRev.RemoveRange(0, last + 1);
            return new string(chars.ToArray());
        }

        public override bool Send(IDataFrame frame)
        {
            lock (_lockObj)
            {
                //连接状态错误
                if (!commDriver.IsConnected())
                {
                    PauseSequence();
                    return false;
                }
                //发送
                bool res = commDriver.Write(frame.Packing());
                LogHelper.logCommunication.Info($"Send Frame :[{frame.GetInData()}]");
                if (res)
                {
                    frame.SetSendTime(DateTime.Now);
                    base.Send(frame);
                }
                else
                {
                    PauseSequence();
                    LogHelper.logCommunication.Error($"Send Frame error :[{frame.GetInData()}]!");
                }
                return res;
            }
        }

        public override IDataFrame Receive()
        {
            lock (_lockObj)
            {
                //连接状态错误
                if (!commDriver.IsConnected())
                {
                    PauseSequence();
                    return null;
                }
                //将接收到的数据添加到列表
                byte[] data = commDriver.Read();
                if ((!data.IsNull()) && data.Length > 0)
                {
                    sbRev.AddRange(Encoding.UTF8.GetString(data, 0, data.Length));
                }
                string strDataFrame = GetDataFrameString();
                //数据为空 返回空帧
                if (strDataFrame.IsNullOrWhiteSpace())
                    return null;
                LogHelper.logCommunication.Info($"Receive Frame :[{strDataFrame}]");

                //使用基类去解析
                IDataFrame dataFrame = new DataFrame()
                {
                    Response = new Response()
                };
                dataFrame.UnPacking(strDataFrame);
                return dataFrame;
            }
        }

        public override void ReStart(int time)
        {
            lock (_lockObj)
            {
                this.PauseSequence();
                this.Dispose();
                //一秒后再重连
                Thread.Sleep(time);
                this.Init();
                this.Connect();
                this.StartSequence();
            }
        }

        /// <summary>
        /// 单例
        /// </summary>
        public static TcpCmdActuators Instance
        {
            get
            {
                if (_instance.IsNull())
                    _instance = new TcpCmdActuators();
                return _instance;
            }
        }
        private static TcpCmdActuators _instance;
    }
}
