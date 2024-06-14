

using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Samples.UserControls;
using Sinboda.SemiAuto.View.Samples.WinView;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SampleRegisterBoardViewModel : NavigationViewModelBase
    {
        private UIElementCollection children1;
        private UIElementCollection children2;
        private UIElementCollection children3;

        #region 数据

        /// <summary>
        /// 板号
        /// </summary>
        private int boardId = 1;
        public int BoardId
        {
            get { return boardId; }
            set { Set(ref boardId, value); }
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        private string templateName;
        public string TemplateName
        {
            get { return templateName; }
            set 
            { 
                Set(ref templateName, value);
                ShowTextTpye();
            }
        }

        /// <summary>
        /// 模板名称列表
        /// </summary>
        private List<string> templateNameList;
        public List<string> TemplateNameList
        {
            get { return templateNameList; }
            set { Set(ref templateNameList, value); }
        }

        #endregion

        #region 命令
        /// <summary>
        /// 登记
        /// </summary>
        public RelayCommand SampleRigesterCmd { get; set; }

        /// <summary>
        /// 创建模板
        /// </summary>
        public RelayCommand CreateTemplateCmd { get; set; }

        /// <summary>
        /// 保存模板
        /// </summary>
        public RelayCommand SaveTemplateCmd { get; set; }
        #endregion

        public SampleRegisterBoardViewModel(UIElementCollection children1, UIElementCollection children2, UIElementCollection children3) 
        {
            this.children1 = children1;
            this.children2 = children2;
            this.children3 = children3;

            BoardId = SampleBusiness.Instance.GetMaxBoardId();
            SampleRigesterCmd = new RelayCommand(SaveBoardSample);
            CreateTemplateCmd = new RelayCommand(CreateTempLate);
            SaveTemplateCmd = new RelayCommand(SaveTemplateList);
            SetTemplateNameAndList();
            ShowTextTpye();
        }

        private void SetTemplateNameAndList()
        {
            TemplateNameList = BoardTemplateBusiness.Instance.GetTemplateNameList();
            if (TemplateNameList != null)
            {
                TemplateNameList.Add(string.Empty);
                TemplateName = string.Empty;
            }
            else
            {
                TemplateNameList = new List<string>();
                TemplateNameList.Add(string.Empty);
                TemplateName = string.Empty;
            }
        }

        private void ShowTextTpye()
        {
            //清除列表
            children1.Clear();
            children2.Clear();
            children3.Clear();

            string tempName = string.IsNullOrEmpty(TemplateName) ? "Default" : TemplateName;
            var TemplateList = BoardTemplateBusiness.Instance.GetBoardList(tempName);

            if (TemplateList != null)
            {
                foreach(var tempItem in TemplateList.OrderBy(o => o.Rack).ThenBy(p => p.Position)) 
                {
                    bool isSample = tempItem.TestType == TestType.Sample;
                    switch (tempItem.Rack) 
                    {
                        case 1:
                            children1.Add(new SpecimensManageItemControl($"{tempItem.Rack}-{tempItem.Position}", tempItem.Rack, tempItem.Position, 
                                tempItem.IsEnable, tempItem.TestType == TestType.Sample, tempItem.TestType == TestType.Calibration, tempItem.ItemName.Equals("AD"), tempItem.ItemName.Equals("PD")));
                            break;
                        case 2:
                            children2.Add(new SpecimensManageItemControl($"{tempItem.Rack}-{tempItem.Position}", tempItem.Rack, tempItem.Position,
                                tempItem.IsEnable, tempItem.TestType == TestType.Sample, tempItem.TestType == TestType.Calibration, tempItem.ItemName.Equals("AD"), tempItem.ItemName.Equals("PD")));
                            break;
                        case 3:
                            children3.Add(new SpecimensManageItemControl($"{tempItem.Rack}-{tempItem.Position}", tempItem.Rack, tempItem.Position,
                                tempItem.IsEnable, tempItem.TestType == TestType.Sample, tempItem.TestType == TestType.Calibration, tempItem.ItemName.Equals("AD"), tempItem.ItemName.Equals("PD")));
                            break;
                    }
                }
            }
            else
            {
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

                if (tempItem == null || !tempItem.IsEnable || tempItem.IsCalibration)
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
        /// 获取列表内容
        /// </summary>
        /// <param name="children"></param>
        /// <returns></returns>
        private void GetTemplateNameList(UIElementCollection children, List<Sin_BoardTemplate> boardTemplateList)
        {
            foreach (var item in children)
            {
                SpecimensManageItemControl tempItem = item as SpecimensManageItemControl;

                if (tempItem == null)
                {
                    continue;
                }

                Sin_BoardTemplate boardTemp = boardTemplateList.Where(o => o.Rack == tempItem.Rack && o.Position == tempItem.Pos).FirstOrDefault();
                boardTemp.TestType = tempItem.IsSample ? TestType.Sample : TestType.Calibration;
                boardTemp.ItemName = tempItem.IsItemAD ? "AD" : "PD";
                boardTemp.IsEnable = tempItem.IsEnable;
            }
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

        private void SaveTemplateList()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "不能保存默认方案"));
                return;
            }
            var TemplateList = BoardTemplateBusiness.Instance.GetBoardList(TemplateName);
           
            GetTemplateNameList(children1, TemplateList);
            GetTemplateNameList(children2, TemplateList);
            GetTemplateNameList(children3, TemplateList);

            BoardTemplateBusiness.Instance.SaveTemplateNameList(TemplateList);
        }

        private void CreateTempLate()
        {
            string tempName = string.IsNullOrEmpty(TemplateName) ? "Default" : TemplateName;
            var TemplateList = BoardTemplateBusiness.Instance.GetBoardList(tempName);

            GetTemplateNameList(children1, TemplateList);
            GetTemplateNameList(children2, TemplateList);
            GetTemplateNameList(children3, TemplateList);

            CreateBoardTemplateWindow createBoardTemplateWindow = new CreateBoardTemplateWindow(TemplateList);
            if (createBoardTemplateWindow.ShowDialog() == true)
            {
                SetTemplateNameAndList();
                ShowTextTpye();
            }
        }
    }
}
