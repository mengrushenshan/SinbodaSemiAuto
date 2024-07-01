using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.View.Results.ViewModel
{
    public class ResultImageViewModel : NavigationViewModelBase
    {

        private Sin_Sample SinSample;

        private int imageIndex;
        public int ImageIndex
        {
            get { return imageIndex; }
            set { Set(ref imageIndex, value); }
        }

        private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { Set(ref filePath, value); }
        }

        private bool beforEnable;
        public bool BeforEnable
        {
            get { return beforEnable; }
            set { Set(ref beforEnable, value); }
        }

        private bool nextEnable;
        public bool NextEnable
        {
            get { return nextEnable; }
            set { Set(ref nextEnable, value); }
        }
        #region 命令

        /// <summary>
        /// 前一张
        /// </summary>
        public RelayCommand BeforCommand { get; set; }

        /// <summary>
        /// 后一张
        /// </summary>
        public RelayCommand NextCommand { get; set; }

        #endregion
        public ResultImageViewModel(Sin_Sample sample, int imagePos) 
        {
            ImageIndex = imagePos;
            SinSample = sample;

            BeforCommand = new RelayCommand(BeforImage);
            NextCommand = new RelayCommand(NextImage);

            SetNextEnable();
            SetBeforEnable();
            SetFilePath();
        }

        private void SetFilePath()
        {
            string samplePath = MapPath.TifPath + "Result\\" + $"{SinSample.TestResult.Test_file_name}\\";
            FilePath = samplePath + $"{SinSample.RackDish}_{SinSample.Position}_{ImageIndex}.jpg";
        }

        private void SetNextEnable()
        {
            if(ImageIndex >= 9)
                NextEnable = false;
            else
                NextEnable = true;
        }

        private void SetBeforEnable()
        {
            if (ImageIndex <= 1)
                BeforEnable = false;
            else
                BeforEnable = true;
        }

        public void NextImage()
        {
            if (ImageIndex >= 9)
                return;

            ImageIndex++;

            SetFilePath();

            SetBeforEnable();
            SetNextEnable();
        }

        public void BeforImage()
        {
            if (ImageIndex <= 1)
                return;

            ImageIndex--;
            SetFilePath();
            SetBeforEnable();
            SetNextEnable();
        }
    }
}
