using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Mapster.Utils;
using OpenCvSharp;
using sin_mole_flu_analyzer.Models.Command;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class AutofocusHelper : TBaseSingleton<AutofocusHelper>
    {

        /// <summary>
        /// 起始坐标
        /// </summary>
        private int zStart;

        /// <summary>
        /// 间隔
        /// </summary>
        private int zInterval;

        /// <summary>
        /// 照片数量
        /// </summary>
        private int tifNum;

        private bool isSaveEnable = false;
        private List<byte[]> mats = new List<byte[]>();
        public void SaveImage(Byte[] srcData)
        {
            lock (_lockObj)
            {
                if (isSaveEnable)
                {
                    //tif编码格式
                    mats.Add(srcData);
                    isSaveEnable = false;
                }
            }
        }

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">起始坐标</param>
        /// <param name="interval">间隔</param>
        /// <param name="num">照片数量</param>
        public int ZPos(Sin_Motor obj,int start, int interval, int num, string file = "", int holdTimeMS = 100)
        {
            try
            {
                //注册通知
                Messenger.Default.Register<byte[]>(this, MessageToken.TokenCameraBuffer, SaveImage);
                //检查文件保存目录
                GlobalData.DirectoryPic.CheckAndCreateDirectory();

                //数据
                zStart = start;
                zInterval = interval;
                tifNum = num;

                isSaveEnable = false;

                for (int i = 0; i < tifNum; i++)
                {
                    //移动z轴
                    CmdMoveAbsolute cmdMoveAbsolute = new CmdMoveAbsolute()
                    {
                        Id = (int)obj.MotorId,
                        TargetPos = obj.TargetPos
                    };
                    cmdMoveAbsolute.Execute();
                    Thread.Sleep(holdTimeMS);
                    //记录图像
                    lock (_lockObj)
                    {
                        isSaveEnable = true;
                    }

                    //等待拍照完成
                    int time = 50;
                    //最多等待十秒
                    int tiemSpan = 10 * 1000 / time;
                    for (int j = 0; j < tiemSpan; j++)
                    {
                        Thread.Sleep(time);
                        lock (_lockObj)
                        {
                            if (!isSaveEnable)
                            {
                                break;
                            }
                        }
                    }
                }
                //存储图像数据
                TiffBitmapEncoder encoder = new TiffBitmapEncoder
                {
                    Compression = TiffCompressOption.Zip
                };
                lock (_lockObj)
                {
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
                }
                string path = $"{GlobalData.DirectoryPic}\\123.tiff";
                if (!file.IsNullOrWhiteSpace())
                    path = file;
                FileStream f = new FileStream(path, FileMode.Create);
                encoder.Save(f);
                //释放流
                f.Close();

                //调用解析程序
                List<int> frameIds = PyHelper.Autofocus(path);

                //计算对焦点坐标
                return frameIds[0] * zInterval + zStart;
            }
            catch (Exception e)
            {
                return 0;
            }
            finally
            {
                //清除数据
                mats.Clear();
                //注销通知
                Messenger.Default.Unregister<byte[]>(this, MessageToken.TokenCameraBuffer, SaveImage);
            }
        }

    }
}
