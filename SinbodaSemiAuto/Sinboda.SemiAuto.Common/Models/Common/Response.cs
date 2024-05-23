using Newtonsoft.Json;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Models.Common
{
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ErrType : Int32
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown = -1,

        /// <summary>
        /// 无错误
        /// </summary>
        None = 0,

        /// <summary>
        /// 超时
        /// </summary>
        TimeOut = 110,
    }

    /// <summary>
    /// 应答基类
    /// </summary>
    public class Response : IResponse
    {
        public Response()
        {
            Cmd = CmdType.None;
            Act = ActionType.None;
            ErrorCode = ErrType.UnKnown;
        }

        public Response(IRequest req)
        {
            ErrorCode = ErrType.UnKnown;
            Cmd = req.GetCmd();
            Act = req.GetAct();
            FrameID = req.ToFrameID();
        }

        /// <summary>
        /// 指令编号
        /// </summary>
        [JsonProperty("FrameID")]
        public int FrameID { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [JsonProperty("CmdType")]
        public CmdType Cmd { get; set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [JsonProperty("ActionID")]
        public ActionType Act { get; set; }

        /// <summary>
        /// 错误类型
        /// </summary>
        [JsonProperty("ErrorCode")]
        public ErrType ErrorCode { get; set; }

        public CmdType GetCmd()
        {
            return Cmd;
        }

        public ErrType GetError()
        {
            return ErrorCode;
        }

        public void SetError(ErrType errType)
        {
            ErrorCode = errType;
        }

        public int ToFrameID()
        {
            return FrameID;
        }

        public virtual IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<Response>();
        }
    }

    /// <summary>
    /// x,y移动应答
    /// </summary>
    public class ResMove : Response
    {
        public ResMove() : base()
        {
            Act = ActionType.MoveCont;
        }

        public ResMove(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 当前坐标
        /// </summary>
        [JsonProperty("CurrPos")]
        public int CurPos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResMove>();
        }
    }
}
