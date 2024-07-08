using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.SemiAuto.Core.CmdHandler.AbstractClass
{
    /// <summary>
    /// 模块上下文基类
    /// </summary>
    public class ModuleContextBase : ModuleInfoModel
    {
        private string statusDisplayValue = SystemResources.Instance.LanguageArray[5501];
        /// <summary>
        /// 仪器状态
        /// </summary>
        public string StatusDisplayValue
        {
            get { return statusDisplayValue; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set(ref statusDisplayValue, value);
                    if (value.Length > 4 && !string.IsNullOrEmpty(SecondRemain))
                    {
                        StatusForShow = value.Substring(0, 4) + "...";
                    }
                    else if (value.Length > 12)
                    {
                        StatusForShow = value.Substring(0, 9) + "...";

                    }
                    else
                    {
                        StatusForShow = value;
                    }
                });
            }
        }
        private string statusForShow = string.Empty;
        /// <summary>
        /// 模块状态栏状态显示文字
        /// </summary>
        public string StatusForShow
        {
            get { return statusForShow; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set(ref statusForShow, value);
                });
            }
        }

        private SystemState currentSystemState = SystemState.OffLine;
        /// <summary>
        /// 系统状态
        /// </summary>
        public SystemState CurrentSystemState
        {
            get { return currentSystemState; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set(ref currentSystemState, value);
                });
            }
        }


        private string secondRemain = string.Empty;
        /// <summary>
        /// 倒计时
        /// </summary>
        public string SecondRemain
        {
            get { return secondRemain; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set(ref secondRemain, value);
                    StatusDisplayValue = StatusDisplayValue;
                });
            }
        }

        private string temperature = string.Empty;
        /// <summary>
        /// 仪器温度
        /// </summary>
        public string Temperature
        {
            get { return temperature; }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Set(ref temperature, value);
                });
            }
        }

        private uint second = 0;
        private Timer timer;
        /// <summary>
        /// 设置倒计时
        /// </summary>
        /// <param name="second"></param>
        /// <param name="selfDescribed">自描述</param>
        public virtual void SetCountDown(uint second, string selfDescribed = "")
        {
            LogHelper.logSoftWare.Debug($"set count down ..... second {second}   selfDescribed {selfDescribed}");

            SecondRemain = string.Empty;
            if (timer != null)
                timer.Dispose();

            this.second = second;
            timer = new Timer(CountDown, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
        private void CountDown(object state)
        {
            try
            {
                if (second <= 0 || second >= uint.MaxValue)
                {
                    SecondRemain = string.Empty;
                    timer.Dispose();
                }
                else
                {
                    TimeSpan ts = new TimeSpan(0, 0, (int)Convert.ToUInt32(second));
                    if (ts.Hours > 0)
                    {
                        //SecondRemain = $"剩余{ts.Hours.ToString()}小时{ts.Minutes.ToString()}分钟{ts.Seconds.ToString()}秒";
                        SecondRemain = String.Format(SystemResources.Instance.LanguageArray[7849], ts.Hours.ToString(), ts.Minutes.ToString(), ts.Seconds.ToString());
                    }
                    if (ts.Hours == 0 & ts.Minutes > 0)
                    {
                        //SecondRemain = $"剩余{ts.Minutes.ToString()}分钟{ts.Seconds.ToString()}秒";
                        SecondRemain = String.Format(SystemResources.Instance.LanguageArray[7851], ts.Minutes.ToString(), ts.Seconds.ToString());
                    }
                    if (ts.Hours == 0 & ts.Minutes == 0)
                    {
                        //SecondRemain = $"剩余{ts.Seconds.ToString()}秒";
                        SecondRemain = String.Format(SystemResources.Instance.LanguageArray[7852], ts.Seconds.ToString());
                    }

                    if (second != 0)
                    {
                        second--;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"failed ModuleContextBase CountDown ..... second {second}   {ex.Message}   {ex.StackTrace}");
                SecondRemain = string.Empty;
                if (timer != null)
                    timer.Dispose();
            }
        }

        #region 状态灯

        /// <summary>
        /// 质控状态灯
        /// </summary>
        public PilotInfo QC_PilotInfo { get; private set; } = new PilotInfo();

        /// <summary>
        /// 校准状态灯
        /// </summary>
        public PilotInfo CAL_PilotInfo { get; private set; } = new PilotInfo();

        /// <summary>
        /// 温度状态灯
        /// </summary>
        public PilotInfo Temp_PilotInfo { get; private set; } = new PilotInfo();

        /// <summary>
        /// 维护状态灯
        /// </summary>
        public PilotInfo Maintain_PilotInfo { get; private set; } = new PilotInfo();

        /// <summary>
        /// 废料状态灯
        /// </summary>
        public PilotInfo Waste_PilotInfo { get; private set; } = new PilotInfo();

        /// <summary>
        /// 试剂状态灯
        /// </summary>
        public PilotInfo Reagent_PilotInfo { get; private set; } = new PilotInfo();

        #endregion


        public ModuleContextBase(ModuleInfoModel module = null)
        {
            if (module != null)
            {
                ModuleID = module.ModuleID;
                ModuleName = module.ModuleName;
                ModuleTypeCode = module.ModuleTypeCode;
            }
        }
    }
}
