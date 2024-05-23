using GalaSoft.MvvmLight.Command;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using System.Windows;
using ximc;
using System;
using Sinboda.Framework.Common.Log;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Messaging;
using Sinboda.SemiAuto.Core.Resources;
using System.Drawing;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using Sinboda.Framework.Control.Controls.Navigation;
using Sinboda.Framework.Core.StaticResource;
using OpenCvSharp;
using Sinboda.SemiAuto.Core.Command;
using System.Linq;
using Sinboda.Framework.Core.Services;
using System.IO;

namespace Sinboda.SemiAuto.View.MachineryDebug.ViewModel
{
    public class MachineryDebugPageViewModel : NavigationViewModelBase
    {
        private XimcHelper ximcController;
        private bool isOpenCamera = false;
        private bool isCameraInitEnable() => !PVCamHelper.Instance.GetInitFlag();
        #region 数据
        /// <summary>
        /// z轴
        /// </summary>
        private XimcArm zaxisMotor;
        public XimcArm ZaxisMotor
        {
            get { return zaxisMotor; }
            set { Set(ref zaxisMotor, value); }
        }

        /// <summary>
        /// 电机列表
        /// </summary>
        private List<MotorData> motorList = new List<MotorData>();
        public List<MotorData> MotorList
        {
            get { return motorList; }
            set { Set(ref motorList, value); }
        }

        /// <summary>
        /// 设备id
        /// </summary>
        private List<XimcArm> _devices;
        public List<XimcArm> Devices
        {
            get { return _devices; }
            set { Set(ref _devices, value); }
        }

        private List<FanData> fanList = new List<FanData>();
        public List<FanData> FanList
        {
            get { return fanList; }
            set { Set(ref fanList, value);}
        }

        /// <summary>
        /// 图像数据
        /// </summary>
        private BitmapSource cameraSouce;
        public BitmapSource CameraSouce
        {
            get { return cameraSouce; }
            set { Set(ref cameraSouce, value); }
        }

        private string cameraButtonText;

        public string CameraButtonText
        {
            get { return cameraButtonText;  }
            set { Set(ref cameraButtonText, value); }
        }

        private string fan1ButtonText;

        public string Fan1ButtonText
        {
            get { return fan1ButtonText; }
            set { Set(ref fan1ButtonText, value); }
        }

        private string fan2ButtonText;

        public string Fan2ButtonText
        {
            get { return fan2ButtonText; }
            set { Set(ref fan2ButtonText, value); }
        }

        private string fan3ButtonText;

        public string Fan3ButtonText
        {
            get { return fan3ButtonText; }
            set { Set(ref fan3ButtonText, value); }
        }
        #endregion

        #region 命令

        #region 电机
        /// <summary>
        /// 电机复位
        /// </summary>
        public RelayCommand<MotorData> ResetCommand { get; set; }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        public RelayCommand<MotorData> ResetLogicalCommand { get; set; }

        /// <summary>
        /// 电机停止
        /// </summary>
        public RelayCommand<MotorData> StopCommand { get; set; }

        /// <summary>
        /// 电机一直移动
        /// </summary>
        public RelayCommand<MotorData> AlwaysCommand { get; set; }

        /// <summary>
        /// 电机设定原点
        /// </summary>
        public RelayCommand<MotorData> SetOriginCommand { get; set; }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        public RelayCommand<MotorData> MoveRelateCommand { get; set; }

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        public RelayCommand<MotorData> MoveAbsoluteCommand { get; set; }

        /// <summary>
        /// 平台电机复位
        /// </summary>
        public RelayCommand PlatformResetCommand { get; set; }

        /// <summary>
        /// 平台电机复位工作原点
        /// </summary>
        public RelayCommand PlatformResetLogicalCommand { get; set; }
        #endregion

        #region z轴
        /// <summary>
        /// 机械原点复位
        /// </summary>
        public RelayCommand<XimcArm> XimcResetCommand { get; set; }

        /// <summary>
        /// 工作原点复位
        /// </summary>
        public RelayCommand<XimcArm> XimcWorkResetCommand { get; set; }

        /// <summary>
        /// 停止
        /// </summary>
        public RelayCommand<XimcArm> XimcStopCommand { get; set; }

        /// <summary>
        /// 一直左移
        /// </summary>
        public RelayCommand<XimcArm> XimcLeftAlwaysCommand { get; set; }

        /// <summary>
        /// 一直右移
        /// </summary>
        public RelayCommand<XimcArm> XimcRightAlwaysCommand { get; set; }

        /// <summary>
        /// 设定原点
        /// </summary>
        public RelayCommand<XimcArm> XimcSetOriginCommand { get; set; }

        /// <summary>
        /// 慢速原点
        /// </summary>
        public RelayCommand<XimcArm> XimcSlowMoveCommand { get; set; }

        /// <summary>
        /// 快速原点
        /// </summary>
        public RelayCommand<XimcArm> XimcFastMoveCommand { get; set; }

        /// <summary>
        /// 保存按钮
        /// </summary>
        public RelayCommand<XimcArm> XimcSaveCommand { get; set; }
        #endregion

        #region 风扇

        /// <summary>
        /// 风扇开关命令
        /// </summary>
        public RelayCommand<FanData> CtrlFanCommand { get; set; }

        #endregion

        /// <summary>
        /// 相机开关命令
        /// </summary>
        public RelayCommand OpenAndCloseCommand { get; set; }

        /// <summary>
        /// 相机初始化命令
        /// </summary>
        public RelayCommand CameraInitCommand { get; set; }
        #endregion

        public MachineryDebugPageViewModel() 
        {
            //界面线程初始化
            DispatcherHelper.Initialize();

            InitXimcCommon();
            InitMotorCommon();
            OpenAndCloseCommand = new RelayCommand(CameraOpenAndClose);
            CameraInitCommand = new RelayCommand(InitCamera, isCameraInitEnable);
            CtrlFanCommand = new RelayCommand<FanData>(FanEnable);

            ChangeButtonText();
            
            Devices = GlobalData.XimcArmsData.XimcArms;
            ximcController = XimcHelper.Instance;

            InitMachinerySource();
        }

        private void InitMotorCommon()
        {
            PlatformResetCommand = new RelayCommand(PlatformResetPhysicalMotor);
            PlatformResetLogicalCommand = new RelayCommand(PlatformResetLogicalMotor);
            ResetCommand = new RelayCommand<MotorData>(ResetPhysicalMotor);
            ResetLogicalCommand = new RelayCommand<MotorData>(ResetLogicalMotor);
            StopCommand = new RelayCommand<MotorData>(StopMotor);
            AlwaysCommand = new RelayCommand<MotorData>(AlawysMove);
            MoveRelateCommand = new RelayCommand<MotorData>(MoveRelate);
            MoveAbsoluteCommand = new RelayCommand<MotorData>(MoveAbsolute);
            SetOriginCommand = new RelayCommand<MotorData>(SetOrigin);
            
        }

        private void InitXimcCommon()
        {
            XimcResetCommand = new RelayCommand<XimcArm>(XimcHome);
            XimcWorkResetCommand = new RelayCommand<XimcArm>(XimcWorkHome);
            XimcStopCommand = new RelayCommand<XimcArm>(XimcStop);
            XimcLeftAlwaysCommand = new RelayCommand<XimcArm>(XimcMoveLeftAlways);
            XimcRightAlwaysCommand = new RelayCommand<XimcArm>(XimcMoveRightAlways);
            XimcSetOriginCommand = new RelayCommand<XimcArm>(XimcLocation);
            XimcSlowMoveCommand = new RelayCommand<XimcArm>(XimcMoveSlow);
            XimcFastMoveCommand = new RelayCommand<XimcArm>(XimcMoveFast);
            XimcSaveCommand = new RelayCommand<XimcArm>(XimcSaveMotroParam);
        }

        private void InitMachinerySource()
        {
            //初始化电机
            MotorList.Clear();
            for (int i = 0; i < 2; i++)
            {
                MotorList.Add(new MotorData() { Id = i});
            }

            //初始化z轴
            if (Devices.Count > 0)
            {
                var zZone = Devices.Where(o => o.CtrlName == SerType.Left_Z);
                if (zZone.Count() > 0)
                {
                    ZaxisMotor = zZone.FirstOrDefault();
                }
            }

            //初始化风扇
            FanList.Clear();
            for (int i = 0; i < 3; i++)
            {
                FanData tempFan = new FanData() { Id = i, State = false };
                FanList.Add(tempFan);
                ChangeFanButtonText(tempFan);
            }
        }

        #region 风扇

        private void FanEnable(FanData obj)
        {
            obj.State = !obj.State;
            CmdFanEnable cmdFanEnable = new CmdFanEnable()
            {
                Id = obj.Id,
                State = obj.State ? 1 : 0
            };

            if (cmdFanEnable.Execute())
            {
                obj.State = !obj.State;
                LogHelper.logSoftWare.Error("FanEnable failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "风扇开关失败"));
            }

            ChangeFanButtonText(obj);
        }

        #endregion

        #region 电机

        /// <summary>
        /// 电机复位机械原点
        /// </summary>
        /// <param name="obj"></param>
        private void PlatformResetPhysicalMotor()
        {
            PlatformResetMotor(0);
        }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        /// <param name="obj"></param>
        private void PlatformResetLogicalMotor()
        {
            PlatformResetMotor(1);
        }

        private void PlatformResetMotor(int isReturnHome)
        {
            CmdPlatformReset cmdPlatformReset = new CmdPlatformReset()
            {
                ReturnHome = isReturnHome
            };

            if (!cmdPlatformReset.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台电机复位失败"));
            }
        }

        /// <summary>
        /// 电机复位机械原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetPhysicalMotor(MotorData obj)
        {
            ResetMotor(obj, 0);
        }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetLogicalMotor(MotorData obj)
        {
            ResetMotor(obj, 1);
        }

        private void ResetMotor(MotorData obj, int isReturnHome) 
        {
            CmdMotorReset cmdMotorReset = new CmdMotorReset()
            {
                Id = obj.Id,
                ReturnHome = isReturnHome
            };

            if (!cmdMotorReset.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机复位失败"));
            }
        }
        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="obj">电机</param>
        private void StopMotor(MotorData obj)
        {
            CmdMoveStop cmdMoveStop = new CmdMoveStop() { Id = obj.Id };

            if (!cmdMoveStop.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机停止失败"));
            }

        }

        /// <summary>
        /// 电机定位原点
        /// </summary>
        /// <param name="obj"></param>
        private void SetOrigin(MotorData obj)
        {
            CmdSetMotorParam cmdSetMotorParam = new CmdSetMotorParam()
            {
                Id = obj.Id,
                ParamID = 2,
                ParamValue = obj.OriginPoint
            };

            if (!cmdSetMotorParam.Execute())
            {
                LogHelper.logSoftWare.Error("SetOrigin failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机定位原点失败"));
            }
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void AlawysMove(MotorData obj)
        {
            CmdMoveCon cmdMoveCon = new CmdMoveCon() 
            { 
                Id = obj.Id,
                Dir = obj.Dir,
                UseFastSpeed = obj.UseFastSpeed
            };

            if (!cmdMoveCon.Execute())
            {
                LogHelper.logSoftWare.Error("AlawysMove failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机持续移动失败"));
            }
        }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void MoveRelate(MotorData obj)
        {
            CmdMoveRelate cmdMoveRelate = new CmdMoveRelate()
            {
                Id = obj.Id,
                Steps = obj.Steps
            };

            if (!cmdMoveRelate.Execute())
            {
                LogHelper.logSoftWare.Error("MoveRelate failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机相对移动失败"));
            }
        }

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void MoveAbsolute(MotorData obj)
        {
            CmdMoveAbsolute cmdMoveAbsolute = new CmdMoveAbsolute()
            {
                Id = obj.Id,
                TargetPos = obj.TargetPos
            };

            if (!cmdMoveAbsolute.Execute())
            {
                LogHelper.logSoftWare.Error("MoveAbsolute failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机绝对移动失败"));
            }
        }
        #endregion

        #region ximc

        /// <summary>
        /// 保存电机参数
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void XimcSaveMotroParam(XimcArm obj)
        {
           
            XimcHelper.Instance.Set_Move_Settings(obj);
            XimcHelper.Instance.Set_Controller_Name(obj);
            XimcHelper.Instance.Command_Save_Settings(obj);

            GlobalData.Save();
        }

        /// <summary>
        /// 保存电机参数
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void XimcSaveParam()
        {
            foreach (var item in Devices)
            {
                XimcHelper.Instance.Set_Move_Settings(item);
                XimcHelper.Instance.Set_Controller_Name(item);
                XimcHelper.Instance.Command_Save_Settings(item);
            }
            GlobalData.Save();
        }

        /// <summary>
        /// 将当前位置 设置为原点
        /// </summary>
        /// <param name="obj"></param>
        private void XimcLocation(XimcArm obj)
        {
            ximcController.Cmd_Zero(obj.DeveiceId);
        }

        /// <summary>
        /// 机械复位指令
        /// </summary>
        /// <param name="obj"></param>
        private void XimcHome(XimcArm obj)
        {
            CmdZResetPhysical cmdZReset = new CmdZResetPhysical() { arm = obj };

            if (!cmdZReset.Execute())
            {
                LogHelper.logSoftWare.Error("XimcHome failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机复位失败"));
            }
        }

        /// <summary>
        /// 机械复位指令
        /// </summary>
        /// <param name="obj"></param>
        private void XimcWorkHome(XimcArm obj)
        {
            CmdZResetLogical cmdZReset = new CmdZResetLogical() { arm = obj };

            if (!cmdZReset.Execute())
            {
                LogHelper.logSoftWare.Error("XimcHome failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机复位失败"));
            }
        }

        /// <summary>
        /// 移动指令
        /// </summary>
        /// <param name="obj"></param>
        private void XimcMoveSlow(XimcArm obj)
        {
            CmdZSlowMove cmdZSlowMove = new CmdZSlowMove() 
            { 
                arm = obj, 
                pos = obj.Postion 
            };

            if (!cmdZSlowMove.Execute())
            {
                LogHelper.logSoftWare.Error("XimcMoveSlow failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机慢速移动失败"));
            }
        }

        /// <summary>
        /// 移动指令
        /// </summary>
        /// <param name="obj"></param>
        private void XimcMoveFast(XimcArm obj)
        {
            CmdZFastMove cmdZFastMove = new CmdZFastMove()
            {
                arm = obj,
                pos = obj.Postion
            };

            if (!cmdZFastMove.Execute())
            {
                LogHelper.logSoftWare.Error("XimcMoveFast failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机快速停止失败"));
            }
        }

        /// <summary>
        /// 全部定位原点
        /// </summary>
        private void XimcLocationAll()
        {
            foreach (var item in Devices)
            {
                ximcController.Cmd_Zero(item.DeveiceId);
            }
        }

        /// <summary>
        /// 所有电机复位
        /// </summary>
        /// <param name="obj"></param>
        private void XimcResetAll()
        {
            foreach (var item in Devices)
            {
                ximcController.Cmd_Home(item.DeveiceId);
            }
        }

        /// <summary>
        /// 停止移动
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void XimcStop(XimcArm obj)
        {

            ximcController.Cmd_SlowStop(obj.DeveiceId);
        }

        /// <summary>
        /// 一直左移
        /// </summary>
        /// <param name="obj"></param>
        private void XimcMoveLeftAlways(XimcArm obj)
        {
            ximcController.Cmd_Left(obj.DeveiceId);
        }

        /// <summary>
        /// 一直右移
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void XimcMoveRightAlways(XimcArm obj)
        {
            ximcController.Cmd_Right(obj.DeveiceId);
        }

        #endregion

        private void InitCamera()
        {
            PVCamHelper.Instance.Init();
        }

        /// <summary>
        /// 开关相机
        /// </summary>
        private void CameraOpenAndClose()
        {
            if (isOpenCamera)
            {
                PVCamHelper.Instance.Dispose();
                isOpenCamera = false;
                ChangeButtonText();
            }
            else
            {
                PVCamHelper.Instance.StartCont();
                isOpenCamera = true;
                ChangeButtonText();
            }
        }

        /// <summary>
        /// 开关相机
        /// </summary>
        private void CameraPause()
        {
            if (isOpenCamera)
            {
                PVCamHelper.Instance.Pause();
            }
        }

        /// <summary>
        /// 相机开关文言调整
        /// </summary>
        private void ChangeButtonText()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                CameraButtonText = isOpenCamera ? SystemResources.Instance.GetLanguage(0, "关闭相机") : SystemResources.Instance.GetLanguage(0, "打开相机");
            });
        }

        /// <summary>
        /// 风扇开关文言调整
        /// </summary>
        private void ChangeFanButtonText(FanData obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                string tempFanText = obj.State ? SystemResources.Instance.GetLanguage(0, "关闭风扇") : SystemResources.Instance.GetLanguage(0, "打开风扇");
                switch (obj.Id) 
                {
                    case 0:
                        Fan1ButtonText = tempFanText;
                        break;
                    case 1:
                        Fan2ButtonText = tempFanText;
                        break;
                    case 2:
                        Fan3ButtonText = tempFanText;
                        break;
                }
            });
        }

        /// <summary>
        /// 进入页面时触发
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnParameterChanged(object parameter)
        {
            // 注册刷新消息
            Messenger.Default.Register<object>(this, MessageToken.TokenCamera, ImageRefersh);
        }

        public void ImageRefersh(object bitmap)
        {
            var bmp = bitmap as Bitmap;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                CameraSouce = bmp.ToBitmapSource();
            });
        }

        /// <summary>
        /// 离开页面时触发
        /// </summary>
        /// <param name="source"></param>
        /// <param name="mode"></param>
        /// <param name="navigationState"></param>
        /// <returns></returns>
        protected override bool NavigatedFrom(object source, NavigationMode mode, object navigationState)
        {
            if (isOpenCamera)
            {
                PVCamHelper.Instance.Dispose();
                isOpenCamera = false;
                ChangeButtonText();
            }

            // 离开页面时删除刷新消息
            Messenger.Default.Unregister<object>(this, MessageToken.TokenCamera, ImageRefersh);
            return base.NavigatedFrom(source, mode, navigationState);
        }
    }
}
