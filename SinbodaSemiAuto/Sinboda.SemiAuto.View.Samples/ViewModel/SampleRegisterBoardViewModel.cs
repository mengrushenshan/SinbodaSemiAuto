

using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Samples.UserControls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SampleRegisterBoardViewModel : NavigationViewModelBase
    {
        private UIElementCollection children1;
        private UIElementCollection children2;
        private UIElementCollection children3;

        #region 数据
        private int boardId = 1;
        public int BoardId
        {
            get { return boardId; }
            set { Set(ref boardId, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 登记
        /// </summary>
        public RelayCommand SampleRigesterCmd { get; set; }
        #endregion

        public SampleRegisterBoardViewModel(UIElementCollection children1, UIElementCollection children2, UIElementCollection children3) 
        {
            this.children1 = children1;
            this.children2 = children2;
            this.children3 = children3;

            BoardId = SampleBusiness.Instance.GetMaxBoardId();
            SampleRigesterCmd = new RelayCommand(SaveBoardSample);

            ShowTextTpye();
        }

        private void ShowTextTpye()
        {
            //清除列表
            children1.Clear();
            children2.Clear();
            children3.Clear();
            
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 10; j++)
                {
                    if (i == 1)
                    {
                        children1.Add(new SpecimensManageItemControl($"{i}-{j}", i, j, true, true, false, true, false));
                    }
                    else if (i == 2)
                    {
                        children2.Add(new SpecimensManageItemControl($"{i}-{j}", i, j, true, true, false, true, false));
                    }
                    else if (i == 3)
                    {
                        children3.Add(new SpecimensManageItemControl($"{i}-{j}", i, j, true, true, false, true, false));
                    }
                }
            }
        }

        /// <summary>
        /// 获取列表内容
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private List<Sin_BoardTemplate> GetManageItemList(UIElementCollection children)
        {
            List<Sin_BoardTemplate> itemList = new List<Sin_BoardTemplate>();

            foreach (var item in children)
            {
                SpecimensManageItemControl tempItem = item as SpecimensManageItemControl;
                Sin_BoardTemplate boardTemp = new Sin_BoardTemplate();

                if (tempItem == null || !tempItem.IsEnable)
                {
                    continue;
                }

                boardTemp.Rack = tempItem.Rack;
                boardTemp.Position = tempItem.Pos;
                boardTemp.TestType = tempItem.IsSample ? TestType.Sample : TestType.Calibration;
                boardTemp.ItemName = tempItem.IsItemAD ? "AD" : "PD";

                itemList.Add(boardTemp);
            }

            return itemList;
        }

        /// <summary>
        /// 登记样本
        /// </summary>
        private void SaveBoardSample()
        {
            int sampleNo = SampleBusiness.Instance.GetMaxSampleCode();
            List<Sin_BoardTemplate> itemList = new List<Sin_BoardTemplate>();

            itemList.AddRange(GetManageItemList(children1));
            itemList.AddRange(GetManageItemList(children2));
            itemList.AddRange(GetManageItemList(children3));

            OperationResult or = new OperationResult();
            LoadingHelper.Instance.ShowLoadingWindow(a =>
            {
                a.Title = SystemResources.Instance.GetLanguage(12495, "正在登记样本，请等待...");
                foreach (var item in itemList)
                {
                    or = SampleBusiness.Instance.CreateSample(sampleNo++, item.Rack, item.Position, "", 1, item.ItemName, BoardId);
                }
            }, 0, a =>
            {
                if (!or.ResultBool)
                {
                    NotificationService.Instance.ShowError(or.Message);
                    return;
                }
                else
                {
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(3086, "登记成功"));
                    BoardId = SampleBusiness.Instance.GetMaxBoardId();
                }
            });
        }
    }
}
