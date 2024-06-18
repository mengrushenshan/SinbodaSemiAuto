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
        EC_NoError = 0,

        EC_False,               // 流程不对但结果正确
        EC_Finish,              // 已经完成
                                //
        EC_InvalidParam,        // 无效的参数
        EC_InvalieActionID,     // 无效的动作ID
        EC_Timeout,             // 超时
        EC_NotSafe,             // 不安全
        EC_RTOSKernal,          // RTOS内核出错
                                // Memory
        EC_NullPointer,         // 空指针
        EC_MemoryAllocate,      // 内存申请失败
        EC_BufferFull,          // 缓冲已满
        EC_Busy,                // 模块忙(指令缓存满)
                                // Motor
        EC_MotorStart,          // 电机起动失败
        EC_MotorStop,           // 电机停止失败
        EC_MotorSoftLimitPos,   // 电机行程超上限
        EC_MotorSoftLimitNeg,   // 电机行程超下限
        EC_MotorForceStopped,   // 电机被强制停止
        EC_Encoder,             // 编码器错误
        EC_OutSensor,           // 出光耦失败(出光耦动作完成后光耦仍然触发)
        EC_InSensor,            // 入光耦失败(入光耦动作完成后光耦仍然未触发)
                                // SPI
        EC_SPI_Status,          // SPI 状态异常
                                // PWM
        EC_PWMStart,            // PWM起动失败
        EC_PWMStop,             // PWM停止失败
                                // DAC
        EC_DACSetValue,         // DAC设置电压失败
        EC_DACStart,            // DAC使能失败
                                // TCP
        EC_TCP_Init,            // TCP 初始化失败
        EC_TCP_Bind,            // TCP 绑定失败
        EC_TCP_Listen,          // TCP 监听失败
        EC_TCP_Accept,          // TCP 连接失败
        EC_TCP_Recive,          // TCP 接收失败
        EC_TCP_Send,            // TCP 发送失败
        EC_TCP_ClientOffline,   // TCP 客户端离线

        EC_Unknown,             // 未知错误

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
    /// 平台复位应答
    /// </summary>
    public class ResPlatformReset : Response
    {
        public ResPlatformReset() : base()
        {
            Act = ActionType.PlatformReset;
        }

        public ResPlatformReset(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// X当前坐标
        /// </summary>
        [JsonProperty("CurrPosX")]
        public int CurrPosX { get; set; }

        /// <summary>
        /// y当前坐标
        /// </summary>
        [JsonProperty("CurrPosY")]
        public int CurrPosY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResPlatformReset>();
        }
    }

     /// <summary>
    /// 获取下位机版本应答
    /// </summary>
    public class ResGetVersion : Response
    {
        public ResGetVersion() : base()
        {
            Act = ActionType.GetVersion;
        }

        public ResGetVersion(IRequest req) : base(req)
        {
        }

        /// <summary>
        /// 下位机核心版本号
        /// </summary>
        [JsonProperty("Core")]
        public string Core { get; set; }

        /// <summary>
        /// 下位机RTOS版本号
        /// </summary>
        [JsonProperty("RTOS")]
        public string RTOS { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IResponse UnPacking(string data)
        {
            return data.JsonDeserialize<ResGetVersion>();
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
    /// 电机状态
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
