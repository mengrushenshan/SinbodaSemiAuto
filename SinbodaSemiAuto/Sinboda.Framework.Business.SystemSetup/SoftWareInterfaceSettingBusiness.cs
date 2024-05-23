using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.BusinessModels;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;

namespace Sinboda.Framework.Business.SystemSetup
{
    /// <summary>
    /// 
    /// </summary>
    public class SoftWareInterfaceSettingBusiness : BusinessBase<SoftWareInterfaceSettingBusiness>
    {
        /// <summary>
        /// 显示设置配置文件存储
        /// </summary>
        string showSettingConfigPath = MapPath.XmlPath + @"SOFTWARESHOWSETTING_CONFIG.xml";
        /// <summary>
        /// 打印设置配置文件存储
        /// </summary>
        string printConfigPath = MapPath.XmlPath + @"SOFTWAREINTERFACE_CONFIG.xml";
        /// <summary>
        /// 基础接口声明
        /// </summary>
        FileHelper _helper = new FileHelper();

        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<DataDictionaryParentInfo> GetShowSettingInfo()
        {
            try
            {
                DataDictionaryParentInfo model = null;
                if (File.Exists(showSettingConfigPath))
                {
                    model = _helper.ReadXML<DataDictionaryParentInfo>(showSettingConfigPath);
                }
                if (model != null)
                {
                    foreach (var item in model.DataDictionaryInfos)
                    {
                        if (item != null)
                        {
                            foreach (var subitem in item.DataDictionaryInfoDetails)
                            {
                                subitem.Values = subitem.LanguageID < SystemResources.Instance.LanguageArray.Length ? SystemResources.Instance.LanguageArray[subitem.LanguageID] : subitem.Values;
                            }
                        }
                    }

                    return Result(OperationResultEnum.SUCCEED, model);
                }
                else
                {
                    return Result<DataDictionaryParentInfo>(OperationResultEnum.FAILED);
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetShowSettingIno", e);
                return Result<DataDictionaryParentInfo>(e);
            }
        }

        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<SoftWareInterfaceModel> GetSettingInfo()
        {
            try
            {
                LogHelper.logSoftWare.Debug("GetSettingInfo SoftWareInterfaceSettingBusiness");
                SoftWareInterfaceModel model = null;
                if (File.Exists(printConfigPath))
                {
                    model = _helper.ReadXML<SoftWareInterfaceModel>(printConfigPath);
                }
                if (model != null)
                {
                    SystemResources.Instance.softModel = model;
                    LogHelper.logSoftWare.Debug("GetSettingInfo softModel " + SystemResources.Instance.softModel.AnalyzerTypeName);
                    return Result(OperationResultEnum.SUCCEED, model);
                }
                else
                {
                    return Result<SoftWareInterfaceModel>(OperationResultEnum.FAILED);
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("GetSettingInfo", e);
                return Result<SoftWareInterfaceModel>(e);
            }
        }



        /// <summary>
        /// 保存设置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OperationResult SetSettingInfo(SoftWareInterfaceModel model)
        {
            try
            {
                if (File.Exists(printConfigPath))
                {
                    SoftWareInterfaceModel _model = _helper.ReadXML<SoftWareInterfaceModel>(printConfigPath);

                    _model.CompanyLogoPath = model.CompanyLogoPath;
                    _model.AnalyzerName = model.AnalyzerName;
                    _model.AnalyzerType = model.AnalyzerType;
                    _model.AnalyzerTypeName = model.AnalyzerTypeName;
                    _model.LanguageID = model.LanguageID;

                    _model.CurrentLanguage = model.CurrentLanguage;
                    _model.CurrentTheme = model.CurrentTheme;
                    _model.CurrentFontSize = model.CurrentFontSize;

                    _model.BackupMaintanceByExit = model.BackupMaintanceByExit;
                    _model.BackupMaintanceByTime = model.BackupMaintanceByTime;
                    _model.BackupTime = model.BackupTime;
                    _model.BackupLocation = model.BackupLocation;
                    _model.ReBackupLocation = model.ReBackupLocation;

                    _model.Logout4StandyEnable = model.Logout4StandyEnable;
                    _model.Logout4StandyByTime = model.Logout4StandyByTime;
                    _model.LogoutEnableDisplay = model.LogoutEnableDisplay;

                    //_model.AutoBackup = model.AutoBackup;
                    _model.PrintSysLog = model.PrintSysLog;
                    bool result = false;
                    result = _helper.SaveXML<SoftWareInterfaceModel>(_model, printConfigPath);
                    if (result)
                    {
                        return Result(OperationResultEnum.SUCCEED);
                    }
                    else
                    {
                        return Result(OperationResultEnum.FAILED);
                    }
                }
                else
                {
                    return Result(OperationResultEnum.FAILED);
                }
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("SetSettingInfo", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }

        /// <summary>
        /// 获取仪器信息
        /// </summary>
        public void GetAnalyzerInfo()
        {
            BitmapImage image = new BitmapImage();
            if (File.Exists(MapPath.ImagesPath + SystemResources.Instance.softModel.CompanyLogoPath))
            {
                image.BeginInit();
                image.UriSource = new Uri(MapPath.ImagesPath + SystemResources.Instance.softModel.CompanyLogoPath, UriKind.RelativeOrAbsolute);
                image.EndInit();
            }
            SystemResources.Instance.AnalyzerInfoLogo = image;
            SystemResources.Instance.AnalyzerInfoType = SystemResources.Instance.softModel.AnalyzerType;
            SystemResources.Instance.AnalyzerInfoTypeName = SystemResources.Instance.softModel.AnalyzerTypeName;
            SystemResources.Instance.AnalyzerInfoName = SystemResources.Instance.softModel.LanguageID != 0 ?
                                                        SystemResources.Instance.LanguageArray[SystemResources.Instance.softModel.LanguageID] :
                                                        SystemResources.Instance.softModel.AnalyzerName;
        }
    }
}
