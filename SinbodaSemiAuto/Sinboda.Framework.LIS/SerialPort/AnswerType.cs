using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 应答枚举类型
    /// </summary>
    public enum AnswerType
    {
        /// <summary>
        /// 询问命令
        /// </summary>
        ENQ = 0,
        /// <summary>
        /// 发送结束命令
        /// </summary>
        SendEOT = 1,
        /// <summary>
        /// 接收结束命令
        /// </summary>
        ReceEOT = 2,
        /// <summary>
        /// 接受命令
        /// </summary>
        ACK = 3,
        /// <summary>
        /// 拒绝命令
        /// </summary>
        NAK = 4,
        /// <summary>
        /// 超时 ,lis无回复
        /// </summary>
        TimeOut = 5,
    }
    /// <summary>
    /// ASTM控制符
    /// </summary>
    public class ASTMCommand
    {
        private static byte _startBlockChar = 0x02;
        /// <summary>
        /// STX:起始帧头（0x02）
        /// </summary>
        public static string StartBlockChar
        {
            get { return "0x" + _startBlockChar.ToString("X2"); }
            set { _startBlockChar = System.Convert.ToByte(value, 16); }
        }

        private static byte _endBlockChar = 0x0d;
        /// <summary>
        /// CR:帧数据结束符 (0x0D)
        /// </summary>
        public static string EndBlockChar
        {
            get { return "0x" + _endBlockChar.ToString("X2"); }
            set { _endBlockChar = System.Convert.ToByte(value, 16); }
        }

        private static byte _etxBlockChar = 0x03;
        /// <summary>
        /// ETX:帧结束符 (0x03)
        /// </summary>
        public static string EtxBlockChar
        {
            get { return "0x" + _etxBlockChar.ToString("X2"); }
            set { _etxBlockChar = System.Convert.ToByte(value, 16); }
        }

        private static byte _etbBlockChar = 0x17;
        /// <summary>
        /// ETB:分帧发送的帧结束符(0x17)
        /// </summary>
        public static string EtbBlockChar
        {
            get { return "0x" + _etbBlockChar.ToString("X2"); }
            set { _etbBlockChar = System.Convert.ToByte(value, 16); }
        }

        private static byte _lfBlockChar = 0x0a;
        /// <summary>
        /// LF:换行
        /// </summary>
        public static string LFBlockChar
        {
            get { return "0x" + _lfBlockChar.ToString("X2"); }
            set { _lfBlockChar = System.Convert.ToByte(value, 16); }
        }
        private static byte _enqBlockChar = 0x05;
        /// <summary>
        /// ENQ:开始
        /// </summary>
        public static string ENQBlockChar
        {
            get { return "0x" + _enqBlockChar.ToString("X2"); }
            set { _enqBlockChar = System.Convert.ToByte(value, 16); }
        }
        private static byte _eotBlockChar = 0x04;
        /// <summary>
        /// EOT:结束
        /// </summary>
        public static string EOTBlockChar
        {
            get { return "0x" + _eotBlockChar.ToString("X2"); }
            set { _eotBlockChar = System.Convert.ToByte(value, 16); }
        }
        private static byte _ackBlockChar = 0x06;
        /// <summary>
        /// ACK:接受
        /// </summary>
        public static string ACKBlockChar
        {
            get { return "0x" + _ackBlockChar.ToString("X2"); }
            set { _ackBlockChar = System.Convert.ToByte(value, 16); }
        }
        private static byte _nakBlockChar = 0x15;
        /// <summary>
        /// NAK:拒绝
        /// </summary>
        public static string NAKBlockChar
        {
            get { return "0x" + _nakBlockChar.ToString("X2"); }
            set { _nakBlockChar = System.Convert.ToByte(value, 16); }
        }
    }

    /// <summary>
    /// lis回复
    /// </summary>
    public class AnswerResult
    {
        /// <summary>
        /// 回复类型：0样本 1结果 2质控
        /// </summary>
        //public int type { get; set; }

        /// <summary>
        /// LIS交互指令
        /// </summary>
        public AnswerType answer { get; set; }

        private readonly ASTMMessage receiveASTMMessage;
        /// <summary>
        /// 接收的ASTMMessage
        /// </summary>
        public ASTMMessage ReceiveASTMMessage
        {
            get { return receiveASTMMessage; }

        }
        public AnswerResult(ASTMMessage recvASTMMessage)
        {
            receiveASTMMessage = recvASTMMessage;
        }
        public AnswerResult()
        {

        }
    }
}
