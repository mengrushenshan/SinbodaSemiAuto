
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ximc;

namespace Sinboda.SemiAuto.Core.Helpers
{
    public class XimcHelper : TBaseSingleton<XimcHelper>
    {
        private API.LoggingCallback callback;

        /// <summary>
        /// 日志回调
        /// </summary>
        /// <param name="loglevel"></param>
        /// <param name="message"></param>
        /// <param name="user_data"></param>
        private void MyLog(API.LogLevel loglevel, string message, IntPtr user_data)
        {
            if (loglevel == API.LogLevel.error)
                LogHelper.logSoftWare.Error(message);
            else
            {
                LogHelper.logSoftWare.Info($"[{loglevel}]:{message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        private void print_status(status_t status)
        {
            LogHelper.logSoftWare.Info($"[rpm]:{status.CurSpeed}  pos: {status.CurPosition}  flags: {status.Flags} ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status_calb"></param>
        private void print_status_calb(status_calb_t status_calb)
        {
            LogHelper.logSoftWare.Info($"speed: {status_calb.CurSpeed}mm/s position: {status_calb.CurPosition}mm flags: {status_calb.Flags}");
        }

        /// <summary>
        /// 缓慢停止电机
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_SlowStop(int devId)
        {
            return API.command_sstp(devId);
        }

        /// <summary>
        /// 立即停止电机 不起作用
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        private Result Cmd_Stop(int devId)
        {
            return API.command_stop(devId);
        }

        /// <summary>
        /// 将当前位置定位为原点
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_Zero(int devId)
        {
            //将当前位置定位为原点
            Result res = API.command_zero(devId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_zero Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 快速移动指令
        /// </summary>
        /// <param name="obj"></param>
        public bool XimcMoveFast(XimcArm arm, int pos)
        {
            if (pos > GlobalData.MaxXimcDistance
                || pos < GlobalData.MinXimcDistance)
            {
                LogHelper.logSoftWare.Error("Invalid value for Position!");
                return false;
            }
            //先将电机速度设置为快速 
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcFastAccel;
            move_Settings_T.Speed = GlobalData.XimcFastSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //移动电机
            return Cmd_Move_Absolute(arm.CtrlName, pos) == Result.ok;
        }

        /// <summary>
        /// 快速移动指令
        /// </summary>
        /// <param name="obj"></param>
        public bool XimcMoveSlow(XimcArm arm, int pos)
        {
            if (pos > GlobalData.MaxXimcDistance
                || pos < GlobalData.MinXimcDistance)
            {
                LogHelper.logSoftWare.Error("Invalid value for Position!");
                return false;
            }
            //先将电机速度设置为慢速 
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcSlowAccel;
            move_Settings_T.Speed = GlobalData.XimcSlowSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //移动电机
            return Cmd_Move_Absolute(arm.CtrlName, pos) == Result.ok;
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool Cmd_Home(XimcArm arm)
        {
            //先将电机速度设置为快速
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcFastAccel;
            move_Settings_T.Speed = GlobalData.XimcFastSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //复位
            Result res = API.command_home(arm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_zero Error " + res.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 复位
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_Home(int devId)
        {
            //复位
            Result res = API.command_home(devId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_zero Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 状态复位
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_Reset(int devId)
        {
            //复位
            Result res = API.command_reset(devId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_reset Error " + res.ToString());
                return res;
            }
            return res;
        }

        public List<XimcArm> XimcArms;

        /// <summary>
        /// 查询所有电机状态
        /// </summary>
        /// <returns></returns>
        public Result Get_All_Motor_Status()
        {
            Result res = Result.no_device;
            foreach (var item in XimcArms)
            {
                status_t status = new status_t();
                res = Get_Status(item.DeveiceId, out status);
                if (res != Result.ok)
                {
                    return res;
                }
                item.MoveStatus.CurSpeed = status.CurSpeed;
                item.MoveStatus.CurPosition = status.CurPosition;
            }
            return res;
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Result Get_Status(int devId, out status_t status)
        {
            //获取设备状态
            Result res = API.get_status(devId, out status);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("get_status Error " + res.ToString());
            }
            //记录电机状态
            print_status(status);
            return res;
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public device_information_t Get_Dev_Info(int devId)
        {
            //设备信息
            device_information_t di;
            Result res = API.get_device_information(devId, out di);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("get_device_information Error " + res.ToString());
            }
            //输出信息
            LogHelper.logSoftWare.Info($"Manufacturer:{di.Manufacturer}" +
                $"Manufacturer Id：{di.ManufacturerId}" +
                $"Product Description：{di.ProductDescription}" +
                $"Major Version:{di.Major}" +
                $"Minor Version:{di.Minor}" +
                $"Release Version:{di.Release}");
            return di;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        public void Init()
        {
            XimcArms = GlobalData.XimcArmsData.XimcArms;
            LogHelper.logSoftWare.Info("开始初始化电机设备！");
            //设备id
            int deviceID = -1;
            try
            {
                //设置日志回调事件
                callback = new API.LoggingCallback(MyLog);
                API.set_logging_callback(callback, IntPtr.Zero);

                // 句柄
                IntPtr device_enumeration;
                const int probe_flags = (int)(Flags.ENUMERATE_PROBE | Flags.ENUMERATE_NETWORK);
                String enumerate_hints = "addr=192.168.1.1,172.16.2.3";
                API.set_bindy_key("keyfile.sqlite");

                // 查看设备
                device_enumeration = API.enumerate_devices(probe_flags, enumerate_hints);
                if (device_enumeration.IsNull())
                {
                    LogHelper.logSoftWare.Error("Error enumerating devices");
                    return;
                }
                // 枚举设备
                int device_count = API.get_device_count(device_enumeration);
                for (int i = 0; i < device_count; ++i)
                {
                    // Gets device name 
                    String dev = API.get_device_name(device_enumeration, i);
                    LogHelper.logSoftWare.Info($"Found device [{dev}]");
                }
                // Get first device name or command-line arg
                String deviceName;
                if (device_count > 0)
                {
                    Dictionary<int, move_settings_t> UnKnownDevices = new Dictionary<int, move_settings_t>();
                    for (int i = 0; i < device_count; i++)
                    {
                        //获取device名称
                        deviceName = API.get_device_name(device_enumeration, i);
                        LogHelper.logSoftWare.Info($"Using device {deviceName}");

                        //打开设备
                        deviceID = API.open_device(deviceName);
                        LogHelper.logSoftWare.Info($"Found device [{deviceID}]");

                        //设备名称
                        controller_name_t controller_Name_T;

                        //获取controller名称
                        if (Get_Controller_Name(deviceID, out controller_Name_T) == Result.ok)
                        {
                            LogHelper.logSoftWare.Info($"Device CtrlName: [{controller_Name_T.ControllerName}]");

                            //设置数据
                            move_settings_t move_settings_T;

                            //绑定设备id
                            XimcArm ximcArm = XimcArms.FirstOrDefault(x => x.ToString() == controller_Name_T.ControllerName);
                            if (ximcArm.IsNull())
                            {
                                LogHelper.logSoftWare.Error($"Device CtrlName: [{controller_Name_T.ControllerName}] is not exsisted");
                                //绑定速度信息
                                if (Get_Move_Settings(deviceID, out move_settings_T) == Result.ok)
                                {
                                    UnKnownDevices.Add(deviceID, move_settings_T);
                                }
                                else
                                {
                                    LogHelper.logSoftWare.Error($"Get_Move_Settings error!");
                                }
                                continue;
                            }
                            else
                            {
                                ximcArm.DeveiceId = deviceID;
                                ximcArm.Controller_Name_T = controller_Name_T;

                                //绑定速度信息
                                if (Get_Move_Settings(deviceID, out move_settings_T) == Result.ok)
                                {
                                    ximcArm.Speed = move_settings_T.Speed;
                                    ximcArm.Accel = move_settings_T.Accel;
                                    ximcArm.Decel = move_settings_T.Decel;
                                    ximcArm.Move_Setting = move_settings_T;
                                }
                                else
                                {
                                    LogHelper.logSoftWare.Error($"Get_Move_Settings error!");
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            LogHelper.logSoftWare.Error($"get_controller_name error!");
                            continue;
                        }

                    }

                    //将没有绑定机械臂的数据绑定
                    foreach (var item in UnKnownDevices)
                    {
                        //绑定设备id
                        XimcArm ximcArm = XimcArms.FirstOrDefault(x => x.DeveiceId < 0);
                        if (ximcArm.IsNull())
                        {
                            LogHelper.logSoftWare.Error($"电机数量不匹配!");
                        }
                        else
                        {
                            ximcArm.DeveiceId = item.Key;
                            ximcArm.CtrlName = SerType.None;
                            ximcArm.Speed = item.Value.Speed;
                            ximcArm.Accel = item.Value.Accel;
                            ximcArm.Decel = item.Value.Decel;
                            ximcArm.Move_Setting = item.Value;
                        }
                    }
                }
                else
                {
                    LogHelper.logSoftWare.Error("No devices");
                    return;
                }

                //记录版本信息
                Get_Version();
            }
            catch (Exception e)
            {
                LogHelper.logSoftWare.Error("Exception " + e.Message);
                LogHelper.logSoftWare.Error(e.StackTrace.ToString());
            }
            finally
            {
                //if (deviceID != -1)
                //    API.close_device(ref deviceID);
            }
        }

        /// <summary>
        /// 关闭机械臂连接
        /// </summary>
        public Result Dispose()
        {
            Result res = Result.no_device;
            if(XimcArms.IsNull())
                return res;
            foreach (var item in XimcArms)
            {
                int devId = item.DeveiceId;
                res = API.close_device(ref devId);
                if (res != Result.ok)
                {
                    LogHelper.logSoftWare.Error("get_engine_settings Error " + res.ToString());
                }
            }

            return res;
        }

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <returns></returns>
        public string Get_Version()
        {
            //获取设备版本 长度应该为最大长度。
            StringBuilder versb = new StringBuilder(256);
            API.ximc_version(versb);
            LogHelper.logSoftWare.Info($"XIMC version: {versb}");
            return versb.ToString();
        }

        /// <summary>
        /// 获取仪器设置信息
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="engine_settings"></param>
        /// <returns></returns>
        public Result Get_Engine_Settings(int devId, out engine_settings_t engine_settings)
        {
            //
            Result res = API.get_engine_settings(devId, out engine_settings);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("get_engine_settings Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Get_Controller_Name(int devId, out controller_name_t controller_Name_T)
        {
            //获取校准信息
            Result res = API.get_controller_name(devId, out controller_Name_T);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("Get_Controller_Name Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 设置设备名称
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Set_Controller_Name(int devId, ref controller_name_t controller_Name_T)
        {
            //设置设备名称
            Result res = API.set_controller_name(devId, ref controller_Name_T);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("Set_Controller_Name Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 设置设备名称
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Set_Controller_Name(XimcArm ximcArm)
        {
            //设置设备名称
            controller_name_t controller_N = ximcArm.Controller_Name_T;
            controller_N.ControllerName = ximcArm.CtrlName.ToString();
            Result res = API.set_controller_name(ximcArm.DeveiceId, ref controller_N);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("set_controller_name Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 数据写入flash
        /// </summary>
        /// <param name="ximcArm"></param>
        /// <returns></returns>
        public Result Command_Save_Settings(XimcArm ximcArm)
        {
            Result res = API.command_save_settings(ximcArm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_save_settings Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 获取设备信息 
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="status_calb"></param>
        /// <param name="calibration"></param>
        /// <returns></returns>
        public Result Get_Status_Calb(int devId, out status_calb_t status_calb, ref calibration_t calibration)
        {
            //获取校准信息
            Result res = API.get_status_calb(devId, out status_calb, ref calibration);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("get_status_calb Error " + res.ToString());
                return res;
            }
            print_status_calb(status_calb);
            return res;
        }

        /// <summary>
        /// 获取设备信息 
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="settings"></param>
        /// <param name="calibration"></param>
        /// <returns></returns>
        public Result Get_Move_Settings(int devId, out move_settings_t settings)
        {
            //获取校准信息
            Result res = API.get_move_settings(devId, out settings);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("Get_Move_Settings Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 设置设备信息 
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="settings"></param>
        /// <param name="calibration"></param>
        /// <returns></returns>
        public Result Set_Move_Settings(XimcArm ximcArm)
        {
            //设置move信息
            move_settings_t move_Settings_T = ximcArm.Move_Setting;
            move_Settings_T.Speed = ximcArm.Speed;
            move_Settings_T.Accel = ximcArm.Accel;
            move_Settings_T.Decel = ximcArm.Decel;
            Result res = Set_Move_Settings(ximcArm.DeveiceId, ref move_Settings_T);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("Set_Move_Settings Error " + res.ToString());
                return res;
            }
            return res;
        }

        /// <summary>
        /// 设置设备信息 
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="settings"></param>
        /// <param name="calibration"></param>
        /// <returns></returns>
        public Result Set_Move_Settings(int devId, ref move_settings_t settings)
        {
            //设置move信息
            Result res = API.set_move_settings(devId, ref settings);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("set_move_settings Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 停止所有电机
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public Result Cmd_StopAll()
        {
            Result res = Result.no_device;
            foreach (var item in XimcArms)
            {
                res = Cmd_SlowStop(item.DeveiceId);
                if (res != Result.ok)
                {
                    LogHelper.logSoftWare.Error("Cmd_StopAll Error " + res.ToString());
                    break;
                }
            }
            return res;
        }

        /// <summary>
        /// 连续向右移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_Right(int devId)
        {
            //连续向右移动
            Result res = API.command_right(devId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_right Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 连续向左移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public Result Cmd_Left(int devId)
        {
            //连续向左移动
            Result res = API.command_left(devId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_right Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 快速向左移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool Cmd_Left_Fast(XimcArm arm)
        {
            //先将电机速度设置为快速
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcFastAccel;
            move_Settings_T.Speed = GlobalData.XimcFastSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //连续向左移动
            Result res = API.command_left(arm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_left Error " + res.ToString());
            }
            return false;
        }

        /// <summary>
        /// 慢速向左移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool Cmd_Left_Slow(XimcArm arm)
        {
            //先将电机速度设置为慢速
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcSlowAccel;
            move_Settings_T.Speed = GlobalData.XimcSlowSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //连续向左移动
            Result res = API.command_left(arm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_left Error " + res.ToString());
            }
            return false;
        }

        /// <summary>
        /// 慢速向右移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool Cmd_Right_Slow(XimcArm arm)
        {
            //先将电机速度设置为慢速
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcSlowAccel;
            move_Settings_T.Speed = GlobalData.XimcSlowSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //连续向右移动
            Result res = API.command_right(arm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_right Error " + res.ToString());
            }
            return false;
        }

        /// <summary>
        /// 快速向右移动
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool Cmd_Right_Fast(XimcArm arm)
        {
            //先将电机速度设置为快速
            move_settings_t move_Settings_T = arm.Move_Setting;
            move_Settings_T.Accel = GlobalData.XimcFastAccel;
            move_Settings_T.Speed = GlobalData.XimcFastSpeed;
            if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                return false;

            //连续向右移动
            Result res = API.command_right(arm.DeveiceId);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_right Error " + res.ToString());
            }
            return false;
        }

        /// <summary>
        /// 电机移动 绝对移动
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Result Cmd_Move_Absolute(SerType ctrlName, int pos)
        {
            XimcArm arm = XimcArms?.FirstOrDefault(x => x.CtrlName == ctrlName);
            //未找到电机
            if (arm.IsNull())
            {
                LogHelper.logSoftWare.Error($"No device for SerId:{ctrlName}");
                return Result.no_device;
            }
            //移动到指定位置 直流电机uPosition忽略
            Result res = API.command_move(arm.DeveiceId, pos, 0);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_move Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 电机移动 相对移动
        /// </summary>
        /// <param name="SerId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Result Cmd_Move_Relative(SerType ctrlName, int pos)
        {
            XimcArm arm = XimcArms?.FirstOrDefault(x => x.CtrlName == ctrlName);
            //未找到电机
            if (arm.IsNull())
            {
                LogHelper.logSoftWare.Error($"No device for ctrlName:{ctrlName}");
                return Result.no_device;
            }
            //相对移动一段距离 直流电机uPosition忽略
            Result res = API.command_movr(arm.DeveiceId, pos, 0);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_movr Error " + res.ToString());
            }
            return res;
        }

        /// <summary>
        /// 电机移动 相对移动
        /// </summary>
        /// <param name="SerId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool Cmd_Move_Relative(XimcArm arm, bool fast, int pos)
        {
            //未找到电机
            if (arm.IsNull())
            {
                LogHelper.logSoftWare.Error($"No device for ctrlName:{arm.CtrlName}");
                return fast;
            }
            if (fast)
            {
                //先将电机速度设置为快速
                move_settings_t move_Settings_T = arm.Move_Setting;
                move_Settings_T.Accel = GlobalData.XimcFastAccel;
                move_Settings_T.Speed = GlobalData.XimcFastSpeed;
                if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                    return false;
            }
            else
            {
                //先将电机速度设置为快速
                move_settings_t move_Settings_T = arm.Move_Setting;
                move_Settings_T.Accel = GlobalData.XimcSlowAccel;
                move_Settings_T.Speed = GlobalData.XimcSlowSpeed;
                if (Set_Move_Settings(arm.DeveiceId, ref move_Settings_T) != Result.ok)
                    return false;
            }

            //相对移动一段距离 直流电机uPosition忽略
            Result res = API.command_movr(arm.DeveiceId, pos, 0);
            if (res != Result.ok)
            {
                LogHelper.logSoftWare.Error("command_movr Error " + res.ToString());
            }
            return true;
        }

    }
}
