using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DataOperation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.SemiAuto.TestFlow.Manager
{
    /// <summary>
    /// 联机模块管理
    /// </summary>
    public class MultiModuleManager : TBaseSingleton<MultiModuleManager>
    {
        public ObservableCollection<ModuleInfoModel> MuduleList { get; } = new ObservableCollection<ModuleInfoModel>();

        /// <summary>
        /// 刷新模块缓存数据
        /// </summary>
        public void Reload()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MuduleList.Clear();

                SemiAutoModuleManager.Instance.ClearModule();

                List<ModuleInfoModel> mims = Module_DataOperation.Instance.QueryModuleInfo().OrderBy(o => o.ModuleID).ToList();
                foreach (ModuleInfoModel mim in mims)
                {
                    // 排除服务器
                    if (mim.ModuleTypeCode == (int)ProductType.Server)
                        continue;

                    ModuleInfoModel info = mim;


                    // 初始化生化模块
                    if (mim.ModuleTypeCode == (int)ProductType.Sinboda001)
                    {
                        SemiAutoModuleContext module = SemiAutoModuleManager.Instance.GetModuleContext(info.ModuleID);
                        info = module ?? SemiAutoModuleManager.Instance.AddModule(mim);
                    }
                    MuduleList.Add(info);
                }

            });

        }


        public MultiModuleManager()
        {
            // TODO：模拟添加已联机模块 - 2018/10/19 haosd
            // 获取配置模块过程：
            // 1. 上位机启动等待中位机上传模块信息
            // 2. 上位机保存本次模块信息到数据库中
            // 3. 如果在未联机情况下打开上位机软件则不显示模块
        }
    }
}
