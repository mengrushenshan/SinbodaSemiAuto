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
using System.Threading;
using System.Windows.Documents;
using Sinboda.SemiAuto.View.MachineryDebug.WinView;
using System.Windows.Threading;
using static OpenCvSharp.Stitcher;
using System.Windows.Forms;
using Sinboda.Framework.Common;
using System.Threading.Tasks;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Control.Loading;
using System.Windows.Media.Media3D;
using System.Runtime.InteropServices;
using System.Windows.Shapes;
using System.Collections;
using System.Runtime.InteropServices.ComTypes;
using Sinboda.SemiAuto.TestFlow;

namespace Sinboda.SemiAuto.View.MachineryDebug.ViewModel
{
    public class MachineryDebugPageViewModel : NavigationViewModelBase
    {
        private bool isOpenCamera = false;
        private const int originalSize = 2048;
        private const int originalBinnerSize = 1024;
        private const int showSize = 512;
        private const int focusMoveStep = 64;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly static object objLock = new object();

        /// <summary>
        /// 点位对应图像
        /// </summary>
        private List<byte[]> tifList = new List<byte[]>();
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
        /// 电机列表
        /// </summary>
        private List<Sin_Motor> motorList = new List<Sin_Motor>();
        public List<Sin_Motor> MotorList
        {
            get { return motorList; }
            set { Set(ref motorList, value); }
        }

        /// <summary>
        /// 风扇列表
        /// </summary>
        private List<FanData> fanList = new List<FanData>();
        public List<FanData> FanList
        {
            get { return fanList; }
            set { Set(ref fanList, value); }
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
            get { return cameraButtonText; }
            set { Set(ref cameraButtonText, value); }
        }

        private double voltage = 0.5;
        public double Voltage
        {
            get { return voltage; }
            set { Set(ref voltage, value); }
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
        /// Z轴位置
        /// </summary>
        private int posZaxis;
        public int PosZaxis
        {
            get { return posZaxis; }
            set { Set(ref posZaxis, value); }
        }

        /// <summary>
        /// Z轴步数
        /// </summary>
        private int stepZaxis;
        public int StepZaxis
        {
            get { return stepZaxis; }
            set { Set(ref stepZaxis, value); }
        }

        /// <summary>
        /// X轴位置
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
        
        /// <summary>
        /// Z轴位置
        /// </summary>
        private int originZaxis;
        public int OriginZaxis
        {
            get { return originZaxis; }
            set { Set(ref originZaxis, value); }
        }

        private bool isCameraInitEnable;
        public bool IsCameraInitEnable
        {
            get { return isCameraInitEnable; }
            set { Set(ref isCameraInitEnable, value); }
        }

        private bool isCameraOpenEnable;
        public bool IsCameraOpenEnable
        {
            get { return isCameraOpenEnable; }
            set { Set(ref isCameraOpenEnable, value); }
        }

        /// <summary>
        /// 升级文件
        /// </summary>
        private string upgradeFile;

        public string UpgradeFile
        {
            get { return upgradeFile; }
            set { Set(ref upgradeFile, value); }
        }

        /// <summary>
        /// 聚焦起始位置
        /// </summary>
        private int focusBeginPos;
        public int FocusBeginPos
        {
            get { return focusBeginPos; }
            set { Set(ref focusBeginPos, value); }
        }

        /// <summary>
        /// 聚焦起始位置
        /// </summary>
        private int focusEndPos;
        public int FocusEndPos
        {
            get { return focusEndPos; }
            set { Set(ref focusEndPos, value); }
        }

        private System.Windows.Point PointBegin = new System.Windows.Point { X = -1, Y = -1 };
        private bool NeedRoi = false;
        #endregion

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
        /// 平台老化
        /// </summary>
        public RelayCommand PlatformAgingCommand { get; set; }
        /// <summary>
        /// 老化停止
        /// </summary>
        public RelayCommand AgingStopCommand { get; set; }

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
        public RelayCommand<Sin_Motor> XimcResetCommand { get; set; }

        /// <summary>
        /// 工作原点复位
        /// </summary>
        public RelayCommand<Sin_Motor> XimcWorkResetCommand { get; set; }

        /// <summary>
        /// 停止
        /// </summary>
        public RelayCommand<Sin_Motor> XimcStopCommand { get; set; }

        /// <summary>
        /// 一直左移
        /// </summary>
        public RelayCommand<Sin_Motor> XimcLeftAlwaysCommand { get; set; }

        /// <summary>
        /// 一直右移
        /// </summary>
        public RelayCommand<Sin_Motor> XimcRightAlwaysCommand { get; set; }

        /// <summary>
        /// 一直右移
        /// </summary>
        public RelayCommand<Sin_Motor> XimcMoveRelativeCommand { get; set; }

        /// <summary>
        /// 设定原点
        /// </summary>
        public RelayCommand<Sin_Motor> XimcSetOriginCommand { get; set; }

        /// <summary>
        /// 慢速原点
        /// </summary>
        public RelayCommand<Sin_Motor> XimcSlowMoveCommand { get; set; }

        /// <summary>
        /// 快速原点
        /// </summary>
        public RelayCommand<Sin_Motor> XimcFastMoveCommand { get; set; }

        /// <summary>
        /// 保存按钮
        /// </summary>
        public RelayCommand<Sin_Motor> XimcSaveCommand { get; set; }
        #endregion

        #region 风扇

        /// <summary>
        /// 风扇开关命令
        /// </summary>
        public RelayCommand<FanData> CtrlFanCommand { get; set; }

        #endregion

        #region 相机

        /// <summary>
        /// 相机开关命令
        /// </summary>
        public RelayCommand OpenAndCloseCommand { get; set; }

        /// <summary>
        /// 相机初始化命令
        /// </summary>
        public RelayCommand CameraInitCommand { get; set; }

        /// <summary>
        /// 大图展示
        /// </summary>
        public RelayCommand BigImageCommand { get; set; }

        /// <summary>
        /// 大图展示
        /// </summary>
        public RelayCommand FocusCommand { get; set; }

        /// <summary>
        /// 设置ROI区域
        /// </summary>
        public RelayCommand SetRoiCommand { get; set; }

        /// <summary>
        /// 重置ROI区域
        /// </summary>
        public RelayCommand ReSetRoiCommand { get; set; }

        /// <summary>
        /// 保存图片
        /// </summary>
        public RelayCommand SaveTiffCommand { get; set; }

        /// <summary>
        /// 保存图片
        /// </summary>
        public RelayCommand PointCollectCommand { get; set; }
        #endregion

        #region 激光器

        /// <summary>
        /// 相机开关命令
        /// </summary>
        public RelayCommand OpenLightCommand { get; set; }

        /// <summary>
        /// 相机初始化命令
        /// </summary>
        public RelayCommand CloseLightCommand { get; set; }

        #endregion

        /// <summary>
        /// 浏览命令
        /// </summary>
        public RelayCommand BrowseCommand { get; set; }

        /// <summary>
        /// 升级命令
        /// </summary>
        public RelayCommand UpgradeCommand { get; set; }

        #endregion

        public MachineryDebugPageViewModel()
        {
            //界面线程初始化
            DispatcherHelper.Initialize();

            InitXimcCommon();
            InitMotorCommon();
            InitPlatformCommon();
            OpenAndCloseCommand = new RelayCommand(CameraOpenAndClose);
            CameraInitCommand = new RelayCommand(InitCamera);
            BigImageCommand = new RelayCommand(BigImageShow);
            FocusCommand = new RelayCommand(CameraFocus);
            SetRoiCommand = new RelayCommand(SetCameraRoi);
            ReSetRoiCommand = new RelayCommand(SetInitRoi);
            SaveTiffCommand = new RelayCommand(SaveTiff);
            PointCollectCommand = new RelayCommand(Point100ImageCollect);

            CtrlFanCommand = new RelayCommand<FanData>(FanEnable);
            OpenLightCommand = new RelayCommand(OpenLight);
            CloseLightCommand = new RelayCommand(CloseLight);

            BrowseCommand = new RelayCommand(BrowseFile);
            UpgradeCommand = new RelayCommand(UpgradeBoard);
            ChangeButtonText();

            InitMachinerySource();
        }

        /// <summary>
        /// 是否仪器空闲
        /// </summary>
        public bool IsIdle
        {
            get { return isIdle; }
            set { Set(ref isIdle, value); }
        }
        private bool isIdle = true;

        private void InvokeAsync(Action act)
        {
            try
            {
                IsIdle = false;
                Task.Run(() =>
                {
                    act.Invoke();
                    IsIdle = true;
                });
            }
            catch (Exception ex)
            {
                IsIdle = true;
            }
            finally
            {
                //IsIdle = true;
            }
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
            PlatformAgingCommand = new RelayCommand(PlatformAging);
            AgingStopCommand = new RelayCommand(AgingStop);
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
            XimcResetCommand = new RelayCommand<Sin_Motor>(ResetPhysicalMotor);
            XimcWorkResetCommand = new RelayCommand<Sin_Motor>(ResetLogicalMotor);
            XimcStopCommand = new RelayCommand<Sin_Motor>(StopMotor);
            XimcLeftAlwaysCommand = new RelayCommand<Sin_Motor>(AlawysMove);
            XimcRightAlwaysCommand = new RelayCommand<Sin_Motor>(AlawysMove);
            XimcMoveRelativeCommand = new RelayCommand<Sin_Motor>(MoveRelate);
            XimcSetOriginCommand = new RelayCommand<Sin_Motor>(SetOrigin);
            XimcSlowMoveCommand = new RelayCommand<Sin_Motor>(MoveAbsolute);
            XimcFastMoveCommand = new RelayCommand<Sin_Motor>(MoveAbsolute);
            //XimcSaveCommand = new RelayCommand<Sin_Motor>(XimcSaveMotroParam);
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
            if (MotorList.Count() != 3)
            {
                MotorList.Clear();
                for (int i = 0; i < 3; i++)
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
                        case MotorId.Zaxis:
                            {
                                PosZaxis = motorItem.TargetPos;
                                OriginZaxis = motorItem.OriginPoint;
                            }
                            break;
                    }
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
            //加载py运动环境
            int cellNum = PyHelper.DataAnalyze("D:\\Sinboda\\simoa\\python\\py\\ProjectData\\AD_20240620_0001", 'A', 1);
            Console.WriteLine("cell number is: " + cellNum);

            obj.State = !obj.State;
            CmdFanEnable cmdFanEnable = new CmdFanEnable()
            {
                Id = obj.Id,
                Enable = obj.State ? 1 : 0
            };

            if (!cmdFanEnable.Execute())
            {
                obj.State = !obj.State;
                LogHelper.logSoftWare.Error("FanEnable failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "风扇开关失败"));
            }

            ChangeFanButtonText(obj);
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

        /// <summary>
        /// 平台复位
        /// </summary>
        /// <param name="isReturnHome"></param>
        private void PlatformResetMotor(int isReturnHome)
        {
            InvokeAsync(() =>
            {
                CmdPlatformReset cmdPlatformReset = new CmdPlatformReset()
                {
                    ReturnHome = isReturnHome,
                    CloseLaser = 0
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
                    PosZaxis = resPlatformReset.CurrPosZ;
                }
            });
        }

        /// <summary>
        /// 平台移动
        /// </summary>
        private void PlatformMove()
        {
            InvokeAsync(() =>
            {
                CmdPlatformMove cmdPlatformMove = new CmdPlatformMove()
                {
                    X = PosXaxis,
                    Y = PosYaxis,
                    Z = PosZaxis
                };

                if (cmdPlatformMove.Execute())
                {
                    ResPlatformReset resPlatformReset = cmdPlatformMove.GetResponse() as ResPlatformReset;
                    PosXaxis = resPlatformReset.CurrPosX;
                    PosYaxis = resPlatformReset.CurrPosY;
                    PosZaxis = resPlatformReset.CurrPosZ;
                }
                else
                {
                    LogHelper.logSoftWare.Error("PlatformMove failed");
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台移动失败"));
                }
            });
        }

        /// <summary>
        /// 平台停止
        /// </summary>
        private void PlatformStop()
        {
            InvokeAsync(() =>
            {
                CmdPlatformStop cmdPlatformStop = new CmdPlatformStop();

                if (cmdPlatformStop.Execute())
                {
                    ResPlatformReset resPlatformReset = cmdPlatformStop.GetResponse() as ResPlatformReset;
                    PosXaxis = resPlatformReset.CurrPosX;
                    PosYaxis = resPlatformReset.CurrPosY;
                    PosZaxis = resPlatformReset.CurrPosZ;
                }
                else
                {
                    LogHelper.logSoftWare.Error("PlatformsStop failed");
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台停止失败"));
                }
            });
        }

        /// <summary>
        /// 设置点位
        /// </summary>
        /// <param name="strIndex"></param>
        private void SetCell(string strIndex)
        {
            InvokeAsync(() =>
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
                    cellItem.Z = PosZaxis;
                }
                else
                {
                    cellItem.X = PosXaxis;
                    cellItem.Y = PosYaxis;
                    cellItem.Z = PosZaxis;
                }

                if (!CellBusiness.Instance.SaveCell(cellItem))
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "点位设置失败"));
                }
            });
        }

        /// <summary>
        /// 计算点位
        /// </summary>
        private void CalCell()
        {
            InvokeAsync(() =>
            {
                if (!CellBusiness.Instance.SetCellDic())
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "点位计算失败"));
                }
                else
                {
                    NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "点位计算完成"));
                }
            });
        }

        /// <summary>
        /// 移动到指定点位
        /// </summary>
        private void MoveCell()
        {
            InvokeAsync(() =>
            {
                string strPos = RackDish + "-" + Position;
                Sin_Cell cellItem = CellBusiness.Instance.GetCell(strPos);

                if (cellItem == null)
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "没有计算过点位"));
                    return;
                }

                MotorList[2].TargetPos= PosZaxis = 3000;
                MoveAbsolute(MotorList[2]);

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
                        PosZaxis = res.CurrPosZ;
                    }
                }
                else
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "平台移动失败"));
                    return;
                }

                MotorList[2].TargetPos = PosZaxis = cellItem.Z;
                MoveAbsolute(MotorList[2]);
            });
        }

        /// <summary>
        /// 电机全部停止
        /// </summary>
        private void StopAllMotor()
        {
            StopMotor(MotorList[0]);
            StopMotor(MotorList[1]);
            StopMotor(MotorList[2]);
        }

        private void PlatformAging()
        {
            InvokeAsync(() =>
            {
                TestFlow.TestFlow.Instance.SetMotorObj(MotorList[0], MotorList[1], MotorList[2]);
                TestFlow.TestFlow.Instance.CreateAgingTest();
                TestFlow.TestFlow.Instance.StartAgingTest();

            });
        }

        private void AgingStop()
        {
            TestFlow.TestFlow.Instance.SetAgingIsChannel(true);
        }
        #endregion

        #region 电机

        /// <summary>
        /// 电机复位机械原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetPhysicalMotor(Sin_Motor obj)
        {
            InvokeAsync(() =>
            {
                ResetMotor(obj, 0);
            });
        }

        /// <summary>
        /// 电机复位工作原点
        /// </summary>
        /// <param name="obj"></param>
        private void ResetLogicalMotor(Sin_Motor obj)
        {
            InvokeAsync(() =>
            {
                ResetMotor(obj, 1);
            });
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
                ResMove resMove = cmdMotorReset.GetResponse() as ResMove;
                obj.TargetPos = resMove.CurPos;
                ChangeTextBoxText(obj);

                MotorBusiness.Instance.SaveMotorItem(obj);
            }
        }
        /// <summary>
        /// 电机停止
        /// </summary>
        /// <param name="obj">电机</param>
        private void StopMotor(Sin_Motor obj)
        {
            InvokeAsync(() =>
            {
                if (MotorBusiness.Instance.StopMotor(obj))
                {
                    ChangeTextBoxText(obj);
                }
            });
        }

        /// <summary>
        /// 电机定位原点
        /// </summary>
        /// <param name="obj"></param>
        private void SetOrigin(Sin_Motor obj)
        {
            InvokeAsync(() =>
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
                        case MotorId.Zaxis:
                            {
                                OriginZaxis = PosZaxis;
                                obj.OriginPoint = PosZaxis;
                            }
                            break;
                    }

                    MotorBusiness.Instance.SaveMotorItem(obj);
                }
            });
        }

        /// <summary>
        /// 电机持续移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void AlawysMove(Sin_Motor obj)
        {
            InvokeAsync(() =>
            {
                MoveCon(obj, (int)obj.Dir, (int)obj.UseFastSpeed);
            });
        }

        private void MoveCon(Sin_Motor obj, int dir, int useFastSpeed)
        {
            CmdMoveCon cmdMoveCon = new CmdMoveCon()
            {
                Id = (int)obj.MotorId,
                Dir = dir,
                UseFastSpeed = useFastSpeed
            };

            if (obj.IsRunning)
            {
                return;
            }

            if (!cmdMoveCon.Execute())
            {
                LogHelper.logSoftWare.Error("AlawysMove failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机持续移动失败"));
            }
            else
            {
                obj.IsRunning = true;
            }
        }
        /// <summary>
        /// 电机相对移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void MoveRelate(Sin_Motor obj)
        {
            InvokeAsync(() =>
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
                    obj.TargetPos = resMove.CurPos;
                    ChangeTextBoxText(obj);


                    MotorBusiness.Instance.SaveMotorItem(obj);
                }
            });
        }

        /// <summary>
        /// 电机绝对移动
        /// </summary>
        /// <param name="obj">电机</param>
        private void MoveAbsolute(Sin_Motor obj)
        {
            InvokeAsync(() =>
            {
                switch (obj.MotorId)
                {
                    case MotorId.Xaxis:
                        obj.TargetPos = PosXaxis;
                        break;
                    case MotorId.Yaxis:
                        obj.TargetPos = PosYaxis;
                        break;
                    case MotorId.Zaxis:
                        obj.TargetPos = PosZaxis;
                        break;
                }

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
                    obj.TargetPos = resMove.CurPos;
                    ChangeTextBoxText(obj);

                    MotorBusiness.Instance.SaveMotorItem(obj);
                }
            });
        }

        /// <summary>
        /// 改变界面位置显示内容
        /// </summary>
        /// <param name="motorId"></param>
        /// <param name="resMove"></param>
        private void ChangeTextBoxText(Sin_Motor motorObj)
        {
            switch (motorObj.MotorId)
            {
                case MotorId.Xaxis:
                    {
                        PosXaxis = motorObj.TargetPos;
                    }
                    break;
                case MotorId.Yaxis:
                    {
                        PosYaxis = motorObj.TargetPos;
                    }
                    break;
                case MotorId.Zaxis:
                    {
                        PosZaxis = motorObj.TargetPos;
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取电机状态
        /// </summary>
        /// <param name="motorId"></param>
        /// <returns></returns>
        private ResMotorStatus GetMotorStatus(int motorId)
        {
            CmdGetMotorStatus cmdGetMotorStatus = new CmdGetMotorStatus() { Id = motorId };

            if (cmdGetMotorStatus.Execute())
            {
                ResMotorStatus resMotorStatus = cmdGetMotorStatus.GetResponse() as ResMotorStatus;

                if (resMotorStatus != null)
                {
                    return resMotorStatus;
                }
            }
            else
            {
                LogHelper.logSoftWare.Error("GetMotorStatus failed");
                NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "电机状态获取失败"));
            }
            return null;
        }
        #endregion

        #region 相机
        private void InitCamera()
        {
            InvokeAsync(() =>
            {
                PVCamHelper.Instance.Init();
                IsCameraInitEnable = !PVCamHelper.Instance.GetInitFlag();
                IsCameraOpenEnable = PVCamHelper.Instance.GetInitFlag();
            });
        }

        /// <summary>
        /// 开关相机
        /// </summary>
        private void CameraOpenAndClose()
        {
            InvokeAsync(() =>
            {
                if (isOpenCamera)
                {
                    PVCamHelper.Instance.Pause();
                    isOpenCamera = false;
                    ChangeButtonText();
                }
                else
                {
                    PVCamHelper.Instance.StartCont();
                    isOpenCamera = true;
                    ChangeButtonText();
                }
            });
        }

        /// <summary>
        /// 大图界面展示
        /// </summary>
        private void BigImageShow()
        {
            //InvokeAsync(() =>
            {
                BigImageWinView bigImageWinView = new BigImageWinView(this);
                bigImageWinView.Show();
            }
            //);
        }

        /// <summary>
        /// 相机暂停
        /// </summary>
        private void CameraPause()
        {
            InvokeAsync(() =>
            {
                if (isOpenCamera)
                {
                    PVCamHelper.Instance.Pause();
                }
            });
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
        /// 调用自动聚焦
        /// </summary>
        private void CameraFocus()
        {
            InvokeAsync(() =>
            {
                string dateText = DateTime.Now.ToString("yyyyMMddHHmmss");
                string filePath = MapPath.TifPath + $"Focus\\{dateText.Substring(0, 8)}\\";
                string fileName = $"{dateText}.tif";

                if (FocusBeginPos > FocusEndPos)
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "聚焦起始位置大于结束位置"));
                    return;
                }

                int focusImageCount = (FocusEndPos - FocusBeginPos) / focusMoveStep;

                if (!isOpenCamera)
                {
                    PVCamHelper.Instance.StartCont();
                    isOpenCamera = true;
                    ChangeButtonText();
                }

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                //开启激光
                ControlBusiness.Instance.LightEnableCtrl(1, 0.5, 1);
                //移动到暂定起始位置
                PosZaxis = FocusBeginPos;
                MoveAbsolute(MotorList[2]);

                //计算聚焦位置
                int autoFocusPos = AutofocusHelper.Instance.ZPos(MotorList[2], FocusBeginPos, focusMoveStep, focusImageCount, filePath + fileName);

                //移动到最佳聚焦位置
                PosZaxis = autoFocusPos;
                MoveAbsolute(MotorList[2]);
                //关闭激光
                ControlBusiness.Instance.LightEnableCtrl(0, 0.5, 1);

                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "聚焦完成"), MessageBoxButton.OK, SinMessageBoxImage.Information);
            });
        }
        /// <summary>
        /// 设置ROI范围
        /// </summary>
        private void SetCameraRoi()
        {
            InvokeAsync(() =>
            {
                int x = 0;
                int y = 0;

                if (PointBegin.X == -1 && PointBegin.Y == -1
                    || !PVCamHelper.Instance.GetIsInitRoi())
                {
                    return;
                }

                SetPoint(PointBegin, originalBinnerSize, originalSize, out x, out y);
                GetPoint(ref x, ref y);

                NeedRoi = false;
                PVCamHelper.Instance.SetIsInitRoi(false);
                PVCamHelper.Instance.SetROI((ushort)(x), (ushort)(y), originalBinnerSize, originalBinnerSize);
            });
        }

        /// <summary>
        /// 重置ROI范围
        /// </summary>
        private void SetInitRoi()
        {
            InvokeAsync(() =>
            {
                PointBegin.X = -1;
                PointBegin.Y = -1;
                NeedRoi = false;
                PVCamHelper.Instance.SetIsInitRoi(true);
                PVCamHelper.Instance.SetROI(0, 0, originalSize, originalSize);
            });
        }

        /// <summary>
        /// 保存图像
        /// </summary>
        private void SaveTiff()
        {
            isSave = true;
            SaveFileDialog FBD = new SaveFileDialog();
            FBD.Title = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            FBD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FBD.Filter = "(*.tif)|*.tif";
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(FBD.FileName))
                {
                    TiffBitmapEncoder encoder = new TiffBitmapEncoder
                    {
                        Compression = TiffCompressOption.Zip
                    };
                    encoder.Frames.Add(BitmapFrame.Create(bmpSouce));
                    FileStream f = new FileStream(FBD.FileName, FileMode.Create);
                    encoder.Save(f);
                    //释放流
                    f.Close();
                }
                else
                {
                    //  ShowMessageError(SystemResources.Instance.LanguageArray[3543]);
                }

            }
            isSave=false;
        }

        private void Point100ImageCollect()
        {
            InvokeAsync(() =>
            {
                if (!isOpenCamera)
                {
                    PVCamHelper.Instance.StartCont();
                    isOpenCamera = true;
                    ChangeButtonText();
                }

                Messenger.Default.Register<byte[]>(this, MessageToken.TokenCameraBuffer, AcquiringImage);
            });
        }

        public void AcquiringImage(byte[] bitmap)
        {
            //收到5张黑图后通知激光由相机采图后开启
            if (tifList.Count == 5)
            {
                ControlBusiness.Instance.LightEnableCtrl(1, 0);
            }


            if (tifList.Count < 100)
            {
                tifList.Add(bitmap);
            }
            else
            {
                //图像采集完成时关闭激光
                ControlBusiness.Instance.LightEnableCtrl(0, 0);
                Messenger.Default.Unregister<byte[]>(this, MessageToken.TokenCameraBuffer, AcquiringImage);
                SaveMatList();
            }
        }

        public void SaveMatList()
        {
            string filePath = MapPath.TifPath + "Collect\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fileName = $"Collect_{DateTime.Now.ToString("HHmmss")}.tif";
            filePath = filePath + fileName;
            AnalysisHelper.Instance.SaveImage(filePath, tifList);
            tifList.Clear();
        }
        #endregion

        #region 激光器

        private void OpenLight()
        {
            ControlBusiness.Instance.LightEnableCtrl(1, Voltage, 1);
        }

        private void CloseLight()
        {
            ControlBusiness.Instance.LightEnableCtrl(0, Voltage, 1);
        }
        #endregion

        #region 外设输入

        /// <summary>
        /// 滚轮事件
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void TMainWinMouseWheelEvent(MouseWheelEvent message)
        {
            //加锁
            Monitor.Enter(objLock);
            try
            {
                //控制左侧 上下机械臂
                if (MouseKeyBoardHelper.IsCtrlDown())
                {
                    if (MouseKeyBoardHelper.IsAltDown())
                    {
                        //移动固定步长
                        MoveRelate(MotorList[2]);
                        //MoveRelativePos(ZaxisMotor, false, message.Delta);
                        LogHelper.logSoftWare.Info($"滚轮事件，相对位移,Right_Z:[{message.Delta}]");
                    }
                    else
                    {
                        //移动固定步长
                        MoveRelate(MotorList[2]);
                        LogHelper.logSoftWare.Info($"滚轮事件，相对位移,Right_Z:[{message.Delta}]");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"TMainWinMouseWheelEvent error:{ex.Message}");
            }
            finally
            {
                Monitor.Exit(objLock);
            }
        }

        /// <summary>
        /// 键盘事件
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="msg"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void MWinKeyEvent(KeyBoardEvent msg)
        {
            //加锁
            Monitor.Enter(objLock);
            try
            {
                //ctrl键 抬起 停止所有电机
                if (!MouseKeyBoardHelper.IsCtrlDown())
                {
                    StopAllMotor();
                    return;
                }
                //左键
                if (msg.KeyCode == System.Windows.Input.Key.Left)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[0], (int)Direction.Forward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[0], (int)Direction.Forward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[0]);
                    }
                }
                //右键
                else if (msg.KeyCode == System.Windows.Input.Key.Right)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[0], (int)Direction.Backward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[0], (int)Direction.Backward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[0]);
                    }
                }
                //上键
                else if (msg.KeyCode == System.Windows.Input.Key.Up)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[1], (int)Direction.Backward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[1], (int)Direction.Backward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[1]);
                    }
                }
                //下键
                else if (msg.KeyCode == System.Windows.Input.Key.Down)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[1], (int)Direction.Forward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[1], (int)Direction.Forward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[1]);
                    }
                }
                else if (msg.KeyCode == System.Windows.Input.Key.W)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[2], (int)Direction.Backward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[2], (int)Direction.Backward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[2]);
                    }
                }
                else if (msg.KeyCode == System.Windows.Input.Key.S)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MoveCon(MotorList[2], (int)Direction.Forward, (int)Rate.slow);
                        else
                            MoveCon(MotorList[2], (int)Direction.Forward, (int)Rate.fast);
                    }
                    else
                    {
                        StopMotor(MotorList[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.logSoftWare.Error($"MWinMouseDown error:{ex.Message}");
            }
            finally
            {
                //释放
                Monitor.Exit(objLock);
            }
        }

        //鼠标输入Roi起始点
        public void SetRoiRange(System.Windows.Point pointBegin, bool needRoi)
        {
            if (needRoi)
            {
                PointBegin = pointBegin;
            }
            else
            {
                PointBegin.X = -1;
                PointBegin.Y = -1;
            }

            NeedRoi = needRoi;
        }
        #endregion

        #region 升级使用
        private void UpgradeBoard()
        {
            List<byte> bytes = new List<byte>();
            bool result = false;

            if (string.IsNullOrEmpty(UpgradeFile))
            {
                ShowMessageError(SystemResources.Instance.GetLanguage(0, "选择文件不存在"));
                return;
            }

            LoadingHelper.Instance.ShowLoadingWindow(a =>
            {

                a.Title = SystemResources.Instance.GetLanguage(0, "正在升级，请等待...");
                using (FileStream fs = new FileStream(UpgradeFile, FileMode.Open, FileAccess.Read))
                {//在using中创建FileStream对象fs，然后执行大括号内的代码段，
                 //执行完后，释放被using的对象fs（后台自动调用了Dispose）
                    byte[] vs = new byte[1024];//数组大小根据自己喜欢设定，太高占内存，太低读取慢。
                    while (true) //因为文件可能很大，而我们每次只读取一部分，因此需要读很多次
                    {
                        int r = fs.Read(vs, 0, vs.Length);
                        bytes.AddRange(vs);
                        if (r == 0) //当读取不到，跳出循环
                        {
                            break;
                        }
                    }
                }
                CmdIAP cmdIAP = new CmdIAP()
                {
                    Data = bytes.ToArray(),
                };
                result = cmdIAP.Execute();

            }, 0, a =>
            {
                if (result)
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "升级成功"));
                    return;
                }
                else
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "升级失败"));
                }

            });

        }

        private void BrowseFile()
        {
            UpgradeFile = GetUpgradeFileMethod();

            if (string.IsNullOrEmpty(UpgradeFile))
            {
                return;
            }

            if (!File.Exists(UpgradeFile))
            {
                ShowMessageError(SystemResources.Instance.GetLanguage(0, "选择文件不存在"));
                return;
            }
        }

        /// <summary>
        /// 获取备份路径方法
        /// </summary>
        private string GetUpgradeFileMethod()
        {
            OpenFileDialog FBD = new OpenFileDialog();
            FBD.Title = SystemResources.Instance.LanguageArray[6442];//"请选择路径";
            FBD.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            FBD.Multiselect = false;
            FBD.Filter = "(*.bin)|*.bin";
            if (FBD.ShowDialog() == DialogResult.OK)
            {

                if (!string.IsNullOrEmpty(FBD.FileName))
                {
                    return FBD.FileName;
                }
                else
                {
                    //  ShowMessageError(SystemResources.Instance.LanguageArray[3543]);
                }

            }
            return string.Empty;
        }
        #endregion

        private void GetPoint(ref int x, ref int y)
        {
            //因为显示图像为翻转图像
            switch (PVCamHelper.Instance.GetRotateFlags())
            {
                case RotateFlags.Rotate90Clockwise:
                    {

                    }
                    break;
                case RotateFlags.Rotate180:
                    {

                    }
                    break;
                case RotateFlags.Rotate90Counterclockwise:
                    {
                        int temp = x;
                        x = (originalSize - y) - originalBinnerSize; //2048 - y 算出翻转前点位对应实际x位置，-1024为算出第一个点位置
                        y = temp;
                    }
                    break;
            }
        }
        private void SetPoint(System.Windows.Point point, int step, int maxRange, out int x, out int y)
        {
            x = (int)point.X * maxRange / showSize;
            y = (int)point.Y * maxRange / showSize;

            //超过图像范围按照终点重新计算
            if (x + step > maxRange)
            {
                x -= x + step - maxRange;
            }
            if (y + step > maxRange)
            {
                y -= y + step - maxRange;
            }
        }
        /// <summary>
        /// 进入页面时触发
        /// </summary>
        /// <param name="parameter"></param>
        protected override void OnParameterChanged(object parameter)
        {
            IsCameraInitEnable = !PVCamHelper.Instance.GetInitFlag();
            IsCameraOpenEnable = PVCamHelper.Instance.GetInitFlag();
            // 注册刷新消息
            Messenger.Default.Register<Mat>(this, MessageToken.TokenCamera, ImageRefersh);
            Messenger.Default.Register<byte[]>(this, MessageToken.TokenCameraBuffer, ImageBufferRefersh);
        }

        public void ImageRefersh(Mat bitmap)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (NeedRoi)
                {
                    int step = showSize;
                    int x = 0;
                    int y = 0;

                    SetPoint(PointBegin, step, originalBinnerSize, out x, out y);

                    OpenCvSharp.Point beginPos = new OpenCvSharp.Point() { X = x, Y = y };
                    OpenCvSharp.Point endPos = new OpenCvSharp.Point() { X = x + showSize, Y = y + showSize };
                    Cv2.Rectangle(bitmap, beginPos, endPos, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias, 0);
                }

                CameraSouce = bitmap.ToBitmapSource();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        BitmapSource bmpSouce = null;

        /// <summary>
        /// 正在保存图像
        /// </summary>
        private bool isSave=false;

        /// <summary>
        /// 刷新界面
        /// </summary>
        /// <param name="src"></param>
        private void ImageBufferRefersh(byte[] src)
        {
            if(isSave)
                return;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                System.Drawing.Size imageSize = new System.Drawing.Size() { Width = PVCamHelper.Instance.GetWidth(), Height = PVCamHelper.Instance.GetHeight() };
                int lenth = imageSize.Width * imageSize.Height;
                byte[] bufBytes = new byte[lenth * 2];

                IntPtr intptr = Marshal.UnsafeAddrOfPinnedArrayElement(src, 0);
                Marshal.Copy(intptr, bufBytes, 0, src.Length);

                BitmapPalette myPalette = BitmapPalettes.Gray16;
                int rawStride = (imageSize.Width * 16 + 7) / 8;

                bmpSouce = BitmapSource.Create(imageSize.Width, imageSize.Height, 96, 96, System.Windows.Media.PixelFormats.Gray16, myPalette, bufBytes, rawStride);

                //TODO::有一定可行性 暂时不可用
                //var dst = bmpSouce.ToMat();
                //Cv2.ConvertScaleAbs(dst, dst,0.5,0.5);   //把dst中的数据映射到0-65535的范围中
                //CameraSouce = dst.ToBitmapSource();
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
            if (!IsIdle)
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "正在执行当前任务，无法进行页面跳转！"));
                return false;
            }
            // 离开页面时删除刷新消息
            Messenger.Default.Unregister<Mat>(this, MessageToken.TokenCamera, ImageRefersh);
            Messenger.Default.Unregister<byte[]>(this, MessageToken.TokenCameraBuffer, ImageBufferRefersh);

            //if (isOpenCamera)
            //{
            //    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            //    {
            //        PVCamHelper.Instance.Pause();
            //    }));
            //    isOpenCamera = false;
            //    ChangeButtonText();

            //}

            return base.NavigatedFrom(source, mode, navigationState);
        }
    }
}
