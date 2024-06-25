using GalaSoft.MvvmLight.Messaging;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class AnalysisHelper : TBaseSingleton<AnalysisHelper>
    {
        public void Init()
        {
            //加载py运行环境
            PyHelper.Init();
        }

        public void Shutdown()
        {
            //关闭加载的python
            PyHelper.Shutdown();
        }

        /// <summary>
        /// 一个孔位九个拍摄点
        /// </summary>
        private int picNum = 9;

        /// <summary>
        /// 计算孔位数据
        /// </summary>
        /// <param name="sin_Test_Result"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void Analysis(Sin_Test_Result sin_Test_Result, int row, int col)
        {
            //检查文件保存目录
            sin_Test_Result.Test_file_name.CheckAndCreateDirectory();
            int cellNum = PyHelper.DataAnalyze(sin_Test_Result.Test_file_name, row, col);
            for (int i = 0; i < picNum; i++)
            {
                //文件存在 添加在目录中
                string file = $"{sin_Test_Result.Test_file_name}\\{(char)row}_{col}_{i+1}.jpg";
                if(file.CheckPath())
                    sin_Test_Result.Result_file_name.Add(file);
            }
            sin_Test_Result.Result_original = cellNum;
        }

        /// <summary>
        /// 存储图片
        /// </summary>
        /// <param name="sin_Test_Result"></param>
        /// <param name="mats">图片数据</param>
        public void SaveImage(Sin_Test_Result sin_Test_Result, List<byte[]> mats)
        {

            //检查文件保存目录
            sin_Test_Result.Test_file_name.CheckAndCreateDirectory();

            //存储图像数据
            TiffBitmapEncoder encoder = new TiffBitmapEncoder
            {
                Compression = TiffCompressOption.Zip
            };

            foreach (var item in mats)
            {
                System.Drawing.Size imageSize = new System.Drawing.Size() { Width = PVCamHelper.Instance.GetWidth(), Height = PVCamHelper.Instance.GetHeight() };
                int lenth = imageSize.Width * imageSize.Height;
                byte[] bufBytes = new byte[lenth * 2];

                IntPtr intptr = Marshal.UnsafeAddrOfPinnedArrayElement(item, 0);
                Marshal.Copy(intptr, bufBytes, 0, item.Length);

                BitmapPalette myPalette = BitmapPalettes.Gray16;
                int rawStride = (imageSize.Width * 16 + 7) / 8;

                BitmapSource image = BitmapSource.Create(imageSize.Width, imageSize.Height, 96, 96, System.Windows.Media.PixelFormats.Gray16, myPalette, bufBytes, rawStride);
                encoder.Frames.Add(BitmapFrame.Create(image));
            }
            FileStream f = new FileStream(sin_Test_Result.Test_file_name, FileMode.Create);
            encoder.Save(f);
            //释放流
            f.Close();
        }
    }
}
