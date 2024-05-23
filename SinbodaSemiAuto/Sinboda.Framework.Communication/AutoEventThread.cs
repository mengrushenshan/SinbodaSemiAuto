using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.Framework.Communication
{
    public class AutoEventThread
    {
        /// <summary>
        /// 当前执行所在的模块号
        /// </summary>
        public string CurrentID { get; private set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int ExpirySeconds { get; private set; }
        /// <summary>
        /// 使用的线程
        /// </summary>
        public Thread MyThread { get; private set; }
        /// <summary>
        /// 信号量
        /// </summary>
        public AutoResetEvent MyResetEvent = new AutoResetEvent(false);
        /// <summary>
        /// 各产品线使用的原始密钥信息
        /// </summary>
        public byte[] OrignalKeyInfo { get; set; } = new byte[8] { 0x01, 0x02, 0x03, 0x04, 0x04, 0x03, 0x02, 0x01 };
        /// <summary>
        /// 各产品线使用的通讯密钥信息
        /// </summary>
        public byte[] NewKeyInfo { get; private set; }

        /// <summary>
        /// 握手命令 
        /// </summary>
        public ushort ShankeHandCommnad { get; set; }

        /// <summary>
        /// 通讯确认命令
        /// </summary>
        public ushort CommunicationEnsureCommand { get; set; }
        /// <summary>
        /// 执行的整体流程是否成功
        /// </summary>
        public bool ExcuteIsSucceed { get; private set; } = false;
        /// <summary>
        /// 执行发送方法
        /// </summary>
        public Func<string, ushort, byte[], byte[], bool> ExcuteSendMessage;
        /// <summary>
        /// 下位机回复信息
        /// </summary>
        public byte[] AnalyzerReturnInfo { get; set; }
        /// <summary>
        /// 随机数
        /// </summary>
        private byte[] randomInfo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="function"></param>
        /// <param name="expiredSeconds"></param>
        /// <param name="id"></param>
        public AutoEventThread(Func<string, ushort, byte[], byte[], bool> function, int expiredSeconds, string id,
            byte[] shakeHandKey, byte[] communicationEnsureKey, ushort shankeHandCommand, ushort communicationEnsureCommand)
        {
            ExcuteSendMessage = function;
            ExpirySeconds = expiredSeconds;
            CurrentID = id;
            OrignalKeyInfo = shakeHandKey;
            NewKeyInfo = communicationEnsureKey;
            ShankeHandCommnad = shankeHandCommand;
            CommunicationEnsureCommand = communicationEnsureCommand;
            MyThread = new Thread(new ThreadStart(CommunicationHandler));
            MyThread.IsBackground = true;
        }
        /// <summary>
        /// 连接握手流程处理方法
        /// </summary>
        public void CommunicationHandler()
        {
            //生成随机码
            randomInfo = CreateRandomInfo();
            //创建发送数据然后加密生成数据，并调用发送接口
            byte[] sendInfoOfPreKey = CreateRandomAndKeyInfo(randomInfo);
            //调用发送方法
            if (ExcuteSendMessage(CurrentID, 0X07, sendInfoOfPreKey, OrignalKeyInfo))
            {
                //开启信号量等待，超时返回false
                if (MyResetEvent.WaitOne(new TimeSpan(0, 0, ExpirySeconds), true))
                {
                    MyResetEvent.Reset();
                    //判断随机码是否为上位机发送使用随机码，是则存储密钥，利用新密钥加密数据，创建发送包，并调用发送接口；否则返回false
                    if (JudgeRandomKeyInfo(AnalyzerReturnInfo))
                    {
                        byte[] sendInfoOfAllKey = CreateCommunicationInfo();
                        //调用发送方法
                        if (ExcuteSendMessage(CurrentID, 0X08, sendInfoOfPreKey, NewKeyInfo))
                        {
                            //开启信号量等待，超时返回false
                            if (MyResetEvent.WaitOne(new TimeSpan(0, 0, ExpirySeconds), true))
                            {
                                //判断是否为成功命令，若是则返回true，否则返回false
                                if (DecryptInfoByNewKeyAndJudge(AnalyzerReturnInfo))
                                    ExcuteIsSucceed = true;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 创建随机数
        /// </summary>
        /// <returns></returns>
        private byte[] CreateRandomInfo()
        {
            Random random = new Random();
            byte[] bytes = new byte[4];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)random.Next(0, 255);
            randomInfo = bytes;
            return bytes;
        }

        /// <summary>
        /// 创建随机数与密钥信息，并加密
        /// </summary>
        /// <param name="inputInfo">上位机要发送的数据</param>
        /// <returns></returns>
        private byte[] CreateRandomAndKeyInfo(byte[] inputInfo)
        {
            List<byte> info = new List<byte>();
            NewKeyInfo = OrignalKeyInfo;
            info.AddRange(inputInfo);
            info.Add(NewKeyInfo[0]);
            info.Add(NewKeyInfo[1]);
            info.Add(NewKeyInfo[2]);
            info.Add(NewKeyInfo[3]);
            return info.ToArray();
        }
        /// <summary>
        /// 解密返回信息并判断随机数
        /// </summary>
        /// <param name="inputInfo">下位机返回数据</param>
        /// <returns></returns>
        private bool JudgeRandomKeyInfo(byte[] inputInfo)
        {
            byte[] randominfo = new byte[4];
            Array.Copy(inputInfo, 0, randominfo, 0, 4);
            if (randominfo == randomInfo)
            {
                byte[] keyInfoOfEnd = new byte[4];
                Array.Copy(inputInfo, 4, keyInfoOfEnd, 0, 4);
                SaveKeyInfo(keyInfoOfEnd);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 存储后四位密钥
        /// </summary>
        /// <param name="inputInfo">数据中的密钥信息</param>
        private void SaveKeyInfo(byte[] inputInfo)
        {
            for (int i = 0; i < inputInfo.Length; i++)
            {
                NewKeyInfo[i + 4] = inputInfo[i];
            }
        }
        /// <summary>
        /// 使用新密钥加密数据
        /// </summary>
        /// <returns></returns>
        private byte[] CreateCommunicationInfo()
        {
            byte[] newData = new byte[4] { 1, 2, 3, 4 };
            return newData;
        }
        /// <summary>
        /// 使用新密钥解密数据然后判断是否成功
        /// </summary>
        /// <param name="inputInfo">下位机返回的数据</param>
        /// <returns></returns>
        private bool DecryptInfoByNewKeyAndJudge(byte[] inputInfo)
        {
            byte[] orignalData = new byte[4] { 1, 2, 3, 4 };
            if (inputInfo == orignalData)
                return true;
            else return false;
        }
    }
}
