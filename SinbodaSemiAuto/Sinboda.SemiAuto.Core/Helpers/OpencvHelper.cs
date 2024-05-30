using DirectShowLib;
using GalaSoft.MvvmLight.Messaging;
using OpenCvSharp;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;


namespace Sinboda.SemiAuto.Core.Helpers
{
    public class OpencvHelper
    {
        /// <summary>
        /// 相机状态
        /// </summary>
        public bool StatusCameraOn = false;

        /// <summary>
        /// 录制状态
        /// </summary>
        public bool StatusRecordOn = false;

        /// <summary>
        /// 拍照状态
        /// </summary>
        public bool StatusPicOn = false;

        /// <summary>
        /// 相机
        /// </summary>
        private VideoCapture video;

        /// <summary>
        /// 录制器
        /// </summary>
        private VideoWriter videoWriter;

        /// <summary>
        /// 视频地址
        /// </summary>
        private string pathVideo;

        /// <summary>
        /// 图片地址
        /// </summary>
        private string pathPic;

        /// <summary>
        /// 是否为显微摄像头
        /// </summary>
        public bool microVideoCapture = true;

        private static OpencvHelper _opencvHelper;

        /// <summary>
        /// 单例
        /// </summary>
        public static OpencvHelper Instance
        {
            get
            {
                if (_opencvHelper.IsNull())
                    _opencvHelper = new OpencvHelper();
                return _opencvHelper;
            }
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        public void CameraOn()
        {
            var devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
            var cameraNames = new List<string>();
            foreach (var device in devices)
            {
                cameraNames.Add(device.Name);
            }


            if (StatusCameraOn)
            {
                LogHelper.logSoftWare.Info($"相机已打开");
                return;
            }
            //Cv2.po
            StatusCameraOn = true;
            video = new VideoCapture();
            //打开相机
            if (microVideoCapture)
            {
                if (!video.Open(0))
                {
                    LogHelper.logSoftWare.Error("摄像头打开失败");
                    return;
                }
            }
            else
            {
                video = new VideoCapture("");
            }
            if (!video.IsOpened())
            {
                LogHelper.logSoftWare.Error("摄像头打开失败");
                return;
            }
            LogHelper.logSoftWare.Info($"打开相机");

            Thread thread = new Thread(() =>
            {
                while (StatusCameraOn)
                {
                    Mat Camera = new Mat();
                    video.Read(Camera);
                    if (Camera.Empty())//读取视频文件时,判定帧是否为空,如果帧为空,则下方的图片处理会报异常
                    {
                        break;
                    }
                    //修改视频的长宽高
                    Cv2.Resize(Camera, Camera, new OpenCvSharp.Size(GlobalData.VideoWidth, GlobalData.VideoHeight));
                    //保存视频帧
                    if ((!videoWriter.IsNull()) && StatusRecordOn)
                        videoWriter.Write(Camera);
                    //保存图片
                    if (StatusPicOn)
                    {
                        StatusPicOn = false;
                        Cv2.ImWrite(pathPic, Camera);
                    }
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        Messenger.Default.Send<Mat>(Camera, MessageToken.TokenCamera);
                    }
                    ));

                    Cv2.WaitKey(30);
                }
                video?.Release();
                Cv2.DestroyAllWindows();
            });
            thread.Start();
        }

        /// <summary>
        /// 录取视频
        /// </summary>
        public void RecordVideoOn()
        {
            GlobalData.DirectoryVideo.CheckAndCreateDirectory();
            pathVideo = $"{GlobalData.DirectoryVideo}\\{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.avi";
            //保存视频
            videoWriter = new VideoWriter(pathVideo, VideoWriter.FourCC(@"XVID"), GlobalData.VideoFPS, new Size(GlobalData.VideoWidth, GlobalData.VideoHeight));
            StatusRecordOn = true;
            LogHelper.logSoftWare.Info($"开始录取视频:[{pathVideo}]");
        }

        /// <summary>
        /// 停止录取视频
        /// </summary>
        public void RecordVideoOff()
        {
            StatusRecordOn = false;
            videoWriter?.Release();
            LogHelper.logSoftWare.Info("停止录取视频");
        }

        /// <summary>
        /// 拍摄照片
        /// </summary>
        public void TakePic()
        {
            GlobalData.DirectoryPic.CheckAndCreateDirectory();
            pathPic = $"{GlobalData.DirectoryPic}\\{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.png";
            StatusPicOn = true;
            LogHelper.logSoftWare.Info($"拍摄照片:[{pathPic}]");
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        public void CameraOff()
        {
            StatusCameraOn = false;
            RecordVideoOff();
            videoWriter?.Release();
            LogHelper.logSoftWare.Info("关闭相机");
        }
    }
}
