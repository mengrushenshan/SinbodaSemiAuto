using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinboda.Framework.Core.BusinessModels;
using System.IO;

namespace Sinboda.Framework.Business.SystemSetup
{
    public class LISCommunicationSettingBusiness : BusinessBase<LISCommunicationSettingBusiness>
    {
        /// <summary>
        /// LIS通信设置配置文件存储
        /// </summary>
        string lisSettingConfigPath = MapPath.XmlPath + @"LISCOMMUNICATIONSETTING_CONFIG.xml";

        /// <summary>
        /// 基础接口声明
        /// </summary>
        FileHelper _helper = new FileHelper();
        /// <summary>
        /// 获取LIS设置信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<LISCommunicationInterfaceModel> GetLISSettingInfo()
        {
            try
            {
                LISCommunicationInterfaceModel model = null;
                if (File.Exists(lisSettingConfigPath))
                {
                    model = _helper.ReadXML<LISCommunicationInterfaceModel>(lisSettingConfigPath);
                }
                if (model != null)
                {
                    return Result(OperationResultEnum.SUCCEED, model);
                }
                else
                {
                    return Result<LISCommunicationInterfaceModel>(OperationResultEnum.FAILED);
                }
            }
            catch (Exception e)
            {
                try
                {
                    CreateLISXmlConfig();
                    LogHelper.logSoftWare.Error("GetLISSettingInfo And CreateLISXmlConfig", e);

                    LISCommunicationInterfaceModel model = null;
                    model = _helper.ReadXML<LISCommunicationInterfaceModel>(lisSettingConfigPath);
                    if (model != null)
                    {
                        return Result(OperationResultEnum.SUCCEED, model);
                    }
                    else
                    {
                        return Result<LISCommunicationInterfaceModel>(OperationResultEnum.FAILED);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                throw e;
            }
        }
        /// <summary>
        /// 创建默认LIS配置文件信息
        /// </summary>
        private void CreateLISXmlConfig()
        {
            LISCommunicationInterfaceModel _writeModel = new LISCommunicationInterfaceModel()
            {
                LISEnabled = false,
                MachineID = string.Empty,
                LISID = string.Empty,
                IsNetWork = true,
                NetworkIP = "192.168.100.100",
                NetworkPort = 5000,

                SerialPort = "COM1",
                BaudRate = "115200",
                StopType = "1",
                CheckType = "1",
                DataType = "8",
            };

            _helper.SaveXML<LISCommunicationInterfaceModel>(_writeModel, lisSettingConfigPath);
        }
        /// <summary>
        /// 保存备份还原设置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public OperationResult SetLISSettingInfo(LISCommunicationInterfaceModel model)
        {
            try
            {
                if (File.Exists(lisSettingConfigPath))
                {
                    LISCommunicationInterfaceModel _model = _helper.ReadXML<LISCommunicationInterfaceModel>(lisSettingConfigPath);
                    _model.LISEnabled = model.LISEnabled;
                    _model.MachineID = model.MachineID;
                    _model.LISID = model.LISID;
                    _model.IsNetWork = model.IsNetWork;
                    _model.NetworkIP = model.NetworkIP;
                    _model.NetworkPort = model.NetworkPort;

                    _model.SerialPort = model.SerialPort;
                    _model.BaudRate = model.BaudRate;
                    _model.StopType = model.StopType;
                    _model.CheckType = model.CheckType;
                    _model.DataType = model.DataType;
                    bool result = false;
                    result = _helper.SaveXML<LISCommunicationInterfaceModel>(_model, lisSettingConfigPath);
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
                LogHelper.logSoftWare.Error("SetLISSettingInfo", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
    }
}
