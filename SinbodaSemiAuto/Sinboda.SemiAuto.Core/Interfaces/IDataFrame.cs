using Sinboda.SemiAuto.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Interfaces
{
    public interface IDataFrame
    {
        /// <summary>
        /// 获取帧编号
        /// </summary>
        /// <returns></returns>
        int FrameID();

        /// <summary>
        /// 组包
        /// </summary>
        /// <returns></returns>
        byte[] Packing();

        /// <summary>
        /// 解包
        /// </summary>
        /// <returns></returns>
        void UnPacking(string data);

        /// <summary>
        /// 获取解包数据
        /// </summary>
        /// <returns></returns>
        string GetUPData(); 
        
        /// <summary>
        /// 获取发送数据
        /// </summary>
        /// <returns></returns>
        string GetInData(); 
        
        /// <summary>
        /// 获取发送时间
        /// </summary>
        /// <returns></returns>
        DateTime GetSendTime();

        /// <summary>
        /// 设置发送时间
        /// </summary>
        /// <returns></returns>
        void SetSendTime(DateTime time);

        /// <summary>
        /// 获取请求类型
        /// </summary>
        /// <returns></returns>
        CmdType GetCmd();

        /// <summary>
        /// 获取动作类型
        /// </summary>
        /// <returns></returns>
        ActionType GetAct();

        /// <summary>
        /// 指令返回结果
        /// </summary>
        /// <returns></returns>
        ErrType GetError();
        
        /// <summary>
        /// 获取应答
        /// </summary>
        /// <returns></returns>
        IResponse GetResponse();

        /// <summary>
        /// 设置指令返回结果
        /// </summary>
        /// <param name="errType"></param>
        void SetError(ErrType errType);
    }
}
