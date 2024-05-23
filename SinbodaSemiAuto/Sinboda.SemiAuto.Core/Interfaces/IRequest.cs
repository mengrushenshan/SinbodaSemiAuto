using Sinboda.SemiAuto.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Interfaces
{
    /// <summary>
    /// 请求
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// 获取帧编号
        /// </summary>
        /// <returns></returns>
        int ToFrameID();

        /// <summary>
        /// 获取动作类型
        /// </summary>
        /// <returns></returns>
        ActionType GetAct();

        /// <summary>
        /// 获取请求类型
        /// </summary>
        /// <returns></returns>
        CmdType GetCmd();

        /// <summary>
        /// 生成请求字符串
        /// </summary>
        /// <returns></returns>
        string ToRequest();
    }

    /// <summary>
    /// 回应
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// 生成应答数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IResponse UnPacking(string data);

        /// <summary>
        /// 指令返回结果
        /// </summary>
        /// <returns></returns>
        ErrType GetError();

        /// <summary>
        /// 设置指令返回结果
        /// </summary>
        /// <param name="errType"></param>
        void SetError(ErrType errType);

        /// <summary>
        /// 获取帧id
        /// </summary>
        /// <returns></returns>
        int ToFrameID();

        /// <summary>
        /// 获取请求类型
        /// </summary>
        /// <returns></returns>
        CmdType GetCmd();
    }
}
