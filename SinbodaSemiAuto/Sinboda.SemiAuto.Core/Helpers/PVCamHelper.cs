using GalaSoft.MvvmLight.Messaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Pvcam;
using Sinboda.SemiAuto.Core.Resources;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Size = System.Drawing.Size;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class PVCamHelper : TBaseSingleton<PVCamHelper>
    {
        /// <summary>
        /// 初始化标志
        /// </summary>
        private bool IsInitSuccess = false;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// 帧id
        /// </summary>
        private ulong m_guiUpdateLastFrameId = ulong.MaxValue;

        /// <summary>
        /// 是否允许任务并行
        /// </summary>
        bool m_allowParallelProcessing = false;

        /// <summary>
        /// 相机图像格式
        /// </summary>
        PVCAM.PL_IMAGE_FORMATS m_imageFormat = PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16;

        /// <summary>
        /// 相机控制器
        /// </summary>
        private PvcamController camCtrl = null;

        /// <summary>
        /// 定时器
        /// </summary>
        bool IsEnabled = false;
        Task m_tempTimer;
        Task m_guiUpdateTimer;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            lock (_lockObj)
            {
                try
                {
                    //订阅相机事件回调
                    camCtrl = new PvcamController();
                    camCtrl.OnCameraNotification += ActiveCamera_OnCameraNotificationReceived;
                    camCtrl.OnPortListChanged += ActiveCamera_OnPortListChanged;
                    camCtrl.OnPostProcessingFeaturesChanged += ActiveCamera_OnPostProcessingFeaturesChanged;
                    camCtrl.OnPortChanged += ActiveCamera_OnPortChanged;
                    camCtrl.OnSpeedChanged += ActiveCamera_OnSpeedChanged;
                    camCtrl.OnGainChanged += ActiveCamera_OnGainChanged;
                    camCtrl.OnSpeedListChanged += ActiveCamera_OnSpeedListChanged;
                    camCtrl.OnGainListChanged += ActiveCamera_OnGainListChanged;

                    //打开相机
                    if (PvcamLibrary.IsInitialized)
                        PvcamLibrary.Uninitialize();
                    PvcamLibrary.Initialize();
                    List<string> cameraList = PvcamLibrary.ListCameras();
                    string camName = cameraList.FirstOrDefault();
                    if (camName.IsNull())
                    {
                        LogHelper.logSoftWare.Error("No camera found in the system !");
                        NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "没有找到相机"));
                        return;
                    }
                    camCtrl.Open(camName);

                    //设置相机参数
                    camCtrl.ExposureTime = GlobalData.ExposureTime;
                    camCtrl.AllowParallelProcessing = m_allowParallelProcessing;
                    IsInitSuccess = true;
                }
                catch (Exception ex)
                {
                    LogHelper.logSoftWare.Error(ex.Message);
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "相机初始化失败"));
                    IsInitSuccess = false;
                }

            }
        }

        /// <summary>
        /// 获取初始化标志
        /// </summary>
        /// <returns></returns>
        public bool GetInitFlag()
        {
            return IsInitSuccess;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                lock (_lockObj)
                {
                    if ((!videoWriter.IsNull()) && videoWriter.IsOpened())
                        videoWriter.Release();
                    //停止定时器
                    IsEnabled = false;

                    //释放相机资源
                    if (camCtrl.IsNull())
                        return;
                    if (camCtrl.IsAcquiring)
                    {
                        camCtrl.StopAcquisition();
                    }
                    camCtrl.Close();
                    camCtrl.OnCameraNotification -= ActiveCamera_OnCameraNotificationReceived;
                    camCtrl.OnPortListChanged -= ActiveCamera_OnPortListChanged;
                    camCtrl.OnPostProcessingFeaturesChanged -= ActiveCamera_OnPostProcessingFeaturesChanged;
                    camCtrl.OnPortChanged -= ActiveCamera_OnPortChanged;
                    camCtrl.OnSpeedChanged -= ActiveCamera_OnSpeedChanged;
                    camCtrl.OnGainChanged -= ActiveCamera_OnGainChanged;
                    camCtrl.OnSpeedListChanged -= ActiveCamera_OnSpeedListChanged;
                    camCtrl.OnGainListChanged -= ActiveCamera_OnGainListChanged;
                    camCtrl = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.Message);
            }
        }

        /// <summary>
        /// 暂停获取数据
        /// </summary>
        public void Pause()
        {
            try
            {
                lock (_lockObj)
                {
                    //停止定时器
                    IsEnabled = false;

                    //释放相机资源
                    if (camCtrl.IsNull())
                        return;
                    if (camCtrl.IsAcquiring)
                    {
                        camCtrl.StopAcquisition();
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error(ex.Message);
            }
        }

        /// <summary>
        /// 启动连续模式
        /// </summary>
        public void StartCont()
        {
            lock (_lockObj)
            {
                IsEnabled = true;
                //设置连续模式 ，获取帧直到结束
                camCtrl.SetupAcquisition(PvcamController.AcquisitionType.Continuous, PvcamController.RUN_UNTIL_STOPPED);
                camCtrl.StartAcquisition();

                m_tempTimer = new Task(tempTimer_Tick);
                m_guiUpdateTimer = new Task(GuiUpdateTick);
                m_guiUpdateTimer.Start();
            }
        }

        /// <summary>
        /// 启动单帧模式
        /// </summary>
        public void StartSingle()
        {
            lock (_lockObj)
            {
                IsEnabled = true;
                //序列模式 获取一帧
                camCtrl.SetupAcquisition(PvcamController.AcquisitionType.Sequence, GlobalData.FrameNum);
                camCtrl.StartAcquisition();

                m_tempTimer = new Task(tempTimer_Tick);
                m_guiUpdateTimer = new Task(GuiUpdateTick);
                m_guiUpdateTimer.Start();
            }
        }

        private void ActiveCamera_OnPortChanged(PvcamController sender, PortOption args)
        {
            SetStatusStripMessage($"PortChanged : [{args.Name}]!");
        }

        private void ActiveCamera_OnPostProcessingFeaturesChanged(PvcamController sender, List<PostProcessingFeature> args)
        {
            try
            {
                //PopulatePostProcessingUnsafe(args);
            }
            catch (Exception exc)
            {
                SetStatusStripMessage(exc.Message);
            }
        }

        private void ActiveCamera_OnSpeedChanged(PvcamController sender, SpeedOption args)
        {
            SetStatusStripMessage($"SpeedChanged : [{args.Name}]!");
        }

        private void ActiveCamera_OnGainChanged(PvcamController sender, GainOption args)
        {
            SetStatusStripMessage($"GainChanged : [{args.Name}]!");
        }

        private void ActiveCamera_OnGainListChanged(PvcamController sender, List<GainOption> args)
        {
            foreach (GainOption g in args)
            {
                SetStatusStripMessage($"GainOption : [{g.Name}]!");
            }
        }

        private void ActiveCamera_OnPortListChanged(PvcamController sender, List<PortOption> args)
        {
            foreach (PortOption p in args)
            {
                SetStatusStripMessage($"find port: [{p.Name}]!");
            }
        }

        private void ActiveCamera_OnSpeedListChanged(PvcamController sender, List<SpeedOption> args)
        {
            foreach (SpeedOption s in camCtrl.SpeedList)
            {
                SetStatusStripMessage($"SpeedOption : [{s.Name}]!");
            }
        }

        /// <summary>
        /// Camera notification handler.
        /// </summary>
        /// <param name="sender">Sender of this notification</param>
        /// <param name="args">Notification arguments</param>
        private void ActiveCamera_OnCameraNotificationReceived(PvcamController sender, CameraNotificationEventArgs args)
        {
            try
            {
                switch (args.Type)
                {
                    case CameraNotificationType.CameraErrorMessage:
                        SetStatusStripMessage(args.Message);
                        break;
                    case CameraNotificationType.CameraStatusMessage:
                        SetStatusStripMessage(args.Message);
                        break;
                    case CameraNotificationType.AcquisitionStarted:
                        break;
                    case CameraNotificationType.AcquisitionFinished:
                    case CameraNotificationType.AcquisitionFailed:
                        {
                            // Let the GUI updater know the acquisition has completed. In the
                            // next run, it will fetch the latest frame, stop itself and enable controls.
                            lock (m_guiUpdateTimer)
                            {
                                IsEnabled = false;
                            }
                            break;
                        }
                    case CameraNotificationType.FrameAcquired:
                        // Not using at the moment. We are polling the camera for latest frame manually.
                        break;
                }
            }
            catch (Exception exc)
            {
                SetStatusStripMessage(exc.Message);
            }
        }

        /// <summary>
        /// 32位图像转24位
        /// </summary>
        /// <param name="bmp32"></param>
        /// <returns></returns>
        public Bitmap Bit32To24(Bitmap bmp32)
        {
            BitmapData data32 = bmp32.LockBits(new Rectangle(0, 0, bmp32.Width, bmp32.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Bitmap bmp24 = new Bitmap(bmp32.Width, bmp32.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData data24 = bmp24.LockBits(new Rectangle(0, 0, bmp24.Width, bmp24.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr32 = (byte*)data32.Scan0.ToPointer();
                byte* ptr24 = (byte*)data24.Scan0.ToPointer();
                for (int i = 0; i < bmp24.Height; i++)
                {
                    for (int j = 0; j < bmp24.Width; j++)
                    {
                        *ptr24++ = *ptr32++;
                        *ptr24++ = *ptr32++;
                        *ptr24++ = *ptr32++;
                        ptr32++;
                    }
                    ptr24 += data24.Stride - bmp24.Width * 3;
                    ptr32 += data32.Stride - bmp32.Width * 4;
                }
            }

            bmp32.UnlockBits(data32);
            bmp24.UnlockBits(data24);
            return bmp24;
            //bmp24.Save();
        }

        private DirectBitmap m_displayableBitmap = null;

        /// <summary>
        /// 数据转图像
        /// </summary>
        /// <param name="srcData"></param>
        /// <param name="imageSize"></param>
        /// <param name="srcFmt"></param>
        /// <param name="srcMin"></param>
        /// <param name="srcMax"></param>
        /// <param name="useParallelProcessing"></param>
        /// <returns></returns>
        public void FrameToBMP(byte[] srcData, Size imageSize, PVCAM.PL_IMAGE_FORMATS srcFmt, double srcMin, double srcMax, bool useParallelProcessing)
        {
            m_displayableBitmap = new DirectBitmap(imageSize);
            m_displayableBitmap.FrameToBMP(srcData, imageSize, srcFmt, srcMin, srcMax, useParallelProcessing);
            //缩放 并且旋转图像
            Mat matTemp = Rehandle(m_displayableBitmap.Bitmap.ToMat(), new OpenCvSharp.Size(1024, 1024));

            //保存视频帧
            if (StatusRecordOn && !videoWriter.IsNull())
            {
                videoWriter.Write(InputArray.Create(matTemp));
            }
            Mat matROI = ROI(matTemp, 0, 1024, 0, 1024);
            Messenger.Default.Send<Mat>(matROI, MessageToken.TokenCamera);
            m_displayableBitmap.Dispose();
        }

        /// <summary>
        /// 图像重新处理
        /// </summary>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <param name="rotateFlags"></param>
        /// <returns></returns>
        private Mat Rehandle(Mat source, OpenCvSharp.Size size, RotateFlags rotateFlags = RotateFlags.Rotate90Counterclockwise)
        {
            Mat mat = new Mat();
            Mat dst = new Mat();
            Cv2.Resize(source, mat, size);
            Cv2.Rotate(mat, dst, RotateFlags.Rotate90Counterclockwise);
            return dst;
        }

        /// <summary>
        /// 拍摄照片
        /// </summary>
        public void TakePicture(Mat bitmap, string fileName = null)
        {
            lock (_lockObj)
            {
                string pathPic;
                GlobalData.DirectoryPic.CheckAndCreateDirectory();
                if (fileName.IsNullOrWhiteSpace())
                {
                    pathPic = $"{GlobalData.DirectoryPic}\\{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.png";
                }
                else
                {
                    pathPic = $"{GlobalData.DirectoryPic}\\{fileName}_{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.png";
                }
                if (!bitmap.IsNull())
                {
                    Cv2.ImWrite(pathPic, bitmap);
                    LogHelper.logSoftWare.Info($"拍摄照片:[{pathPic}]");
                }
                else
                {
                    LogHelper.logSoftWare.Error($"拍摄照片失败！");
                }
            }
        }

        /// <summary>
        /// 录取视频
        /// </summary>
        public void RecordVideoOn(string fileName = null)
        {
            lock (_lockObj)
            {
                GlobalData.DirectoryVideo.CheckAndCreateDirectory();
                if (fileName.IsNullOrWhiteSpace())
                {
                    pathVideo = $"{GlobalData.DirectoryVideo}\\{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.avi";
                    //pathVideo = $"345678.avi";
                }
                else
                {
                    pathVideo = $"{GlobalData.DirectoryVideo}\\{fileName}_{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.avi";
                }

                //保存视频
                videoWriter = new VideoWriter(pathVideo, VideoWriter.FourCC(@"XVID"), GlobalData.VideoFPS, new OpenCvSharp.Size(GlobalData.VideoWidth, GlobalData.VideoHeight));
                StatusRecordOn = true;
                LogHelper.logSoftWare.Info($"开始录取视频:[{pathVideo}]");
            }
        }

        /// <summary>
        /// 录制器
        /// </summary>
        private VideoWriter videoWriter;

        /// <summary>
        /// 录制视频状态
        /// </summary>
        private bool StatusRecordOn = false;

        /// <summary>
        /// 视频地址
        /// </summary>
        private string pathVideo;

        /// <summary>
        /// 停止录取视频
        /// </summary>
        public void RecordVideoOff()
        {
            StatusRecordOn = false;
            LogHelper.logSoftWare.Info("停止录取视频");
        }

        /// <summary>
        /// GUI刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuiUpdateTick()
        {
            while (IsEnabled)
            {
                lock (_lockObj)
                {
                    if (camCtrl.IsNull() || camCtrl.LatestFrameData.IsNull())
                    {
                        return;
                    }
                    if (m_guiUpdateLastFrameId != camCtrl.FrameSequenceNumber)
                    {
                        m_guiUpdateLastFrameId = camCtrl.FrameSequenceNumber;
                        ProcessLatestFrame();
                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 统计图像数据
        /// </summary>
        /// <param name="useParallelProcessing"></param>
        /// <param name="finalStats"></param>
        private void CalculateImageStatistics(bool useParallelProcessing, ImageStats finalStats)
        {
            finalStats.Reset();
            // Limit the max number of threads for processing to a resonable amount
            int tN = Math.Min(Environment.ProcessorCount, 16);
            byte[] data = camCtrl.LatestFrameData;
            int dataLength = camCtrl.LatestFrameData.Length;

            switch (camCtrl.ImageFormat)
            {
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO8:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER8:
                    if (useParallelProcessing)
                    {
                        Parallel.For(0, tN,
                            () => new ImageStats(),
                            (int tIdx, ParallelLoopState state, ImageStats localStats) =>
                            {
                                int jInc = tN;
                                for (int j = tIdx * 1; j < dataLength; j += jInc)
                                {
                                    byte pixVal = data[j];
                                    localStats.Push(pixVal);
                                }
                                return localStats;
                            },
                            (ImageStats localStats) =>
                            {
                                lock (_lockObj)
                                {
                                    finalStats.Push(localStats);
                                }
                            });
                    }
                    else
                    {
                        for (int i = 0; i < dataLength; i++)
                        {
                            byte pixVal = data[i];
                            finalStats.Push(pixVal);
                        }
                    }
                    break;
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16:
                case PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_BAYER16:
                    if (useParallelProcessing)
                    {
                        Parallel.For(0, tN,
                            () => new ImageStats(),
                            (int tIdx, ParallelLoopState state, ImageStats localStats) =>
                            {
                                int jInc = tN * 2;
                                for (int j = tIdx * 2; j < dataLength; j += jInc)
                                {
                                    ushort b0 = data[j + 0];
                                    ushort b1 = data[j + 1];
                                    ushort pixVal = (ushort)(b0 + (b1 << 8));
                                    localStats.Push(pixVal);
                                }
                                return localStats;
                            },
                            (ImageStats localStats) =>
                            {
                                lock (_lockObj)
                                {
                                    finalStats.Push(localStats);
                                }
                            });
                    }
                    else
                    {
                        for (int i = 0; i < dataLength; i += 2)
                        {
                            ushort b0 = data[i + 0];
                            ushort b1 = data[i + 1];
                            ushort pixVal = (ushort)(b0 + (b1 << 8));
                            finalStats.Push(pixVal);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 处理最后一帧
        /// </summary>
        private void ProcessLatestFrame()
        {
            //图像处理状态
            ImageStats m_imageStats = new ImageStats();

            //计算图像尺寸
            CalculateImageStatistics(m_allowParallelProcessing, m_imageStats);

            //数据处理
            ProcessFrame(camCtrl.LatestFrameInfo.FrameNr, camCtrl.LatestFrameData, m_imageStats, camCtrl.LatestFrameMetadata, camCtrl.BitDepth, camCtrl.OutputImageSize, m_allowParallelProcessing);

            //记录日志
            SetStatusStripMessage($"Analysis proc.: {camCtrl.FrameRate}");

        }

        /// <summary>
        /// ROI算法
        /// </summary>
        public Mat ROI(Mat bitmap, int rowStart, int rowEnd, int colStart, int colEnd)
        {
            lock (_lockObj)
            {
                return bitmap.SubMat(rowStart, rowEnd, colStart, colEnd);
            }

        }

        /// <summary>
        /// 处理数据帧
        /// </summary>
        /// <param name="frameNr"></param>
        /// <param name="srcData"></param>
        /// <param name="imgStats"></param>
        /// <param name="embMetaData"></param>
        /// <param name="m_imageBitDepth"></param>
        /// <param name="m_imageSize"></param>
        /// <param name="useParallelProcessing"></param>
        public void ProcessFrame(int frameNr, byte[] srcData, ImageStats imgStats, FrameMetadata embMetaData, int m_imageBitDepth, Size m_imageSize, bool useParallelProcessing)
        {

            byte[] m_imageData = new byte[srcData.Length];

            // Copy the original image data. Use parallel processing for frames
            // larger than 256kB
            if (useParallelProcessing && srcData.Length > 256 * 1024)
            {
                int tN = Math.Min(Environment.ProcessorCount, 8);
                Utils.ParallelBlockCopy(srcData, 0, m_imageData, 0, srcData.Length, tN);
            }
            else
            {
                System.Buffer.BlockCopy(srcData, 0, m_imageData, 0, srcData.Length);
            }

            // By default, we do not contrast-scale the image, just convert to 0-255 without changing the range
            double scaleMin = 0;
            double scaleMax = (1 << m_imageBitDepth) - 1;

            if (imgStats.Min != imgStats.Max)
            {
                scaleMin = imgStats.Min;
                scaleMax = imgStats.Max;
            }

            //日志记录元数据
            ProcessMetadata(frameNr, embMetaData, imgStats);

            //转换图像
            FrameToBMP(m_imageData, m_imageSize, m_imageFormat, scaleMin, scaleMax, m_allowParallelProcessing);
        }

        /// <summary>
        /// 记录元数据
        /// </summary>
        /// <param name="frameNr"></param>
        /// <param name="embMeta"></param>
        /// <param name="imgStats"></param>
        private void ProcessMetadata(int frameNr, FrameMetadata embMeta, ImageStats imgStats)
        {
            StringBuilder frameInfoSb = new StringBuilder();
            frameInfoSb.Clear();
            frameInfoSb.Append("Frame Info");
            frameInfoSb.Append(Environment.NewLine);
            frameInfoSb.Append(" Frame #:  ");
            frameInfoSb.Append(frameNr.ToString());
            frameInfoSb.Append(Environment.NewLine);
            if (imgStats != null)
            {
                frameInfoSb.Append("Image Statistics");
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(string.Format(" Min: {0:0.0}", imgStats.Min));
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(string.Format(" Max: {0:0.0}", imgStats.Max));
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(string.Format(" Avg: {0:0.0}", imgStats.Mean));
                frameInfoSb.Append(Environment.NewLine);
            }
            if (embMeta != null)
            {
                frameInfoSb.Append("Embedded Metadata");
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" Frame #:  ");
                frameInfoSb.Append(embMeta.FrameNr);
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" ROI cnt.: ");
                frameInfoSb.Append(embMeta.RoiCount);
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" BOF tm.:  ");
                frameInfoSb.Append(embMeta.TimeStampBOF);
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" EOF tm.:  ");
                frameInfoSb.Append(embMeta.TimeStampEOF);
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" Readout:  ");
                frameInfoSb.Append(embMeta.TimeStampEOF - embMeta.TimeStampBOF);
                frameInfoSb.Append(Environment.NewLine);
                frameInfoSb.Append(" Exp. tm.: ");
                frameInfoSb.Append(embMeta.ExpTime);
            }
            SetStatusStripMessage(frameInfoSb.ToString());
        }

        /// <summary>
        /// 数据记录
        /// </summary>
        /// <param name="message"></param>
        private void SetStatusStripMessage(string message)
        {
            LogHelper.logSoftWare.Info(message);
        }

        private void tempTimer_Tick()
        {
            try
            {
                while (IsEnabled)
                {
                    lock (_lockObj)
                    {
                        short temp = camCtrl.Temperature;
                        TemperatureUpdate(temp / 100.0);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception exc)
            {
                LogHelper.logSoftWare.Error("Failed to update temperature: " + exc.Message);
                // Stop the timer so we don't keep polling
                IsEnabled = false;
            }
        }

        /// <summary>
        /// 温度刷新
        /// </summary>
        /// <param name="temp"></param>
        private void TemperatureUpdate(double temp)
        {
            lock (_lockObj)
            {
                //if (labelCurTemp.InvokeRequired)
                //{
                //    GuiUpdateTemperatureDelegate d = new GuiUpdateTemperatureDelegate(TemperatureUpdate);
                //    labelCurTemp.BeginInvoke(d, new object[] { temp });
                //}
                //else
                //{
                //    labelCurTemp.Text = string.Format("{0:0.00}", temp);
                //}
            }
        }
    }
}
