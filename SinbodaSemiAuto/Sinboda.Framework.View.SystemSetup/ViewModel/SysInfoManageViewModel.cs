using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Business.SystemSetup;
using Sinboda.Framework.Common.CommonFunc;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.View.SystemSetup.Win;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sinboda.Framework.View.SystemSetup.ViewModel
{
    /// <summary>
    /// 信息管理界面逻辑实现类
    /// </summary>
    public class SysInfoManageViewModel : NavigationViewModelBase
    {
        /// <summary>
        /// 系统信息业务逻辑接口
        /// </summary>
        SystemInfoBusiness business = new SystemInfoBusiness();
        /// <summary>
        /// 构造函数
        /// </summary>   
        public SysInfoManageViewModel()
        {
            PageTitle = SystemResources.Instance.LanguageArray[1458];
            InitializeDataDicTypeCommand();
            InitializeDataDicCommand();
        }

        public void Init()
        {
            GetDataDictionaryType();
        }

        #region 基础信息类型设置
        #region 属性
        private List<DataDictionaryTypeModel> _DataDictionaryTypeList = new List<DataDictionaryTypeModel>();
        /// <summary>
        ///基础信息类型列表
        /// </summary>
        public List<DataDictionaryTypeModel> DataDictionaryTypeList
        {
            get { return _DataDictionaryTypeList; }
            set { Set(ref _DataDictionaryTypeList, value); }
        }
        private DataDictionaryTypeModel _SelectDataDictionaryType = new DataDictionaryTypeModel();
        /// <summary>
        /// 基础信息类型表选中项
        /// </summary>
        public DataDictionaryTypeModel SelectDataDictionaryType
        {
            get { return _SelectDataDictionaryType; }
            set
            {
                Set(ref _SelectDataDictionaryType, value);
                if (SelectDataDictionaryType != null)
                {
                    EntityTypeModel = CloneExtends.DeepCloneObject(SelectDataDictionaryType);
                    IsShowHotKey = SelectDataDictionaryType.IsSetHotKey == true ? Visibility.Visible : Visibility.Hidden;
                    IsGridShowHotKey = SelectDataDictionaryType.IsSetHotKey;
                    IsShowDefault = SelectDataDictionaryType.IsSetDefault == true ? Visibility.Visible : Visibility.Hidden;
                    IsGridShowDefault = SelectDataDictionaryType.IsSetDefault;
                    IsShowDepartmentList = SelectDataDictionaryType.IsSetParentCode == true ? Visibility.Visible : Visibility.Hidden;
                    IsGridShowDepartmentList = SelectDataDictionaryType.IsSetParentCode;
                    if (IsGridShowDepartmentList)
                        GetParentNameList();
                    MaxLengthValue = SelectDataDictionaryType.MaxLengthValue;
                    RegexTextValue = SelectDataDictionaryType.RegexTextValue;
                    IsCanDelete = SelectDataDictionaryType.IsChildCanDelete;
                }
                else
                {
                    EntityTypeModel = new DataDictionaryTypeModel();
                    IsShowDepartmentList = Visibility.Hidden;
                    IsGridShowDepartmentList = false;
                    IsShowHotKey = Visibility.Hidden;
                    IsGridShowHotKey = false;
                    IsShowDefault = Visibility.Hidden;
                    IsGridShowDefault = false;

                    MaxLengthValue = 50;
                    RegexTextValue = string.Empty;
                    IsCanDelete = true;
                }
                GetDataDictionaryInfo();
            }
        }
        private List<DataDictionaryTypeModel> _DataDictionaryTypeAllList = new List<DataDictionaryTypeModel>();
        /// <summary>
        ///基础信息类型列表
        /// </summary>
        public List<DataDictionaryTypeModel> DataDictionaryTypeAllList
        {
            get { return _DataDictionaryTypeAllList; }
            set { Set(ref _DataDictionaryTypeAllList, value); }
        }
        private DataDictionaryTypeModel _SelectType = new DataDictionaryTypeModel();
        /// <summary>
        /// 基础信息类型表选中项
        /// </summary>
        public DataDictionaryTypeModel SelectType
        {
            get { return _SelectType; }
            set
            {
                Set(ref _SelectType, value);
                List<DataDictionaryTypeModel> listtmp = CloneExtends.DeepCloneList(DataDictionaryTypeAllList).ToList();
                if (SelectType != null)
                    listtmp.Remove(listtmp.FirstOrDefault(o => o.Id == SelectType.Id));
                listtmp.Insert(0, new DataDictionaryTypeModel());
                ParentTypeList = listtmp;
                if (SelectType != null)
                    TypeModel = CloneExtends.DeepCloneObject(SelectType);
                else
                    TypeModel = new DataDictionaryTypeModel();
            }
        }
        private DataDictionaryTypeModel typeModel = new DataDictionaryTypeModel();
        /// <summary>
        /// 新增类型实体
        /// </summary>
        public DataDictionaryTypeModel TypeModel
        {
            get { return typeModel; }
            set { Set(ref typeModel, value); }
        }
        private List<DataDictionaryTypeModel> parentTypeList = new List<DataDictionaryTypeModel>();
        /// <summary>
        ///基础信息类型列表
        /// </summary>
        public List<DataDictionaryTypeModel> ParentTypeList
        {
            get { return parentTypeList; }
            set { Set(ref parentTypeList, value); }
        }
        private DataDictionaryTypeModel entityTypeModel = new DataDictionaryTypeModel();
        /// <summary>
        /// 新增类型实体
        /// </summary>
        public DataDictionaryTypeModel EntityTypeModel
        {
            get { return entityTypeModel; }
            set { Set(ref entityTypeModel, value); }
        }
        #endregion

        #region 命令
        private void InitializeDataDicTypeCommand()
        {
            AddTypeCommand = new RelayCommand(AddTypeMethod);
            InsertDataDicTypeCommand = new RelayCommand(InsertDataDicTypeMethod);
            UpdateDataDicTypeCommand = new RelayCommand(UpdateDataDicTypeMethod);
            DeleteDataDicTypeCommand = new RelayCommand(DeleteDataDicTypeMethod);
        }
        /// <summary>
        /// 添加类型命令
        /// </summary>
        public RelayCommand AddTypeCommand { get; set; }
        /// <summary>
        /// 添加类型方法
        /// </summary>
        private void AddTypeMethod()
        {
            DataDictionayTypeSetWin win = new DataDictionayTypeSetWin();
            win.DataContext = this;
            List<DataDictionaryTypeModel> listtmp = CloneExtends.DeepCloneList(DataDictionaryTypeAllList).ToList();
            listtmp.Insert(0, new DataDictionaryTypeModel());
            ParentTypeList = listtmp;
            win.ShowDialog();
            GetDataDictionaryType();
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertDataDicTypeCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertDataDicTypeMethod()
        {
            if (ValidationDataDictionaryType())
            {
                if (DataDictionaryTypeAllList.Where(p => p.TypeCode == TypeModel.TypeCode).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6448]);// "编码已经存在请重新填写"
                    return;
                }
                if (DataDictionaryTypeAllList.Where(p => p.TypeValues == TypeModel.TypeValues).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写
                    return;
                }
                if (DataDictionaryTypeAllList.Where(p => p.ShowOrder == TypeModel.ShowOrder).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6413]); //"显示顺序已经存在请重新填写
                    return;
                }
                DataDictionaryTypeModel model = new DataDictionaryTypeModel();
                model.TypeCode = TypeModel.TypeCode;
                model.LanguageID = TypeModel.LanguageID;
                model.TypeValues = TypeModel.TypeValues.TrimStart().TrimEnd();
                model.IsSetHotKey = TypeModel.IsSetHotKey;
                model.IsSetDefault = TypeModel.IsSetDefault;
                model.IsEnable = TypeModel.IsEnable;
                model.ShowOrder = TypeModel.ShowOrder;
                model.ParentCode = TypeModel.ParentCode;
                model.IsSetParentCode = string.IsNullOrEmpty(TypeModel.ParentCode) == true ? false : true;
                model.IsChildCanDelete = TypeModel.IsChildCanDelete;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectType = new DataDictionaryTypeModel();
                    GetDataDictionaryType();
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[30], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[6379], model.TypeValues), SystemResources.Instance.LanguageArray[6413]);//29提示609保存成功 //TODO 翻译
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
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateDataDicTypeCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateDataDicTypeMethod()
        {
            if (SelectType == null || string.IsNullOrEmpty(SelectType.TypeCode))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            if (ValidationDataDictionaryType())
            {
                if (DataDictionaryTypeAllList.Where(p => p.TypeCode == TypeModel.TypeCode).Count() > 0 &&
                    SelectType.TypeCode != TypeModel.TypeCode)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6448]); //"编码已经存在请重新填写"); //TODO 翻译
                    return;
                }
                if (DataDictionaryTypeAllList.Where(p => p.TypeValues == TypeModel.TypeValues).Count() > 0 &&
                    SelectType.TypeValues != TypeModel.TypeValues)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写"); //TODO 翻译
                    return;
                }
                if (DataDictionaryTypeAllList.Where(p => p.ShowOrder == TypeModel.ShowOrder).Count() > 0 &&
                   SelectType.ShowOrder != TypeModel.ShowOrder)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6413]); //"显示顺序已经存在请重新填写"); //TODO 翻译
                    return;
                }
                DataDictionaryTypeModel model = new DataDictionaryTypeModel();
                model.Id = SelectType.Id;
                model.TypeCode = TypeModel.TypeCode;
                model.LanguageID = TypeModel.LanguageID;
                model.TypeValues = TypeModel.TypeValues.TrimStart().TrimEnd();
                model.IsSetHotKey = TypeModel.IsSetHotKey;
                model.IsSetDefault = TypeModel.IsSetDefault;
                model.IsEnable = TypeModel.IsEnable;
                model.ShowOrder = TypeModel.ShowOrder;
                model.ParentCode = TypeModel.ParentCode;
                model.IsSetParentCode = string.IsNullOrEmpty(TypeModel.ParentCode) == true ? false : true;
                model.IsChildCanDelete = TypeModel.IsChildCanDelete;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    SelectType = new DataDictionaryTypeModel();
                    GetDataDictionaryType();
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[32], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[6379], model.TypeValues), "基础信息设置");//29提示609保存成功 //TODO 翻译
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
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteDataDicTypeCommand { get; set; }
        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteDataDicTypeMethod()
        {
            if (SelectType == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！
                return;
            }

            DataDictionaryTypeModel model = SelectType;
            OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Delete);
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                SelectType = new DataDictionaryTypeModel();
                GetDataDictionaryType();
                WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[33], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[6379], model.TypeValues), "基础信息设置");//29提示609保存成功 //TODO 翻译
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Message))
                    ShowMessageError(result.Message);
                else
                    ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
            }
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 校验基础信息类型设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationDataDictionaryType()
        {
            if (string.IsNullOrEmpty(TypeModel.TypeCode))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6420]); //"请填写编码"); 翻译
                return false;
            }
            if (string.IsNullOrEmpty(TypeModel.TypeValues))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6421]); //"请填写名称"); 译
                return false;
            }
            if (string.IsNullOrEmpty(TypeModel.LanguageIDString))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6422]); //"请填写正确的语言编号"); 
                return false;
            }
            try
            {
                if (Convert.ToInt32(TypeModel.LanguageIDString) < 1)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6422]); //"请填写正确的语言编号"); 
                    return false;
                }
            }
            catch
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6422]); //"请填写正确的语言编号"); 
                return false;
            }
            if (string.IsNullOrEmpty(TypeModel.ShowOrderString))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6412]); //"请填写正确的显示顺序"); 
                return false;
            }
            try
            {
                if (Convert.ToInt32(TypeModel.ShowOrderString) < 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6412]); //"请填写正确的显示顺序"); 
                    return false;
                }
            }
            catch
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6412]); //"请填写正确的显示顺序"); 
                return false;
            }
            TypeModel.LanguageID = Convert.ToInt32(TypeModel.LanguageIDString);
            TypeModel.ShowOrder = Convert.ToInt32(TypeModel.ShowOrderString);
            return true;
        }
        /// <summary>
        /// 获取基础信息类型
        /// </summary>
        private void GetDataDictionaryType()
        {
            OperationResult<List<DataDictionaryTypeModel>> result = business.GetDataDictionaryTypeList();
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                SetDataDictionaryTypeString(result.Results);
                DataDictionaryService.Instance.InitializeBusinessDictionary();
                DataDictionaryTypeList = new List<DataDictionaryTypeModel>(result.Results.Where(o => o.IsEnable).OrderBy(o => o.ShowOrder));
                DataDictionaryTypeAllList = new List<DataDictionaryTypeModel>(result.Results.OrderBy(o => o.ShowOrder));
                if (DataDictionaryTypeList.Count > 0)
                    SelectDataDictionaryType = DataDictionaryTypeList[0];
            }
        }
        private void SetDataDictionaryTypeString(List<DataDictionaryTypeModel> typeList)
        {
            foreach (var item in typeList)
            {
                item.LanguageIDString = item.LanguageID.ToString();
                item.ShowOrderString = item.ShowOrder.ToString();
            }
        }
        #endregion
        #endregion

        #region 基础信息信息设置
        #region 属性
        private List<DataDictionaryInfoModel> _DataDictionaryInfoList = new List<DataDictionaryInfoModel>();
        /// <summary>
        /// 基础信息信息列表
        /// </summary>
        public List<DataDictionaryInfoModel> DataDictionaryInfoList
        {
            get { return _DataDictionaryInfoList; }
            set { Set(ref _DataDictionaryInfoList, value); }
        }
        private DataDictionaryInfoModel _SelectDataDictionaryInfo = new DataDictionaryInfoModel();
        /// <summary>
        /// 基础信息信息选中项
        /// </summary>
        public DataDictionaryInfoModel SelectDataDictionaryInfo
        {
            get { return _SelectDataDictionaryInfo; }
            set
            {
                Set(ref _SelectDataDictionaryInfo, value);

                if (_SelectDataDictionaryInfo != null)
                {
                    DataDicCode = _SelectDataDictionaryInfo.Code;
                    DataDicValue = _SelectDataDictionaryInfo.DisplayValues;
                    Hotkey = SelectDataDictionaryType.IsSetHotKey == true ? _SelectDataDictionaryInfo.HotKey : string.Empty;
                    IsDefault = SelectDataDictionaryType.IsSetDefault == true ? _SelectDataDictionaryInfo.IsDefault : false;
                    //IsEnable = SelectDataDictionaryType.IsEnable == true ? _SelectDataDictionaryInfo.IsEnable : false;
                    IsEnable = true;
                    ShowOrder = _SelectDataDictionaryInfo.ShowOrder.ToString();
                    SelectParentName = SelectDataDictionaryType.IsSetParentCode == true ? ParentNameList.FirstOrDefault(o => o.Id == _SelectDataDictionaryInfo.ParentCode) : null;
                }
                else
                {
                    //先赋值，触发控件事件，再赋值string.Empty
                    DataDicCode = "1";
                    DataDicCode = string.Empty;
                    DataDicValue = " ";
                    DataDicValue = string.Empty;
                    Hotkey = " ";
                    Hotkey = string.Empty;
                    IsDefault = false;
                    IsEnable = true;
                    ShowOrder = string.Empty;
                    SelectParentName = null;
                }
            }
        }

        private string _DataDicCode;
        /// <summary>
        /// 编码
        /// </summary>
        public string DataDicCode
        {
            get { return _DataDicCode; }
            set { Set(ref _DataDicCode, value); }
        }
        private string _DataDicValue;
        /// <summary>
        /// 基础信息名称
        /// </summary>
        public string DataDicValue
        {
            get { return _DataDicValue; }
            set { Set(ref _DataDicValue, value); }
        }
        private Visibility isShowHotKey = Visibility.Hidden;
        /// <summary>
        /// 是否显示助记符
        /// </summary>
        public Visibility IsShowHotKey
        {
            get { return isShowHotKey; }
            set { Set(ref isShowHotKey, value); }
        }
        private bool isGridShowHotKey;
        /// <summary>
        /// 表格是否显示助记符
        /// </summary>
        public bool IsGridShowHotKey
        {
            get { return isGridShowHotKey; }
            set { Set(ref isGridShowHotKey, value); }
        }
        private string hotkey;
        /// <summary>
        /// 助记符属性
        /// </summary>
        public string Hotkey
        {
            get { return hotkey; }
            set { Set(ref hotkey, value); }
        }
        private Visibility isShowDefault = Visibility.Hidden;
        /// <summary>
        /// 是否显示默认值
        /// </summary>
        public Visibility IsShowDefault
        {
            get { return isShowDefault; }
            set { Set(ref isShowDefault, value); }
        }
        private bool isGridShowDefault;
        /// <summary>
        /// 表格是否显示默认值
        /// </summary>
        public bool IsGridShowDefault
        {
            get { return isGridShowDefault; }
            set { Set(ref isGridShowDefault, value); }
        }
        private bool isEnable;
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable
        {
            get { return isEnable; }
            set { Set(ref isEnable, value); }
        }
        private string showOrder;
        /// <summary>
        /// 显示顺序
        /// </summary>
        public string ShowOrder
        {
            get { return showOrder; }
            set { Set(ref showOrder, value); }
        }
        private bool isDefault;
        /// <summary>
        /// 是否为默认值
        /// </summary>
        public bool IsDefault
        {
            get { return isDefault; }
            set { Set(ref isDefault, value); }
        }
        private Visibility isShowDepartmentList = Visibility.Hidden;
        /// <summary>
        /// 是否显示所属科室
        /// </summary>
        public Visibility IsShowDepartmentList
        {
            get { return isShowDepartmentList; }
            set { Set(ref isShowDepartmentList, value); }
        }
        private bool isGridShowDepartmentList;
        /// <summary>
        /// 表格是否显示所属上级
        /// </summary>
        public bool IsGridShowDepartmentList
        {
            get { return isGridShowDepartmentList; }
            set { Set(ref isGridShowDepartmentList, value); }
        }
        private List<DataDictionaryInfoModel> parentNameList = new List<DataDictionaryInfoModel>();
        /// <summary>
        /// 上级列表属性
        /// </summary>
        public List<DataDictionaryInfoModel> ParentNameList
        {
            get { return parentNameList; }
            set { Set(ref parentNameList, value); }
        }
        private DataDictionaryInfoModel selectParentName;
        /// <summary>
        /// 选中的上级
        /// </summary>
        public DataDictionaryInfoModel SelectParentName
        {
            get { return selectParentName; }
            set { Set(ref selectParentName, value); }
        }

        private int maxLengthValue;
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLengthValue
        {
            get { return maxLengthValue; }
            set
            {
                Set(ref maxLengthValue, value);
                if (DataDicValue != null && DataDicValue.Length > MaxLengthValue)
                    DataDicValue = DataDicValue.Substring(0, MaxLengthValue);
            }
        }

        private string regexTextValue;
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegexTextValue
        {
            get { return regexTextValue; }
            set { Set(ref regexTextValue, value); }
        }
        private bool isCanDelete;
        /// <summary>
        /// 子项是否可以删除
        /// </summary>
        public bool IsCanDelete
        {
            get { return isCanDelete; }
            set { Set(ref isCanDelete, value); }
        }
        #endregion

        #region 命令
        private void InitializeDataDicCommand()
        {
            InsertDataDicCommand = new RelayCommand(InsertDataDicMethod);
            UpdateDataDicCommand = new RelayCommand(UpdateDataDicMethod);
            DeleteDataDicCommand = new RelayCommand(DeleteDataDicMethod);

            TopwardCommand = new RelayCommand(TopwardMethod);
            BottomwardCommand = new RelayCommand(BottomwardMethod);
            BeforewardCommand = new RelayCommand(BeforewardMethod);
            NextwardCommand = new RelayCommand(NextwardMethod);
        }
        /// <summary>
        /// 添加命令
        /// </summary>
        public RelayCommand InsertDataDicCommand { get; set; }
        /// <summary>
        /// 添加方法
        /// </summary>
        private void InsertDataDicMethod()
        {
            DataDicCode = DataDicCode.TrimStart().TrimStart();
            DataDicValue = DataDicValue.TrimStart().TrimEnd();
            Hotkey = Hotkey.TrimStart().TrimEnd();

            if (ValidationDataDictionary())
            {

                if (DataDictionaryInfoList.Where(p => p.Code == DataDicCode).Count() > 0)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6448]);//"编码已经存在请重新填写"); //TODO 翻译
                    return;
                }
                if (DataDictionaryInfoList.Where(p => p.Values == DataDicValue).Count() > 0)
                {
                    if (!string.IsNullOrEmpty(SelectDataDictionaryType.ParentCode))
                    {
                        if (DataDictionaryInfoList.Where(p => p.Values == DataDicValue && p.ParentCode == SelectParentName.Id).Count() > 0)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写
                            return;
                        }
                    }
                    else
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写"); //TODO 翻译
                        return;
                    }
                }
                if (Hotkey == DataDicValue || DataDictionaryInfoList.Where(p => p.HotKey == DataDicValue).Count() > 0)//防止输入名称时按助记符进行查询
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6452]); //"名称与助记符重复请重新填写"); //TODO 翻译
                    return;
                }
                if (SelectDataDictionaryType.IsSetHotKey)
                {
                    if (DataDictionaryInfoList.Where(p => p.HotKey == Hotkey).Count() > 0)
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6453]); //"助记符已经存在请重新填写"); //TODO 翻译
                        return;
                    }
                    if (Hotkey == DataDicValue || DataDictionaryInfoList.Where(p => p.Values == Hotkey).Count() > 0)//防止输入名称时按助记符进行查询
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6454]); //"名称与助记符重复请重新填写"); //TODO 翻译
                        return;
                    }
                }
                DataDictionaryInfoModel existModel = null;
                if (SelectDataDictionaryType.IsSetDefault && IsDefault && DataDictionaryInfoList.Where(p => p.IsDefault == true).Count() > 0)
                {
                    existModel = DataDictionaryInfoList.Where(p => p.IsDefault == true).FirstOrDefault();
                    //ShowMessageWarning(SystemResources.Instance.LanguageArray[6455]); //"默认值已经存在请重新填写"); //TODO 翻译
                    //return;
                }
                if (!string.IsNullOrEmpty(SelectDataDictionaryType.ParentCode) && (SelectParentName == null || SelectParentName.Id == Guid.Empty))
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[74]); //"请输入科室名称"); //TODO 翻译
                    return;
                }
                DataDictionaryInfoModel model = new DataDictionaryInfoModel();
                model.Code = DataDicCode;
                model.Values = DataDicValue;
                model.LanguageID = 0;
                model.HotKey = Hotkey.ToLower();
                model.IsDefault = IsDefault;
                model.IsEnable = true;
                model.ShowOrder = DataDictionaryInfoList.LastOrDefault() != null ? DataDictionaryInfoList.LastOrDefault().ShowOrder + 1 : 1;
                model.ParentCode = SelectParentName == null ? Guid.Empty : SelectParentName.Id;
                model.CodeGroupID = SelectDataDictionaryType.Id;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Add);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    UpdateDataDicIsDefault(existModel);
                    GetDataDictionaryInfo();
                    SelectDataDictionaryInfo = new DataDictionaryInfoModel();
                    if (SelectDataDictionaryType.LanguageID > SystemResources.Instance.LanguageArray.Length)
                    {
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[30], SystemResources.Instance.LanguageArray[3966], SelectDataDictionaryType.TypeValues, model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                    }
                    else
                    {
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[30], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[SelectDataDictionaryType.LanguageID], model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                    }
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
        /// 更新IsEnable
        /// </summary>
        /// <param name="existModel"></param>
        public void UpdateDataDicIsEnable()
        {
            if (SelectDataDictionaryInfo != null)
            {
                DataDictionaryInfoModel model = new DataDictionaryInfoModel();
                model.Id = SelectDataDictionaryInfo.Id;
                model.LanguageID = SelectDataDictionaryInfo.LanguageID;
                model.Code = SelectDataDictionaryInfo.Code;
                model.Values = SelectDataDictionaryInfo.Values;
                model.HotKey = SelectDataDictionaryInfo.HotKey;
                //如果设置IsEnable==false，那么IsDefault也是false 2020.03.12
                model.IsEnable = SelectDataDictionaryInfo.IsEnable;
                model.IsDefault = !SelectDataDictionaryInfo.IsEnable ? false : SelectDataDictionaryInfo.IsDefault;
                model.ShowOrder = SelectDataDictionaryInfo.ShowOrder;
                model.ParentCode = SelectDataDictionaryInfo.ParentCode;
                model.CodeGroupID = SelectDataDictionaryInfo.CodeGroupID;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                Guid guid = SelectDataDictionaryInfo == null ? Guid.Empty : SelectDataDictionaryInfo.Id;
                GetDataDictionaryInfo();

                if (DataDictionaryInfoList.Count > 0)
                {
                    var item = DataDictionaryInfoList.FirstOrDefault(o => o.Id == guid);
                    SelectDataDictionaryInfo = item == null ? DataDictionaryInfoList[0] : item;
                }

                IsEnable = SelectDataDictionaryInfo.IsEnable;
                IsDefault = SelectDataDictionaryInfo.IsDefault;
            }
        }
        public void UpdateDataDicIsDefault()
        {
            if (SelectDataDictionaryInfo != null)
            {
                if (SelectDataDictionaryInfo.IsDefault)
                {
                    List<DataDictionaryInfoModel> existModels = null;
                    if (SelectDataDictionaryType.IsSetDefault && DataDictionaryInfoList.Where(p => p.IsDefault == true).Count() > 0)
                    {
                        existModels = DataDictionaryInfoList.Where(p => p.IsDefault == true).ToList();
                    }
                    foreach (var item in existModels)
                    {
                        DataDictionaryInfoModel modelOther = new DataDictionaryInfoModel();
                        modelOther.Id = item.Id;
                        modelOther.Code = item.Code;
                        modelOther.LanguageID = item.LanguageID;
                        modelOther.Values = item.Values;
                        modelOther.HotKey = item.HotKey;
                        modelOther.IsDefault = !item.IsDefault;
                        modelOther.IsEnable = item.IsEnable;
                        modelOther.ShowOrder = item.ShowOrder;
                        modelOther.ParentCode = item.ParentCode;
                        modelOther.CodeGroupID = item.CodeGroupID;
                        OperationResult resultOther = business.OperateT(modelOther, Core.ModelsOperation.OperationEnum.Modify);
                    }
                }
                DataDictionaryInfoModel model = new DataDictionaryInfoModel();
                model.Id = SelectDataDictionaryInfo.Id;
                model.Code = SelectDataDictionaryInfo.Code;
                model.LanguageID = SelectDataDictionaryInfo.LanguageID;
                model.Values = SelectDataDictionaryInfo.Values;
                model.HotKey = SelectDataDictionaryInfo.HotKey;
                model.IsDefault = SelectDataDictionaryInfo.IsDefault;
                model.IsEnable = SelectDataDictionaryInfo.IsEnable;
                model.ShowOrder = SelectDataDictionaryInfo.ShowOrder;
                model.ParentCode = SelectDataDictionaryInfo.ParentCode;
                model.CodeGroupID = SelectDataDictionaryInfo.CodeGroupID;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                IsDefault = SelectDataDictionaryInfo.IsDefault;

                Guid guid = SelectDataDictionaryInfo == null ? Guid.Empty : SelectDataDictionaryInfo.Id;

                GetDataDictionaryInfo();

                if (DataDictionaryInfoList.Count > 0)
                {
                    var item = DataDictionaryInfoList.FirstOrDefault(o => o.Id == guid);
                    SelectDataDictionaryInfo = item == null ? DataDictionaryInfoList[0] : item;
                }
            }
        }
        /// <summary>
        /// 更新IsDefault
        /// </summary>
        /// <param name="existModel"></param>
        public void UpdateDataDicIsDefault(DataDictionaryInfoModel existModel)
        {
            if (existModel != null)
            {
                DataDictionaryInfoModel model = new DataDictionaryInfoModel();
                model.Id = existModel.Id;
                model.Code = existModel.Code;
                model.Values = existModel.Values;
                model.HotKey = existModel.HotKey;
                model.IsDefault = false;
                model.IsEnable = existModel.IsEnable;
                model.ShowOrder = existModel.ShowOrder;
                model.ParentCode = existModel.ParentCode;
                model.CodeGroupID = existModel.CodeGroupID;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
            }
        }
        /// <summary>
        /// 修改命令
        /// </summary>
        public RelayCommand UpdateDataDicCommand { get; set; }
        /// <summary>
        /// 修改方法
        /// </summary>
        private void UpdateDataDicMethod()
        {
            if (SelectDataDictionaryInfo == null || string.IsNullOrEmpty(SelectDataDictionaryInfo.Values))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]); //"未选择任何数据，无法执行操作！"); 
                return;
            }

            DataDicCode = DataDicCode.TrimStart().TrimStart();
            DataDicValue = DataDicValue.TrimStart().TrimEnd();
            Hotkey = Hotkey.TrimStart().TrimEnd();

            if (ValidationDataDictionary())
            {

                if (DataDictionaryInfoList.Where(p => p.Code == DataDicCode).Count() > 0 && SelectDataDictionaryInfo.Code != DataDicCode)
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6448]); //"编码已经存在请重新填写"); 
                    return;
                }

                if (SelectDataDictionaryInfo.ParentCode == Guid.Empty)
                {
                    if (DataDictionaryInfoList.Where(p => p.DisplayValues == DataDicValue && SelectDataDictionaryInfo.Id != p.Id).Count() > 0)
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写"); 
                        return;
                    }
                }
                else
                {
                    if (DataDictionaryInfoList.Where(p => p.DisplayValues == DataDicValue && p.ParentCode == SelectParentName.Id
                            && SelectDataDictionaryInfo.Id != p.Id).Count() > 0)
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6449]); //"名称已经存在请重新填写"); 
                        return;
                    }
                }


                if (Hotkey == DataDicValue || DataDictionaryInfoList.Where(p => p.HotKey == DataDicValue).Count() > 0)//防止输入名称时按助记符进行查询
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[6452]); //"名称与助记符重复请重新填写"); 
                    return;
                }
                if (SelectDataDictionaryType.IsSetHotKey)
                {
                    if (DataDictionaryInfoList.Where(p => p.HotKey == Hotkey).Count() > 0 &&
                        SelectDataDictionaryInfo.HotKey != Hotkey)
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6453]);// "助记符已经存在请重新填写"); //TODO 翻译
                        return;
                    }
                    if (DataDictionaryInfoList.Where(p => p.Values == Hotkey).Count() > 0 || Hotkey == DataDicValue)//防止输入名称时按助记符进行查询
                    {
                        ShowMessageWarning(SystemResources.Instance.LanguageArray[6454]); //"名称与助记符重复请重新填写"); //TODO 翻译
                        return;
                    }
                }
                DataDictionaryInfoModel existModel = null;
                if (SelectDataDictionaryType.IsSetDefault && IsDefault && DataDictionaryInfoList.Where(p => p.IsDefault == true && p.Id != SelectDataDictionaryInfo.Id).Count() > 0)
                {
                    existModel = DataDictionaryInfoList.Where(p => p.IsDefault == true && p.Id != SelectDataDictionaryInfo.Id).FirstOrDefault();
                    //ShowMessageWarning(SystemResources.Instance.LanguageArray[6455]); //"默认值已经存在请重新填写"); //TODO 翻译
                    //return;
                }

                if (!string.IsNullOrEmpty(SelectDataDictionaryType.ParentCode) && (SelectParentName == null || selectParentName.Id == Guid.Empty))
                {
                    ShowMessageWarning(SystemResources.Instance.LanguageArray[74]);// "请输入科室名称"); //TODO 翻译
                    return;
                }

                DataDictionaryInfoModel model = new DataDictionaryInfoModel();
                model.Id = SelectDataDictionaryInfo.Id;
                model.Code = DataDicCode;
                model.Values = DataDicValue;
                model.LanguageID = (SelectDataDictionaryInfo.DisplayValues != DataDicValue ? 0 : SelectDataDictionaryInfo.LanguageID);
                model.HotKey = Hotkey.ToLower();
                model.IsDefault = IsDefault;
                model.IsEnable = true;
                model.ShowOrder = SelectDataDictionaryInfo.ShowOrder;
                model.ParentCode = SelectParentName == null ? Guid.Empty : SelectParentName.Id;
                model.CodeGroupID = SelectDataDictionaryType.Id;
                OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Modify);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    UpdateDataDicIsDefault(existModel);
                    GetDataDictionaryInfo();
                    SelectDataDictionaryInfo = new DataDictionaryInfoModel();

                    if (SelectDataDictionaryType.LanguageID > SystemResources.Instance.LanguageArray.Length)
                    {
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[32], SystemResources.Instance.LanguageArray[3966], SelectDataDictionaryType.TypeValues, model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                    }
                    else
                    {
                        WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[32], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[SelectDataDictionaryType.LanguageID], model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                    }
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
        /// 删除命令
        /// </summary>
        public RelayCommand DeleteDataDicCommand { get; set; }
        /// <summary>
        /// 删除方法
        /// </summary>
        private void DeleteDataDicMethod()
        {
            if (SelectDataDictionaryInfo == null || string.IsNullOrEmpty(SelectDataDictionaryInfo.Values))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6386]);// "未选择任何数据，无法执行操作！"); //TODO 翻译
                return;
            }

            if (!EnableDeleteDataDicInfo())
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[508]);// "选择项目包含子项目，不能够删除！");
                return;
            }

            // 中间件提出删除增加确认提示。 sunch 20210817
            if (NotificationService.Instance.ShowMessage(SystemResources.Instance.LanguageArray[224], MessageBoxButton.YesNo, SinMessageBoxImage.Question) == MessageBoxResult.No)
                return;

            DataDictionaryInfoModel model = SelectDataDictionaryInfo;
            int showOrder = model.ShowOrder;
            int index = DataDictionaryInfoList.IndexOf(SelectDataDictionaryInfo);
            List<DataDictionaryInfoModel> others = new List<DataDictionaryInfoModel>();
            if (index != DataDictionaryInfoList.Count - 1)
            {
                for (int i = index + 1; i < DataDictionaryInfoList.Count; i++)
                {
                    others.Add(DataDictionaryInfoList[i]);
                }
            }
            OperationResult result = business.OperateT(model, Core.ModelsOperation.OperationEnum.Delete);
            if (result.ResultEnum == OperationResultEnum.SUCCEED)
            {
                if (others.Count > 0)
                {
                    business.ExchangeDeleteOrder(showOrder, others);
                }
                //SelectDataDictionaryInfo = new DataDictionaryInfoModel();
                GetDataDictionaryInfo();
                SelectDataDictionaryInfo = new DataDictionaryInfoModel();

                if (SelectDataDictionaryType.LanguageID > SystemResources.Instance.LanguageArray.Length)
                {
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[33], SystemResources.Instance.LanguageArray[3966], SelectDataDictionaryType.TypeValues, model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                }
                else
                {
                    WriteOperateLog(string.Format(SystemResources.Instance.LanguageArray[33], SystemResources.Instance.LanguageArray[3966], SystemResources.Instance.LanguageArray[SelectDataDictionaryType.LanguageID], model.Values), "基础信息设置");//29提示609保存成功 //TODO 翻译
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Message))
                    ShowMessageError(result.Message);
                else
                    ShowMessageError(SystemResources.Instance.LanguageArray[2546]);//29提示2546数据错误!
            }
        }

        private bool EnableDeleteDataDicInfo()
        {
            if (SelectDataDictionaryInfo == null || string.IsNullOrEmpty(SelectDataDictionaryInfo.Values))
            {
                return false;
            }

            if (SelectDataDictionaryInfo.ParentCode != Guid.Empty)
            {
                return true;
            }

            if (HasChildDataDictionaryInfo(SelectDataDictionaryInfo.Id))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 置顶命令
        /// </summary>
        public RelayCommand TopwardCommand { get; set; }
        private void TopwardMethod()
        {
            if (SelectDataDictionaryInfo != null)
            {
                if (SelectDataDictionaryInfo.ShowOrder != 0)
                {
                    if (SelectDataDictionaryInfo.ShowOrder != 1)
                    {
                        int index = DataDictionaryInfoList.IndexOf(SelectDataDictionaryInfo);
                        OperationResult result = business.ExchangeOrderTop(SelectDataDictionaryInfo);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6405]);// "置顶失败"); //TODO 翻译
                        }
                        else
                        {
                            GetDataDictionaryInfo();
                            //SelectDataDictionaryInfo = DataDictionaryInfoList[index - 1];
                        }

                    }
                    else
                    {
                        //ShowMessageWarning("不可以置顶"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以置顶"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以置顶"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 置底命令
        /// </summary>
        public RelayCommand BottomwardCommand { get; set; }
        private void BottomwardMethod()
        {
            if (SelectDataDictionaryInfo != null)
            {
                if (SelectDataDictionaryInfo.ShowOrder != 0)
                {
                    if (SelectDataDictionaryInfo.ShowOrder != DataDictionaryInfoList.Count)
                    {
                        OperationResult result = business.ExchangeOrderBottom(SelectDataDictionaryInfo);
                        int index = DataDictionaryInfoList.IndexOf(SelectDataDictionaryInfo);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6406]);// "置底失败"); //TODO 翻译
                        }
                        else
                        {
                            GetDataDictionaryInfo();
                            //SelectDataDictionaryInfo = DataDictionaryInfoList[index];
                        }

                    }
                    else
                    {
                        //ShowMessageWarning("不可以置底"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以置底"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以置底"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 向上命令
        /// </summary>
        public RelayCommand BeforewardCommand { get; set; }
        private void BeforewardMethod()
        {
            if (SelectDataDictionaryInfo != null)
            {
                if (SelectDataDictionaryInfo.ShowOrder != 0)
                {
                    if (SelectDataDictionaryInfo.ShowOrder != 1)
                    {
                        int index = DataDictionaryInfoList.IndexOf(SelectDataDictionaryInfo);
                        OperationResult result = business.ExchangeOrder(SelectDataDictionaryInfo, DataDictionaryInfoList[index - 1]);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                        {
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6407]);// "向上失败"); //TODO 翻译
                        }
                        else
                        {
                            GetDataDictionaryInfo();
                            SelectDataDictionaryInfo = DataDictionaryInfoList[index - 1];
                        }

                    }
                    else
                    {
                        //ShowMessageWarning("不可以向上"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以向上"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以向上"); //TODO 翻译
                return;
            }
        }
        /// <summary>
        /// 向下命令
        /// </summary>
        public RelayCommand NextwardCommand { get; set; }
        private void NextwardMethod()
        {
            if (SelectDataDictionaryInfo != null)
            {
                if (SelectDataDictionaryInfo.ShowOrder != 0)
                {
                    if (SelectDataDictionaryInfo.ShowOrder != DataDictionaryInfoList.Count())
                    {
                        int index = DataDictionaryInfoList.IndexOf(SelectDataDictionaryInfo);
                        OperationResult result = business.ExchangeOrder(SelectDataDictionaryInfo, DataDictionaryInfoList[index + 1]);
                        if (result.ResultEnum == OperationResultEnum.FAILED)
                            ShowMessageWarning(SystemResources.Instance.LanguageArray[6408]); //"向下失败"); //TODO 翻译
                        else
                        {
                            GetDataDictionaryInfo();
                            SelectDataDictionaryInfo = DataDictionaryInfoList[index + 1];
                        }
                    }
                    else
                    {
                        //ShowMessageWarning("不可以向下"); //TODO 翻译
                        return;
                    }
                }
                else
                {
                    //ShowMessageWarning("不可以向下"); //TODO 翻译
                    return;
                }
            }
            else
            {
                //ShowMessageWarning("不可以向下"); //TODO 翻译
                return;
            }
        }

        public void DoubleClickSelectTypeItem()
        {
            if (null != SelectDataDictionaryType)
            {
                // 数据字典自定义类型，必须以custom为键值，eg:custom1 自定义1  custom2 自定义2
                // 尿分产线暂定如此对应 
                if (SelectDataDictionaryType.TypeCode.ToUpper().Contains("CUSTOM"))
                {
                    Guid recordId = SelectDataDictionaryType.Id;

                    Messenger.Default.Send<object>(SelectDataDictionaryType, "DictionaryTypeChange");

                    Init();
                    SelectDataDictionaryType = DataDictionaryTypeAllList.Find(elem => elem.Id == recordId);
                }
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 校验基础信息设置
        /// </summary>
        /// <returns></returns>
        private bool ValidationDataDictionary()
        {
            if (SelectDataDictionaryType == null || SelectDataDictionaryType.Id == Guid.Empty)
            {
                ShowMessageWarning(StringResourceExtension.GetLanguage(146, "请选择类型后进行操作")); //"请选择类型后进行操作"); //TODO 翻译
                return false;
            }

            if (string.IsNullOrEmpty(DataDicCode))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6420]); //"请填写编码"); //TODO 翻译
                return false;
            }
            if (string.IsNullOrEmpty(DataDicValue))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6421]); //"请填写名称"); //TODO 翻译
                return false;
            }
            if (SelectDataDictionaryType.IsSetHotKey && string.IsNullOrEmpty(Hotkey))
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[6457]); //"请填写助记符"); //TODO 翻译
                return false;
            }
            if (SelectDataDictionaryType.IsSetParentCode && SelectParentName == null)
            {
                ShowMessageWarning(SystemResources.Instance.LanguageArray[74]); //"请输入科室名称"); //TODO 翻译
                return false;
            }
            return true;
        }
        private bool HasChildDataDictionaryInfo(Guid dataId)
        {
            if (Guid.Empty == dataId)
            {
                return false;
            }

            OperationResult<List<DataDictionaryInfoModel>> result = business.GetChildDataDictionaryInfoList(dataId);
            if (result.ResultEnum == OperationResultEnum.SUCCEED && null != result.Results)
            {
                return result.Results.Count > 0;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取基础信息信息
        /// </summary>
        private void GetDataDictionaryInfo()
        {
            if (SelectDataDictionaryType != null)
            {
                OperationResult<List<DataDictionaryInfoModel>> result = business.GetDataDictionaryInfoList(SelectDataDictionaryType.Id);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    DataDictionaryService.Instance.InitializeBusinessDictionary();
                    Guid guid = SelectDataDictionaryInfo == null ? Guid.Empty : SelectDataDictionaryInfo.Id;

                    List<DataDictionaryInfoModel> dictInfoList = new List<DataDictionaryInfoModel>(result.Results.OrderBy(o => o.ShowOrder));
                    if (SelectDataDictionaryType.IsSetParentCode)
                    {
                        foreach (var item in dictInfoList)
                        {
                            var temp = ParentNameList.Find(o => o.Id == item.ParentCode);
                            item.ParentName = temp != null ? temp.Values : "";
                        }
                    }

                    if (dictInfoList.Count > 0)
                    {
                        DataDictionaryInfoList = dictInfoList;
                        var item = dictInfoList.FirstOrDefault(o => o.Id == guid);
                        SelectDataDictionaryInfo = item == null ? dictInfoList[0] : item;
                    }
                    else
                    {
                        SelectDataDictionaryInfo = null;
                        DataDictionaryInfoList = new List<DataDictionaryInfoModel>();
                    }
                }
            }
            else
                DataDictionaryInfoList = new List<DataDictionaryInfoModel>();
        }
        /// <summary>
        /// 获取所属上级列表
        /// </summary>
        private void GetParentNameList()
        {
            if (SelectDataDictionaryType != null)
            {

                Guid id = DataDictionaryTypeList.FirstOrDefault(o => o.TypeCode == SelectDataDictionaryType.ParentCode).Id;
                OperationResult<List<DataDictionaryInfoModel>> result = business.GetDataDictionaryInfoList(id);
                if (result.ResultEnum == OperationResultEnum.SUCCEED)
                {
                    ParentNameList = new List<DataDictionaryInfoModel>(result.Results);
                    // ParentNameList.Insert(0, new DataDictionaryInfoModel());
                }
            }
            else
                ParentNameList = new List<DataDictionaryInfoModel>();
        }
        #endregion
        #endregion
    }
}
