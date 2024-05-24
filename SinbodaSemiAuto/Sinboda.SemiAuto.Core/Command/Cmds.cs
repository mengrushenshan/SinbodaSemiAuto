using Newtonsoft.Json;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using Sinboda.SemiAuto.Core.Models.Common;
using Sinboda.SemiAuto.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ximc;

namespace Sinboda.SemiAuto.Core.Command
{
    /// <summary>
    /// 命令
    /// </summary>
    public abstract class CmdBase : ICmd
    {
        public abstract bool Execute();
        public abstract bool ExecuteAsync();

        protected bool GetExeRes(IDataFrame frame)
        {
            int tSleep = 300;
            int num = GlobalData.TimeOut * 1000 / tSleep;
            for (int i = 0; i < num; i++)
            {
                if (frame.GetError() == ErrType.UnKnown)
                {
                    Thread.Sleep(tSleep);
                    continue;
                }
                return frame.GetError() == ErrType.None;
            }
            //超时
            return false;
        }

        protected bool ExeInternal(IRequest req, IResponse res)
        {
            //指令内容
            DataFrame frame = new DataFrame();
            frame.Request = req;
            frame.Response = res;
            if (!TcpCmdActuators.Instance.Send(frame))
                return false;
            return GetExeRes(frame);
        }

        protected bool ExeAsyncInternal(IRequest req, IResponse res)
        {
            //指令内容
            DataFrame frameReset = new DataFrame();
            frameReset.Request = req;
            frameReset.Response = res;
            if (!TcpCmdActuators.Instance.SendAsync(frameReset))
                return false;
            return GetExeRes(frameReset);
        }

        //等待Z电机停止动作
        protected bool WaitZStop(XimcArm arm, int tiemOut = -1)
        {
            //等待电机启动
            Thread.Sleep(300);

            int tSleep = 300;
            //等待 复位 测试最长用时为7秒82
            int num = 8 * 1000 / tSleep;
            if (tiemOut > 0)
                num = tiemOut * 1000 / tSleep;
            for (int i = 0; i < num; i++)
            {
                status_t status_T = new status_t();
                if (XimcHelper.Instance.Get_Status(arm.DeveiceId, out status_T) == Result.ok)
                {
                    if (status_T.CurSpeed == 0)
                        return true;
                    Thread.Sleep(tSleep);
                    continue;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// 应答信息
        /// </summary>
        private IResponse response;

        public IResponse GetResponse()
        {
            return response;
        }
    }

    /// <summary>
    /// 等待指令 
    /// </summary>
    public class CmdWait : CmdBase
    {
        /// <summary>
        /// 等待时间
        /// </summary>
        public int ms = 1000;

        public override bool Execute()
        {
            Thread.Sleep(ms);
            return true;
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 平台复位指令 (回到逻辑原点)
    /// </summary>
    public class CmdPlatformReset : CmdBase
    {
        /// <summary>
        /// 0：不回工作原点，1：回工作原点，默认为 0
        /// </summary>
        public int ReturnHome { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqPlatformReset()
            {
                ReturnHome = ReturnHome,
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqPlatformReset()
            {
                ReturnHome = ReturnHome,
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// Z轴复位指令（到工作原点）
    /// </summary>
    public class CmdZResetLogical : CmdBase
    {
        /// <summary>
        /// 电机id
        /// </summary>
        public XimcArm arm;

        /// <summary>
        /// 原点坐标
        /// </summary>
        public int pos = 0;

        public override bool Execute()
        {
            if (arm.IsNull())
                return false;
            //指令执行错误或者 未等到结束
            if (!XimcHelper.Instance.Cmd_Home(arm))
                return false;
            if (!WaitZStop(arm))
                return false;
            if (!XimcHelper.Instance.XimcMoveFast(arm, pos))
                return false;
            return WaitZStop(arm);
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Z轴复位指令（到机械原点）
    /// </summary>
    public class CmdZResetPhysical : CmdBase
    {
        /// <summary>
        /// 电机id
        /// </summary>
        public XimcArm arm;

        /// <summary>
        /// 原点坐标
        /// </summary>
        public int pos = 0;

        public override bool Execute()
        {
            if (arm.IsNull())
                return false;
            //指令执行错误或者 未等到结束
            if (!XimcHelper.Instance.Cmd_Home(arm))
                return false;
            return WaitZStop(arm);
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Z轴快速移动指令
    /// </summary>
    public class CmdZFastMove : CmdBase
    {
        /// <summary>
        /// 电机id
        /// </summary>
        public XimcArm arm;

        /// <summary>
        /// 坐标
        /// </summary>
        public int pos = 0;

        public override bool Execute()
        {
            if (arm.IsNull())
                return false;
            //指令执行错误或者 未等到结束
            if (!XimcHelper.Instance.XimcMoveFast(arm, pos))
                return false;
            return WaitZStop(arm);
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Z轴慢速移动指令
    /// </summary>
    public class CmdZSlowMove : CmdBase
    {
        /// <summary>
        /// 电机id
        /// </summary>
        public XimcArm arm;

        /// <summary>
        /// 坐标
        /// </summary>
        public int pos = 0;

        public override bool Execute()
        {
            if (arm.IsNull())
                return false;
            //指令执行错误或者 未等到结束
            if (!XimcHelper.Instance.XimcMoveSlow(arm, pos))
                return false;
            return WaitZStop(arm, 30);
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 电机连续移动指令
    /// </summary>
    public class CmdMoveCon : CmdBase
    {
        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 方向 1:正向 2:反向
        /// </summary>
        public int Dir { get; set; }

        /// <summary>
        /// 是否使用高速 1:高速 2:慢速
        /// </summary>
        public int UseFastSpeed { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMoveCon()
            {
                Id = Id,
                UseFastSpeed = UseFastSpeed,
                Dir = Dir
            };
            IResponse res = new ResMove(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMoveCon()
            {
                Id = Id,
                UseFastSpeed = UseFastSpeed,
                Dir = Dir
            };
            IResponse res = new ResMove(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机相对移动指令
    /// </summary>
    public class CmdMoveRelate : CmdBase
    {
        /// <summary>
        /// 步数(符号代表运动方向)
        /// </summary>
        public int Steps { get; set; }

        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMoveRelate()
            {
                Id = Id,
                Steps = Steps,
            };
            IResponse res = new ResMove(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMoveRelate()
            {
                Id = Id,
                Steps = Steps,
            };
            IResponse res = new ResMove(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机绝对移动指令
    /// </summary>
    public class CmdMoveAbsolute : CmdBase
    {
        /// <summary>
        /// 目标位置
        /// </summary>
        public int TargetPos { get; set; }

        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMoveAbsolute()
            {
                Id = Id,
                TargetPos = TargetPos,
            };
            IResponse res = new ResMove(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMoveAbsolute()
            {
                Id = Id,
                TargetPos = TargetPos,
            };
            IResponse res = new ResMove(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机停止移动指令
    /// </summary>
    public class CmdMoveStop : CmdBase
    {
        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMoveStop()
            {
                Id = Id
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            throw new Exception();
        }
    }

    /// <summary>
    /// 风扇控制指令
    /// </summary>
    public class CmdFanEnable : CmdBase
    {
        /// <summary>
        /// 风扇编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 0=关 1=开
        /// </summary>
        public int State { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqFanEnable()
            {
                Id = Id,
                State = State
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqFanEnable()
            {
                Id = Id,
                State = State
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机使能指令
    /// </summary>
    public class CmdMotorEnable : CmdBase
    {
        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 0=关 1=开
        /// </summary>
        public int State { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMotorEnable()
            {
                Id = Id,
                State = State
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMotorEnable()
            {
                Id = Id,
                State = State
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机复位指令
    /// </summary>
    public class CmdMotorReset : CmdBase
    {
        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 0：不回工作原点，1：回工作原点，默认为 0
        /// </summary>
        public int ReturnHome { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMotorReset()
            {
                Id = Id,
                ReturnHome = ReturnHome
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMotorReset()
            {
                Id = Id,
                ReturnHome = ReturnHome
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 电机参数设置指令
    /// </summary>
    public class CmdSetMotorParam : CmdBase
    {
        /// <summary>
        /// 电机编号
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 参数编号
        /// </summary>
        public int ParamID { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public int ParamValue { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqSetMotorParam()
            {
                Id = Id,
                ParamID = ParamID,
                ParamValue = ParamValue,
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqSetMotorParam()
            {
                Id = Id,
                ParamID = ParamID,
                ParamValue = ParamValue,
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }


}
