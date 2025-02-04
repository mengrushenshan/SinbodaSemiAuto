﻿using Newtonsoft.Json;
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
    /// 请求类型
    /// </summary>
    public enum CmdType : Int32
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 指令帧
        /// </summary>
        Command = 1,

        /// <summary>
        /// 确认帧
        /// </summary>
        Confirm = 2,

        /// <summary>
        /// 结果帧
        /// </summary>
        Result = 3,

        /// <summary>
        /// 消息帧（下位机主推
        /// </summary>
        Message = 4,
    }

    /// <summary>
    /// 动作类型
    /// </summary>
    public enum ActionType : Int32
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 平台参数设置
        /// </summary>
        PlatformParam = 101,

        /// <summary>
        /// 平台复位
        /// </summary>
        PlatformReset = 102,

        /// <summary>
        /// 坐标移动
        /// </summary>
        MovePos = 103,

        /// <summary>
        /// 电机参数设置
        /// </summary>
        MotorParam = 201,

        /// <summary>
        /// 电机使能
        /// </summary>
        MotorEnable = 202,

        /// <summary>
        /// 电机复位
        /// </summary>
        MotorReset = 203,

        /// <summary>
        /// 电机连续移动
        /// </summary>
        MoveCont = 204,

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        MoveAbsolute = 205,

        /// <summary>
        /// 电机相对移动
        /// </summary>
        MoveRelative = 206,

        /// <summary>
        /// 电机移动停止
        /// </summary>
        MoveStop = 207,

        /// <summary>
        /// 风扇参数设置
        /// </summary>
        FanParam = 301,

        /// <summary>
        /// 风扇控制
        /// </summary>
        FanEnable = 302,

        /// <summary>
        /// 光源参数设置
        /// </summary>
        LightParam = 401,

        /// <summary>
        /// 光源控制
        /// </summary>
        LightEnable = 402,

        /// <summary>
        /// 蜂鸣器参数设置
        /// </summary>
        BuzzerParam = 501,

        /// <summary>
        /// 蜂鸣器控制
        /// </summary>
        BuzzerEnable = 502,


    }

    /// <summary>
    /// 请求基类
    /// </summary>
    public abstract class Request : IRequest
    {
        public Request()
        {
            FrameID = GlobalData.FrameID;
            Cmd = CmdType.Command;
        }

        /// <summary>
        /// 指令编号
        /// </summary>
        [JsonProperty("FrameID")]
        public int FrameID { get; protected set; }

        /// <summary>
        /// 帧类型
        /// </summary>
        [JsonProperty("CmdType")]
        public CmdType Cmd { get; protected set; }

        /// <summary>
        /// 命令类型
        /// </summary>
        [JsonProperty("ActionID")]
        public ActionType Act { get; protected set; }

        public ActionType GetAct()
        {
            return Act;
        }

        public CmdType GetCmd()
        {
            return Cmd;
        }

        public int ToFrameID()
        {
            return FrameID;
        }

        public virtual string ToRequest()
        {
            return this.JsonSerialize();
        }
    }

    /// <summary>
    /// 复位请求
    /// </summary>
    public class ReqPlatformReset : Request
    {
        public ReqPlatformReset() : base()
        {
            Act = ActionType.PlatformReset;
        }

        /// <summary>
        /// 0：不回工作原点，1：回工作原点，默认为 0
        /// </summary>
        [JsonProperty("ReturnHome")]
        public int ReturnHome { get; set; }
    }

    /// <summary>
    /// 移动请求
    /// </summary>
    public class ReqMovePos : Request
    {
        public ReqMovePos() : base()
        {
            Act = ActionType.MovePos;
        }

        /// <summary>
        /// x坐标
        /// </summary>
        [JsonProperty("X")]
        public int X { get; set; }

        /// <summary>
        /// y坐标
        /// </summary>
        [JsonProperty("Y")]
        public int Y { get; set; }
    }

    /// <summary>
    /// 连续移动请求
    /// </summary>
    public class ReqMoveCon : Request
    {
        public ReqMoveCon() : base()
        {
            Act = ActionType.MoveCont;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 方向 1:正向 2:反向
        /// </summary>
        [JsonProperty("Dir")]
        public int Dir { get; set; }

        /// <summary>
        /// 是否使用高速 1:高速 2:慢速
        /// </summary>
        [JsonProperty("UseFastSpeed")]
        public int UseFastSpeed { get; set; }
    }

    /// <summary>
    /// 电机参数设置请求
    /// </summary>
    public class ReqSetMotorParam : Request
    {
        public ReqSetMotorParam() : base()
        {
            Act = ActionType.MotorParam;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 参数编号
        /// </summary>
        [JsonProperty("ParamID")]
        public int ParamID { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        [JsonProperty("ParamValue")]
        public int ParamValue { get; set; }
    }

    /// <summary>
    /// 相对移动请求
    /// </summary>
    public class ReqMoveRelate : Request
    {
        public ReqMoveRelate() : base()
        {
            Act = ActionType.MoveRelative;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 步数(符号代表运动方向)
        /// </summary>
        [JsonProperty("Steps")]
        public int Steps { get; set; }
    }

    /// <summary>
    /// 绝对移动请求
    /// </summary>
    public class ReqMoveAbsolute : Request
    {
        public ReqMoveAbsolute() : base()
        {
            Act = ActionType.MoveAbsolute;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        [JsonProperty("TargetPos")]
        public int TargetPos { get; set; }
    }

    /// <summary>
    /// 停止移动请求
    /// </summary>
    public class ReqMoveStop : Request
    {
        public ReqMoveStop() : base()
        {
            Act = ActionType.MoveStop;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }
    }

    /// <summary>
    /// 风扇使能请求
    /// </summary>
    public class ReqFanEnable : Request
    {
        public ReqFanEnable() : base()
        {
            Act = ActionType.FanEnable;
        }

        /// <summary>
        /// 风扇编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 0=关 1=开
        /// </summary>
        [JsonProperty("State")]
        public int State { get; set; }
    }

    /// <summary>
    /// 电机使能请求
    /// </summary>
    public class ReqMotorEnable : Request
    {
        public ReqMotorEnable() : base()
        {
            Act = ActionType.MotorEnable;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 0:失能；1:使能
        /// </summary>
        [JsonProperty("State")]
        public int State { get; set; }
    }

      /// <summary>
    /// 电机复位请求
    /// </summary>
    public class ReqMotorReset : Request
    {
        public ReqMotorReset() : base()
        {
            Act = ActionType.MotorReset;
        }

        /// <summary>
        /// 电机编号
        /// </summary>
        [JsonProperty("ID")]
        public int Id { get; set; }

        /// <summary>
        /// 0：不回工作原点，1：回工作原点，默认为 0
        /// </summary>
        [JsonProperty("ReturnHome")]
        public int ReturnHome { get; set; }
    }



}
