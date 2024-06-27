using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SampleDeleteViewModel : NavigationViewModelBase
    {
        #region 数据
        private string filterText;
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string FilterText
        {
            get { return filterText; }
            set
            {
                if (string.IsNullOrEmpty(value) || Regex.IsMatch(value, @"^[1-9]{1}[0-9]{0,3}((-{1})|(-{1}[1-9]{1}[0-9]{0,3}))?$"))
                    Set(ref filterText, value);
            }
        }
        #endregion

        #region 命令
        public RelayCommand<Window> DelCommand { get; set; }
        #endregion
        public SampleDeleteViewModel() 
        {
            DelCommand = new RelayCommand<Window>(Delete, GetIfDelete);
        }

        private bool IfDelete = true;
        private bool GetIfDelete(Window win)
        {
            return IfDelete;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="win"></param>
        public void Delete(Window win)
        {
            IfDelete = false;
            var codes = Utils.Utils.GetSampleCodeRange(FilterText);
            if (codes.Count != 2)
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(8760, "输入不符合规则"));
                IfDelete = true;
                return;
            }
            if (codes[0] > codes[1])
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(2691, "结束样本号小于开始样本号,请重新输入"));
                return;
            }

            //"确认删除吗？"
            if (NotificationService.Instance.ShowQuestion(SystemResources.Instance.GetLanguage(41, "确认删除吗？")) == MessageBoxResult.No)
            {
                IfDelete = true;
                return;
            }


            var beginData = DateTime.Now.Date;
            var endData = beginData.AddDays(1);

            OperationResult<List<string>> or = new OperationResult<List<string>>();
            LoadingHelper.Instance.ShowLoadingWindow(anc =>
            {
                anc.Title = SystemResources.Instance.GetLanguage(8761, "正在删除样本，请等待...");// "正在删除样本，请等待...";
                or = SampleBusiness.Instance.DeleteSampleAndResult(codes[0], codes[1], beginData, endData);
            }, 0, anc =>
            {
                IfDelete = true;
                if (or.ResultBool)
                {
                    if (or.Results.Count > 0)
                    {
                        // 部分成功
                        SinMessageBoxEx b = new SinMessageBoxEx(SystemResources.Instance.GetLanguage(11122, "注意"),
                            SystemResources.Instance.GetLanguage(13321, "删除成功。但有部分样本不符合要求，具体原因请查看详细信息。"), SinMessageBoxImage.Warning, or.Results);
                        b.ShowDialog();
                    }
                    else
                    {
                        // 全部成功
                        //NotificationService.Instance.ShowCompleted(SystemResources.Instance.GetLanguage(368, "成功"));
                    }
                    if (codes[0] == codes[1])
                    {
                        WriteOperateLog(SystemResources.Instance.GetLanguage(11226, "样本号: {0} 样本删除成功", codes[0].ToString()), "");

                    }
                    else
                    {
                        WriteOperateLog(SystemResources.Instance.GetLanguage(11227, "样本号: {0}-{1} 样本删除成功", codes[0].ToString(), codes[1].ToString()), "");
                    }
                }
                else
                {
                    if (or.Results.Count > 0)
                    {
                        SinMessageBoxEx b = new SinMessageBoxEx(SystemResources.Instance.GetLanguage(816, "错误"),
                            SystemResources.Instance.GetLanguage(13322, "删除失败。数据不符合要求，具体原因请查看详细信息。"), SinMessageBoxImage.Error, or.Results);
                        b.ShowDialog();
                    }
                    else
                    {
                        NotificationService.Instance.ShowError(or.Message);
                    }
                    return;
                }


                win.DialogResult = or.ResultBool;
                win.Close();

            });
        }
    }
}
