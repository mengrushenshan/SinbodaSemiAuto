using Sinboda.Framework.Common;
using Sinboda.Framework.Common.FileOperateHelper;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Business.SystemSetup
{
    [Serializable]
    public class LoginInfo
    {
        public bool RememberName { get; set; }
        public string Name { get; set; }
        public string MaxMenu { get; set; }
    }
    public class LoginSettingBusiness : BusinessBase<LoginSettingBusiness>
    {
        #region 属性
        public LoginInfo RememberLogin { get; set; }
        /// <summary>
        /// 配置文件存储路径
        /// </summary>
        string loginConfigPath = MapPath.XmlPath + @"LOGINSETTING_CONFIG.xml";

        /// <summary>
        /// 基础接口声明
        /// </summary>
        FileHelper _helper = new FileHelper();
        #endregion


        public LoginSettingBusiness()
        {
            var ret = GetLoginInfo();
            if (ret.ResultEnum == OperationResultEnum.SUCCEED)
            {
                RememberLogin = ret.Results;
            }
        }
        public string GetRemeberName()
        {
            if (null != RememberLogin && RememberLogin.RememberName)
            {
                return RememberLogin.Name;
            }

            return string.Empty;
        }

        public void SetLoginName(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                return;
            }

            if (null != RememberLogin)
            {
                RememberLogin.Name = Name;

                SetLoginSettingInfo(RememberLogin);
            }
        }

        public void SetRemember(bool bRemember)
        {
            if (null != RememberLogin)
            {
                RememberLogin.RememberName = bRemember;

                SetLoginSettingInfo(RememberLogin);
            }
        }

        public int? GetMaxMenuCount()
        {
            if (null == RememberLogin)
            {
                return null;
            }

            int nCount = 0;
            if (int.TryParse(RememberLogin.MaxMenu, out nCount))
            {
                return nCount;
            }

            return null;
        }

        public void SetMaxMenuCount(int nCount)
        {
            if (null != RememberLogin)
            {
                RememberLogin.MaxMenu = nCount.ToString();

                SetLoginSettingInfo(RememberLogin);
            }
        }

        /// <summary>
        /// 获取LIS设置信息
        /// </summary>
        /// <returns></returns>
        public OperationResult<LoginInfo> GetLoginInfo()
        {
            try
            {
                LoginInfo model = null;
                if (File.Exists(loginConfigPath))
                {
                    model = _helper.ReadXML<LoginInfo>(loginConfigPath);
                }
                else
                {
                    CreateLoginConfig();
                }

                if (model != null)
                {
                    return Result(OperationResultEnum.SUCCEED, model);
                }
                else
                {
                    return Result<LoginInfo>(OperationResultEnum.FAILED);
                }
            }
            catch (Exception e)
            {
                try
                {
                    CreateLoginConfig();
                    LogHelper.logSoftWare.Error("GetLoginInfo And CreateLoginConfig", e);

                    LoginInfo model = null;
                    model = _helper.ReadXML<LoginInfo>(loginConfigPath);
                    if (model != null)
                    {
                        return Result(OperationResultEnum.SUCCEED, model);
                    }
                    else
                    {
                        return Result<LoginInfo>(OperationResultEnum.FAILED);
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
        private void CreateLoginConfig()
        {
            LoginInfo _writeModel = new LoginInfo()
            {
                RememberName = false,
                Name = string.Empty
            };

            _helper.SaveXML<LoginInfo>(_writeModel, loginConfigPath);
        }
        /// <summary>
        /// 保存备份还原设置信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private OperationResult SetLoginSettingInfo(LoginInfo model)
        {
            try
            {
                if (File.Exists(loginConfigPath))
                {
                    LoginInfo _model = _helper.ReadXML<LoginInfo>(loginConfigPath);
                    _model.RememberName = model.RememberName;
                    _model.Name = model.Name;
                    _model.MaxMenu = model.MaxMenu;
                    bool result = false;
                    result = _helper.SaveXML<LoginInfo>(_model, loginConfigPath);
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
                LogHelper.logSoftWare.Error("SetLoginSettingInfo", e);
                return Result(OperationResultEnum.FAILED, e);
            }
        }
    }
}
