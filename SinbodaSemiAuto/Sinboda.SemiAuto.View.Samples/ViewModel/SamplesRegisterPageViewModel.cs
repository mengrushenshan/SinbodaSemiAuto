using Sinboda.Framework.Control.Loading;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.StaticResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.XtraExport.Helpers.TagTableCell;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.Framework.Control.Controls;
using Sinboda.Framework.Core.Services;
using Sinboda.SemiAuto.View.Samples.WinView;
using Sinboda.SemiAuto.Business.Items;
using Sinboda.Framework.Common.Log;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using System.Threading;
using Sinboda.SemiAuto.Core.Resources;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using OpenCvSharp;
using GalaSoft.MvvmLight.Threading;
using OpenCvSharp.WpfExtensions;
using Sinboda.Framework.Control.Controls.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Xml.Linq;
using static Sinboda.Framework.Control.DateTimePickers.TMinSexView;
using Sinboda.Framework.Common;
using System.IO;
using DevExpress.CodeParser;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Sinboda.SemiAuto.View.Samples.ViewModel
{
    public class SamplesRegisterPageViewModel : NavigationViewModelBase
    {
        public Action RefTemplateBoard;
        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly static object objLock = new object();

        private bool isOpenCamera = false;
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
        /// 样本号
        /// </summary>
        private int? sampleCode;
        public int? SampleCode
        {
            get { return sampleCode; }
            set { Set(ref sampleCode, value); }
        }

        /// <summary>
        /// 登记数量
        /// </summary>
        private int? count = 1;
        public int? Count
        {
            get { return count; }
            set { Set(ref count, value); }
        }

        /// <summary>
        /// 条码号
        /// </summary>
        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set { Set(ref barcode, value); }
        }

        /// <summary>
        /// 项目集合
        /// </summary>
        private List<string> sinItemSource;
        public List<string> SinItemSource
        {
            get { return sinItemSource; }
            set { Set(ref sinItemSource, value); }
        }

        /// <summary>
        /// 选择项目
        /// </summary>
        private string selectItem;
        public string SelectItem
        {
            get { return selectItem; }
            set { Set(ref selectItem, value); }
        }

        /// <summary>
        /// X电机
        /// </summary>
        private Sin_Motor xAxisMotor;
        public Sin_Motor XAxisMotor
        {
            get { return xAxisMotor; }
            set { Set(ref xAxisMotor, value); }
        }

        /// <summary>
        /// Y电机
        /// </summary>
        private Sin_Motor yAxisMotor;
        public Sin_Motor YAxisMotor
        {
            get { return yAxisMotor; }
            set { Set(ref yAxisMotor, value); }
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
        /// 图像数据
        /// </summary>
        private BitmapSource cameraSouce;
        public BitmapSource CameraSouce
        {
            get { return cameraSouce; }
            set { Set(ref cameraSouce, value); }
        }

        /// <summary>
        /// 初始化按钮使能
        /// </summary>
        private bool isCameraInitEnable;
        public bool IsCameraInitEnable 
        { 
            get { return isCameraInitEnable; } 
            set { Set(ref isCameraInitEnable, value); }
        }

        /// <summary>
        /// 相机开关使能
        /// </summary>
        private bool isCameraOpenEnable;
        public bool IsCameraOpenEnable 
        { 
            get { return isCameraOpenEnable; }
            set { Set (ref isCameraOpenEnable, value); }
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

        private int focusImageCount = 100;
        public int FocusImageCount
        {
            get { return focusImageCount; }
            set { Set(ref focusImageCount, value); }
        }

        /// <summary>
        /// 模板名称
        /// </summary>
        private string templateName;
        public string TemplateName
        {
            get { return templateName; }
            set
            {
                Set(ref templateName, value);
            }
        }

        /// <summary>
        /// 模板名称列表
        /// </summary>
        private List<string> templateNameList;
        public List<string> TemplateNameList
        {
            get { return templateNameList; }
            set { Set(ref templateNameList, value); }
        }

        /// <summary>
        /// 当前孔位列表
        /// </summary>
        private List<Sin_Board> curBoardItemList;
        public List<Sin_Board> CurBoardItemList
        {
            get { return curBoardItemList; }
            set { Set(ref curBoardItemList, value); }
        }

        /// <summary>
        /// 前页孔位列表
        /// </summary>
        private List<Sin_Board> beforBoardItemList;
        public List<Sin_Board> BeforBoardItemList
        {
            get { return beforBoardItemList; }
            set { Set(ref beforBoardItemList, value); }
        }

        /// <summary>
        /// 前页孔位列表
        /// </summary>
        private List<Sin_Board> nextBoardItemList;
        public List<Sin_Board> NextBoardItemList
        {
            get { return nextBoardItemList; }
            set { Set(ref nextBoardItemList, value); }
        }

        /// <summary>
        /// 板号
        /// </summary>
        private int boardId = 1;
        public int BoardId
        {
            get { return boardId; }
            set { Set(ref boardId, value); }
        }

        /// <summary>
        /// 模板队列
        /// </summary>
        private List<Sin_BoardTemplate> templateList;
        public List<Sin_BoardTemplate> TemplateList
        {
            get { return templateList; }
            set { Set(ref templateList, value); }
        }

        private TestType boardType;
        public TestType BoardType
        {
            get { return boardType; }
            set { Set(ref boardType, value);}
        }

        private bool isBoardEnable;

        public bool IsBoardEnable
        {
            get { return isBoardEnable; }
            set { Set(ref isBoardEnable, value); }
        }
        #endregion

        #region 命令

        /// <summary>
        /// 登记命令
        /// </summary>
        public RelayCommand SampleRigesterCmd { get; set; }

        /// <summary>
        /// 样本删除
        /// </summary>
        public RelayCommand SampleDeleteCommand { get; set; }

        /// <summary>
        /// 释放位置
        /// </summary>
        public RelayCommand ResetCommand { get; set; }

        /// <summary>
        /// 开始采集
        /// </summary>
        public RelayCommand TestPointStartCommand { get; set; }

        /// <summary>
        /// 模板编辑
        /// </summary>
        public RelayCommand EditTemplateCommand { get; set; }

        /// <summary>
        /// 模板编辑
        /// </summary>
        public RelayCommand UseTemplateCommand { get; set; }

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
        /// 相机聚焦
        /// </summary>
        public RelayCommand CameraFocusCommand { get; set; }

        /// <summary>
        /// 相机聚焦
        /// </summary>
        public RelayCommand TestStartCommand { get; set; }
        #endregion

        #endregion

        public SamplesRegisterPageViewModel()
        {
            DispatcherHelper.Initialize();
            SampleRigesterCmd = new RelayCommand(SampleRigester);
            SampleDeleteCommand = new RelayCommand(SampleDelete);
            ResetCommand = new RelayCommand(Reset);
            OpenAndCloseCommand = new RelayCommand(CameraOpenAndClose);
            CameraInitCommand = new RelayCommand(InitCamera);
            BigImageCommand = new RelayCommand(BigImageShow);
            CameraFocusCommand = new RelayCommand(CameraFocus);
            TestPointStartCommand = new RelayCommand(TestPointStart);
            TestStartCommand = new RelayCommand(TestStart);
            EditTemplateCommand = new RelayCommand(EditTemplate);
            UseTemplateCommand = new RelayCommand(UseTemplateList);
            SetBoardId();
            CurBoardItemList = NewNoneBoard();
            ChangeButtonText();
            InitSamplesRegisterPage();
            InitMachinerySource();
            SetTemplateNameAndList();
        }

        /// <summary>
        /// 设置板号
        /// </summary>
        private void SetBoardId()
        {
            BoardId = BoardBusiness.Instance.GetMaxBoardId();
        }

        /// <summary>
        /// 模板名称获取
        /// </summary>
        private void SetTemplateNameAndList()
        {
            TemplateNameList = BoardTemplateBusiness.Instance.GetTemplateNameList();
            if (TemplateNameList != null)
            {
                TemplateNameList.Add(string.Empty);
                TemplateName = string.Empty;
            }
            else
            {
                TemplateNameList = new List<string>();
                TemplateNameList.Add(string.Empty);
                TemplateName = string.Empty;
            }
        }

        /// <summary>
        /// 创建新的板
        /// </summary>
        /// <param name="BoardItemList"></param>
        private List<Sin_Board> NewNoneBoard()
        {
            List<Sin_Board> BoardItemList = new List<Sin_Board>();
            for (int rack = 0; rack < 8; rack++)
            {
                for (int pos = 1; pos <= 12; pos++)
                {
                    BoardItemList.Add(new Sin_Board()
                    {
                        Id = Guid.NewGuid(),
                        Rack = Convert.ToChar('A' + rack).ToString(),
                        Position = pos,
                        TestType = TestType.None,
                        ItemName = string.Empty,
                        IsEnable = false,
                        RegistDate = DateTime.Now,
                    });
                }
            }
            return BoardItemList;
        }
        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitSamplesRegisterPage()
        {
            SampleCode = SampleBusiness.Instance.GetMaxSampleCode();
            RackDish = RackSouce.Count > 0 ? RackSouce[0] : 1;
            Position = PosSouce.Count > 0 ? PosSouce[0] : 1;
            List<string> itemNameList = ItemBusiness.Instance.GetItemNames();
            if (itemNameList != null)
            {
                SinItemSource = itemNameList;
                SelectItem = itemNameList[0];
            }
            else
            {
                LogHelper.logSoftWare.Error("InitSamplesRegisterPage error not have item");
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitMachinerySource()
        {
            //初始化电机
            var MotorList = MotorBusiness.Instance.GetMotorList();
            if (MotorList.Count() != 2)
            {
                XAxisMotor = new Sin_Motor() { MotorId = MotorId.Xaxis };
                YAxisMotor = new Sin_Motor() { MotorId = MotorId.Yaxis };
            }
            else
            {
                foreach (var motorItem in MotorList)
                {
                    switch (motorItem.MotorId)
                    {
                        case MotorId.Xaxis:
                            {
                                XAxisMotor = motorItem;
                            }
                            break;
                        case MotorId.Yaxis:
                            {
                                YAxisMotor = motorItem;
                            }
                            break;
                    }
                }
            }

            var Devices = GlobalData.XimcArmsData.XimcArms;
            //初始化z轴
            if (Devices.Count > 0)
            {
                var zZone = Devices.Where(o => o.CtrlName == SerType.Left_Z);
                if (zZone.Count() > 0)
                {
                    ZaxisMotor = zZone.FirstOrDefault();
                    MotorBusiness.Instance.SetXimcStatus(ZaxisMotor);
                }
            }
        }

        private void EditTemplate()
        {
            SampleRegisterBoardWindow sampleRegisterBoardWindow = new SampleRegisterBoardWindow();
            sampleRegisterBoardWindow.ShowDialog();
        }

        private void SampleRigester()
        {
            if (CurBoardItemList.Count(o => o.IsEnable == true) == 0)
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "没有可登记样本"));
                return;
            }

            var boardList = BoardBusiness.Instance.GetBoardListByBoardId(BoardId);
            if (boardList == null || boardList.Count == 0)
            {
                if (CurBoardItemList != null)
                {
                    CurBoardItemList.ForEach(o => o.BoardId = BoardId);
                    OperationResult or = new OperationResult();
                    LoadingHelper.Instance.ShowLoadingWindow(a =>
                    {

                        a.Title = SystemResources.Instance.GetLanguage(12495, "正在登记样本，请等待...");
                        or = BoardBusiness.Instance.CreateTemplateList(CurBoardItemList);

                    }, 0, a =>
                    {
                        if (!or.ResultBool)
                        {
                            NotificationService.Instance.ShowError(or.Message);
                            return;
                        }
                        else
                        {
                            NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "登记成功"));
                            ResetBoardPage();
                        }

                    });
                }
            }
            else
            {
                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "板号已经存在"));
                return;
            }
        }

        private void ResetBoardPage()
        {
            SetBoardId();
            CurBoardItemList = NewNoneBoard();
            RefTemplateBoard();
        }
        private void SampleDelete()
        {
            SampleDeleteWindow win = new SampleDeleteWindow();
            if (win.ShowDialog() == true)
            { 

            }
        }

        private void Reset()
        {
            ResetBoardPage();
        }

        /// <summary>
        /// 电机全部停止
        /// </summary>
        private void StopAllMotor()
        {
            MotorBusiness.Instance.StopMotor(XAxisMotor);
            MotorBusiness.Instance.StopMotor(YAxisMotor);
            MotorBusiness.Instance.XimcStop(ZaxisMotor);
        }

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
                        MotorBusiness.Instance.MoveRelativePos(ZaxisMotor, false, message.Delta);
                        LogHelper.logSoftWare.Info($"滚轮事件，相对位移,Right_Z:[{message.Delta}]");
                    }
                    else
                    {
                        MotorBusiness.Instance.MoveRelativePos(ZaxisMotor, true, message.Delta);
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
                            MotorBusiness.Instance.MoveCon(XAxisMotor, (int)Direction.Forward, (int)Rate.slow);
                        else
                            MotorBusiness.Instance.MoveCon(XAxisMotor, (int)Direction.Forward, (int)Rate.fast);
                    }
                    else
                    {
                        MotorBusiness.Instance.StopMotor(XAxisMotor);
                    }
                }
                //右键
                else if (msg.KeyCode == System.Windows.Input.Key.Right)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MotorBusiness.Instance.MoveCon(XAxisMotor, (int)Direction.Backward, (int)Rate.slow);
                        else
                            MotorBusiness.Instance.MoveCon(XAxisMotor, (int)Direction.Backward, (int)Rate.fast);
                    }
                    else
                    {
                        MotorBusiness.Instance.StopMotor(XAxisMotor);
                    }
                }
                //上键
                else if (msg.KeyCode == System.Windows.Input.Key.Up)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MotorBusiness.Instance.MoveCon(YAxisMotor, (int)Direction.Backward, (int)Rate.slow);
                        else
                            MotorBusiness.Instance.MoveCon(YAxisMotor, (int)Direction.Backward, (int)Rate.fast);
                    }
                    else
                    {
                        MotorBusiness.Instance.StopMotor(YAxisMotor);
                    }
                }
                //下键
                else if (msg.KeyCode == System.Windows.Input.Key.Down)
                {
                    //按下
                    if (msg.IsKeyDown)
                    {
                        if (MouseKeyBoardHelper.IsAltDown())
                            MotorBusiness.Instance.MoveCon(YAxisMotor, (int)Direction.Forward, (int)Rate.slow);
                        else
                            MotorBusiness.Instance.MoveCon(YAxisMotor, (int)Direction.Forward, (int)Rate.fast);
                    }
                    else
                    {
                        MotorBusiness.Instance.StopMotor(YAxisMotor);
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
        #endregion

        #region 相机
        private void InitCamera()
        {
            PVCamHelper.Instance.Init();
            IsCameraInitEnable = !PVCamHelper.Instance.GetInitFlag();
            IsCameraOpenEnable = PVCamHelper.Instance.GetInitFlag();
        }

        /// <summary>
        /// 开关相机
        /// </summary>
        private void CameraOpenAndClose()
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
        }

        /// <summary>
        /// 大图界面展示
        /// </summary>
        private void BigImageShow()
        {
            //BigImageWinView bigImageWinView = new BigImageWinView(this);
            //bigImageWinView.Show();
        }

        /// <summary>
        /// 相机暂停
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

        /// <summary>
        /// 调用自动聚焦
        /// </summary>
        private void CameraFocus()
        {
            string dateText = DateTime.Now.ToString("yyyyMMddHHmmss");
            string filePath = MapPath.TifPath + $"Focus\\{dateText.Substring(0, 8)}\\";
            string fileName = $"{dateText}.tif";

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

            Task.Run(() =>
            {
                //开启激光
                ControlBusiness.Instance.LightEnableCtrl(1, 1);
                //移动到暂定起始位置
                MotorBusiness.Instance.XimcMoveFast(ZaxisMotor, 5033000);
                //获取Z轴位置
                MotorBusiness.Instance.SetXimcStatus(ZaxisMotor);

                //计算聚焦位置
                int autoFocusPos = AutofocusHelper.Instance.ZPos(ZaxisMotor, ZaxisMotor.TargetPos, 64, FocusImageCount, filePath + fileName);

                //移动到最佳聚焦位置
                MotorBusiness.Instance.XimcMoveFast(ZaxisMotor, autoFocusPos);
                //关闭激光
                ControlBusiness.Instance.LightEnableCtrl(0, 1);

                NotificationService.Instance.ShowMessage(SystemResources.Instance.GetLanguage(0, "聚焦完成"), MessageBoxButton.OK, SinMessageBoxImage.Information);
            });

        }

        /// <summary>
        /// 开始测试
        /// </summary>
        private void TestPointStart()
        {
            LoadingHelper.Instance.ShowLoadingWindow(ancBegin =>
            {
                // 1.初始化
                ancBegin.Title = SystemResources.Instance.GetLanguage(12396, "正在准备测试数据，请等待...");

                TestFlow.TestFlow.Instance.SetMotorObj(XAxisMotor, YAxisMotor, ZaxisMotor);
                TestFlow.TestFlow.Instance.CreateTest();

                LogHelper.logSoftWare.Debug($"prepare test complete ..... ");

            }, 0, ancBegin =>
            {
                if (!isOpenCamera)
                {
                    PVCamHelper.Instance.StartCont();
                    isOpenCamera = true;
                    ChangeButtonText();
                }
                Task.Run(() =>
                {
                    TestFlow.TestFlow.Instance.StartItemTest();
                });
            });

            
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        private void TestStart()
        {
            
        }

        public Sin_Board GetSinBoard(string tag)
        {
            if (!tag.Contains("-"))
            {
                return null;
            }

            string[] strRackAndPos = tag.Split('-');
            string rack = strRackAndPos[0];
            int pos = int.Parse(strRackAndPos[1]);

            Sin_Board board = CurBoardItemList.Where(o => o.Rack == rack && o.Position == pos).First();
            return board;
        }

        /// <summary>
        /// 获取模板
        /// </summary>
        private void SetTemplateList()
        {
            if (string.IsNullOrEmpty(TemplateName))
            {
                TemplateList = BoardTemplateBusiness.Instance.GetBoardList("Default");
            }
            else
            {
                TemplateList = BoardTemplateBusiness.Instance.GetBoardList(TemplateName);
            }
        }

        /// <summary>
        /// 应用模板
        /// </summary>
        private void UseTemplateList()
        {
            SetTemplateList();

            if(TemplateList == null)
                return;

            foreach (var template in TemplateList)
            {
                Sin_Board boradItem = CurBoardItemList.Where(o => o.Rack == template.Rack && o.Position == template.Position).First();
                boradItem.GetTemplateAttribute(template);
            }

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                RefTemplateBoard();
            });
            
        }

        /// <summary>
        /// 选中数据
        /// </summary>
        /// <param name="reagent"></param>
        public void ShowBoardInfo(Sin_Board board)
        {
            RefTemplateBoard();

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                BoardType = board.TestType;
                IsBoardEnable = board.IsEnable;
            });
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
        }

        /// <summary>
        /// 刷新图像
        /// </summary>
        /// <param name="bitmap"></param>
        public void ImageRefersh(Mat bitmap)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                CameraSouce = bitmap.ToBitmapSource();
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
            // 离开页面时删除刷新消息
            Messenger.Default.Unregister<Mat>(this, MessageToken.TokenCamera, ImageRefersh);

            if (isOpenCamera)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                {
                    PVCamHelper.Instance.Pause();
                }));
                isOpenCamera = false;
                ChangeButtonText();
            }

            return base.NavigatedFrom(source, mode, navigationState);
        }
    }
}
