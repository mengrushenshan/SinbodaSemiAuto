using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class LISCommunicationSettingViewModel : NavigationViewModelBase
    {
        LISCommunicationSettingBusiness lisBusiness = new LISCommunicationSettingBusiness();
        /// <summary>
        /// 
        /// </summary>
        public LISCommunicationSettingViewModel()
        {
            if (DesignHelper.IsInDesignMode) return;

            InitItemsSource();
            GetCurrentModuleInfoList();
            SaveCommand = new RelayCommand(SaveMethod);
        }
        #region 属性
        private bool _LisEnabled = false;
        /// <summary>
        /// LIS是否启用
        /// </summary>
        public bool LisEnabled
        {
            get { return _LisEnabled; }
            set { Set(ref _LisEnabled, value); }
        }

        private string _MachineID;
        /// <summary>
        /// 仪器编号
        /// </summary>
        public string MachineID
        {
            get { return _MachineID; }
            set { Set(ref _MachineID, value); }
        }

        private string _LisID;
        /// <summary>
        /// LIS主机编号
        /// </summary>
        public string LisID
        {
            get { return _LisID; }
            set { Set(ref _LisID, value); }
        }

        private bool _IsNetwork = true;
        /// <summary>
        /// 网络连接
        /// </summary>
        public bool IsNetwork
        {
            get { return _IsNetwork; }
            set
            {
                Set(ref _IsNetwork, value);
                if (_IsNetwork)
                    IsSerialPort = false;
            }
        }

        private bool _IsSerialPort = false;
        /// <summary>
        /// 串口连接
        /// </summary>
        public bool IsSerialPort
        {
            get { return _IsSerialPort; }
            set
            {
                Set(ref _IsSerialPort, value);
                if (_IsSerialPort)
                    IsNetwork = false;
            }
        }

        #region 网口属性
        private string _NetworkIP;
        /// <summary>
        /// IP地址
        /// </summary>
        public string NetworkIP
        {
            get { return _NetworkIP; }
            set { Set(ref _NetworkIP, value); }
        }

        private int _NetworkPort;
        /// <summary>
        /// 端口号
        /// </summary>
        public int NetworkPort
        {
            get { return _NetworkPort; }
            set { Set(ref _NetworkPort, value); }
        }
        #endregion

        #region 串口属性
        private List<DataDictionaryInfoDetail> _SerialPortIDItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 串口号列表
        /// </summary>
        public List<DataDictionaryInfoDetail> SerialPortIDItemsSource
        {
            get { return _SerialPortIDItemsSource; }
            set { Set(ref _SerialPortIDItemsSource, value); }
        }
        private DataDictionaryInfoDetail _SerialPortID = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的串口号
        /// </summary>
        public DataDictionaryInfoDetail SerialPortID
        {
            get { return _SerialPortID; }
            set { Set(ref _SerialPortID, value); }
        }

        private List<DataDictionaryInfoDetail> _BaudRateItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 波特率列表
        /// </summary>
        public List<DataDictionaryInfoDetail> BaudRateItemsSource
        {
            get { return _BaudRateItemsSource; }
            set { Set(ref _BaudRateItemsSource, value); }
        }
        private DataDictionaryInfoDetail _BaudRate = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的波特率
        /// </summary>
        public DataDictionaryInfoDetail BaudRate
        {
            get { return _BaudRate; }
            set { Set(ref _BaudRate, value); }
        }
        private List<DataDictionaryInfoDetail> _DataBitItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 数据位列表
        /// </summary>
        public List<DataDictionaryInfoDetail> DataBitItemsSource
        {
            get { return _DataBitItemsSource; }
            set { Set(ref _DataBitItemsSource, value); }
        }
        private DataDictionaryInfoDetail _DataBit = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的数据位
        /// </summary>
        public DataDictionaryInfoDetail DataBit
        {
            get { return _DataBit; }
            set { Set(ref _DataBit, value); }
        }
        private List<DataDictionaryInfoDetail> _StopBitItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 停止位列表
        /// </summary>
        public List<DataDictionaryInfoDetail> StopBitItemsSource
        {
            get { return _StopBitItemsSource; }
            set { Set(ref _StopBitItemsSource, value); }
        }
        private DataDictionaryInfoDetail _StopBit = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的停止位
        /// </summary>
        public DataDictionaryInfoDetail StopBit
        {
            get { return _StopBit; }
            set { Set(ref _StopBit, value); }
        }
        private List<DataDictionaryInfoDetail> _CheckBitItemsSource = new List<DataDictionaryInfoDetail>();
        /// <summary>
        /// 校验位列表
        /// </summary>
        public List<DataDictionaryInfoDetail> CheckBitItemsSource
        {
            get { return _CheckBitItemsSource; }
            set { Set(ref _CheckBitItemsSource, value); }
        }
        private DataDictionaryInfoDetail _CheckBit = new DataDictionaryInfoDetail();
        /// <summary>
        /// 选中的校验位
        /// </summary>
        public DataDictionaryInfoDetail CheckBit
        {
            get { return _CheckBit; }
            set { Set(ref _CheckBit, value); }
        }
        #endregion

        /// <summary>
        /// 波特率字符串
        /// </summary>
        string[] listBaudRate = { "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "38400", "56000", "57600", "115200", "128000", "256000" };
        /// <summary>
        /// 停止位字符串
        /// </summary>
        string[] listStopType = { "1", "1.5", "2" };
        /// <summary>
        /// 校验位字符串
        /// </summary>
        string[] listCheckType = { "None", "Odd", "Even", "Mark", "Space" };
        #endregion


        /// <summary>
        /// 初始化资源信息
        /// </summary>
        private void InitItemsSource()
        {
            //数据位绑定
            for (int i = 5; i < 9; i++)
            {
                var itemdic = new DataDictionaryInfoDetail()
                {
                    Code = i.ToString(),
                    Values = i.ToString(),
                };
                DataBitItemsSource.Add(itemdic);
            }

            //停止位绑定
            for (int i = 0; i < 3; i++)
            {
                var itemdic = new DataDictionaryInfoDetail()
                {
                    Code = listStopType[i],
                    Values = listStopType[i],
                };
                StopBitItemsSource.Add(itemdic);
            }

            //校验位绑定
            for (int i = 0; i < 5; i++)
            {
                var itemdic = new DataDictionaryInfoDetail()
                {
                    Code = i.ToString(),
                    Values = listCheckType[i],
                };
                CheckBitItemsSource.Add(itemdic);
            }

            //波特率绑定
            for (int i = 0; i < 15; i++)
            {
                var itemdic = new DataDictionaryInfoDetail()
                {
                    Code = listBaudRate[i],
                    Values = listBaudRate[i],
                };
                BaudRateItemsSource.Add(itemdic);
            }

            string[] PortNames = SerialPort.GetPortNames();

            //串口号绑定
            for (int i = 0; i < PortNames.Length; i++)
            {
                var itemdic = new DataDictionaryInfoDetail()
                {
                    Code = PortNames[i],
                    Values = PortNames[i],
                };
                SerialPortIDItemsSource.Add(itemdic);
            }
        }

        /// <summary>
        /// 修改LIS方法
        /// </summary>
        private void ModuleInfoSaveMethod()
        {
            if (ValidationCurrentModuleInfo())
            {
                LISCommunicationInterfaceModel model = new LISCommunicationInterfaceModel();
                model.LISEnabled = LisEnabled;
                model.MachineID = MachineID;
                model.LISID = LisID;
                model.IsNetWork = IsNetwork;
                model.NetworkIP = NetworkIP;
                model.NetworkPort = NetworkPort;

                model.SerialPort = SerialPortID.Code;
                model.BaudRate = BaudRate.Code;
                model.StopType = StopBit.Code;
                model.CheckType = CheckBit.Code;
                model.DataType = DataBit.Code;
                OperationResult result = lisBusiness.SetLISSettingInfo(model);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    ShowMessageComplete(SystemResources.Instance.LanguageArray[609]);//29提示609保存成功
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Message))
                        ShowMessageError(result.Message);
                    else
                        ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
                }
            }
        }

        /// <summary>
        /// 校验LIS设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationCurrentModuleInfo()
        {
            if (LisEnabled && string.IsNullOrEmpty(MachineID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6440]); //"仪器ID不能为空！
                return false;
            }
            if (LisEnabled && string.IsNullOrEmpty(LisID))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6441]); //"LIS主机ID不能为空！
                return false;
            }
            if (IsNetwork && string.IsNullOrEmpty(NetworkIP))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2828]); //"IP地址不能为空！
                return false;
            }
            if (IsNetwork && string.IsNullOrEmpty(NetworkPort.ToString()))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[2829]); //"端口不能为空！
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取LIS信息
        /// </summary>
        private void GetCurrentModuleInfoList()
        {
            OperationResult<LISCommunicationInterfaceModel> result = lisBusiness.GetLISSettingInfo();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                if (result.Results != null)
                {
                    LisEnabled = result.Results.LISEnabled;
                    MachineID = result.Results.MachineID;
                    LisID = result.Results.LISID;
                    IsNetwork = result.Results.IsNetWork;
                    IsSerialPort = !result.Results.IsNetWork;
                    NetworkIP = result.Results.NetworkIP;
                    NetworkPort = result.Results.NetworkPort;

                    var port = SerialPortIDItemsSource.FirstOrDefault(p => p.Code == result.Results.SerialPort);
                    if (port != null)
                        SerialPortID = port;// SerialPortIDItemsSource.FirstOrDefault(p => p.Code == result.Results.SerialPort);
                    else
                        SerialPortID = SerialPortIDItemsSource.FirstOrDefault();
                    BaudRate = BaudRateItemsSource.FirstOrDefault(p => p.Code == result.Results.BaudRate);
                    StopBit = StopBitItemsSource.FirstOrDefault(p => p.Code == result.Results.StopType);
                    CheckBit = CheckBitItemsSource.FirstOrDefault(p => p.Code == result.Results.CheckType);
                    DataBit = DataBitItemsSource.FirstOrDefault(p => p.Code == result.Results.DataType);
                }
                else
                {
                    LisEnabled = false;
                    MachineID = string.Empty;
                    LisID = string.Empty;
                    IsNetwork = true;
                    IsSerialPort = false;
                    NetworkIP = "192.168.100.100";
                    NetworkPort = 5000;

                    SerialPortID = SerialPortIDItemsSource.FirstOrDefault();
                    BaudRate = BaudRateItemsSource.FirstOrDefault();
                    StopBit = StopBitItemsSource.FirstOrDefault();
                    CheckBit = CheckBitItemsSource.FirstOrDefault();
                    DataBit = DataBitItemsSource.FirstOrDefault();
                }
            }
            else
                ShowMessageError(result.Message);
        }
        public RelayCommand SaveCommand { get; set; }
        /// <summary>
        /// 保存方法
        /// </summary>
        private void SaveMethod()
        {
            ModuleInfoSaveMethod();
        }
    }
}
