

using DevExpress.CodeParser;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Samples.UserControls;
using Sinboda.SemiAuto.View.Samples.WinView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SampleRegisterBoardViewModel : NavigationViewModelBase
    {
        public Action RefTemplateBoard;
        #region 数据

        private Sin_BoardTemplate sinBoardTemplate;
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
                RefTemplatePage();
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


        private List<Sin_BoardTemplate> templateList;
        public List<Sin_BoardTemplate> TemplateList
        {
            get { return templateList; }
            set { Set(ref templateList, value);}
        }

        /// <summary>
        /// 孔位是否启用
        /// </summary>
        private bool isTemplateEnable;
        public bool IsTemplateEnable
        {
            get { return isTemplateEnable; }
            set { Set(ref isTemplateEnable, value); }
        }

        /// <summary>
        /// 孔位类型
        /// </summary>
        private TestType templateType;
        public TestType TemplateType
        {
            get { return templateType; }
            set { Set(ref templateType, value); }
        }

        /// <summary>
        /// 架子
        /// </summary>
        private List<string> rackSouce = new List<string>() { "", SystemResources.Instance.GetLanguage(1719, "全部"),"A", "B", "C" };
        public List<string> RackSouce
        {
            get { return rackSouce; }
            set { Set(ref rackSouce, value); }
        }

        /// <summary>
        /// 位置
        /// </summary>
        private List<string> posSouce = new List<string>() { "", SystemResources.Instance.GetLanguage(1719, "全部"), "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        public List<string> PosSouce
        {
            get { return posSouce; }
            set { Set(ref posSouce, value); }
        }

        /// <summary>
        /// 架号
        /// </summary>
        private string rack;
        public string Rack
        {
            get { return rack; }
            set { Set(ref rack, value);}
        }

        /// <summary>
        /// 位置
        /// </summary>
        private string position;
        public string Position
        {

            get { return position; }
            set { Set(ref position, value); }
        }

        /// <summary>
        /// 模板对应项目
        /// </summary>
        private string templateItem;
        public string TemplateItem
        {
            get { return templateItem; }
            set 
            { 
                Set(ref templateItem, value);
                SetTemplateItemName();
            }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 删除模板
        /// </summary>
        public RelayCommand DeleteTemplateCmd { get; set; }

        /// <summary>
        /// 创建模板
        /// </summary>
        public RelayCommand CreateTemplateCmd { get; set; }

        /// <summary>
        /// 保存模板
        /// </summary>
        public RelayCommand SaveTemplateCmd { get; set; }

        /// <summary>
        /// 应用
        /// </summary>
        public RelayCommand CommitCmd { get; set; }

        /// <summary>
        /// 登记
        /// </summary>
        public RelayCommand RegisterCmd { get; set; }
        #endregion
        public SampleRegisterBoardViewModel()
        {
            BoardId = SampleBusiness.Instance.GetMaxBoardId();
            DeleteTemplateCmd = new RelayCommand(DeleteTemplate);
            CreateTemplateCmd = new RelayCommand(CreateTempLate);
            SaveTemplateCmd = new RelayCommand(SaveTemplateList);
            CommitCmd = new RelayCommand(SetTempLateInfo);
            RegisterCmd = new RelayCommand(RegisterBoard);
            SetTemplateNameAndList();
            InitTemplateList();
        }
        private void RefTemplatePage()
        {
            InitTemplateList();
            if (RefTemplateBoard != null)
            {
                RefTemplateBoard();
            }
        }

        private void InitTemplateList()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                TemplateList = BoardTemplateBusiness.Instance.GetBoardList("Default");
            }
            else
            {
                TemplateList = BoardTemplateBusiness.Instance.GetBoardList(TemplateName);
            }

            TemplateItem = TemplateList.Select(o => o.ItemName).FirstOrDefault();
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

        /// <summary>
        /// 选中数据
        /// </summary>
        /// <param name="reagent"></param>
        public void ShowTemplateInfo(Sin_BoardTemplate boardTemplate)
        {
            RefTemplateBoard();
            sinBoardTemplate = boardTemplate;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TemplateType = boardTemplate.TestType;
                IsTemplateEnable = boardTemplate.IsEnable;
            });
        }

        /// <summary>
        /// 获取孔位数据
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Sin_BoardTemplate GetSinBoardTemplate(string tag)
        {
            if (!tag.Contains("-"))
            {
                return null;
            }

            string[] strRackAndPos = tag.Split('-');
            string rack = strRackAndPos[0];
            int pos = int.Parse(strRackAndPos[1]);

            var tempList = TemplateList.Where(o => o.Rack == rack && o.Position == pos).ToList();
            if (tempList.Count == 0)
            {
                return null;
            }

            return tempList.FirstOrDefault();
        }

        /// <summary>
        /// 删除模板
        /// </summary>
        private void DeleteTemplate()
        {
            if(string.IsNullOrEmpty(TemplateName))
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "默认模板不能删除"));
                return; 
            }

            //"确认删除吗？"
            if (NotificationService.Instance.ShowQuestion(SystemResources.Instance.GetLanguage(41, "确认删除吗？")) == MessageBoxResult.No)
            {
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (BoardTemplateBusiness.Instance.DeleteTemplateNameList(TemplateList))
                {
                    WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板删除成功"));
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "模板删除成功"));
                    SetTemplateNameAndList();
                    InitTemplateList();
                }
                else
                {
                    WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板删除失败"));
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "模板删除失败"));
                }
            });
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        private void SaveTemplateList()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "默认模板不能修改"));
                return;
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (BoardTemplateBusiness.Instance.SaveTemplateNameList(TemplateList))
                {
                    WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板保存完成"));
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "模板保存完成"));
                    
                }
                else
                {
                    WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板保存失败"));
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "模板保存失败"));
                }
            });
            
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        private void CreateTempLate()
        {
            CreateBoardTemplateWindow createBoardTemplateWindow = new CreateBoardTemplateWindow(TemplateList);
            if (createBoardTemplateWindow.ShowDialog() == true)
            {
                WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板创建成功"));
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "模板创建成功"));
                SetTemplateNameAndList();
                InitTemplateList();
            }
            else
            {
                WriteOperateLog(SystemResources.Instance.GetLanguage(0, "模板创建失败"));
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "模板创建失败"));
            }
        }

        private void SetTempLateInfo()
        {
            var templateBoard = TemplateList.Find(o => o.Id == sinBoardTemplate.Id);
            if (templateBoard != null)
            {
                templateBoard.IsEnable = IsTemplateEnable;
                templateBoard.TestType = TemplateType;
            }
            RefTemplateBoard();
        }

        private void SetTemplateItemName()
        {
            Task.Run(() => {
                TemplateList.ForEach(o => o.ItemName = TemplateItem);
            });
        }

        private void RegisterBoard()
        {
            int sampleNo = SampleBusiness.Instance.GetMaxSampleCode();
            List<Sin_BoardTemplate> itemList = new List<Sin_BoardTemplate>();

            itemList = TemplateList.Where(o => o.TestType == TestType.Sample).OrderBy(p => p.Rack).ThenBy(q => q.Position).ToList();

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
