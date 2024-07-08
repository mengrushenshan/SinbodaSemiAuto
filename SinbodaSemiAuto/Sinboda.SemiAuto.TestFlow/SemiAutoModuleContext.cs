using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.CmdHandler.AbstractClass;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Manager;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class SemiAutoModuleContext : ModuleContextBase
    {
        /// <summary>
        /// 系统状态
        /// </summary>
        public SystemState ModuleState { get; private set; } = SystemState.OffLine;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="module">模块信息</param>
        public SemiAutoModuleContext(ModuleInfoModel module) : base(module)

        {

            // 初始化模块是否禁用
            bool? is_module_enabled = ModuleSettingManager.Instance.IsModuleEnabled(module.ModuleID);

            if (is_module_enabled.HasValue)
            {
                if (!is_module_enabled.Value)
                {
                    //禁用
                    SetModuleStatus(SystemState.OffLine);
                }
                else
                {
                    //TODO 是否休眠
                    bool? is_module_sleep = ModuleSettingManager.Instance.IsModulSleep(module.ModuleID);
                    if (is_module_sleep.HasValue)
                    {
                        if (is_module_sleep.Value)
                        {
                            SetModuleStatus(SystemState.Sleep);
                        }
                        else
                        {
                            SetModuleStatus(SystemState.OffLine);
                        }
                    }
                    else
                    {
                       
                        SetModuleStatus(SystemState.OffLine);
                    }
                }
            }
            else
            {
                if (!TcpCmdActuators.Instance.IsConnected())
                {

                    SetModuleStatus(SystemState.OffLine);
                }
                else
                {
                    SetModuleStatus(SystemState.StandBy);
                }
            }
            //add by huangxy @ 2019-10-09 end
        }

        /// <summary>
        /// 模块状态
        /// </summary>
        public void SetModuleStatus(SystemState temp)
        {
            CurrentSystemState = ModuleState = temp;
            switch (temp)
            {
                //0x01 仪器待机，固相试剂混匀
                case SystemState.StandBy:
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(4207, "待机");// 待机
                    SetCountDown(0);
                    break;
                //0x02 仪器测试
                case SystemState.Testing:
                    #region 仪器测试
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(1237, "测试中");//测试中
                    #endregion
                    break;
                //0x03 仪器故障
                case SystemState.Error:
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(3249, "仪器故障");//"仪器故障";
                    SetCountDown(0);
                    break;
                case SystemState.MainTenance:
                    break;

                //0xfe 休眠
                case SystemState.Sleep:
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(543, "休眠");
                    SetCountDown(0);
                    break;

                //0xff 连接失败
                case SystemState.OffLine:
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(5501, "连接失败");
                    SetCountDown(0);
                    break;

                default:
                    CurrentSystemState = ModuleState = SystemState.None;
                    StatusDisplayValue = SystemResources.Instance.GetLanguage(7319, "状态获取中");//状态获取中
                    break;
            }

            LogHelper.logSoftWare.Debug($"MachineStatusCmdHandler，模块 {ModuleID}，状态 {StatusDisplayValue}");

            //发送发光仪器状态变化通知
            //原来在函数开始时发送，但状态未更新，现改为状态更新完毕后发送
            //Messenger.Default.Send(ModuleID, MessageToken.CMEXAM_STATUS);
            Messenger.Default.Send(ModuleID, MessageToken.EXAM_STATUS);
        }
    }
}
