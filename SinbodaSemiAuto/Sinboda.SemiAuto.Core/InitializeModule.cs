﻿using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core
{
    public class InitializeModule : IModule
    {
        public void FinalizeResource()
        {
            TcpCmdActuators.Instance.Dispose();
            //释放相机资源
            PVCamHelper.Instance.Dispose();
        }

        public List<ModuleMenuItem> GetMenus()
        {
            throw new NotImplementedException();
        }

        public InitTaskResult InitializeResource()
        {
            //设置Python环境
            PyHelper.Init();

            //加载参数
            GlobalData.Init();

            //相机初始化
            PVCamHelper.Instance.Init();

            //tcp指令通讯器初始化 并连接
            TcpCmdActuators.Instance.Init();
            if (!TcpCmdActuators.Instance.Connect())
            {
                SystemResources.Instance.SysAlarmInstance.SoftWareAlarmHandler("0-1", SystemResources.Instance.GetLanguage(8303, "未连接至LIS服务器"), (int)ProductType.Sinboda001);
            }
            TcpCmdActuators.Instance.StartSequence();

            return new InitTaskResult();
        }
    }
}
