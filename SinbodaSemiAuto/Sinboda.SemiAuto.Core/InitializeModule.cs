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

        }

        public List<ModuleMenuItem> GetMenus()
        {
            throw new NotImplementedException();
        }

        public InitTaskResult InitializeResource()
        {
            //PrintHelper.Instance.Init("FUJIFILM Apeos C2560");
            //Mat img1 = Cv2.ImRead(@"C:\Users\Lenovo\Desktop\1.jpg", ImreadModes.AnyColor);
            //PrintHelper.Instance.Print(img1.ToBitmap()); 
            //PrintHelper.Instance.Print("测试","we are all down the god" +
            //    "but .................." +
            //    ".......");

            //AnalysisHelper.Instance.Init();
            //AnalysisHelper.Instance.Analysis(new Model.DatabaseModel.SemiAuto.Sin_Test_Result() { Test_file_name = "E:\\Result" },'A',1);

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
