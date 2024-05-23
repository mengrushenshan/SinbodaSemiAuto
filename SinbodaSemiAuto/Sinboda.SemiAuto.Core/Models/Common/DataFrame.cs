using Newtonsoft.Json;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models.Common
{
    public abstract class DataFrameBase : IDataFrame
    {
        /// <summary>
        /// 发送数据
        /// </summary>
        [JsonIgnore]
        public string InFrame { get; set; }

        /// <summary>
        /// 请求
        /// </summary>
        [JsonIgnore]
        public IRequest Request { get; set; }

        /// <summary>
        /// 回应
        /// </summary>
        [JsonIgnore]
        public IResponse Response { get; set; }

        /// <summary>
        /// 指令发送时间
        /// </summary>
        [JsonIgnore]
        protected DateTime DtSend;

        /// <summary>
        /// 解析数据
        /// </summary>
        [JsonIgnore]
        public string OutFrame { get; set; }

        public abstract int FrameID();

        public DateTime GetSendTime()
        {
            return DtSend;
        }

        public abstract byte[] Packing();

        public void SetSendTime(DateTime time)
        {
            DtSend = time;
        }

        public abstract void UnPacking(string data);

        public CmdType GetCmd()
        {
            if (!Request.IsNull())
                return Request.GetCmd();
            else if (!Response.IsNull())
                return Response.GetCmd();
            return CmdType.None;
        }

        public ErrType GetError()
        {
            if (Response.IsNull())
                return ErrType.None;
            return Response.GetError();
        }

        public void SetError(ErrType errType)
        {
            Response.SetError(errType);
        }

        public string GetUPData()
        {
            return OutFrame;
        }

        public ActionType GetAct()
        {
            if (!Request.IsNull())
                return Request.GetAct();
            else if (!Response.IsNull())
                return Response.GetAct();
            return ActionType.None;
        }

        public IResponse GetResponse()
        {
            return Response;
        }
    }

    public class DataFrame : DataFrameBase
    {
        public override int FrameID()
        {
            if (!Request.IsNull())
                return Request.ToFrameID();
            else if (!Response.IsNull())
                return Response.ToFrameID();
            else
                throw new Exception();
        }

        public override byte[] Packing()
        {
            if (!Request.IsNull())
                InFrame = Request.ToRequest();
            else
                LogHelper.logSoftWare.Error("Request is null !");
            byte[] bytes = Encoding.UTF8.GetBytes(InFrame);
            return bytes;
        }

        public override void UnPacking(string data)
        {
            OutFrame = data;
            if (!Response.IsNull())
                Response = Response.UnPacking(OutFrame);
            else
                LogHelper.logSoftWare.Error("Response is null !");
        }
    }
}
