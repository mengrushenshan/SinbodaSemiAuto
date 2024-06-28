using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sinboda.Framework.Common;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.View.Results.WinView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Sinboda.SemiAuto.View.Results.ViewModel
{
    public class ResultQueryPageViewModel : NavigationViewModelBase
    {
        #region 数据
        /// <summary>
        /// 样本查询条件
        /// </summary>
        public Expression<Func<Sin_Sample, bool>> SampleExp = null;

        /// <summary>
        /// 患者信息查询条件
        /// </summary>
        public Expression<Func<Sin_Patient, bool>> PatientExp = null;

        /// <summary>
        /// 样本源
        /// </summary>
        private List<Sin_Sample> sampleSource;
        public List<Sin_Sample> SampleSource
        {
            get { return sampleSource; }
            set { Set(ref sampleSource, value); }
        }

        private Sin_Sample selectSample;
        public Sin_Sample SelectSample
        {
            get { return selectSample; }
            set 
            { 
                Set(ref selectSample, value);
                if (selectSample != null)
                {
                    SetTestResultList();
                }
            }
        }

        private List<Sin_Test_Result> testResultSource;
        public List<Sin_Test_Result> TestResultSource
        {
            get { return testResultSource; }
            set { Set(ref testResultSource, value);}
        }

        private Sin_Test_Result selectTestResult;
        public Sin_Test_Result SelectTestResult
        {
            get { return selectTestResult; }
            set { 
                Set(ref selectTestResult, value);
                SetImagePath();
            }
        }

        private string imagePath1;
        public string ImagePath1
        {
            get { return imagePath1; }
            set { Set(ref imagePath1, value); }
        }

        private string imagePath2;
        public string ImagePath2
        {
            get { return imagePath2; }
            set { Set(ref imagePath2, value); }
        }

        private string imagePath3;
        public string ImagePath3
        {
            get { return imagePath3; }
            set { Set(ref imagePath3, value); }
        }

        private string imagePath4;
        public string ImagePath4
        {
            get { return imagePath4; }
            set { Set(ref imagePath4, value); }
        }

        private string imagePath5;
        public string ImagePath5
        {
            get { return imagePath5; }
            set { Set(ref imagePath5, value); }
        }

        private string imagePath6;
        public string ImagePath6
        {
            get { return imagePath6; }
            set { Set(ref imagePath6, value); }
        }

        private string imagePath7;
        public string ImagePath7
        {
            get { return imagePath7; }
            set { Set(ref imagePath7, value); }
        }

        private string imagePath8;
        public string ImagePath8
        {
            get { return imagePath8; }
            set { Set(ref imagePath8, value); }
        }

        private string imagePath9;
        public string ImagePath9
        {
            get { return imagePath9; }
            set { Set(ref imagePath9, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 样本删除
        /// </summary>
        public RelayCommand SampleDeleteCommand { get; set; }

        /// <summary>
        /// 患者信息
        /// </summary>
        public RelayCommand PatientInfoCommand { get; set; }

        #endregion
        public ResultQueryPageViewModel() 
        {
            // UI线程同步
            DispatcherHelper.Initialize();

            PatientInfoCommand = new RelayCommand(PatientInfo);
            SampleDeleteCommand = new RelayCommand(SampleDelete);
        }

        private void InitResultQueryPage()
        {

            SampleSource = SampleBusiness.Instance.GetSampleListToday();
            if (SampleSource != null)
            {
                SelectSample = SampleSource[0];
            }
            else
            {
                SelectSample = null;
                TestResultSource = null;
                selectTestResult = null;
            }
        }

        private void SetTestResultList()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (SelectSample == null)
                {
                    return;
                }

                TestResultSource = TestResultBusiness.Instance.GetTestResultListBySampleId(SelectSample.Id);

                if (TestResultSource != null)
                {
                    SelectTestResult = TestResultSource[0];
                }
            });
        }

        /// <summary>
        /// 样本删除
        /// </summary>
        private void SampleDelete()
        {
            SampleDeleteWindow win = new SampleDeleteWindow();
            if (win.ShowDialog() == true)
            {
                InitResultQueryPage();
            }
        }

        /// <summary>
        /// 患者信息
        /// </summary>
        private void PatientInfo()
        {

        }

        private void SetImagePath()
        {
            if (SelectTestResult == null || SelectSample == null)
            {
                return;
            }
            string samplePath = MapPath.TifPath + "Result\\" + $"{SelectTestResult.Test_file_name}\\";
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ImagePath1 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_1.jpg";
                ImagePath2 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_2.jpg";
                ImagePath3 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_3.jpg";
                ImagePath4 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_4.jpg";
                ImagePath5 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_5.jpg";
                ImagePath6 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_6.jpg";
                ImagePath7 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_7.jpg";
                ImagePath8 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_8.jpg";
                ImagePath9 = samplePath + $"{SelectSample.RackDish}_{SelectSample.Position}_9.jpg";
            });
        }

        private void SetImageShowAttr(string imagePath, Visibility imageVisibility)
        {
            if (File.Exists(imagePath))
            {
                imageVisibility = Visibility.Visible;
            }
            else
            {
                imageVisibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// 进入页面时触发
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnParameterChanged(object parameter)
        {
            InitResultQueryPage();
        }

        /// <summary>
        /// 离开页面时触发
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="navigationState"></param>
        /// <returns></returns>
        protected override bool NavigatedFrom(object source, NavigationMode mode, object navigationState)
        {
            return base.NavigatedFrom(source, mode, navigationState);
        }
    }
}
