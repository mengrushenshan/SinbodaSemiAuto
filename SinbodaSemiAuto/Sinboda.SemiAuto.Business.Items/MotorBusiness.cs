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
        private Sin_Motor xMotor;
        private Sin_Motor yMotor;
        private Sin_Motor zMotor;
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

            foreach (var motor in motorList)
            {
                switch (motor.MotorId)
                {
                    case MotorId.Xaxis:
                        {
                            xMotor = motor;
                        }
                        break;
                    case MotorId.Yaxis:
                        {
                            yMotor = motor;
                        }
                        break;
                    case MotorId.Zaxis:
                        {
                            zMotor = motor;
                        }
                        break;
                }
            }

            return motorList;
        }

        /// <summary>
        /// 整机复位
        /// </summary>
        /// <returns></returns>
        public bool MachineReset()
        {
            CmdPlatformReset cmdPlatformReset = new CmdPlatformReset()
            {
                ReturnHome = 1,
                CloseLaser = 1
            };

            if (!cmdPlatformReset.Execute())
            {
                LogHelper.logSoftWare.Error("MachineReset failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "整机复位失败"));
                return false;
            }
            return true;
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
        #region 平台
        /// <summary>
        /// 平台机械原点复位
        /// </summary>
        public void PlatformMachineReset()
        {
            PlatformResetMotor(0);
        }

        /// <summary>
        /// 平台工作原点复位
        /// </summary>
        public void PlatformWorkReset()
        {
            PlatformResetMotor(1);
        }

        /// <summary>
        /// 平台复位
        /// </summary>
        /// <param name="isReturnHome"></param>
        private void PlatformResetMotor(int isReturnHome)
        {
            CmdPlatformReset cmdPlatformReset = new CmdPlatformReset()
            {
                ReturnHome = isReturnHome,
                CloseLaser = 1
            };

            if (!cmdPlatformReset.Execute())
            {
                LogHelper.logSoftWare.Error("PlatformResetMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台电机复位失败"));
            }
            else
            {
                ResPlatformReset resPlatformReset = cmdPlatformReset.GetResponse() as ResPlatformReset;
            }
        }
        #endregion
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
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MoveRelative(int motorId, int pos)
        {
            CmdMoveRelate cmdMoveRelative = new CmdMoveRelate()
            {
                Id = motorId,
                Steps = pos
            };

            if (!cmdMoveRelative.Execute())
            {
                LogHelper.logSoftWare.Error("MoveRelative failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机相对移动失败"));
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

        #region X电机
        /// <summary>
        /// X电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorX_MoveAbsolute(int pos)
        {
            if (xMotor == null)
            {
                return;
            }

            MoveAbsolute((int)xMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorX_MoveRelative(int pos)
        {
            if (xMotor == null)
            {
                return;
            }

            MoveRelative((int)xMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="useFastSpeed"></param>
        public void MotorX_MoveCon(Sin_Motor obj, int dir, int useFastSpeed)
        {
            if (xMotor == null)
            {
                return;
            }

            MoveCon(xMotor, dir, useFastSpeed);
        }

        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="motorObj"></param>
        /// <returns></returns>
        public bool MotorX_StopMotor()
        {
            if (xMotor == null)
            {
                return false;
            }

            return StopMotor(xMotor);
        }

        /// <summary>
        /// 获取电机状态
        /// </summary>
        /// <param name="motorId"></param>
        /// <returns></returns>
        public ResMotorStatus MotorX_GetMotorStatus()
        {
            if (xMotor == null)
            {
                return null;
            }

            return GetMotorStatus((int)xMotor.MotorId);
        }
        #endregion

        #region Y电机
        /// <summary>
        /// X电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorY_MoveAbsolute(int pos)
        {
            if (yMotor == null)
            {
                return;
            }

            MoveAbsolute((int)yMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorY_MoveRelative(int pos)
        {
            if (yMotor == null)
            {
                return;
            }

            MoveRelative((int)yMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="useFastSpeed"></param>
        public void MotorY_MoveCon(Sin_Motor obj, int dir, int useFastSpeed)
        {
            if (yMotor == null)
            {
                return;
            }

            MoveCon(yMotor, dir, useFastSpeed);
        }

        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="motorObj"></param>
        /// <returns></returns>
        public bool MotorY_StopMotor()
        {
            if (yMotor == null)
            {
                return false;
            }

            return StopMotor(yMotor);
        }

        /// <summary>
        /// 获取电机状态
        /// </summary>
        /// <param name="motorId"></param>
        /// <returns></returns>
        public ResMotorStatus MotorY_GetMotorStatus()
        {
            if (yMotor == null)
            {
                return null;
            }

            return GetMotorStatus((int)yMotor.MotorId);
        }
        #endregion

        #region Z电机
        /// <summary>
        /// X电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorZ_MoveAbsolute(int pos)
        {
            if (zMotor == null)
            {
                return;
            }

            MoveAbsolute((int)zMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        public void MotorZ_MoveRelative(int pos)
        {
            if (zMotor == null)
            {
                return;
            }

            MoveRelative((int)zMotor.MotorId, pos);
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dir"></param>
        /// <param name="useFastSpeed"></param>
        public void MotorZ_MoveCon(Sin_Motor obj, int dir, int useFastSpeed)
        {
            if (zMotor == null)
            {
                return;
            }

            MoveCon(zMotor, dir, useFastSpeed);
        }

        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="motorObj"></param>
        /// <returns></returns>
        public bool MotorZ_StopMotor()
        {
            if (zMotor == null)
            {
                return false;
            }

            return StopMotor(zMotor);
        }

        /// <summary>
        /// 获取电机状态
        /// </summary>
        /// <param name="motorId"></param>
        /// <returns></returns>
        public ResMotorStatus MotorZ_GetMotorStatus()
        {
            if (zMotor == null)
            {
                return null;
            }

            return GetMotorStatus((int)zMotor.MotorId);
        }
        #endregion

    }
}
