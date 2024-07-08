using GalaSoft.MvvmLight.Messaging;
using OpenCvSharp;
using Sinboda.Framework.Common;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.Framework.Core.Services;
using Sinboda.Framework.Core.StaticResource;
using Sinboda.SemiAuto.Business.Items;
using Sinboda.SemiAuto.Business.Samples;
using Sinboda.SemiAuto.Core.Helpers;
using Sinboda.SemiAuto.Core.Models;
using Sinboda.SemiAuto.Core.Models.Common;
using Sinboda.SemiAuto.Core.Resources;
using Sinboda.SemiAuto.Model.DatabaseModel.Enum;
using Sinboda.SemiAuto.Model.DatabaseModel.SemiAuto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.TestFlow
{
    public class TestFlow : TBaseSingleton<TestFlow>
    {
        /// <summary>
        /// 聚焦步长
        /// </summary>
        private const int focusMoveStep = 4;

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        /// <summary>
        /// 等待拍照结束通知
        /// </summary>
        private AutoResetEvent AcquiringImageFinish = new AutoResetEvent(false);

        /// <summary>
        /// 初始化完成标志
        /// </summary>
        private bool isInitComplete = false;

        /// <summary>
        /// 测试流水每次加一，关机后更新
        /// </summary>
        private int testId = 0;

        /// <summary>
        /// 测试板号
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// 基础位置X
        /// </summary>
        public int X {  get; set; }

        /// <summary>
        /// 基础位置Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 基础位置Z
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// 当前测试
        /// </summary>
        public TestItem CurTestItem { get; set; } = null;

        /// <summary>
        /// 测试队列
        /// </summary>
        public List<TestItem> Items { get; set; } = new List<TestItem>();

        private bool agingIsChannel = false;
        /// <summary>
        /// 测试流程初始化
        /// </summary>
        private bool Init()
        {
            AcquiringImageFinish.Reset();
            DateTime today = DateTime.Now.Date;
            DateTime tomorrow = DateTime.Now.AddDays(1).Date;
            if (isInitComplete)
            {
                return true;
            }

            //启动测试时，实时获取电机所在位置
            ResMotorStatus xStatus = MotorBusiness.Instance.MotorX_GetMotorStatus();
            ResMotorStatus yStatus = MotorBusiness.Instance.MotorY_GetMotorStatus();
            ResMotorStatus zStatus = MotorBusiness.Instance.MotorZ_GetMotorStatus();

            Z = zStatus.CurrPos;

            if (xStatus != null)
            {
                X = xStatus.CurrPos;
            }
            else
            {
                return false ;
            }

            if (yStatus != null)
            {
                Y = yStatus.CurrPos;
            }
            else
            {
                return false;
            }
            
            if (yStatus != null)
            {
                Z = zStatus.CurrPos;
            }
            else
            {
                return false;
            }

            Messenger.Default.Register<object>(this, MessageToken.AcquiringImageComplete, ReceiveImageComplete);

            return isInitComplete = true;
        }

        /// <summary>
        /// 老化停止标志
        /// </summary>
        /// <param name="isChannel"></param>
        public void SetAgingIsChannel(bool isChannel)
        {
            agingIsChannel = isChannel;
        }

        /// <summary>
        /// 测试板号设置
        /// </summary>
        /// <param name="id"></param>
        public void SetBoardId(int id)
        {
            BoardId = id;
        }
        /// <summary>
        /// 根据1位置创建测试缓存
        /// </summary>
        public bool CreateTest()
        {
            bool isOrder = false;
            if (!Init())
            {
                return false;
            }

            List<Sin_Board> BoardList = BoardBusiness.Instance.GetBoardListByBoardId(BoardId);
            if (BoardList == null || BoardList.Count() == 0)
            {
                return false;
            }

            string samplePath = MapPath.TifPath + "Result\\" + $"{BoardList.First().ItemName}\\";
            if (!Directory.Exists(samplePath))
            {
                Directory.CreateDirectory(samplePath);
            }

            for (char rack = 'A'; rack <= 'H'; rack++)
            {
                isOrder = !isOrder;
                for (int i = 1; i <= 12; i++)
                {
                    var pos = isOrder ? i : 13 - i;
                    Sin_Board boardItem = BoardList.Where(o => o.Rack == rack.ToString() && o.Position ==  pos).First();

                    if (!boardItem.IsEnable)
                    {
                        continue;
                    }

                    string fileName = boardItem.Rack + "_" + boardItem.Position;
                    TestItem testItem = new TestItem();
                    testItem.ItemBoard = boardItem;
                    testItem.Type = boardItem.TestType;
                    testItem.Testid = ++testId;
                    testItem.State = TestState.Untested;
                    testItem.SetTestItemPos(X, Y, Z);
                    if (testItem.Type == TestType.Sample)
                    {
                        testItem.ItemSample = SampleBusiness.Instance.GetSampleByRackPos(boardItem.BoardId, boardItem.Rack, boardItem.Position);
                        testItem.CreatePoint(fileName, samplePath);
                    }

                    Items.Add(testItem);
                }
            }

            if (Items.Count == 0)
            {
                return false;
            }
            else
            {
                CurTestItem = Items[0];

                if (CurTestItem.X != X && CurTestItem.Y != Y)
                {
                    MoveTestItemPos();
                }
            }

            return true;
        }

        /// <summary>
        /// 老化测试
        /// </summary>
        public void CreateAgingTest()
        {
            SetAgingIsChannel(false);
            ResMotorStatus xStatus = MotorBusiness.Instance.MotorX_GetMotorStatus();
            ResMotorStatus yStatus = MotorBusiness.Instance.MotorY_GetMotorStatus();
            ResMotorStatus zStatus = MotorBusiness.Instance.MotorZ_GetMotorStatus();

            Z = zStatus.CurrPos;

            if (xStatus != null)
            {
                X = xStatus.CurrPos;
            }
            else
            {
                return;
            }

            if (yStatus != null)
            {
                Y = yStatus.CurrPos;
            }
            else
            {
                return;
            }

            if (zStatus != null)
            {
                Z = zStatus.CurrPos;
            }
            else
            {
                return;
            }

            List<Sin_Sample> SampleList = NewNoneBoard();

            foreach (var sampleItem in SampleList)
            {
                string fileName = sampleItem.RackDish + "_" + sampleItem.Position;
                TestItem testItem = new TestItem();
                testItem.ItemSample = sampleItem;
                testItem.Testid = ++testId;
                testItem.State = TestState.Untested;
                testItem.SetTestItemPos(X, Y, Z);
                testItem.CreatePoint();

                Items.Add(testItem);
            }

            CurTestItem = Items[0];

        }

        /// <summary>
        /// 创建新的板
        /// </summary>
        /// <param name="BoardItemList"></param>
        private List<Sin_Sample> NewNoneBoard()
        {
            bool NeedCut = true;
            List<Sin_Sample> SampleList = new List<Sin_Sample>();
            //for (int rack = 0; rack < 8; rack++)
            for (int rack = 1; rack < 7; rack++)
            {
                NeedCut = !NeedCut;
                for (int pos = 2; pos <= 11; pos++)
                {
                    SampleList.Add(new Sin_Sample()
                    {
                        RackDish = Convert.ToChar('A' + rack).ToString(),
                        Position = NeedCut ? 13 - pos : pos,
                    });
                }
            }
            for (int i=0; i< SampleList.Count - 1; i++)
            {
                Console.WriteLine(SampleList[i].RackDish);
            }
            return SampleList;
        }

        /// <summary>
        /// 移动到聚焦位置
        /// </summary>
        public void MoveFocusPos()
        {
            MotorBusiness.Instance.MotorZ_MoveAbsolute(Z);
        }

        /// <summary>
        /// 移动测试位置
        /// </summary>
        public void MoveTestItemPos()
        {
            CurTestItem.MoveTestItemXPos();
            CurTestItem.MoveTestItemYPos();
        }

        /// <summary>
        /// 移动测试点
        /// </summary>
        public void MoveTestPointPos()
        {
            //移动xy
            CurTestItem.CurTestPoint.MoveTestItemXPos();
            CurTestItem.CurTestPoint.MoveTestItemYPos();
        }

        /// <summary>
        /// 测试流程
        /// </summary>
        public void StartItemTest()
        {
            foreach (var item in Items)
            {
                if (item.State == TestState.Complete)
                {
                    continue;
                }

                if (CurTestItem.State == TestState.Complete)
                {
                    CurTestItem = item;
                }

                if (CurTestItem.X != X && CurTestItem.Y != Y)
                {
                    MoveTestItemPos();
                }
                switch (CurTestItem.Type)
                {
                    case TestType.Sample:
                        {
                            SampleTestFlow();
                        }
                        break;
                    case TestType.Focus:
                        {
                            FocusTestFlow(Z - 100, Z + 100);
                        }
                        break;


                }
                
            }
        }
        private void SampleTestFlow()
        {
            PointAcquiringImage();

            //点位测试完成后，移动到下一个点
            if (CurTestItem.points.Where(o => o.Status == TestState.Complete).Count() == CurTestItem.points.Count)
            {
                CurTestItem.State = TestState.Complete;
                string rack = CurTestItem.ItemSample.RackDish;
                int pos = CurTestItem.ItemSample.Position;
                Task.Run(async () => {
                    await semaphoreSlim.WaitAsync();
                    //AnalysisHelper.Instance.Init();
                    AnalysisHelper.Instance.Analysis(CurTestItem.ItemSample.TestResult, rack.ToCharArray()[0], pos);
                    //AnalysisHelper.Instance.Shutdown();
                    semaphoreSlim.Release();
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void FocusTestFlow(int FocusBeginPos, int FocusEndPos)
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

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            //开启激光
            ControlBusiness.Instance.LightEnableCtrl(1, 0.5, 1);
            //移动到暂定起始位置
            Z = FocusBeginPos;
            MoveFocusPos();

            //计算聚焦位置
            int autoFocusPos = AutofocusHelper.Instance.ZPos(FocusBeginPos, focusMoveStep, focusImageCount, filePath + fileName);

            //移动到最佳聚焦位置
            Z = autoFocusPos;
            MoveFocusPos();
            //关闭激光
            ControlBusiness.Instance.LightEnableCtrl(0, 0.5, 1);
        }

        /// <summary>
        /// 测试流程
        /// </summary>
        public void StartAgingTest()
        {
            while (true)
            {
                if (agingIsChannel)
                {
                    break;
                }

                foreach (var item in Items)
                {
                    CurTestItem = item;

                    if (agingIsChannel)
                    {
                        break;
                    }

                    if (CurTestItem.X != X && CurTestItem.Y != Y)
                    {
                        MoveTestItemPos();
                    }

                    PointAgingTest();

                }
            }
            
        }

        /// <summary>
        /// 9点数据采集
        /// </summary>
        private void PointAgingTest()
        {
            //同一孔位9点测试
            List<TestPoint> points = CurTestItem.points.Where(o => o.Status == TestState.Untested).ToList();

            foreach (var point in points)
            {
                if (agingIsChannel)
                {
                    break;
                }
                CurTestItem.CurTestPoint = point;
                MoveTestPointPos();
            }
        }

        /// <summary>
        /// 9点数据采集
        /// </summary>
        private void PointAcquiringImage()
        {
            //同一孔位9点测试
            List<TestPoint> points = CurTestItem.points.Where(o => o.Status == TestState.Untested).ToList();

            foreach (var point in points)
            {
                CurTestItem.CurTestPoint = point;
                CurTestItem.CurTestPoint.Status = TestState.Testing;
                MoveTestPointPos();
                //开始拍照
                CurTestItem.CurTestPoint.StartAcquiringImage();
                //100张图曝光30us，增加1秒超时时间
                if (AcquiringImageFinish.WaitOne(35000))
                {
                    AcquiringImageFinish.Reset();
                    //等待记录100张图像
                    CurTestItem.CurTestPoint.Status = TestState.Complete;
                }
                else
                {
                    NotificationService.Instance.ShowError(SystemResources.Instance.GetLanguage(0, "图像采集失败"));
                    break;
                }

            }
        }

        public void CannelTest()
        {
            Messenger.Default.Unregister<object>(this, MessageToken.AcquiringImageComplete, ReceiveImageComplete);
            CurTestItem.ChannelPoint();
            CurTestItem = null;
            isInitComplete = false;
        }

        public void ReceiveImageComplete(object obj)
        {
            AcquiringImageFinish.Set();
        }
    }
}
