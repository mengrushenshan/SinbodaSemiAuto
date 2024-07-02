using Sinboda.SemiAuto.Core.Command;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Interfaces;
using Sinboda.SemiAuto.Core.Models.Common;
using Sinboda.SemiAuto.Core.Models;
using System;
using System.Threading;
using ximc;
using Newtonsoft.Json;
using log4net.Repository.Hierarchy;
using Sinboda.Framework.Common.Log;

namespace sin_mole_flu_analyzer.Models.Command
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
                response = frame.GetResponse();
                return frame.GetError() == ErrType.EC_NoError;
            }
            //超时
            response = frame.GetResponse();
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
            IResponse res = new ResPlatformReset(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqPlatformReset()
            {
                ReturnHome = ReturnHome,
            };
            IResponse res = new ResPlatformReset(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 平台移动指令
    /// </summary>
    public class CmdPlatformMove : CmdBase
    {
        /// <summary>
        /// x坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// y坐标
        /// </summary>
        public int Y { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMovePos()
            {
                X = X,
                Y = Y,
            };
            IResponse res = new ResPlatformReset(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMovePos()
            {
                X = X,
                Y = Y,
            };
            IResponse res = new ResPlatformReset(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 平台停止指令
    /// </summary>
    public class CmdPlatformStop : CmdBase
    {
        public override bool Execute()
        {
            IRequest req = new ReqStopMove()
            {
            };
            IResponse res = new ResPlatformReset(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqStopMove()
            {
            };
            IResponse res = new ResPlatformReset(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 查询电机状态
    /// </summary>
    public class CmdGetMotorStatus : CmdBase
    {
        /// <summary>
        /// 0-1: 电机编号
        /// </summary>
        public int Id { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqGetMotorStatus()
            {
                Id = Id,
            };
            IResponse res = new ResMotorStatus(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqGetMotorStatus()
            {
                Id = Id,
            };
            IResponse res = new ResMotorStatus(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 查询仓门状态
    /// </summary>
    public class CmdGetDoorStatus : CmdBase
    {

        public override bool Execute()
        {
            IRequest req = new ReqGetDoorStatus()
            {
            };
            IResponse res = new ResDoorStatus(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqGetDoorStatus()
            {
            };
            IResponse res = new ResDoorStatus(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 查询下位机版本
    /// </summary>
    public class CmdGetVersion : CmdBase
    {

        public override bool Execute()
        {
            IRequest req = new ReqGetVersion()
            {
            };
            IResponse res = new ResGetVersion(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqGetVersion()
            {
            };
            IResponse res = new ResGetVersion(req);
            return ExeAsyncInternal(req, res);
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
        /// 是否使用高速 1:高速 0:慢速
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
        /// 步数
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

            IResponse res = new ResMove(req);
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
        public int Enable { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqFanEnable()
            {
                Id = Id,
                Enable = Enable
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqFanEnable()
            {
                Id = Id,
                Enable = Enable
            };
            IResponse res = new Response(req);
            return ExeAsyncInternal(req, res);
        }
    }

    /// <summary>
    /// 激光控制指令
    /// </summary>
    public class CmdLightEnable : CmdBase
    {
        /// <summary>
        /// 0=关 1=开
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// 输出电压 0 - 2.5(精确到0.001)
        /// </summary>
        public double Voltage { get; set; } = 0.5;

        /// <summary>
        /// 是否输出 (0:不输出(等待相机爆光信号) 1:输出)
        /// </summary>
        public int Output { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqLightEnable()
            {
                Voltage = Voltage,
                Enable = Enable,
                Output = Output
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqLightEnable()
            {
                Voltage = Voltage,
                Enable = Enable,
                Output = Output
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
        public int Enable { get; set; }

        public override bool Execute()
        {
            IRequest req = new ReqMotorEnable()
            {
                Id = Id,
                Enable = Enable
            };
            IResponse res = new Response(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMotorEnable()
            {
                Id = Id,
                Enable = Enable
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
            IResponse res = new ResMove(req);
            return ExeInternal(req, res);
        }

        public override bool ExecuteAsync()
        {
            IRequest req = new ReqMotorReset()
            {
                Id = Id,
                ReturnHome = ReturnHome
            };
            IResponse res = new ResMove(req);
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

    /// <summary>
    /// IAP指令
    /// </summary>
    public class CmdIAP : CmdBase
    {
        /// <summary>
        /// IAP数据
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 等待iap响应
        /// </summary>
        private int IAPWait = 10 * 1000;

        /// <summary>
        /// 帧长度
        /// </summary>
        private int frameLen = 512;

        public override bool Execute()
        {
            //获取版本
            CmdGetVersion cmdGetVersion = new CmdGetVersion();
            if (!cmdGetVersion.Execute())
            {
                LogHelper.logSoftWare.Info($"已进入boot模式 无法获取升级前版本！");
            }
            else
            {
                LogHelper.logSoftWare.Info($"更新前软件版本 Core:[{(cmdGetVersion.GetResponse() as ResGetVersion).Core}] " +
                    $"RTOS:[{(cmdGetVersion.GetResponse() as ResGetVersion).RTOS}] ");
            }

            //进入IAP模式
            IRequest req = new IAPOn()
            {
            };
            IResponse res = new Response(req);
            if (!ExeInternal(req, res))
            {
                LogHelper.logSoftWare.Info($"已进入boot模式 进入IAP失败,可正常开始升级！");
            }
            else
            {
                //重启tcp连接
                TcpCmdActuators.Instance.ReStart(IAPWait);
            }

            //IAP开始
            req = new IAPStart()
            {
                FileSize = Data.Length,
            };
            if (!ExeInternal(req, res))
                return false;

            //计算需要发送多少帧
            int frameNum = Data.Length / frameLen;
            int lastFrameLen = Data.Length % frameLen;
            byte[] bytes;

            //循环发送帧
            for (int i = 0; i < frameNum; i++)
            {
                bytes = new byte[frameLen];
                Array.Copy(Data, i * frameLen, bytes, 0, frameLen);
                req = new IAPIn()
                {
                    Data = Convert.ToBase64String(bytes),
                };
                if (!ExeInternal(req, res))
                    return false;
            }

            //最后一帧
            bytes = new byte[lastFrameLen];
            Array.Copy(Data, frameNum * frameLen, bytes, 0, lastFrameLen);
            req = new IAPIn()
            {
                Data = Convert.ToBase64String(bytes),
            };
            if (!ExeInternal(req, res))
                return false;

            //IAP结束
            req = new IAPFinish()
            {
            };
            if (!ExeInternal(req, res))
                return false;

            //退出IAP模式
            req = new IAPOff()
            {
            };
            if (!ExeInternal(req, res))
                return false;

            //重启tcp连接
            TcpCmdActuators.Instance.ReStart(IAPWait);

            //获取版本ok 即为更新完成
            cmdGetVersion = new CmdGetVersion();
            bool result = cmdGetVersion.Execute();
            if (result)
                LogHelper.logSoftWare.Info($"更新后软件版本 Core:[{(cmdGetVersion.GetResponse() as ResGetVersion).Core}] " +
                   $"RTOS:[{(cmdGetVersion.GetResponse() as ResGetVersion).RTOS}] ");
            return result;
        }

        public override bool ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }


}


