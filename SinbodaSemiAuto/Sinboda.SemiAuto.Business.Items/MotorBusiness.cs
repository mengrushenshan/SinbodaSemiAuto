using sin_mole_flu_analyzer.Models.Command;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Models.Common;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Model.DataOperation.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ximc;

namespace Sinboda.SemiAuto.Business.Items
{
    public class MotorBusiness : BusinessBase<MotorBusiness>
    {
        /// <summary>
        /// 获取全部电机列表
        /// </summary>
        /// <returns>电机列表</returns>
        public List<Sin_Motor> GetMotorList()
        {
            List<Sin_Motor> motorList = Sin_Motor_DataOperation.Instance.Query(o => true).OrderBy(p => p.MotorId).ToList();
            if (motorList == null || motorList.Count == 0)
            {
                return null;
            }

            return motorList;
        }

        /// <summary>
        /// 保存电机参数
        /// </summary>
        /// <returns></returns>
        public bool SaveMotorItem(Sin_Motor motorItem)
        {
            bool result = false;
            try
            {
                if (motorItem != null)
                {
                    var tempMotor = Sin_Motor_DataOperation.Instance.Find(motorItem.Id);
                    if (tempMotor != null)
                    {
                        Sin_Motor_DataOperation.Instance.Update(motorItem);
                        result = true;
                    }
                    else
                    {
                        LogHelper.logSoftWare.Error($"SaveMotorItem error: Sin_Motor don't have motorItem Id={motorItem.Id} ");
                    }
                }
                else
                {
                    LogHelper.logSoftWare.Error($"SaveMotorItem error: Sin_Motor is null");
                }

                return result;
            }
            catch (Exception ex) 
            {
                LogHelper.logSoftWare.Error($"SaveMotorItem error: {ex.Message}");
                return result;
            }
        }

        #region 电机

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MoveAbsolute(int motorId, int pos)
        {
            CmdMoveAbsolute cmdMoveAbsolute = new CmdMoveAbsolute()
            {
                Id = motorId,
                TargetPos = pos
            };

            if (!cmdMoveAbsolute.Execute())
            {
                LogHelper.logSoftWare.Error("MoveAbsolute failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机绝对移动失败"));
            }
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="useFastSpeed"></param>
        public void MoveCon(Sin_Motor obj, int dir, int useFastSpeed)
        {
            CmdMoveCon cmdMoveCon = new CmdMoveCon()
            {
                Id = (int)obj.MotorId,
                Dir = dir,
                UseFastSpeed = useFastSpeed
            };

            if (obj.IsRunning)
            {
                return;
            }

            if (!cmdMoveCon.Execute())
            {
                LogHelper.logSoftWare.Error("AlawysMove failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机持续移动失败"));
            }
            else
            {
                obj.IsRunning = true;
            }
        }

        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="motorObj"></param>
        /// <returns></returns>
        public bool StopMotor(Sin_Motor motorObj)
        {
            CmdMoveStop cmdMoveStop = new CmdMoveStop() { Id = (int)motorObj.MotorId };

            if (!motorObj.IsRunning)
            {
                return false;
            }

            if (!cmdMoveStop.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机停止失败"));

                return false;
            }
            else
            {
                ResMove resMove = cmdMoveStop.GetResponse() as ResMove;
                motorObj.TargetPos = resMove.CurPos;
                motorObj.IsRunning = false;

                return true;
            }
        }

        /// <summary>
        /// 获取电机状态
        /// </summary>
        /// <param name="motorId"></param>
        /// <returns></returns>
        public ResMotorStatus GetMotorStatus(int motorId)
        {
            CmdGetMotorStatus cmdGetMotorStatus = new CmdGetMotorStatus() { Id = motorId };

            if (cmdGetMotorStatus.Execute())
            {
                ResMotorStatus resMotorStatus = cmdGetMotorStatus.GetResponse() as ResMotorStatus;

                if (resMotorStatus != null)
                {
                    return resMotorStatus;
                }
            }
            else
            {
                LogHelper.logSoftWare.Error("GetMotorStatus failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机状态获取失败"));
            }
            return null;
        }
        #endregion

        #region Ximc

        /// <summary>
        /// 快速移动指令
        /// </summary>
        /// <param name="obj"></param>
        public void XimcMoveFast(XimcArm obj, int pos)
        {
            CmdZFastMove cmdZFastMove = new CmdZFastMove()
            {
                arm = obj,
                pos = pos
            };

            if (!cmdZFastMove.Execute())
            {
                LogHelper.logSoftWare.Error("XimcMoveFast failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机快速移动失败"));
            }
        }

        /// <summary>
        /// 相对移动
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isFast"></param>
        /// <param name="nStep"></param>
        public bool MoveRelativePos(XimcArm obj, bool isFast, int nStep)
        {
            status_t status_T = new status_t();
            XimcHelper.Instance.Get_Status(obj.DeveiceId, out status_T);
            Cmd_Move_Relative cmd_Move_Relative = new Cmd_Move_Relative()
            {
                arm = obj,
                fast = isFast,
                pos = nStep
            };

            if (status_T.CurSpeed != 0)
            {
                return false;
            }

            if (cmd_Move_Relative.Execute())
            {
                SetXimcStatus(obj);
                return true;
            }
            else
            {
                LogHelper.logSoftWare.Error("XimcMoveRelative failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机相对移动失败"));
                return false;
            }
        }

        /// <summary>
        /// 获取电机位置
        /// </summary>
        /// <param name="deveiceId"></param>
        /// <returns></returns>
        public bool SetXimcStatus(XimcArm obj)
        {
            bool result = false;
            status_t status_T = new status_t();

            XimcHelper.Instance.Get_Status(obj.DeveiceId, out status_T);
            obj.TargetPos = status_T.CurPosition;

            return result = true;
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        public bool XimcStop(XimcArm obj)
        {
            XimcHelper.Instance.Cmd_SlowStop(obj.DeveiceId);

            return true;
        }
        #endregion
    }
}
