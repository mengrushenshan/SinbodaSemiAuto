using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View.ViewModels
{
    public class AboutViewModel : NavigationViewModelBase
    {
        #region 数据源
        /// <summary>
        /// 是否已注册
        /// </summary>
        public short IsRegisted { get; set; }

        private string coreVersion;
        public string CoreVersion
        {
            get { return coreVersion; }
            set
            {
                Set(ref coreVersion, value);
            }
        }
        private ObservableCollection<ModuleVersionModel> equipmentInfoList;
        public ObservableCollection<ModuleVersionModel> EquipmentInfoList
        {
            get { return equipmentInfoList; }
            set
            {
                Set(ref equipmentInfoList, value);
            }
        }
        /// <summary>
        /// 软件信息
        /// </summary>
        private ObservableCollection<ModuleVersionModel> softwearInfoList;
        public ObservableCollection<ModuleVersionModel> SoftwearInfoList
        {
            get { return softwearInfoList; }
            set
            {
                Set(ref softwearInfoList, value);
            }
        }
        /// <summary>
        /// 模块信息
        /// </summary>
        private ObservableCollection<ModuleVersionModel> moduleInfoList;
        public ObservableCollection<ModuleVersionModel> ModuleInfoList
        {
            get { return moduleInfoList; }
            set
            {
                Set(ref moduleInfoList, value);
            }
        }
        /// <summary>
        /// 算法信息
        /// </summary>
        private ObservableCollection<ModuleVersionModel> algorithmInfoList;
        public ObservableCollection<ModuleVersionModel> AlgorithmInfoList
        {
            get { return algorithmInfoList; }
            set
            {
                Set(ref algorithmInfoList, value);
            }
        }
        #endregion

        #region 初始化

        public AboutViewModel()
        {
        }
        #endregion

        /// 信息初始化
        /// </summary>
        internal void InitInfo()
        {
            CoreVersion = SystemResources.Instance.CurrentSoftwareVersion;
            if (Object.Equals(null, EquipmentInfoList))
            {
                EquipmentInfoList = new ObservableCollection<ModuleVersionModel>();
            }
            //上位机软件信息
            EquipmentInfoList.Add(new ModuleVersionModel()
            {
                ModuleID = 0,
                UnitOrder = 1,
                MachineShowName = "1_" + VirtualModuleCacheManager.InfoList[0],
                UnitShowName = SoftWareVersionCacheManager.InfoList.FirstOrDefault(o => o.BoardId == 1).BoardName,
                VersionInfo = GetCpuInfo(),
                CreatTimeShowInfo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            EquipmentInfoList.Add(new ModuleVersionModel()
            {
                ModuleID = 0,
                UnitOrder = 2,
                MachineShowName = "1_" + VirtualModuleCacheManager.InfoList[0],
                UnitShowName = SoftWareVersionCacheManager.InfoList.FirstOrDefault(o => o.BoardId == 2).BoardName,
                VersionInfo = CoreVersion,
                CreatTimeShowInfo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            EquipmentInfoList.Add(new ModuleVersionModel()
            {
                ModuleID = 0,
                UnitOrder = 3,
                MachineShowName = "1_" + VirtualModuleCacheManager.InfoList[0],
                UnitShowName = SoftWareVersionCacheManager.InfoList.FirstOrDefault(o => o.BoardId == 3).BoardName,
                VersionInfo = "SINBODA"/*((AssemblyCopyrightAttribute[])(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute))))[0].Copyright*/,
                CreatTimeShowInfo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            EquipmentInfoList = new ObservableCollection<ModuleVersionModel>(EquipmentInfoList.OrderBy(o => o.ModuleID).ThenBy(o => o.UnitOrder).ToList());
        }

        /// <summary>
        /// 获取处理器型号
        /// </summary>
        /// <returns></returns>
        internal static string GetCpuInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\CIMV2", "SELECT Name FROM Win32_Processor");
            string cpuId = string.Empty;
            foreach (ManagementObject obj2 in searcher.Get())
            {
                try
                {
                    //return (obj2.GetPropertyValue("Name").ToString() + ", " + Environment.ProcessorCount.ToString());
                    cpuId = (obj2.GetPropertyValue("Name").ToString());
                    break;
                }
                catch (Exception ex)
                {
                    LogHelper.logSoftWare.Error("GetCpuInfo", ex);
                    continue;
                }
            }
            return cpuId;
        }
    }
}
