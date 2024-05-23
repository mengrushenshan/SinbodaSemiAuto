using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.XtraExport.Helpers.TagTableCell;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;
using Sinboda.SemiAuto.View.Samples.WinView;
using Sinboda.SemiAuto.Business.Items;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SamplesRegisterPageViewModel : NavigationViewModelBase
    {
        #region 数据
        /// <summary>
        /// 三排孔位
        /// </summary>
        private List<int> rackSouce = new List<int>() { 1, 2, 3 };
        public List<int> RackSouce
        {
            get { return rackSouce; }
            set { Set(ref rackSouce, value); }
        }

        /// <summary>
        /// 十个孔位
        /// </summary>
        private List<int> posSouce = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<int> PosSouce
        {
            get { return posSouce; }
            set { Set(ref posSouce, value); }
        }

        /// <summary>
        /// 架号
        /// </summary>
        private int rackDish;

        public int RackDish
        {
            get { return rackDish; }
            set { Set(ref rackDish, value); }
        }

        /// <summary>
        /// 位置
        /// </summary>
        private int position;

        public int Position
        {
            get { return position; }
            set { Set(ref position, value); }
        }

        /// <summary>
        /// 样本号
        /// </summary>
        private int? sampleCode;
        public int? SampleCode
        {
            get { return sampleCode; }
            set { Set(ref sampleCode, value); }
        }

        /// <summary>
        /// 登记数量
        /// </summary>
        private int? count = 1;
        public int? Count
        {
            get { return count; }
            set { Set(ref count, value); }
        }

        /// <summary>
        /// 条码号
        /// </summary>
        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set { Set(ref barcode, value); }
        }
        /// <summary>
        /// 项目集合
        /// </summary>
        private List<string> sinItemSource;
        public List<string> SinItemSource
        {
            get { return sinItemSource; }
            set { Set(ref sinItemSource, value); }
        }

        private string selectItem;
        public string SelectItem
        {
            get { return selectItem; }
            set { Set(ref selectItem, value);}
        }
        #endregion

        #region 命令

        /// <summary>
        /// 登记命令
        /// </summary>
        public RelayCommand SampleRigesterCmd { get; set; }

        /// <summary>
        /// 样本删除
        /// </summary>
        public RelayCommand SampleDeleteCommand { get; set; }

        

        /// <summary>
        /// 释放位置
        /// </summary>
        public RelayCommand ResetCommand { get; set; }

        #endregion

        /// <summary>
        /// 孔位变化数据
        /// </summary>
        /// <param name="data">孔位号</param>
        public void HoleIndexChange(string data)
        {

        }

        public SamplesRegisterPageViewModel()
        {
            SampleRigesterCmd = new RelayCommand(SampleRigester);
            SampleDeleteCommand = new RelayCommand(SampleDelete);
            ResetCommand = new RelayCommand(Reset);

            InitSamplesRegisterPage();
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitSamplesRegisterPage()
        {
            SampleCode = SampleBusiness.Instance.GetMaxSampleCode();
            RackDish = RackSouce.Count > 0 ? RackSouce[0] : 1;
            Position = PosSouce.Count > 0 ? PosSouce[0] : 1;
            List<string> itemNameList = ItemBusiness.Instance.GetItemNames();
            if (itemNameList != null)
            {
                SinItemSource = itemNameList;
                SelectItem = itemNameList[0];
            }
            else
            {
                LogHelper.logSoftWare.Error("InitSamplesRegisterPage error not have item");
            }
        }

        private void SampleRigester()
        {
            if (SampleCode == null || SampleCode < 1 || SampleCode > 99999999)
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(2689, "样本号为空，请重新输入"), MessageBoxButton.OK, SinMessageBoxImage.Information);
                return;
            }

            if (Count != 1 && !string.IsNullOrEmpty(Barcode))
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(3751, "条码号不可重复，无法进行批量登记"), MessageBoxButton.OK, SinMessageBoxImage.Information);
                return;
            }

            Sin_Sample sin_Sample = SampleBusiness.Instance.SampleCodeHaveSample(SampleCode ?? 0);
            if (sin_Sample == null)
            {
                OperationResult or = new OperationResult();
                LoadingHelper.Instance.ShowLoadingWindow(a =>
                {

                    a.Title = SystemResources.Instance.GetLanguage(12495, "正在登记样本，请等待...");
                    or = SampleBusiness.Instance.CreateSample(SampleCode ?? 0, RackDish, Position, Barcode, Count ?? 0, SelectItem);

                }, 0, a =>
                {
                    if (!or.ResultBool)
                    {
                        NotificationService.Instance.ShowError(or.Message);
                        return;
                    }

                });
            }
            
        }

        private void SampleDelete()
        {
            SampleDeleteWindow win = new SampleDeleteWindow();
            if (win.ShowDialog() == true)
            { 

            }
        }

        private void Reset()
        {
            SampleCode = SampleBusiness.Instance.GetMaxSampleCode();
            RackDish = 1;
            Position = 1;
            Count = 1;
            Barcode = string.Empty;
        }
    }
}
