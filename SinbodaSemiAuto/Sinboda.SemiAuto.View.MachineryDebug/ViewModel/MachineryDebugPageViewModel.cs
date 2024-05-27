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
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Business.Items;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using sin_mole_flu_analyzer.Models.Command;
using Sinboda.SemiAuto.Core.Models.Common;

namespace Sinboda.SemiAuto.View.MachineryDebug.ViewModel
{
    public class MachineryDebugPageViewModel : NavigationViewModelBase
    {
        private XimcHelper ximcController;
        private bool isOpenCamera = false;
        private bool isCameraInitEnable() => !PVCamHelper.Instance.GetInitFlag();
        #region 数据

        /// <summary>
        /// 三排孔位
        /// </summary>
        private List<int> rackSouce = new List<int>() { 1, 2, 3 };
        public List<int> RackSouce
        {
            get { return rackSouce; }
            set { Set(ref rackSouce, value); }
        }

        /// <summary>
        /// 十个孔位
        /// </summary>
        private List<int> posSouce = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<int> PosSouce
        {
            get { return posSouce; }
            set { Set(ref posSouce, value); }
        }

        /// <summary>
        /// 架号
        /// </summary>
        private int rackDish;

        public int RackDish
        {
            get { return rackDish; }
            set { Set(ref rackDish, value); }
        }

        /// <summary>
        /// 位置
        /// </summary>
        private int position;

        public int Position
        {
            get { return position; }
            set { Set(ref position, value); }
        }

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
        private List<Sin_Motor> motorList = new List<Sin_Motor>();
        public List<Sin_Motor> MotorList
        {
            get { return motorList; }
            set { Set(ref motorList, value); }
        }

        /// <summary>
        /// Z轴设备
        /// </summary>
        private List<XimcArm> _devices;
        public List<XimcArm> Devices
        {
            get { return _devices; }
            set { Set(ref _devices, value); }
        }

        /// <summary>
        /// 风扇列表
        /// </summary>
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

        /// <summary>
        /// 相机按钮文言
        /// </summary>
        private string cameraButtonText;

        public string CameraButtonText
        {
            get { return cameraButtonText;  }
            set { Set(ref cameraButtonText, value); }
        }

        #region 风扇按钮文言

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

        #endregion

        /// <summary>
        /// X轴位置
        /// </summary>
        private int posXaxis;
        public int PosXaxis
        {
            get { return posXaxis; }
            set { Set(ref posXaxis, value); }
        }

        /// <summary>
        /// Y轴位置
        /// </summary>
        private int posYaxis;
        public int PosYaxis
        {
            get { return posYaxis; }
            set { Set(ref posYaxis, value); }
        }

        /// <summary>
        /// Y轴位置
        /// </summary>
        private int originXaxis;
        public int OriginXaxis
        {
            get { return originXaxis; }
            set { Set(ref originXaxis, value); }
        }

        /// <summary>
        /// Y轴位置
        /// </summary>
        private int originYaxis;
        public int OriginYaxis
        {
            get { return originYaxis; }
            set { Set(ref originYaxis, value); }
        }

        #region 命令

        #region 平台

        /// <summary>
        /// 平台电机复位
        /// </summary>
        public RelayCommand PlatformResetCommand { get; set; }

        /// <summary>
        /// 平台电机复位工作原点
        /// </summary>
        public RelayCommand PlatformResetLogicalCommand { get; set; }

        /// <summary>
        /// 平台移动
        /// </summary>
        public RelayCommand PlatformMoveCommand { get; set; }

        /// <summary>
        /// 平台移动
        /// </summary>
        public RelayCommand PlatformStopCommand { get; set; }

        /// <summary>
        /// 设置点位
        /// </summary>
        public RelayCommand<string> SetCellCommand { get; set; }

        /// <summary>
        /// 计算点位
        /// </summary>
        public RelayCommand CalCellCommand { get; set; }

        /// <summary>
        /// 计算点位
        /// </summary>
        public RelayCommand MoveCellCommand { get; set; }

        #endregion

        #region 电机
        /// <summary>
        /// 电机复位
        /// </summary>
        public RelayCommand<Sin_Motor> ResetCommand { get; set; }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        public RelayCommand<Sin_Motor> ResetLogicalCommand { get; set; }

        /// <summary>
        /// 电机停止
        /// </summary>
        public RelayCommand<Sin_Motor> StopCommand { get; set; }

        /// <summary>
        /// 电机一直移动
        /// </summary>
        public RelayCommand<Sin_Motor> AlwaysCommand { get; set; }

        /// <summary>
        /// 电机设定原点
        /// </summary>
        public RelayCommand<Sin_Motor> SetOriginCommand { get; set; }

        /// <summary>
        /// 电机相对移动
        /// </summary>
        public RelayCommand<Sin_Motor> MoveRelateCommand { get; set; }

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        public RelayCommand<Sin_Motor> MoveAbsoluteCommand { get; set; }

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
            InitPlatformCommon();
            OpenAndCloseCommand = new RelayCommand(CameraOpenAndClose);
            CameraInitCommand = new RelayCommand(InitCamera, isCameraInitEnable);
            CtrlFanCommand = new RelayCommand<FanData>(FanEnable);

            ChangeButtonText();
            
            Devices = GlobalData.XimcArmsData.XimcArms;
            ximcController = XimcHelper.Instance;

            InitMachinerySource();
        }

        /// <summary>
        /// 初始化平台指令
        /// </summary>
        private void InitPlatformCommon()
        {
            PlatformResetCommand = new RelayCommand(PlatformResetPhysicalMotor);
            PlatformResetLogicalCommand = new RelayCommand(PlatformResetLogicalMotor);
            SetCellCommand = new RelayCommand<string>(SetCell);
            CalCellCommand = new RelayCommand(CalCell);
            MoveCellCommand = new RelayCommand(MoveCell);
            PlatformMoveCommand = new RelayCommand(PlatformMove);
            PlatformStopCommand = new RelayCommand(PlatformStop);
        }

        /// <summary>
        /// 初始化电机指令
        /// </summary>
        private void InitMotorCommon()
        {
            ResetCommand = new RelayCommand<Sin_Motor>(ResetPhysicalMotor);
            ResetLogicalCommand = new RelayCommand<Sin_Motor>(ResetLogicalMotor);
            StopCommand = new RelayCommand<Sin_Motor>(StopMotor);
            AlwaysCommand = new RelayCommand<Sin_Motor>(AlawysMove);
            MoveRelateCommand = new RelayCommand<Sin_Motor>(MoveRelate);
            MoveAbsoluteCommand = new RelayCommand<Sin_Motor>(MoveAbsolute);
            SetOriginCommand = new RelayCommand<Sin_Motor>(SetOrigin);
            
        }

        /// <summary>
        /// 初始化Z轴指令
        /// </summary>
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

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitMachinerySource()
        {
            RackDish = RackSouce[0];
            Position = PosSouce[0];

            //初始化电机
            MotorList.Clear();

            MotorList = MotorBusiness.Instance.GetMotorList();
            if (MotorList.Count() != 2)
            {
                MotorList.Clear();
                for (int i = 0; i < 2; i++)
                {
                    MotorList.Add(new Sin_Motor() { MotorId = (MotorId)i });
                }
            }
            else
            {
                foreach (var motorItem in MotorList)
                {
                    switch (motorItem.MotorId)
                    {
                        case MotorId.Xaxis:
                            {
                                PosXaxis = motorItem.TargetPos;
                                OriginXaxis = motorItem.OriginPoint;
                            }
                            break;
                        case MotorId.Yaxis:
                            {
                                PosYaxis = motorItem.TargetPos;
                                OriginYaxis = motorItem.OriginPoint;
                            }
                            break;
                    }
                }
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

        #region 平台

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
            else
            {
                ResPlatformReset resPlatformReset = cmdPlatformReset.GetResponse() as ResPlatformReset;
                PosXaxis = resPlatformReset.CurrPosX;
                PosYaxis = resPlatformReset.CurrPosY;
            }
        }

        /// <summary>
        /// 平台移动
        /// </summary>
        private void PlatformMove()
        {
            CmdPlatformMove cmdPlatformMove = new CmdPlatformMove()
            {
                X = PosXaxis,
                Y = PosYaxis
            };

            if (cmdPlatformMove.Execute())
            {
                ResPlatformReset resPlatformReset = cmdPlatformMove.GetResponse() as ResPlatformReset;
                PosXaxis = resPlatformReset.CurrPosX;
                PosYaxis = resPlatformReset.CurrPosY;
            }
            else
            {
                LogHelper.logSoftWare.Error("PlatformMove failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台移动失败"));
            }
        }

        /// <summary>
        /// 平台停止
        /// </summary>
        private void PlatformStop()
        {
            CmdPlatformStop cmdPlatformStop = new CmdPlatformStop();

            if (cmdPlatformStop.Execute())
            {
                ResPlatformReset resPlatformReset = cmdPlatformStop.GetResponse() as ResPlatformReset;
                PosXaxis = resPlatformReset.CurrPosX;
                PosYaxis = resPlatformReset.CurrPosY;
            }
            else
            {
                LogHelper.logSoftWare.Error("PlatformsStop failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台停止失败"));
            }
        }

        private void SetCell(string strIndex)
        {
            int index = 0;
            if (!int.TryParse(strIndex, out index))
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "参数设置错误"));
            }

            Sin_Cell cellItem = CellBusiness.Instance.GetCellByIndex(index);
            if (cellItem == null)
            {
                cellItem = new Sin_Cell();
                cellItem.Id = Guid.NewGuid();
                cellItem.Index = index;
                cellItem.X = PosXaxis;
                cellItem.Y = PosYaxis;
                cellItem.Z = ZaxisMotor.Postion;
            }
            else
            {
                cellItem.X = PosXaxis;
                cellItem.Y = PosYaxis;
                cellItem.Z = ZaxisMotor.Postion;
            }

            if (!CellBusiness.Instance.SaveCell(cellItem))
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "点位设置失败"));
            }
        }

        private void CalCell()
        {
            if (!CellBusiness.Instance.SetCellDic())
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "点位计算失败"));
            }
            else
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "点位计算完成"));
            }
        }

        private void MoveCell()
        {
            string strPos = RackDish + "-" + Position;
            Sin_Cell cellItem = CellBusiness.Instance.GetCell(strPos);

            if (cellItem == null)
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "没有计算过点位"));
                return;
            }
            ZaxisMotor.Postion = 3000;//写入配置
            XimcMoveFast(ZaxisMotor);

            CmdPlatformMove cmdPlatformMove = new CmdPlatformMove()
            {
                X = cellItem.X,
                Y = cellItem.Y,
            };

            if (cmdPlatformMove.Execute())
            {
                ResPlatformReset res = cmdPlatformMove.GetResponse() as ResPlatformReset;
                if (res != null) 
                {
                    PosXaxis = res.CurrPosX; 
                    PosYaxis = res.CurrPosY;
                }
            }
            else
            {
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台移动失败"));
                return;
            }

            ZaxisMotor.Postion = cellItem.Z;
            XimcMoveFast(ZaxisMotor);
        }

        #endregion

        #region 电机

        /// <summary>
        /// 电机复位机械原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetPhysicalMotor(Sin_Motor obj)
        {
            ResetMotor(obj, 0);
        }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetLogicalMotor(Sin_Motor obj)
        {
            ResetMotor(obj, 1);
        }

        /// <summary>
        /// 复位动作
        /// </summary>
        /// <param name="obj">电机对象</param>
        /// <param name="isReturnHome">是否为机械原点</param>
        private void ResetMotor(Sin_Motor obj, int isReturnHome) 
        {
            CmdMotorReset cmdMotorReset = new CmdMotorReset()
            {
                Id = (int)obj.MotorId,
                ReturnHome = isReturnHome
            };

            if (!cmdMotorReset.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机复位失败"));
            }
            else
            {
                ResMove resMove =  cmdMotorReset.GetResponse() as ResMove;

                ChangeTextBoxText(obj.MotorId, resMove);

                obj.TargetPos = resMove.CurPos;
                MotorBusiness.Instance.SaveMotorItem(obj);
            }
        }
        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="obj">电机</param>
        private void StopMotor(Sin_Motor obj)
        {
            CmdMoveStop cmdMoveStop = new CmdMoveStop() { Id = (int)obj.MotorId };

            if (!cmdMoveStop.Execute())
            {
                LogHelper.logSoftWare.Error("StopMotor failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机停止失败"));
            }
            else
            {
                ResMove resMove = cmdMoveStop.GetResponse() as ResMove;
                ChangeTextBoxText(obj.MotorId, resMove);
            }

        }

        /// <summary>
        /// 电机定位原点
        /// </summary>
        /// <param name="obj"></param>
        private void SetOrigin(Sin_Motor obj)
        {
            CmdSetMotorParam cmdSetMotorParam = new CmdSetMotorParam()
            {
                Id = (int)obj.MotorId,
                ParamID = 2,
                ParamValue = obj.TargetPos
            };

            if (!cmdSetMotorParam.Execute())
            {
                LogHelper.logSoftWare.Error("SetOrigin failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机定位原点失败"));
            }
            else
            {
                switch (obj.MotorId)
                {
                    case MotorId.Xaxis:
                        {
                            OriginXaxis = PosXaxis;
                            obj.OriginPoint = PosXaxis;
                        }
                        break;
                    case MotorId.Yaxis:
                        {
                            OriginYaxis = PosYaxis;
                            obj.OriginPoint = PosYaxis;
                        }
                        break;
                }

                MotorBusiness.Instance.SaveMotorItem(obj);
            }
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void AlawysMove(Sin_Motor obj)
        {
            CmdMoveCon cmdMoveCon = new CmdMoveCon() 
            { 
                Id = (int)obj.MotorId,
                Dir = (int)obj.Dir,
                UseFastSpeed = (int)obj.UseFastSpeed
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
        private void MoveRelate(Sin_Motor obj)
        {
            CmdMoveRelate cmdMoveRelate = new CmdMoveRelate()
            {
                Id = (int)obj.MotorId,
                Steps = obj.Steps
            };

            if (!cmdMoveRelate.Execute())
            {
                LogHelper.logSoftWare.Error("MoveRelate failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机相对移动失败"));
            }
            else
            {
                ResMove resMove = cmdMoveRelate.GetResponse() as ResMove;
                ChangeTextBoxText(obj.MotorId, resMove);

                obj.TargetPos = resMove.CurPos;
                MotorBusiness.Instance.SaveMotorItem(obj);
            }
        }

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void MoveAbsolute(Sin_Motor obj)
        {
            obj.TargetPos = obj.MotorId == MotorId.Xaxis ? PosXaxis : PosYaxis;

            CmdMoveAbsolute cmdMoveAbsolute = new CmdMoveAbsolute()
            {
                Id = (int)obj.MotorId,
                TargetPos = obj.TargetPos
            };

            if (!cmdMoveAbsolute.Execute())
            {
                LogHelper.logSoftWare.Error("MoveAbsolute failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机绝对移动失败"));
            }
            else
            {
                ResMove resMove = cmdMoveAbsolute.GetResponse() as ResMove;
                ChangeTextBoxText(obj.MotorId, resMove);

                obj.TargetPos = resMove.CurPos;
                MotorBusiness.Instance.SaveMotorItem(obj);
            }
        }

        /// <summary>
        /// 改变界面位置显示内容
        /// </summary>
        /// <param name="motorId"></param>
        /// <param name="resMove"></param>
        private void ChangeTextBoxText(MotorId motorId, ResMove resMove)
        {
            switch (motorId)
            {
                case MotorId.Xaxis:
                    {
                        PosXaxis = resMove.CurPos;
                    }
                    break;
                case MotorId.Yaxis:
                    {
                        PosYaxis = resMove.CurPos;
                    }
                    break;
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
        /// 将当前位置 设置为原点
        /// </summary>
        /// <param name="obj"></param>
        private void XimcLocation(XimcArm obj)
        {
            obj.Originption = obj.Postion;
            GlobalData.Save();
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
        /// 工作原点复位指令
        /// </summary>
        /// <param name="obj"></param>
        private void XimcWorkHome(XimcArm obj)
        {
            CmdZResetLogical cmdZReset = new CmdZResetLogical() { arm = obj, pos = obj.Originption };

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

        #region 相机
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

        #endregion

        #region 风扇

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

        #endregion

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
