using DevExpress.Xpf.Docking;
using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.SemiAuto.View.Results.ViewModel
{
    public class ResultHistoryQueryViewModel : NavigationViewModelBase
    {
        private ResultQueryPageViewModel _ViewModel;

        #region 数据源

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 样本号
        /// </summary>
        public string SampleCode { get; set; }

        /// <summary>
        /// 条码号
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// 病历号
        /// </summary>
        public string MedicalNum { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }

        /// <summary>
        /// 病区
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 病房
        /// </summary>
        public string Ward { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string Bed { get; set; }

        /// <summary>
        /// 检验者
        /// </summary>
        public string TestDoctor { get; set; }

        /// <summary>
        /// 送检医生
        /// </summary>
        public string SendDoctor { get; set; }

        /// <summary>
        /// 收费类别
        /// </summary>
        public string ChargeType { get; set; }

        /// <summary>
        /// 就诊类别
        /// </summary>
        public string TreatmentType { get; set; }

        /// <summary>
        /// 临床诊断
        /// </summary>
        public string ClinicDiag {  get; set; }

        /// <summary>
        /// 民族数据源
        /// </summary>
        public List<DataDictionaryInfoModel> NationSource { get; set; }

        /// <summary>
        /// 收费类别
        /// </summary>
        public List<DataDictionaryInfoModel> ChargeSource { get; set; }

        /// <summary>
        /// 就诊类别
        /// </summary>
        public List<DataDictionaryInfoModel> TreatmentTypeSource { get; set; }

        /// <summary>
        /// 病区数据源
        /// </summary>
        public List<DataDictionaryInfoModel> SectionSource { get; set; }

        /// <summary>
        /// 病房数据源
        /// </summary>
        public List<DataDictionaryInfoModel> WardSource { get; set; }

        /// <summary>
        /// 送检科室数据源
        /// </summary>
        public List<DataDictionaryInfoModel> SendofficeSource { get; set; }

        private List<DataDictionaryInfoModel> senddoctorSource;
        /// <summary>
        /// 送检医生数据源
        /// </summary>
        public List<DataDictionaryInfoModel> SenddoctorSource
        {
            get { return senddoctorSource; }
            set { Set(ref senddoctorSource, value); }
        }

        /// <summary>
        /// 临床诊断数据源
        /// </summary>
        public List<DataDictionaryInfoModel> ClinicSource { get; set; }

        /// <summary>
        /// 备注数据源
        /// </summary>
        public List<DataDictionaryInfoModel> RemarkSource { get; set; }

        private string selectSendoffice;
        /// <summary>
        /// 选中科室
        /// </summary>
        public string SelectSendoffice
        {
            get { return selectSendoffice; }
            set
            {
                Set(ref selectSendoffice, value);
                OnSelectSendofficeChanged();
            }
        }
        #endregion

        #region 命令

        /// <summary>
        /// 查询
        /// </summary>
        public RelayCommand<Window> QueryCommand { get; set; }

        /// <summary>
        /// 恢复默认
        /// </summary>
        public RelayCommand DefaultCommand { get; set; }
        #endregion

        public ResultHistoryQueryViewModel(ResultQueryPageViewModel viewModel) 
        {
            QueryCommand = new RelayCommand<Window>(Query);
            DefaultCommand = new RelayCommand(Default);

            NationSource = InitDataDictionary("nation");
            ChargeSource = InitDataDictionary("charge");
            TreatmentTypeSource = InitDataDictionary("treatmentType");
            SectionSource = InitDataDictionary("section");
            WardSource = InitDataDictionary("ward");
            SendofficeSource = InitDataDictionary("sendoffice");
            SenddoctorSource = InitDataDictionary("senddoctor");
            ClinicSource = InitDataDictionary("clinic");
            RemarkSource = InitDataDictionary("remark");
        }

        private List<DataDictionaryInfoModel> InitDataDictionary(string key)
        {
            var result = new List<DataDictionaryInfoModel>();
            if (!DataDictionaryService.Instance.ListTypeAndInfo.ContainsKey(key))
                return result;

            result.Add(new DataDictionaryInfoModel { });
            var list = DataDictionaryService.Instance.ListTypeAndInfo[key];
            result.AddRange(list);
            return result;
        }

        /// <summary>
        /// 送检科室选中时
        /// </summary>
        private void OnSelectSendofficeChanged()
        {
            SenddoctorSource = new List<DataDictionaryInfoModel>();
            var selectItem = SendofficeSource.FirstOrDefault(o => o.Values == SelectSendoffice);
            if (selectItem == null)
                return;

            if (DataDictionaryService.Instance.ListTypeAndInfo.ContainsKey("senddoctor"))
            {
                SenddoctorSource = DataDictionaryService.Instance.ListTypeAndInfo["senddoctor"].Where(o => o.ParentCode == selectItem.Id).ToList();
            }
        }

        private void Query(Window win)
        {
            WriteOperateLog(SystemResources.Instance.GetLanguage(11131, "查询样本结果"), "");
            win.DialogResult = true;
        }

        private void Default()
        {
            SelectSendoffice = string.Empty;
            BeginDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            SampleCode = string.Empty;
            Barcode = string.Empty;
            MedicalNum = string.Empty;
            Name = string.Empty;
            Sex = string.Empty;
            Nation = string.Empty;
            Area = string.Empty;
            Ward = string.Empty;
            Bed = string.Empty;
            TestDoctor = string.Empty;
            SendDoctor = string.Empty;
            ChargeType = string.Empty;
            TreatmentType = string.Empty;
            ClinicDiag = string.Empty;
        }
    }
}
