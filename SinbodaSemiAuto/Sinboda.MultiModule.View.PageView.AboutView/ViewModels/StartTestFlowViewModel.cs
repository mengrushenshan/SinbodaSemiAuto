using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View.ViewModels
{
    public class StartTestFlowViewModel : NavigationViewModelBase
    {
        private int boardId = 1;
        public int BoardId
        {
            get { return boardId; }
            set { Set(ref boardId, value); }
        }
        /// <summary>
        /// 启动测试
        /// </summary>
        public RelayCommand StartTestCommand { get; set; }

        public StartTestFlowViewModel() 
        {
            StartTestCommand = new RelayCommand(StartTestFlow);
        }

        public void StartTestFlow()
        {
            
            LoadingHelper.Instance.ShowLoadingWindow(ancBegin =>
            {
                // 1.初始化
                ancBegin.Title = SystemResources.Instance.GetLanguage(12396, "正在准备测试数据，请等待...");

                TestFlow.TestFlow.Instance.SetBoardId(BoardId);
                TestFlow.TestFlow.Instance.CreateTest();

                LogHelper.logSoftWare.Debug($"prepare test complete ..... ");

            }, 0, ancBegin =>
            {
                if (!PVCamHelper.Instance.GetOpenFlag())
                {
                    PVCamHelper.Instance.StartCont();
                }
                Task.Run(() =>
                {
                    TestFlow.TestFlow.Instance.StartItemTest();
                });
            });
        }
    }
}
