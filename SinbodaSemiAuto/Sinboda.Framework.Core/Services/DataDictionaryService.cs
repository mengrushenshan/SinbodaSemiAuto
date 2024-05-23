using Microsoft.Practices.ServiceLocation;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common.ResourceExtensions;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.ModelsOperation;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Core.Services
{
    /// <summary>
    /// 配置信息初始化操作
    /// </summary>
    public class DataDictionaryService : TBaseSingleton<DataDictionaryService>
    {
        /// <summary>
        /// 基础信息类型及信息字典数据集合
        /// </summary>
        public Dictionary<string, List<DataDictionaryInfoModel>> ListTypeAndInfo { get; internal set; } = new Dictionary<string, List<DataDictionaryInfoModel>>();

        /// <summary>
        /// 返回系统基础信息
        /// </summary>
        public Dictionary<string, List<SystemTypeValueModel>> SystemTypeValueDictionary { get; internal set; } = new Dictionary<string, List<SystemTypeValueModel>>();

        /// <summary>
        /// 联机模块类型信息
        /// </summary>
        public Dictionary<int, ModuleTypeModel> ModuleTypeInfo { get; internal set; } = new Dictionary<int, ModuleTypeModel>();

        /// <summary>
        /// 联机模块信息
        /// </summary>
        public List<ModuleInfoModel> ModuleInfoList { get; internal set; } = new List<ModuleInfoModel>();

        /// <summary>
        /// 联机模块信息
        /// </summary>
        public Dictionary<int, List<ModuleInfoModel>> ModuleTypeAndInfo { get; internal set; } = new Dictionary<int, List<ModuleInfoModel>>();

        /// <summary>
        /// 所有模块版本信息（上位机、算法、下位机）
        /// </summary>
        public List<ModuleVersionModel> ModuleVersionInfos { get; internal set; } = new List<ModuleVersionModel>();

        /// <summary>
        /// 获得当前是否为多模块模式
        /// </summary>
        public bool GetIsMultiModuleMode { get; set; }

        /// <summary>
        /// 初始化版本信息表
        /// </summary>
        public void InitializeVersionInfo()
        {
            using (DBContextBase db = new DBContextBase())
            {
                if (db.VersionModel.AsNoTracking().Where(o => o.DBI_NAME == "DatabaseVersion").FirstOrDefault() != null)
                    return;
                #region 
                var databaseVersion = new VersionModel()
                {
                    Id = Guid.NewGuid(),
                    DBI_NAME = "DatabaseVersion",
                    DBI_VALUE = "1.0.0",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.VersionModel.Add(databaseVersion);

                var decimalSeparator = new VersionModel()
                {
                    Id = Guid.NewGuid(),
                    DBI_NAME = "DecimalSeparator",
                    DBI_VALUE = ".",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.VersionModel.Add(decimalSeparator);

                db.SaveChanges();
                #endregion
            }
        }

        /// <summary>
        /// 初始化字典资源
        /// </summary>
        public void InitializeDataDictionary()
        {
            InitializeModuleInfoDictionary();
            InitializeSystemTypeDictionary();
            InitializeBusinessDictionary();
        }
        /// <summary>
        /// 初始化联机模块配置信息资源
        /// </summary>
        public void InitializeModuleInfoDictionary()
        {
            ModuleTypeInfo.Clear();
            ModuleInfoList.Clear();
            ModuleTypeAndInfo.Clear();
            using (DBContextBase db = new DBContextBase())
            {
                var moduleList = db.ModuleTypeModel.OrderBy(o => o.ModuleTypeCode).ToList();
                var subModuleList = db.ModuleInfoModel.OrderBy(o => o.ModuleID).ToList();
                foreach (var item in moduleList)
                {
                    ModuleTypeInfo.Add(item.ModuleTypeCode, item);
                    var subModuleListTmp = subModuleList.Where(o => o.ModuleType == item.Id).OrderBy(o => o.ModuleID).ToList();
                    foreach (var subItem in subModuleListTmp)
                    {
                        subItem.ModuleTypeCode = item.ModuleTypeCode;
                        subItem.ModuleTypeName = item.ModuleTypeName;
                    }
                    ModuleInfoList.AddRange(subModuleListTmp);
                    ModuleTypeAndInfo.Add(item.ModuleTypeCode, subModuleListTmp);
                }
                if (ModuleTypeAndInfo.Count > 0)
                    GetIsMultiModuleMode = true;
                try
                {
                    /// 平台实现是否为多模块联机为true
                    /// 业务可以重新实现来决定是否为多模块联机
                    /// appconfig里面增加接口对应类的声明
                    var result = ServiceLocator.Current.GetInstance<IGetIsMultiModules>();
                    GetIsMultiModuleMode = result.GetIsMulti();
                }
                catch (Exception e)
                {
                    LogHelper.logSoftWare.Debug($"接口 MultiModules 未实现，是否联机使用默认值 {GetIsMultiModuleMode}");
                }
            }
        }
        /// <summary>
        /// 初始化系统资源信息
        /// </summary>
        public void InitializeSystemTypeDictionaryInfo()
        {
            using (SystemValueContext db = new SystemValueContext())
            {
                db.Database.ExecuteSqlCommand("DELETE FROM SYSTEM_TYPE;DELETE FROM SYSTEM_TYPE_VALUE");
                if (db.SystemTypeModel.AsNoTracking().Where(o => o.Code == "logType").FirstOrDefault() != null)
                    return;
                #region 日志类型、报警级别[图标]、系统报警级别、报警类别
                var logType = new SystemTypeModel()
                {
                    Id = Guid.NewGuid(),
                    Code = "logType",
                    Values = "日志类型",
                    Order = 1,
                    IsEnable = true,
                    LanguageID = "1951",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeModel.Add(logType);

                var alarmPicLevel = new SystemTypeModel()
                {
                    Id = Guid.NewGuid(),
                    Code = "alarmPicLevel",
                    Values = "报警级别[图标]",
                    Order = 2,
                    IsEnable = true,
                    LanguageID = "1702",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeModel.Add(alarmPicLevel);

                var alarmLevel = new SystemTypeModel()
                {
                    Id = Guid.NewGuid(),
                    Code = "alarmLevel",
                    Values = "报警级别",
                    Order = 3,
                    IsEnable = true,
                    LanguageID = "1702",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeModel.Add(alarmLevel);

                var alarmStyle = new SystemTypeModel()
                {
                    Id = Guid.NewGuid(),
                    Code = "alarmStyle",
                    Values = "报警类别",
                    Order = 4,
                    IsEnable = true,
                    LanguageID = "4387",
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeModel.Add(alarmStyle);

                db.SaveChanges();

                #region 日志
                var loginLog = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 1,
                    CodeGroupID = logType.Id,
                    LanguageID = 1952,
                    DisplayValue = SystemResources.Instance.LanguageArray[1952],//"登录日志",
                    Order = 1,
                    IsDefault = true,
                    IsEnable = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(loginLog);

                var operationLog = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 2,
                    CodeGroupID = logType.Id,
                    LanguageID = 27,
                    DisplayValue = SystemResources.Instance.LanguageArray[27],//"操作日志",
                    Order = 2,
                    IsDefault = true,
                    IsEnable = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(operationLog);
                #endregion

                #region 报警级别 图标使用
                var alarmPicNone = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 0,
                    CodeGroupID = alarmPicLevel.Id,
                    LanguageID = 0,
                    DisplayValue = "无报警",
                    Order = 1,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmPicNone);

                var alarmPicWarning = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 1,
                    CodeGroupID = alarmPicLevel.Id,
                    LanguageID = 4258,
                    DisplayValue = SystemResources.Instance.LanguageArray[4258],//"注意级别",
                    Order = 2,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmPicWarning);

                var alarmPicStop = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 2,
                    CodeGroupID = alarmPicLevel.Id,
                    LanguageID = 4256,
                    DisplayValue = SystemResources.Instance.LanguageArray[4256],//"停止级别",
                    Order = 3,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmPicStop);
                #endregion

                #region 系统报警级别
                var alarmAll = new SystemTypeValueModel()//全部
                {
                    Id = Guid.NewGuid(),
                    Code = 0,
                    CodeGroupID = alarmLevel.Id,
                    LanguageID = 1719,
                    DisplayValue = SystemResources.Instance.LanguageArray[1719],
                    Order = 1,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmAll);

                var alarmCaution = new SystemTypeValueModel()//注意级别
                {
                    Id = Guid.NewGuid(),
                    Code = 1,
                    CodeGroupID = alarmLevel.Id,
                    LanguageID = 4258,
                    DisplayValue = SystemResources.Instance.LanguageArray[4258],
                    Order = 2,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmCaution);

                var alarmSampleAdding = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 2,
                    CodeGroupID = alarmLevel.Id,
                    LanguageID = 67,
                    DisplayValue = SystemResources.Instance.LanguageArray[67],//"加样停止级别",
                    Order = 3,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmSampleAdding);

                var alarmStop = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 3,
                    CodeGroupID = alarmLevel.Id,
                    LanguageID = 4256,
                    DisplayValue = SystemResources.Instance.LanguageArray[4256],//"停止级别",
                    Order = 4,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmStop);

                var alarmDebug = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 4,
                    CodeGroupID = alarmLevel.Id,
                    LanguageID = 4259,
                    DisplayValue = SystemResources.Instance.LanguageArray[4259],//"调试级别",
                    Order = 5,
                    IsEnable = false,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmDebug);
                #endregion

                #region 报警类别
                var alarmStyleAll = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 0,
                    CodeGroupID = alarmStyle.Id,
                    LanguageID = 1719,
                    DisplayValue = SystemResources.Instance.LanguageArray[1719],//"全部",
                    Order = 1,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmStyleAll);

                var alarmStyleData = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 1,
                    CodeGroupID = alarmStyle.Id,
                    LanguageID = 1700,
                    DisplayValue = SystemResources.Instance.LanguageArray[1700], //"数据报警",
                    Order = 2,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmStyleData);

                var alarmStyleError = new SystemTypeValueModel()
                {
                    Id = Guid.NewGuid(),
                    Code = 2,
                    CodeGroupID = alarmStyle.Id,
                    LanguageID = 1701,
                    DisplayValue = SystemResources.Instance.LanguageArray[1701],//"故障报警",
                    Order = 3,
                    IsEnable = true,
                    IsDefault = true,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.SystemTypeValueModel.Add(alarmStyleError);
                #endregion

                db.SaveChanges();
                #endregion
            }
        }
        /// <summary>
        /// 初始化系统基础信息资源
        /// </summary>
        public void InitializeSystemTypeDictionary()
        {
            SystemTypeValueDictionary.Clear();
            using (SystemValueContext db = new SystemValueContext())
            {
                // 初始化系统基础信息
                var sysList = db.SystemTypeModel.Where(o => o.IsEnable).ToList();
                var subSysList = db.SystemTypeValueModel.Where(o => o.IsEnable).ToList();
                foreach (var item in sysList)
                {
                    var subListTmp = subSysList.Where(o => o.CodeGroupID == item.Id).OrderBy(o => o.Order).ToList();
                    foreach (var subItem in subListTmp)
                    {
                        subItem.DisplayValue = subItem.LanguageID != 0 ?
                            subItem.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[subItem.LanguageID] : subItem.DisplayValue :
                            subItem.DisplayValue;
                    }
                    SystemTypeValueDictionary.Add(item.Code, subListTmp);
                }
            }
        }
        /// <summary>
        /// 将系统级初始化资源与枚举进行相结合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal List<SystemTypeValue<T>> GetSystemTypeValueEnum<T>(string typeName) where T : struct
        {
            if (!SystemTypeValueDictionary.ContainsKey(typeName))
                return new List<SystemTypeValue<T>>();


            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new Exception(StringResourceExtension.GetLanguage(147, "类型'{0}'必须是枚举", enumType.ToString()));

            List<SystemTypeValue<T>> result = new List<SystemTypeValue<T>>();
            foreach (var item in SystemTypeValueDictionary[typeName])
            {
                if (!Enum.IsDefined(enumType, item.Code))
                    continue;

                SystemTypeValue<T> systemTypeValue = new SystemTypeValue<T>
                {
                    Code = item.Code,
                    CodeGroupID = item.CodeGroupID,
                    Value = (T)Enum.ToObject(enumType, item.Code),
                    DisplayValue = item.LanguageID != 0 ?
                                    item.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[item.LanguageID] : item.DisplayValue :
                                    item.DisplayValue,
                    Id = item.Id,
                    IsDefault = item.IsDefault,
                    IsEnable = item.IsEnable,
                    Order = item.Order,
                    Create_time = item.Create_time,
                    Create_user = item.Create_user
                };
                result.Add(systemTypeValue);
            }

            return result;
        }
        /// <summary>
        /// 初始化业务字典类型数据
        /// </summary>
        public void InitialzeBusinessDictionaryTypeInfo()
        {
            using (DBContextBase db = new DBContextBase())
            {
                if (db.DataDictionaryTypeModel.AsNoTracking().Where(o => o.TypeCode == "sendoffice").FirstOrDefault() != null)
                    return;

                #region 信息管理部分
                #region 类型
                var sendoffice = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 253,
                    TypeCode = "sendoffice",
                    TypeValues = "送检科室",
                    ShowOrder = 4,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(sendoffice);
                var senddoctor = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 252,
                    TypeCode = "senddoctor",
                    TypeValues = "送检医生",
                    ShowOrder = 5,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = true,
                    ParentCode = "sendoffice",
                    MaxLengthValue = 50,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(senddoctor);
                var nation = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 201,
                    TypeCode = "nation",
                    TypeValues = "民族",
                    ShowOrder = 6,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(nation);
                var section = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 102,
                    TypeCode = "section",
                    TypeValues = "病区",
                    ShowOrder = 7,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(section);
                var ward = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 101,
                    TypeCode = "ward",
                    TypeValues = "病房",
                    ShowOrder = 8,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(ward);
                var treatmentType = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 172,
                    TypeCode = "treatmentType",
                    TypeValues = "就诊类别",
                    ShowOrder = 9,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 30,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(treatmentType);
                var clinic = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 187,
                    TypeCode = "clinic",
                    TypeValues = "临床诊断",
                    ShowOrder = 10,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 100,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(clinic);
                var remark = new DataDictionaryTypeModel()
                {
                    Id = Guid.NewGuid(),
                    LanguageID = 57,
                    TypeCode = "remark",
                    TypeValues = "备注",
                    ShowOrder = 11,
                    IsSetHotKey = true,
                    IsSetDefault = true,
                    IsEnable = true,
                    IsSetParentCode = false,
                    MaxLengthValue = 100,
                    Create_user = "Sinboda",
                    Create_time = DateTime.Now
                };
                db.DataDictionaryTypeModel.Add(remark);
                db.SaveChanges();
                #endregion

                #endregion
            }
        }
        /// <summary>
        /// 初始化业务字典资源
        /// </summary>
        public void InitializeBusinessDictionary()
        {
            ListTypeAndInfo.Clear();
            using (DBContextBase db = new DBContextBase())
            {
                var list = db.DataDictionaryTypeModel.Where(o => o.IsEnable).OrderBy(o => o.ShowOrder).ToList();
                var subList = db.DataDictionaryInfoModel.ToList();
                foreach (var item in list)
                {
                    item.TypeValues = item.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[item.LanguageID] : item.TypeValues;
                    var subListTmp = subList.Where(o => o.CodeGroupID == item.Id).OrderBy(o => o.ShowOrder).ToList();
                    foreach (var subItem in subListTmp)
                    {
                        subItem.Values = subItem.LanguageID != 0 ?
                                        subItem.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[subItem.LanguageID] : subItem.Values :
                                        subItem.Values;
                    }
                    ListTypeAndInfo.Add(item.TypeCode, subListTmp);
                }
            }
        }

        /// <summary>
        /// 添加联机模块信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool AddModuleInfoDictionary(List<ModuleInfoModel> models)
        {
            int result = 0;
            using (DBContextBase db = new DBContextBase())
            {
                foreach (var model in models)
                {

                    model.Id = Guid.NewGuid();
                    db.Set<ModuleInfoModel>().Add(model);
                }
                result = db.SaveChanges();
            }
            InitializeModuleInfoDictionary();
            return result > 0;
        }
        /// <summary>
        /// 清空联机模块信息
        /// </summary>
        /// <returns></returns>
        public bool ClearModuleInfoDictionary()
        {
            int result = 0;
            using (DBContextBase db = new DBContextBase())
            {
                db.ModuleInfoModel.RemoveRange(db.ModuleInfoModel.ToList());
                result = db.SaveChanges();
            }
            InitializeModuleInfoDictionary();
            return result > 0;
        }

        /// <summary>
        /// 初始化模块版本信息
        /// </summary>
        public void InitModuleVersionInfoList() => new ModuleVersionOperation().InitializeModuleVersionInfo();
        /// <summary>
        /// 添加模块版本信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public bool AddModuleVersionInfo(List<ModuleInfoModel> models) => new ModuleVersionOperation().AddModuleInfoDictionary(models);
        /// <summary>
        /// 清空模块版本信息
        /// </summary>
        /// <returns></returns>
        public bool ClearModuleVersionInfo() => new ModuleVersionOperation().ClearModuleInfoDictionary();
    }

    public interface IGetIsMultiModules
    {
        bool GetIsMulti();
    }
    /// <summary>
    /// 平台实现是否为多模块联机为true
    /// 业务可以重新实现来决定是否为多模块联机
    /// appconfig里面增加接口对应类的声明
    /// </summary>
    public class GetIsMultiModules : IGetIsMultiModules
    {
        public bool GetIsMulti()
        {
            return true;
        }
    }
}
