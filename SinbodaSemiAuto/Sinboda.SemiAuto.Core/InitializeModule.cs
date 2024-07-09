using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
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
            //PVCamHelper.Instance.Init();

            //tcp指令通讯器初始化 并连接
            TcpCmdActuators.Instance.Init();
            TcpCmdActuators.Instance.Connect();
            TcpCmdActuators.Instance.StartSequence();

            return new InitTaskResult();
        }
    }
}
