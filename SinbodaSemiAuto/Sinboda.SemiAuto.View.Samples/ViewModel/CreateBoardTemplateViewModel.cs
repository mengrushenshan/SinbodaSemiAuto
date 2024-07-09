using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class CreateBoardTemplateViewModel : NavigationViewModelBase
    {
        private List<Sin_BoardTemplate> BoardTemplateList;

        /// <summary>
        /// 模板名称
        /// </summary>
        private string templateName;
        public string TemplateName
        {
            get { return templateName; }
            set { Set(ref templateName, value); }
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        public RelayCommand<Window> CreateCommand { get; set; }

        public CreateBoardTemplateViewModel(List<Sin_BoardTemplate> boardTemplateList) 
        {
            BoardTemplateList = boardTemplateList;

            CreateCommand = new RelayCommand<Window>(CreateTemplate);
        }

        private void CreateTemplate(Window win)
        {
            var tempList = BoardTemplateBusiness.Instance.GetBoardList(TemplateName);
            if (tempList != null && tempList.Count != 0)
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "方案名已经存在"), MessageBoxButton.OK, SinMessageBoxImage.Information);
                return;
            }
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (var boardTemplate in BoardTemplateList)
                {
                    boardTemplate.Id = Guid.NewGuid();
                    boardTemplate.TemplateName = TemplateName;
                    boardTemplate.Create_user = "Sinboda";
                    boardTemplate.Create_time = DateTime.Now;
                }

                if (BoardTemplateBusiness.Instance.CreateTemplateList(BoardTemplateList))
                {
                    win.DialogResult = true;
                }
            });
            
            
        }
    }
}
