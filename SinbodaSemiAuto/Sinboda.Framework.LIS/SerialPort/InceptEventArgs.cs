using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    /// <summary>
    /// 接收数据事件
    /// </summary>
    public class InceptEventArgs : EventArgs
    {
        private readonly ASTMMessage receiveASTMMessage;
        private readonly AnswerType receiveAnswer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RecvASTMMessage"></param>
        /// <param name="answer"></param>
        public InceptEventArgs(ASTMMessage RecvASTMMessage, AnswerType answer)
        {
            receiveASTMMessage = RecvASTMMessage;
            receiveAnswer = answer;
        }
        /// <summary>
        ///  接收到的消息
        /// </summary>
        public ASTMMessage ReceiveASTMMessage
        {
            get { return receiveASTMMessage; }
        }
        /// <summary>
        ///  接收到的应答指令
        /// </summary>
        public AnswerType ReceiveAnswer
        {
            get { return receiveAnswer; }
        }

    }
}
