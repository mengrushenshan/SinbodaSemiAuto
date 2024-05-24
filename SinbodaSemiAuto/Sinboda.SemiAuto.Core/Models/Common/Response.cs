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

        public ActionType GetAct()
        {
            return Act;
        }

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

    /// <summary>
    /// x,y电机原点
    /// </summary>
    public class ResMotorOrigin : Response
    {
        public ResMotorOrigin() : base()
        {
            Act = ActionType.MotorOrigin;
        }

        public ResMotorOrigin(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 0-1: 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 0:离开 1:进入
        /// </summary>
        [JsonProperty("InOrg")]
        public int InOrg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResMotorOrigin>();
        }
    }

    /// <summary>
    /// 风扇状态
    /// </summary>
    public class ResFanStatus : Response
    {
        public ResFanStatus() : base()
        {
            Act = ActionType.FanStatus;
        }

        public ResFanStatus(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 0-1: 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 0:正常 1:堵转
        /// </summary>
        [JsonProperty("Status")]
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResFanStatus>();
        }
    }

    /// <summary>
    /// 仓门状态
    /// </summary>
    public class ResDoorStatus : Response
    {
        public ResDoorStatus() : base()
        {
            Act = ActionType.DoorStatus;
        }

        public ResDoorStatus(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 0-1: 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int ID { get; set; }

        /// <summary>
        /// 0:打开 1:关闭
        /// </summary>
        [JsonProperty("DoorClosed")]
        public int DoorClosed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResDoorStatus>();
        }
    }

    /// <summary>
    /// 风扇状态
    /// </summary>
    public class ResMotorStatus : Response
    {
        public ResMotorStatus() : base()
        {
            Act = ActionType.GetMotorStatus;
        }

        public ResMotorStatus(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        ///0:离开 1:进入
        /// </summary>
        [JsonProperty("InOrg")]
        public int InOrg { get; set; }

        /// <summary>
        ///0:停止 1:运动
        /// </summary>
        [JsonProperty("Running")]
        public int Running { get; set; }

        /// <summary>
        ///当前位置(编码器)
        /// </summary>
        [JsonProperty("CurrPos")]
        public int CurrPos { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResMotorStatus>();
        }
    }
}
