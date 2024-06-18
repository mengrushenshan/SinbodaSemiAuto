using sin_mole_flu_analyzer.Models.Command;
using Sinboda.Framework.Infrastructure;
using Sinboda.Framework.Infrastructure.Model;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
            //加载参数
            GlobalData.Init();

            ////电机初始化
            //XimcHelper.Instance.Init();

            ////相机初始化
            //PVCamHelper.Instance.Init();

            //tcp指令通讯器初始化 并连接
            TcpCmdActuators.Instance.Init();
            TcpCmdActuators.Instance.Connect();
            TcpCmdActuators.Instance.StartSequence();

            List<byte > bytes = new List<byte>();
            string file = @"D:\微信\WEChatData\WeChat Files\wxid_go079ztc5hde21\FileStorage\File\2024-06\SimdaX-100 01.00.02.0001.bin";
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {//在using中创建FileStream对象fs，然后执行大括号内的代码段，
             //执行完后，释放被using的对象fs（后台自动调用了Dispose）
                byte[] vs = new byte[1024];//数组大小根据自己喜欢设定，太高占内存，太低读取慢。
                while (true) //因为文件可能很大，而我们每次只读取一部分，因此需要读很多次
                {
                    int r = fs.Read(vs, 0, vs.Length);
                    bytes.AddRange  (vs);
                    if (r == 0) //当读取不到，跳出循环
                    {
                        break;
                    }
                }
            }
            CmdIAP cmdIAP   =new CmdIAP()
            {
                Data=bytes.ToArray(),
            };
            cmdIAP.Execute();
            return new InitTaskResult();
        }
    }
}
