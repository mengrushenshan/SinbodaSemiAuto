using Sinboda.SemiAuto.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    /// <summary>
    /// Notification types sent by this class.
    /// </summary>
    public enum CameraNotificationType
    {
        AcquisitionStarted = 0,
        AcquisitionFinished,
        AcquisitionFailed,
        FrameAcquired,

        CameraStatusMessage,
        CameraErrorMessage
    }
    /// <summary>
    /// Arguments for camera notification events.
    /// </summary>
    public class CameraNotificationEventArgs
    {
        public CameraNotificationEventArgs(CameraNotificationType type)
        {
            Type = type;
        }
        public CameraNotificationEventArgs(CameraNotificationType type, string message)
        {
            Type = type;
            Message = message;
        }
        public CameraNotificationType Type { get; private set; }
        public string Message { get; private set; }
    }
    /// <summary>
    /// Callback context can be passed to PVCAM when registering to callback
    /// notifications. Since the PVCAM callback function is static, native C/C++
    /// applications usually use 'this' as a context. When callback function is
    /// called by PVCAM, the provided context pointer is then casted to 'Camera'
    /// class or similar, which will allow the static function to access the caller
    /// properties.
    /// However, in .NET, thanks to the use of delegates, the context isn't really
    /// needed and it's kept here only for demonstation purposes.
    /// </summary>
    struct CallbackContext
    {
        public uint Binning { get; set; }
        public uint ExposureTime { get; set; }
    }

    /// <summary>
    /// Photometrics cameras may provide multiple readout ports, each port
    /// has a Number, Name and a list of Speeds under this port.
    /// Each Speed has an Index, a Readout rate in MHz, optional Name and
    /// a list of Gains available under the particular speed.
    /// Each Gain has an Index, an associated native bit depth and optional Name.
    /// </summary>
    public class GainOption
    {
        public short Index { get; private set; }
        public string Name { get; private set; }
        public short BitDepth { get; private set; }
        public GainOption(short gainIndex, string name, short bitDepth)
        {
            Index = gainIndex;
            Name = name;
            BitDepth = bitDepth;
        }
    }
    /// <summary>
    /// Photometrics cameras may provide multiple readout ports, each port
    /// has a Number, Name and a list of Speeds under this port.
    /// Each Speed has an Index, a Readout rate in MHz, optional Name and
    /// a list of Gains available under the particular speed.
    /// Each Gain has an Index, an associated native bit depth and optional Name.
    /// </summary>
    public class SpeedOption
    {
        public short Index { get; private set; }
        public ushort PixTimeNs { get; private set; }
        public string Name { get; private set; }
        public double RateMHz { get { return 1000.0 / PixTimeNs; } }
        public List<GainOption> GainList;
        public SpeedOption(short speedIndex, ushort pixTimeNs, string name, List<GainOption> gainList)
        {
            Index = speedIndex;
            PixTimeNs = pixTimeNs;
            Name = name;
            GainList = gainList;
        }
    }
    /// <summary>
    /// Photometrics cameras may provide multiple readout ports, each port
    /// has a Number, Name and a list of Speeds under this port.
    /// Each Speed has an Index, a Readout rate in MHz, optional Name and
    /// a list of Gains available under the particular speed.
    /// Each Gain has an Index, an associated native bit depth and optional Name.
    /// </summary>
    public class PortOption
    {
        public int Number { get; private set; }
        public string Name { get; private set; }
        public List<SpeedOption> SpeedList { get; private set; }
        public PortOption(int portNumber, string portName, List<SpeedOption> speedList)
        {
            Number = portNumber;
            Name = portName;
            SpeedList = speedList;
        }
    }

    /// <summary>
    /// Selected Photometrics cameras support hardware post processing. The post processing
    /// table is dynamic and different for each camera model. The table consings of post
    /// processing 'features', each feature consists of one or more 'parameters'.
    /// This class holds information about post processing 'parameter'.
    /// </summary>
    public class PostProcessingParameter
    {
        public PostProcessingParameter(uint id, string name, uint minVal, uint maxVal, uint defaultVal, uint currentVal)
        {
            ID = id;
            Name = name;
            MinValue = minVal;
            MaxValue = maxVal;
            DefaultValue = defaultVal;
            CurrentValue = currentVal;
        }
        public uint ID { get; private set; }
        public string Name { get; private set; }
        public uint MinValue { get; private set; }
        public uint MaxValue { get; private set; }
        public uint DefaultValue { get; private set; }
        public uint CurrentValue { get; set; }
    }

    /// <summary>
    /// Selected Photometrics cameras support hardware post processing. The post processing
    /// table is dynamic and different for each camera model. The table consings of post
    /// processing 'features', each feature consists of one or more 'parameters'.
    /// This class holds information about post processing 'feature'.
    /// </summary>
    public class PostProcessingFeature
    {
        public PostProcessingFeature(uint id, string name, List<PostProcessingParameter> parameters)
        {
            ID = id;
            Name = name;
            FunctionList = parameters;
        }
        public uint ID { get; private set; }
        public string Name { get; private set; }
        public List<PostProcessingParameter> FunctionList { get; private set; }
    }

    /// <summary>
    /// This class is a wrapper over native frame hardware metadata. Each 'FrameMetadata' has
    /// one or more 'FrameRoiMetadata' metadata, depending on the number of regions
    /// the frame contains.
    /// This class holds selected information, not all native fields may be present.
    /// </summary>
    public class FrameMetadata
    {
        public uint FrameNr;
        public ushort RoiCount;
        public ulong TimeStampBOF;
        public ulong TimeStampEOF;
        public ulong ExpTime;
        public FrameRoiMetadata[] RoiMetadata;

        public FrameMetadata(int expectedRoiCount)
        {
            FrameNr = 0;
            RoiCount = 0;
            TimeStampBOF = 0;
            TimeStampEOF = 0;
            ExpTime = 0;
            RoiMetadata = new FrameRoiMetadata[expectedRoiCount];
        }
    }

    /// <summary>
    /// This structure is a wrapper over native frame hardware metadata. Each 'FrameMetadata' has
    /// one or more 'FrameRoiMetadata' metadata, depending on the number of regions
    /// the frame contains.
    /// This class holds selected information, not all native fields may be present.
    /// </summary>
    public struct FrameRoiMetadata
    {
        public uint RoiNr;
        public ushort S1;
        public ushort S2;
        public ushort P1;
        public ushort P2;
    }

    /// <summary>
    /// This class provides simplified access and acquisition controls of a PVCAM camera.
    /// 
    /// Please note that we currently do send an event for every acquired frame. However, with
    /// cameras reaching 10000+ frames per second, the caller should rather poll this class
    /// for latest frames and use those for display. If streaming to RAM or Disk is needed,
    /// such streaming should be done directly in the PVCAM callback method and only progress
    /// notifications should be sent to the GUI or user of this class.
    /// </summary>
    public class PvcamController
    {
        /// <summary>
        /// Checks if given parameter is available.
        /// Please note that if the parameter is completely unknown to PVCAM (i.e. when
        /// asking older PVCAM versions for a parameter introduced in newer versions, the
        /// pl_get_param() function fails. This function does not throw an exception in such case
        /// and simply treats the given parameter as unavailable.
        /// </summary>
        /// <param name="hCam">Camera handle</param>
        /// <param name="paramId">Paramter ID</param>
        /// <returns></returns>
        public static bool IsParamAvailable(short hCam, PVCAM.PL_PARAMS paramId)
        {
            bool retVal = false;
            IntPtr unmngUInt16 = Marshal.AllocHGlobal(sizeof(ushort));
            if (PVCAM.pl_get_param(hCam, (uint)paramId, (short)PVCAM.PL_PARAM_ATTRIBUTES.ATTR_AVAIL, unmngUInt16))
            {
                retVal = Convert.ToBoolean(Marshal.ReadInt16(unmngUInt16));
            }
            Marshal.FreeHGlobal(unmngUInt16);
            return retVal;
        }

        /// <summary>
        /// 读参数
        /// </summary>
        /// <param name="hCam"></param>
        /// <param name="paramId"></param>
        /// <param name="attrId"></param>
        /// <returns></returns>
        public static bool ReadParamBool(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            // rs_bool is ushort type in PVCAM, use existing methods
            ushort ret = ReadParamUInt16(hCam, paramId, attrId);
            return (ret != (ushort)PVCAM.BoolValue.FALSE);
        }

        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="hCam"></param>
        /// <param name="paramId"></param>
        /// <param name="val"></param>
        public static void WriteParamBool(short hCam, PVCAM.PL_PARAMS paramId, bool val)
        {
            // rs_bool is ushort type in PVCAM, use existing methods
            WriteParamUInt16(hCam, paramId, val ? (ushort)PVCAM.BoolValue.TRUE : (ushort)PVCAM.BoolValue.FALSE);
        }
        public static sbyte ReadParamInt8(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngInt8 = Marshal.AllocHGlobal(sizeof(sbyte));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngInt8))
            {
                Marshal.FreeHGlobal(unmngInt8);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            sbyte value = (sbyte)Marshal.ReadByte(unmngInt8);
            Marshal.FreeHGlobal(unmngInt8);
            return value;
        }
        public static void WriteParamInt8(short hCam, PVCAM.PL_PARAMS paramId, sbyte val)
        {
            IntPtr unmngInt8 = Marshal.AllocHGlobal(sizeof(sbyte));
            Marshal.WriteByte(unmngInt8, (byte)val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngInt8))
            {
                Marshal.FreeHGlobal(unmngInt8);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngInt8);
        }
        public static byte ReadParamUInt8(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngUInt8 = Marshal.AllocHGlobal(sizeof(byte));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngUInt8))
            {
                Marshal.FreeHGlobal(unmngUInt8);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            byte value = Marshal.ReadByte(unmngUInt8);
            Marshal.FreeHGlobal(unmngUInt8);
            return value;
        }
        public static void WriteParamUInt8(short hCam, PVCAM.PL_PARAMS paramId, byte val)
        {
            IntPtr unmngUInt8 = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(unmngUInt8, val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngUInt8))
            {
                Marshal.FreeHGlobal(unmngUInt8);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngUInt8);
        }
        public static short ReadParamInt16(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngInt16 = Marshal.AllocHGlobal(sizeof(short));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngInt16))
            {
                Marshal.FreeHGlobal(unmngInt16);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            short value = Marshal.ReadInt16(unmngInt16);
            Marshal.FreeHGlobal(unmngInt16);
            return value;
        }
        public static void WriteParamInt16(short hCam, PVCAM.PL_PARAMS paramId, short val)
        {
            IntPtr unmngInt16 = Marshal.AllocHGlobal(sizeof(short));
            Marshal.WriteInt16(unmngInt16, val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngInt16))
            {
                Marshal.FreeHGlobal(unmngInt16);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngInt16);
        }
        public static ushort ReadParamUInt16(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngUInt16 = Marshal.AllocHGlobal(sizeof(ushort));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngUInt16))
            {
                Marshal.FreeHGlobal(unmngUInt16);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            ushort value = (ushort)Marshal.ReadInt16(unmngUInt16);
            Marshal.FreeHGlobal(unmngUInt16);
            return value;
        }
        public static void WriteParamUInt16(short hCam, PVCAM.PL_PARAMS paramId, ushort val)
        {
            IntPtr unmngUInt16 = Marshal.AllocHGlobal(sizeof(ushort));
            Marshal.WriteInt16(unmngUInt16, (short)val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngUInt16))
            {
                Marshal.FreeHGlobal(unmngUInt16);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngUInt16);
        }
        public static int ReadParamInt32(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngInt32 = Marshal.AllocHGlobal(sizeof(int));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngInt32))
            {
                Marshal.FreeHGlobal(unmngInt32);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            int value = Marshal.ReadInt32(unmngInt32);
            Marshal.FreeHGlobal(unmngInt32);
            return value;
        }
        public static void WriteParamInt32(short hCam, PVCAM.PL_PARAMS paramId, int val)
        {
            IntPtr unmngInt32 = Marshal.AllocHGlobal(sizeof(int));
            Marshal.WriteInt32(unmngInt32, val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngInt32))
            {
                Marshal.FreeHGlobal(unmngInt32);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngInt32);
        }
        public static uint ReadParamUInt32(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngUInt32 = Marshal.AllocHGlobal(sizeof(uint));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngUInt32))
            {
                Marshal.FreeHGlobal(unmngUInt32);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            uint value = (uint)Marshal.ReadInt32(unmngUInt32);
            Marshal.FreeHGlobal(unmngUInt32);
            return value;
        }
        public static void WriteParamUInt32(short hCam, PVCAM.PL_PARAMS paramId, uint val)
        {
            IntPtr unmngUInt32 = Marshal.AllocHGlobal(sizeof(uint));
            Marshal.WriteInt32(unmngUInt32, (int)val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngUInt32))
            {
                Marshal.FreeHGlobal(unmngUInt32);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngUInt32);
        }
        public static long ReadParamInt64(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngInt64 = Marshal.AllocHGlobal(sizeof(long));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngInt64))
            {
                Marshal.FreeHGlobal(unmngInt64);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            long value = Marshal.ReadInt64(unmngInt64);
            Marshal.FreeHGlobal(unmngInt64);
            return value;
        }
        public static void WriteParamInt64(short hCam, PVCAM.PL_PARAMS paramId, uint val)
        {
            IntPtr unmngInt64 = Marshal.AllocHGlobal(sizeof(long));
            Marshal.WriteInt64(unmngInt64, val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngInt64))
            {
                Marshal.FreeHGlobal(unmngInt64);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngInt64);
        }
        public static ulong ReadParamUInt64(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            IntPtr unmngUInt64 = Marshal.AllocHGlobal(sizeof(ulong));
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngUInt64))
            {
                Marshal.FreeHGlobal(unmngUInt64);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            ulong value = (ulong)Marshal.ReadInt64(unmngUInt64);
            Marshal.FreeHGlobal(unmngUInt64);
            return value;
        }
        public static void WriteParamUInt64(short hCam, PVCAM.PL_PARAMS paramId, ulong val)
        {
            IntPtr unmngUInt64 = Marshal.AllocHGlobal(sizeof(ulong));
            Marshal.WriteInt64(unmngUInt64, (long)val);
            if (!PVCAM.pl_set_param(hCam, (uint)paramId, unmngUInt64))
            {
                Marshal.FreeHGlobal(unmngUInt64);
                throw new PvcamSetParamException("Failed to write camera parameter", paramId, PVCAM.pl_error_code());
            }
            Marshal.FreeHGlobal(unmngUInt64);
        }
        public static float ReadParamFlt32(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            // Things are done a bit differently for floats as Marshal does not have ReadSingle() and ReadDouble().
            byte[] array = new byte[sizeof(float)];
            GCHandle pinnedArray = GCHandle.Alloc(array, GCHandleType.Pinned);
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, pinnedArray.AddrOfPinnedObject()))
            {
                pinnedArray.Free();
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            float value = BitConverter.ToSingle(array, 0);
            pinnedArray.Free();
            return value;
        }
        public static double ReadParamFlt64(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            // Things are done a bit differently for floats as Marshal does not have ReadSingle() and ReadDouble().
            byte[] array = new byte[sizeof(double)];
            GCHandle pinnedArray = GCHandle.Alloc(array, GCHandleType.Pinned);
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, pinnedArray.AddrOfPinnedObject()))
            {
                pinnedArray.Free();
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            double value = BitConverter.ToDouble(array, 0);
            pinnedArray.Free();
            return value;
        }
        public static string ReadParamString(short hCam, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attrId, int stringLength)
        {
            IntPtr unmngString = Marshal.AllocHGlobal(stringLength);
            if (!PVCAM.pl_get_param(hCam, (uint)paramId, (short)attrId, unmngString))
            {
                Marshal.FreeHGlobal(unmngString);
                throw new PvcamGetParamException("Failed to read camera parameter", paramId, attrId, PVCAM.pl_error_code());
            }
            string value = Marshal.PtrToStringAnsi(unmngString);
            Marshal.FreeHGlobal(unmngString);
            return value;
        }
        public static List<Tuple<int, string>> ReadParamEnumList(short hCam, PVCAM.PL_PARAMS paramId)
        {
            uint count = ReadParamUInt32(hCam, paramId, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_COUNT);
            List<Tuple<int, string>> list = new List<Tuple<int, string>>();

            for (uint i = 0; i < count; ++i)
            {
                if (!PVCAM.pl_enum_str_length(hCam, (uint)paramId, i, out uint strLength))
                    throw new PvcamException("Failed to read enum value name length", PVCAM.pl_error_code());

                StringBuilder enumNameSb = new StringBuilder((int)strLength);
                if (!PVCAM.pl_get_enum_param(hCam, (uint)paramId, i, out int enumValue, enumNameSb, strLength))
                    throw new PvcamException("Failed to read enum value name", PVCAM.pl_error_code());

                list.Add(new Tuple<int, string>(enumValue, enumNameSb.ToString()));
            }
            return list;
        }

        public delegate void CameraNotificationHandler(PvcamController sender, CameraNotificationEventArgs args);
        public event CameraNotificationHandler OnCameraNotification;

        public const int RUN_UNTIL_STOPPED = int.MaxValue;

        /// <summary>
        /// 数据获取模式
        /// </summary>
        public enum AcquisitionType { Sequence, Continuous }

        public AcquisitionType AcqType { get; private set; } = AcquisitionType.Sequence;

        /// <summary>
        /// 曝光时间
        /// </summary>
        public uint ExposureTime { get; set; } = 0;

        /// <summary>
        /// 最大曝光时间
        /// </summary>
        public ulong ExposureTimeMax { get; private set; } = 0;

        /// <summary>
        /// 最小曝光时间
        /// </summary>
        public ulong ExposureTimeMin { get; private set; } = 0;

        public List<Tuple<int, string>> ExposureResolutions { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_EXP_RES_MODES ExposureResolution
        {
            get
            {
                int cur = ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_EXP_RES, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return (PVCAM.PL_EXP_RES_MODES)cur;
            }
            set
            {
                int pvcamVal = (int)value;
                WriteParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_EXP_RES, pvcamVal);
                // If the exposure resolution changes, re-read the exposure ranges
                ExposureTimeMax = ReadParamUInt64(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                ExposureTimeMin = ReadParamUInt64(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
                ExposureTime = 0; // Reset the exposure time
            }
        }

        public List<Tuple<int, string>> PModes { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_PMODES PMode
        {
            get
            {
                int cur = ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PMODE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return (PVCAM.PL_PMODES)cur;
            }
            set
            {
                // Validation
                int pvcamVal = (int)value;
                if (PModes.FindIndex(m => m.Item1 == (int)value) == -1)
                    throw new Exception("Mode not available");
                WriteParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PMODE, pvcamVal);
            }
        }

        public List<Tuple<int, string>> TriggerModes { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_EXPOSURE_MODES TriggerMode
        {
            get { return m_triggerMode; }
            set
            {
                // Trigger mode is only applied when pl_exp_setup_seq() or pl_exp_setup_cont() is called
                // this we just remember the value and set it later when setting up the acquisition.

                // Validation
                if (TriggerModes.FindIndex(m => m.Item1 == (int)value) == -1)
                    throw new Exception("Mode not available");

                m_triggerMode = value;
            }
        }

        public List<Tuple<int, string>> ExposeOutModes { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_EXPOSE_OUT_MODES ExposeOutMode
        {
            get { return m_exposeOutMode; }
            set
            {
                // Expose out mode is only applied when pl_exp_setup_seq() or pl_exp_setup_cont() is called
                // this we just remember the value and set it later when setting up the acquisition.

                // Validation
                if (ExposeOutModes.FindIndex(m => m.Item1 == (int)value) == -1)
                    throw new Exception("Mode not available");

                m_exposeOutMode = value;
            }
        }

        public List<Tuple<int, string>> ClearModes { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_CLEAR_MODES ClearMode
        {
            get
            {
                int cur = ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_MODE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return (PVCAM.PL_CLEAR_MODES)cur;
            }
            set
            {
                int pvcamVal = (int)value;
                WriteParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_MODE, pvcamVal);
            }
        }

        public ushort ClearCycles
        {
            get
            {
                ushort val = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_CYCLES, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return val;
            }
            set
            {
                WriteParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_CYCLES, value);
            }
        }

        public List<Tuple<int, string>> FanSpeedSetpoints { get; private set; } = new List<Tuple<int, string>>();
        public PVCAM.PL_FAN_SPEEDS FanSpeedSetpoint
        {
            get
            {
                int cur = ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_FAN_SPEED_SETPOINT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return (PVCAM.PL_FAN_SPEEDS)cur;
            }
            set
            {
                int pvcamVal = (int)value;
                WriteParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_FAN_SPEED_SETPOINT, pvcamVal);
            }
        }

        public short BitDepth { get; private set; } = 0;

        /// <summary>
        /// Sets the camera regions of interest (ROI). Use this property to set a single
        /// region of interest. To set multiple regions of interests for cameras that
        /// support the feature, use the RegionList property.
        /// This is a conventience property for applications that do not have to work
        /// with multiple regions. Setting this field will reset the RegionList property
        /// to a single region.
        /// </summary>
        public PVCAM.rgn_type Region
        {
            get { return m_regions[0]; }
            set
            {
                m_regions = new PVCAM.rgn_type[1];
                m_regions[0] = value;
            }
        }
        /// <summary>
        /// List of regions of interests for cameras that support the multiple-ROI feature.
        /// </summary>
        public PVCAM.rgn_type[] RegionList
        {
            get { return m_regions; }
            set { m_regions = value; }
        }

        /// <summary>
        /// Maximum number of user-defined ROIs this camera supports.
        /// </summary>
        public ushort RegionCountMax { get; private set; } = 0;
        /// <summary>
        /// List of available binning factors.
        /// If the camera supports reporting of the binning factors, this list will
        /// contain the camera reported values. If this is an old CCD camera, the list
        /// will contain a preselected list of most common binnings.
        /// </summary>
        public List<Tuple<ushort, ushort>> RegionBinningFactors { get; private set; }
        /// <summary>
        /// Set or get the current binning factor.
        /// </summary>
        public Tuple<ushort, ushort> RegionBinningFactor
        {
            get
            {
                // Currently, all ROIs must have the same binning, thus, just return the first one
                return new Tuple<ushort, ushort>(m_regions[0].sbin, m_regions[0].pbin);
            }
            set
            {
                // Validate
                if (RegionBinningFactors.FindIndex(s => (s.Item1 == value.Item1) && (s.Item2 == value.Item2)) == -1)
                {
                    throw new Exception("Invalid binning combination");
                }
                // Set to all ROIs
                for (int i = 0; i < RegionList.Length; i++)
                {
                    m_regions[i].sbin = value.Item1;
                    m_regions[i].pbin = value.Item2;
                }
            }
        }

        public bool CentroidsSupported { get; private set; } = false;
        public ushort CentroidsCountMin { get; private set; } = 0;
        public ushort CentroidsCountMax { get; private set; } = 0;
        public ushort CentroidsRadiusMin { get; private set; } = 0;
        public ushort CentroidsRadiusMax { get; private set; } = 0;

        /// <summary>
        /// Controls the Centroiding feature enabled state.
        /// This property should not be accessed if CentroindsSupported is false.
        /// </summary>
        public bool CentroidsEnabled
        {
            get
            {
                return ReadParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_ENABLED, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            }
            set
            {
                WriteParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_ENABLED, value);
                m_centroidsEnabledCache = value;
            }
        }
        private bool m_centroidsEnabledCache = false;
        /// <summary>
        /// Controls the Centroiding feature radius value.
        /// This property should not be accessed if CentroindsSupported is false.
        /// </summary>
        public ushort CentroidsRadius
        {
            get
            {
                ushort val = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_RADIUS, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return val;
            }
            set
            {
                WriteParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_RADIUS, value);
                m_centroidsRadiusCache = value;
            }
        }
        private ushort m_centroidsRadiusCache = 0;
        /// <summary>
        /// Controls the Centroiding feature count value.
        /// This property should not be accessed if CentroindsSupported is false.
        /// </summary>
        public ushort CentroidsCount
        {
            get
            {
                ushort val = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_COUNT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return val;
            }
            set
            {
                WriteParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_COUNT, value);
                m_centroidsCountCache = value;
            }
        }
        private ushort m_centroidsCountCache = 0;

        /// <summary>
        /// Camera system name as reported by pl_cam_get_name(). This usually corresponds
        /// to something like "PmUsbCam01" or similar.
        /// </summary>
        public string CameraName { get; private set; } = string.Empty;

        public int FramesAcquired { get; private set; } = 0;
        public int FramesToAcquire { get; private set; } = 0;

        public ushort SensorSizeX { get; private set; } = 0;
        public ushort SensorSizeY { get; private set; } = 0;
        public ushort SensorPixelSizeX { get; private set; } = 0;
        public ushort SensorPixelSizeY { get; private set; } = 0;
        public ushort SensorPixelDistanceX { get; private set; } = 0;
        public ushort SensorPixelDistanceY { get; private set; } = 0;
        public string ChipName { get; private set; } = "";
        public string SystemName { get; private set; } = "";
        public string VendorName { get; private set; } = "";
        public string ProductName { get; private set; } = "";
        public string PartNumber { get; private set; } = "";
        public string SerialNumber { get; private set; } = "";
        public ushort FirmwareVersionMajor { get; private set; } = 0;
        public ushort FirmwareVersionMinor { get; private set; } = 0;
        public PVCAM.PL_COLOR_MODES SensorColorMode { get; private set; } = PVCAM.PL_COLOR_MODES.COLOR_NONE;

        /// <summary>
        /// Latest acquired frame data. Use this property with the properties below
        /// to retrieve the latest frame and corresponding latest metadata.
        /// Use the LatestFrameLock when accessing these properties.
        /// </summary>
        public ref byte[] LatestFrameData
        {
            get { return ref m_outputPixelData; }
        }
        /// <summary>
        /// Contains Embedded frame metadata, if enabled.
        /// Returns null if frame metadata is not enabled for current acquisition.
        /// </summary>
        internal FrameMetadata LatestFrameMetadata { get; private set; } = null;
        /// <summary>
        /// Returns latest FRAME_INFO.
        /// </summary>
        public PVCAM.FRAME_INFO LatestFrameInfo { get { return m_frameInfoLatest; } }
        /// <summary>
        /// Use this lock when accessing the LatestFrameData and metadata. Copy all necessary
        /// information for display and unlock the frame.
        /// </summary>
        public object LatestFrameLock { get { return m_acqLock; } }
        /// <summary>
        /// Frame number generated by the application. Ever increasing during camera
        /// lifetime. Can be used as unique identifier of a frame (for example for
        /// GUI to check if this frame was already displayed or not)
        /// </summary>
        public ulong FrameSequenceNumber { get; private set; } = 0;
        /// <summary>
        /// Current frame rate. The rate will depend on a computer performance.
        /// </summary>
        public double FrameRate { get; private set; } = 0.0;

        public bool MultiplicationGainSupported { get; private set; } = false;
        public ushort MultiplicationGainMax { get; private set; } = 0;
        public ushort MultiplicationGain
        {
            get
            {
                return ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_GAIN_MULT_FACTOR, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            }
            set
            {
                WriteParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_GAIN_MULT_FACTOR, value);
            }
        }

        public bool SmartStreamingSupported { get; private set; } = false;

        /// <summary>
        /// Reads the current camera temperature.
        /// Do not access this parameter while acquisition is running.
        /// </summary>
        public short Temperature
        {
            get
            {
                short val = ReadParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_TEMP, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return val;
            }
        }
        public short TemperatureSetpoint
        {
            get
            {
                short val = ReadParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_TEMP_SETPOINT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                return val;
            }
            set
            {
                if (value < TemperatureSetpointMin || value > TemperatureSetpointMax)
                {
                    throw new Exception("Value out of range");
                }
                WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_TEMP_SETPOINT, value);
            }
        }
        public short TemperatureSetpointMin { get; private set; } = 0;
        public short TemperatureSetpointMax { get; private set; } = 0;

        /// <summary>
        /// Current camera readout time, this value becomes available after the acquisition is setup.
        /// Not all cameras support this feature.
        /// </summary>
        public uint ReadoutTime { get; private set; } = 0;
        public bool ReadoutTimeSupported { get; private set; } = false;
        /// <summary>
        /// Current image format. Correct value is assigned after the acquisition is setup.
        /// </summary>
        public PVCAM.PL_IMAGE_FORMATS ImageFormat { get; private set; } = PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16;

        public bool FrameMetadataSupported { get; private set; } = false;
        public bool FrameMetadataEnabled
        {
            get
            {
                return ReadParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_METADATA_ENABLED, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            }
            set
            {
                WriteParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_METADATA_ENABLED, value);
                // Cache the actual value because we use it internally during acquisition
                m_frameMetadataEnabledCache = value;
            }
        }
        private bool m_frameMetadataEnabledCache = false;

        public List<PostProcessingFeature> PostProcessingFeatures { get; private set; } = new List<PostProcessingFeature>();

        public delegate void PostProcessingFeaturesEventHandler(PvcamController sender, List<PostProcessingFeature> args);
        public event PostProcessingFeaturesEventHandler OnPostProcessingFeaturesChanged;

        /// <summary>
        /// Final Image size. The size depends on whether multi-ROI or Centoiding feature is used. For simple frames,
        /// the size corresponds to the selected region size. For Multi-ROI frames or Centroiding frames, the size
        /// will be reported as full frame size as those frames are recomposed into black-filled sensor-sized image.
        /// </summary>
        public Size OutputImageSize { get; private set; } = new Size();

        /// <summary>
        /// 是否正在获取数据
        /// </summary>
        public bool IsAcquiring { get; private set; } = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PvcamController()
        {
            m_eofCallbackDelegate = new PVCAM.PMCallBack_Ex3(PvcamCallbackEof);
            m_eofCallbackContext = new CallbackContext();
        }

        private void CommandThreadFunc()
        {
            lock (m_acqLock)
            {
                while (m_commandThreadCommand != Command.Exit)
                {
                    if (m_commandThreadCommand != Command.None)
                    {
                        // Execute
                        if (m_commandThreadCommand == Command.Exit)
                            break;
                        switch (m_commandThreadCommand)
                        {
                            case Command.Abort:
                                if (!PVCAM.pl_exp_abort(m_hCam, (short)PVCAM.PL_CCS_ABORT_MODES.CCS_CLEAR))
                                {
                                    // throw new PvcamException("Failed to abort the acquisition", PVCAM.pl_error_code());
                                }
                                OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.AcquisitionFinished));
                                break;
                        }
                        m_commandThreadCommand = Command.None;
                    }
                    else
                    {
                        // Unlock and wait
                        Monitor.Wait(m_acqLock);
                    }
                }
            }
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="PvcamException"></exception>
        public void Open(string name)
        {
            StringBuilder sb = new StringBuilder(name);
            if (!PVCAM.pl_cam_open(sb, out m_hCam, PVCAM.PL_OPEN_MODES.OPEN_EXCLUSIVE))
            {
                throw new PvcamException("Failed to open the camera", PVCAM.pl_error_code());
            }

            CameraName = name;

            Initialize();

            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Camera '{0}' opened", name)));
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <param name="force"></param>
        /// <exception cref="PvcamException"></exception>
        public void Close(bool force = false)
        {
            // Uninitialize and close the camera, errors are only treated as warnings.
            // Do not error out, make best effort to close everything despite errors.

            Uninitialize();

            // Close the camera
            if (!PVCAM.pl_cam_close(m_hCam))
            {
                if (!force)
                    throw new PvcamException("Failed to close the camera", PVCAM.pl_error_code());
            }

            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, "Camera closed"));
        }

        /// <summary>
        /// 设置获取数据的模式
        /// </summary>
        /// <param name="acqType"></param>
        /// <param name="framesToAcquire"></param>
        /// <exception cref="PvcamException"></exception>
        public void SetupAcquisition(AcquisitionType acqType, int framesToAcquire)
        {
            // If SMART streaming mode is on, the exposure time passed to setup functions
            // must be non-zero value and all exposures are defined by SMART streaming parameters
            // (supported on selected camera models)
            if (m_isSmartStreamingActive)
                ExposureTime = 10;

            // Exposure mode is an or-ed value of trigger mode and expose-out mode. Cameras that do not support
            // expose-out mode will still work fine as those have the m_exposeOutMode set to 0.
            short expMode = (short)((int)m_triggerMode | (int)m_exposeOutMode);

            // Metadata should be enabled for MultiROI or centroiding. Enable it if it is not already done
            if (((RegionList.Length > 1) || (m_centroidsEnabledCache)) && (m_frameMetadataEnabledCache == false))
            {
                FrameMetadataEnabled = true; // Enable metadata
            }

            if (acqType == AcquisitionType.Sequence)
            {
                if (framesToAcquire > ushort.MaxValue)
                {
                    OnCameraNotification(this, new CameraNotificationEventArgs(
                        CameraNotificationType.CameraErrorMessage, "Sequences do not support more than 65535 frames"));
                    return;
                }
                ushort expTotal = (ushort)framesToAcquire;

                // Use sequences to acquire finite number of frames. In sequences, the camera counts the frames
                // and automatically stops when desired number of frames is acquires. This mode is useful to short
                // bursts, single snaps and where output signals must match the desired number of frames.
                // Sequences, however, are limited by number of frames and buffer size.
                // Optimization hint: if the acquisition parameters do not change, the "setup" routines can be called
                // only once, and then "start" repeatedly as many times as needed - this allows quick "software" triggering.

                if (!PVCAM.pl_exp_setup_seq(m_hCam, 1, (ushort)RegionList.Length, ref RegionList[0], expMode, ExposureTime,
                    out m_setupBufferSize))
                {
                    OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.CameraErrorMessage, "Single acquisition setup failed"));
                    return;
                }

                FramesToAcquire = framesToAcquire;
                FramesAcquired = 0;
            }
            else if (acqType == AcquisitionType.Continuous)
            {
                // Continuous mode is useful in situations where number of frames is not known in advance
                // or where a large number of frames needs to be acquired (usually, where sequences cannot be used).
                // In countinous mode, the application need to stop the acquisition manually.

                if (!PVCAM.pl_exp_setup_cont(m_hCam, (ushort)RegionList.Length, ref RegionList[0], expMode, ExposureTime,
                    out m_setupBufferSize, (short)PVCAM.PL_CIRC_MODES.CIRC_OVERWRITE))
                {
                    throw new PvcamException("Failed to setup continous acquisition", PVCAM.pl_error_code());
                }

                //populate fields of our example/test structure pointer to which
                //is being passed to callback registration function
                m_eofCallbackContext.ExposureTime = ExposureTime;
                m_eofCallbackContext.Binning = m_regions[0].sbin;

                FramesToAcquire = framesToAcquire;
                FramesAcquired = 0;
            }

            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_IMAGE_FORMAT))
            {
                ImageFormat = (PVCAM.PL_IMAGE_FORMATS)ReadParamInt32(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_IMAGE_FORMAT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            }
            else
            {
                // Parameter not available, expect 16 bit data
                ImageFormat = PVCAM.PL_IMAGE_FORMATS.PL_IMAGE_FORMAT_MONO16;
            }

            // Get the actual bit depth
            BitDepth = ReadParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_BIT_DEPTH, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

            //Allocate buffers based on frame size return by exposure setup
            SetupAcquisitionBuffers(acqType);

            // Get the estimated readout time for this acquisition
            if (ReadoutTimeSupported)
                ReadoutTime = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_READOUT_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

            AcqType = acqType;
        }

        /// <summary>
        /// 开始获取数据
        /// </summary>
        /// <exception cref="Exception"></exception>
        /// <exception cref="PvcamException"></exception>
        public void StartAcquisition()
        {
            lock (m_acqLock)
            {
                if (IsAcquiring)
                    throw new Exception("Already acquiring");

                // Set the acquiring flag here because at the time we exit this function,
                // the camera may already start sending frames (if we are working with very
                // fast camera).
                IsAcquiring = true;
                FrameRate = 0;
                m_fpsCounter = 0;
                m_fpsDuration = 0;
                m_fpsStopwatch.Start();

                switch (AcqType)
                {
                    case AcquisitionType.Sequence:
                        // Start the acquisition, PVCAM will start writing incoming frames
                        // into the given sequence buffer.
                        if (!PVCAM.pl_exp_start_seq(m_hCam, m_frameBuffer))
                        {
                            throw new PvcamException("Failed to start sequence acquisition", PVCAM.pl_error_code());
                        }
                        break;
                    case AcquisitionType.Continuous:
                        // Start the acquisition, PVCAM will start writing incoming frames
                        // into the given circular buffer.
                        if (!PVCAM.pl_exp_start_cont(m_hCam, m_frameBuffer, (uint)m_frameBuffSize))
                        {
                            throw new PvcamException("Failed to start continuous acquisition", PVCAM.pl_error_code());
                        }
                        break;
                    default:
                        throw new Exception("Unsupported acquisition type");
                }
            }
            OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.AcquisitionStarted));
        }

        public void SwTrigger()
        {
            uint flags = 0;
            uint value = 0;
            if (!PVCAM.pl_exp_trigger(m_hCam, ref flags, value))
            {
                throw new PvcamException("Failed to issue software trigger", PVCAM.pl_error_code());
            }
        }

        /// <summary>
        /// 停止获取数据
        /// </summary>
        /// <param name="force"></param>
        public void StopAcquisition(bool force = false)
        {
            lock (m_acqLock)
            {
                if (!IsAcquiring && !force)
                    return;

                IsAcquiring = false;

                // Send the abort command to the camera
                m_commandThreadCommand = Command.Abort;
                Monitor.Pulse(m_acqLock);
            }
        }

        public void WaitForFullAcquisitionStop()
        {
            if (!IsAcquiring)
            {
                return;
            }
        }

        //Reads post processing features on the camera and stores in structure.
        private void ReadPostProcessingFeatures()
        {
            PostProcessingFeatures.Clear();
            if (!IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_INDEX))
                return;

            // Get count of Post Processing *features*
            short featCount = ReadParamInt16(
                m_hCam, PVCAM.PL_PARAMS.PARAM_PP_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_COUNT);

            for (short i = 0; i < featCount; ++i)
            {
                // Set the Feature index
                WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_INDEX, i);
                // Get the Feature Name, ID and number of parameters this feature has
                string featureName = ReadParamString(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_PP_FEAT_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_PP_NAME_LEN);
                ushort featureID = ReadParamUInt16(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_PP_FEAT_ID, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                short paramCount = ReadParamInt16(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_COUNT);

                List<PostProcessingParameter> parameterList = new List<PostProcessingParameter>();

                // Loop through the parameters
                for (short j = 0; j < paramCount; ++j)
                {
                    // Set the parameters index
                    WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM_INDEX, j);
                    // Get the parameter name, ID and value range
                    string functionName = ReadParamString(
                        m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_PP_NAME_LEN);
                    ushort functionID = ReadParamUInt16(
                        m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM_ID, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                    uint minValue = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
                    uint maxValue = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                    uint defValue = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_DEFAULT);
                    uint curValue = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

                    PostProcessingParameter ppParameter = new PostProcessingParameter(functionID, functionName, minValue, maxValue, defValue, curValue);
                    parameterList.Add(ppParameter);
                }
                PostProcessingFeature ppFeature = new PostProcessingFeature(featureID, featureName, parameterList);
                PostProcessingFeatures.Add(ppFeature);
            }
        }

        //Write updated post processing features/function to the camera 
        public void SetPostProcessingFeature(short featIdx, short funcIdx, uint value)
        {
            // Set the feature and function indexes first
            WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_INDEX, featIdx);
            WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM_INDEX, funcIdx);
            // Now set the value
            WriteParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, value);
            // Read the value back to confirm
            uint newVal = ReadParamUInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_PP_PARAM, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

            // Cache the current value
            PostProcessingFeature ppFeature = PostProcessingFeatures[featIdx];
            PostProcessingParameter ppParam = ppFeature.FunctionList[funcIdx];
            ppParam.CurrentValue = newVal;
            PostProcessingFeatures[featIdx] = ppFeature;

            // Check if the camera actually applied it
            if (value != newVal)
            {
                throw new Exception(string.Format("Post-processing value '{0}' not accepted. Current value is '{1}'", value, newVal));
            }

            string msg = "PP feature '" + ppFeature.Name + "', parameter '" + ppParam.Name + "' set to " + value.ToString();

            OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.CameraStatusMessage, msg));
        }

        /// <summary>
        /// Reads basic camera information, identification and capabilities.
        /// </summary>
        private void ReadCameraInformation()
        {
            // Get serial (X) and parallel (Y) size of the sensor
            SensorSizeX = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_SER_SIZE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            SensorSizeY = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PAR_SIZE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Sensor size: {0}x{1}", SensorSizeX, SensorSizeY)));
            // Pixel Size
            SensorPixelSizeX = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PIX_SER_SIZE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            SensorPixelSizeY = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PIX_PAR_SIZE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Sensor pixel size: {0}x{1}", SensorPixelSizeX, SensorPixelSizeY)));
            // Pixel distance
            SensorPixelDistanceX = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PIX_SER_DIST, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            SensorPixelDistanceY = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_PIX_PAR_DIST, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Sensor pixel distance: {0}x{1}", SensorPixelDistanceX, SensorPixelDistanceY)));
            // Sensor/chip Name
            ChipName = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_CHIP_NAME))
                ChipName = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_CHIP_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.CCD_NAME_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Chip name: {0}", ChipName)));
            // System Name
            SystemName = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_SYSTEM_NAME))
                SystemName = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_SYSTEM_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_SYSTEM_NAME_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("System name: {0}", SystemName)));
            // Vendor Name
            VendorName = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_VENDOR_NAME))
                VendorName = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_VENDOR_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_VENDOR_NAME_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Vendor name: {0}", VendorName)));
            // Product Name
            ProductName = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_PRODUCT_NAME))
                ProductName = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_PRODUCT_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_PRODUCT_NAME_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Product name: {0}", ProductName)));
            // Camera part number
            PartNumber = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_CAMERA_PART_NUMBER))
                PartNumber = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_CAMERA_PART_NUMBER, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_CAM_PART_NUM_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Part number: {0}", PartNumber)));
            // Camera Head Serial number
            SerialNumber = "";
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_HEAD_SER_NUM_ALPHA))
                SerialNumber = ReadParamString(m_hCam, PVCAM.PL_PARAMS.PARAM_HEAD_SER_NUM_ALPHA, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_ALPHA_SER_NUM_LEN);
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Serial number: {0}", SerialNumber)));
            // Camera Firmware version
            FirmwareVersionMajor = 0;
            FirmwareVersionMinor = 0;
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_CAM_FW_VERSION))
            {
                ushort fwVerRaw = ReadParamUInt16(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_CAM_FW_VERSION, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                FirmwareVersionMajor = (ushort)((fwVerRaw & 0xFF00) >> 8);
                FirmwareVersionMinor = (ushort)((fwVerRaw & 0x00FF));
            }
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, string.Format("Firmware version: {0}.{1}", FirmwareVersionMajor, FirmwareVersionMinor)));
            // Color mode (if this is a color camera)
            SensorColorMode = PVCAM.PL_COLOR_MODES.COLOR_NONE;
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_COLOR_MODE))
                SensorColorMode = (PVCAM.PL_COLOR_MODES)ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_COLOR_MODE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            if (SensorColorMode != PVCAM.PL_COLOR_MODES.COLOR_NONE)
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "Sensor color mode: Color"));
            else
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "Sensor color mode: Monochrome"));
        }

        // Initialize the camera
        private void Initialize()
        {
            // Read basic camera information and capabilities
            ReadCameraInformation();

            // Find out if the sensor is the Frame Transfer type.
            // This is a legacy feature available only on some CCD camera models.
            m_isFrameTransferCapable = false;
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_FRAME_CAPABLE))
            {
                bool capable = ReadParamBool(
                    m_hCam, PVCAM.PL_PARAMS.PARAM_FRAME_CAPABLE, (short)PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                m_isFrameTransferCapable = capable;
            }
            if (m_isFrameTransferCapable)
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "Camera with Frame Transfer capability"));

            // Find out whether the camera supports multiplication gain - typically all Frame Transfer cameras
            // support EM (Multiplication) gain while none of the Interline/sCMOS cameras have multiplication gain.
            // This is a legacy feature available on CCD cameras only.
            MultiplicationGainSupported = IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_GAIN_MULT_FACTOR);
            if (MultiplicationGainSupported)
            {
                // If multiplication gain is available find its range (1 to ATTR_MAX)
                MultiplicationGainMax = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_GAIN_MULT_FACTOR, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                // Reset the EM gain to 1 upon camera opening
                MultiplicationGain = 1;
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "Multiplication gain is available"));
            }

            // Detect whether the camera supports SMART streaming mode or not
            SmartStreamingSupported = IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_SMART_STREAM_MODE);

            // If SMART streaming is supported, read maximum number of exposures in the
            // SMART streaming exposure list
            if (SmartStreamingSupported)
            {
                // To read the maximum number of exposures in the list allocate smart_stream_type structure
                // The maximum possible exposures will be returned in the .entries field of the
                // smart_stream_type variable
                PVCAM.smart_stream_type smartStreamVals = new PVCAM.smart_stream_type();
                IntPtr unmngSsStruct = Marshal.AllocHGlobal(Marshal.SizeOf(smartStreamVals));

                // Send the SMART streaming structure to camera so we can receive back the same structure
                // With .entries field populated with the maximum number of possible exposures
                if (!PVCAM.pl_get_param(m_hCam, (uint)PVCAM.PL_PARAMS.PARAM_SMART_STREAM_EXP_PARAMS, (short)PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX, unmngSsStruct))
                {
                    Marshal.FreeHGlobal(unmngSsStruct);
                    throw new PvcamException("Failed to read SMART streaming parameters", PVCAM.pl_error_code());
                }

                // Read the structure from unmanaged environment
                smartStreamVals = (PVCAM.smart_stream_type)Marshal.PtrToStructure(unmngSsStruct, typeof(PVCAM.smart_stream_type));
                Marshal.FreeHGlobal(unmngSsStruct);

                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, string.Format("Camera supports {0} S.M.A.R.T streaming exposures", smartStreamVals.entries)));
            }

            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_PMODE))
            {
                PModes = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_PMODE);
            }
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_MODE))
            {
                TriggerModes = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_MODE);
            }

            // Check if Expose Out Parameter is available on the camera
            ExposeOutModes.Clear();
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSE_OUT_MODE))
            {
                ExposeOutModes = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSE_OUT_MODE);
            }
            // Check Exposure resolutions
            ExposureResolutions.Clear();
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_EXP_RES))
            {
                ExposureResolutions = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_EXP_RES);
            }

            // Check if Clear Modes are available
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_MODE))
            {
                ClearModes = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_CLEAR_MODE);
            }

            // Check if camera supports embedded frame metadata and enable it
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_METADATA_ENABLED))
            {
                FrameMetadataSupported = true;
            }
            else
            {
                FrameMetadataSupported = false;
            }
            if (FrameMetadataSupported)
            {
                FrameMetadataEnabled = true; // Enable the metadata
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "Camera supports embedded frame metadata"));
            }

            // Check whether camera supports multiple-ROIs and how many
            RegionCountMax = 1;
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_ROI_COUNT))
            {
                RegionCountMax = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_ROI_COUNT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
            }

            // Allocate the region array
            m_regions = new PVCAM.rgn_type[1];
            // Set the actual ROI to full frame
            m_regions[0].s1 = 0;
            m_regions[0].s2 = (ushort)(SensorSizeX - 1);
            m_regions[0].sbin = 1;
            m_regions[0].p1 = 0;
            m_regions[0].p2 = (ushort)(SensorSizeY - 1);
            m_regions[0].pbin = 1;

            // Check if camera supports Centroiding feature
            CentroidsSupported = false;
            CentroidsCountMin = 0;
            CentroidsCountMax = 0;
            CentroidsRadiusMin = 0;
            CentroidsRadiusMax = 0;
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_ENABLED))
            {
                CentroidsCountMin = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_COUNT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
                CentroidsCountMax = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_COUNT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                CentroidsRadiusMin = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_RADIUS, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
                CentroidsRadiusMax = ReadParamUInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_CENTROIDS_RADIUS, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                CentroidsSupported = true;
            }

            // For Metadata and Centroiding, we need an 'md_frame' structure that will be used to decode
            // the incoming metadata-enabled frames into a structure allowing us to read the frame metadata and pixel data.
            // See also pl_md_frame_decode() function.
            // Preallocate the structure once, for max possible ROIs that we could ever expect.
            {
                m_mdFramePtr = Marshal.AllocHGlobal(Marshal.SizeOf(m_mdFrame));
                ushort maxRoiTotal = Math.Max(RegionCountMax, CentroidsCountMax);
                // Create the structure, we will share it across acquisitions
                if (!PVCAM.pl_md_create_frame_struct_cont(ref m_mdFramePtr, maxRoiTotal))
                {
                    Marshal.FreeHGlobal(m_mdFramePtr);
                    throw new PvcamException("Failed to create metadata structure", PVCAM.pl_error_code());
                }
                else
                {
                    m_mdFrame = (PVCAM.md_frame)Marshal.PtrToStructure(m_mdFramePtr, typeof(PVCAM.md_frame));
                }
                // Also create a set of ROI structure descriptors that we will be using when decoding frames.
                // The size of the structure will be needed to calculate offsets to next ROI structure.
                m_mdFrameRoiStructs = new PVCAM.md_frame_roi[maxRoiTotal];
                m_mdFrameRoiStructSize = Marshal.SizeOf(m_mdFrameRoiStructs[0]);
            }

            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_FAN_SPEED_SETPOINT))
            {
                FanSpeedSetpoints = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_FAN_SPEED_SETPOINT);
                //m_currentFanSpeed =  ReadParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_FAN_SPEED_SETPOINT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            }

            ReadPortTable();
            ReadPostProcessingFeatures();

            // Modern cameras support "extended binning", where the camera reports the list
            // of available binning combinations. Old CCD cameras had 'free' binnings where
            // we could set any value for the binning factor. Here we decide if the camera reports
            // the values and use them. If it does not report the factors, we use predefined set
            // of binnings, just to make it simple for the applications.
            RegionBinningFactors = new List<Tuple<ushort, ushort>>();
            if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_BINNING_SER))
            {
                List<Tuple<int, string>> binSer = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_BINNING_SER);
                List<Tuple<int, string>> binPar = ReadParamEnumList(m_hCam, PVCAM.PL_PARAMS.PARAM_BINNING_PAR);
                // Both these lists are supposed to have the same number of items
                for (int i = 0; i < binSer.Count; ++i)
                    RegionBinningFactors.Add(new Tuple<ushort, ushort>((ushort)binSer[i].Item1, (ushort)binPar[i].Item1));
            }
            else
            {
                // Most common binning factors, not all may be supported by a specific camera though
                ushort[] commonBins = new ushort[] { 1, 2, 3, 4, 8, 16, 32 };
                for (int x = 0; x < commonBins.Length; ++x)
                    for (int y = 0; y < commonBins.Length; ++y)
                        RegionBinningFactors.Add(new Tuple<ushort, ushort>(commonBins[x], commonBins[y]));
            }

            // Read cooling parameters
            TemperatureSetpointMin = ReadParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_TEMP_SETPOINT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
            TemperatureSetpointMax = ReadParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_TEMP_SETPOINT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);

            // Read camera exposure range
            ExposureTimeMax = ReadParamUInt64(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
            ExposureTimeMin = ReadParamUInt64(m_hCam, PVCAM.PL_PARAMS.PARAM_EXPOSURE_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);

            // Check if the camera supports Readout Time reporting
            ReadoutTimeSupported = IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_READOUT_TIME);

            // Pointer to structure passed to unmanaged environment should not change so tell
            // garbage collector to pin it
            m_eofCallbackContextHandle = GCHandle.Alloc(m_eofCallbackContext, GCHandleType.Pinned);

            // Register a callback function. PVCAM will be calling this function each time a new
            // frame is acquired in the buffer (EOF = end of frame readout)
            if (!PVCAM.pl_cam_register_callback_ex3(m_hCam, PVCAM.PL_CALLBACK_EVENT.PL_CALLBACK_EOF,
                m_eofCallbackDelegate, m_eofCallbackContextHandle.AddrOfPinnedObject()))
            {
                throw new PvcamException("Failed to register EOF callback", PVCAM.pl_error_code());
            }

            m_commandThread = new Thread(new ThreadStart(CommandThreadFunc));
            m_commandThread.Start();
        }

        private void Uninitialize()
        {
            // Opposite to Initialize()
            // Check everything before tearing it down, camera may be half initialized and not all
            // resources may be allocated. Do not error out, do best effort to uninit the class.

            if (m_commandThread != null)
            {
                lock (m_acqLock)
                {
                    m_commandThreadCommand = Command.Exit;
                    Monitor.Pulse(m_acqLock);
                }
                m_commandThread.Join();
                m_commandThread = null;
            }

            // Deregister the callback
            if (!PVCAM.pl_cam_deregister_callback(m_hCam, PVCAM.PL_CALLBACK_EVENT.PL_CALLBACK_EOF))
            {
                PvcamException exc = new PvcamException("Failed to deregister PVCAM callback", PVCAM.pl_error_code());
                OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.CameraErrorMessage, exc.Message));
            }

            if (m_eofCallbackContextHandle.IsAllocated)
                m_eofCallbackContextHandle.Free();

            if (m_mdFramePtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_mdFramePtr);
                m_mdFramePtr = IntPtr.Zero;
            }
        }

        #region ================================================== PORT OPTIONS
        private List<PortOption> m_portListCurrent;
        public List<PortOption> PortList
        {
            get { return m_portListCurrent; }
            private set
            {
                m_portListCurrent = value;
                OnPortListChanged(this, value);
            }
        }
        public delegate void PortListEventHandler(PvcamController sender, List<PortOption> args);
        public event PortListEventHandler OnPortListChanged;

        private PortOption m_portCurrent;
        public PortOption Port
        {
            get { return m_portCurrent; }
            set
            {
                WriteParamInt32(m_hCam, PVCAM.PL_PARAMS.PARAM_READOUT_PORT, value.Number);

                m_portCurrent = value;

                SpeedList = value.SpeedList;
                Speed = value.SpeedList[0];

                OnPortChanged(this, value);
            }
        }
        public delegate void PortEventHandler(PvcamController sender, PortOption args);
        public event PortEventHandler OnPortChanged;
        #endregion

        #region ================================================= SPEED OPTIONS
        private List<SpeedOption> m_speedList;
        public List<SpeedOption> SpeedList
        {
            get { return m_speedList; }
            private set
            {
                m_speedList = value;
                OnSpeedListChanged(this, value);
            }
        }
        public delegate void SpeedListEventHandler(PvcamController sender, List<SpeedOption> args);
        public event SpeedListEventHandler OnSpeedListChanged;

        private SpeedOption m_speedCurrent;
        public SpeedOption Speed
        {
            get { return m_speedCurrent; }
            set
            {
                WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_INDEX, value.Index);

                m_speedCurrent = value;

                GainList = value.GainList;
                Gain = value.GainList[0];

                ReadPostProcessingFeatures();

                OnSpeedChanged(this, value);
                OnPostProcessingFeaturesChanged(this, PostProcessingFeatures);
            }
        }
        public delegate void SpeedEventHandler(PvcamController sender, SpeedOption args);
        public event SpeedEventHandler OnSpeedChanged;
        #endregion

        #region ================================================== GAIN OPTIONS

        private List<GainOption> m_gainListCurrent;
        public List<GainOption> GainList
        {
            get { return m_gainListCurrent; }
            private set
            {
                m_gainListCurrent = value;
                OnGainListChanged(this, value);
            }
        }
        public delegate void GainListEventHandler(PvcamController sender, List<GainOption> args);
        public event GainListEventHandler OnGainListChanged;
        public GainOption Gain
        {
            get { return m_gainCurrent; }
            set
            {
                WriteParamInt16(m_hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, value.Index);

                m_gainCurrent = value;

                OnGainChanged(this, value);
            }
        }
        public delegate void GainEventHandler(PvcamController sender, GainOption args);
        public event GainEventHandler OnGainChanged;
        private GainOption m_gainCurrent;

        #endregion

        /// <summary>
        /// Configures SMART streaming, if supported.
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="expTimes"></param>
        public void SetSmartStreaming(bool enable, List<int> expTimes)
        {
            if (enable)
            {
                //SMART streaming uses a special structure that holds the number of exposures
                //that camera should cycle through and the exposure values
                //create and configure the structure
                PVCAM.smart_stream_type smartStreamVals = new PVCAM.smart_stream_type();

                //Check possible max exposure camera supports
                IntPtr unmngSsStruct = Marshal.AllocHGlobal(Marshal.SizeOf(smartStreamVals));

                //send the SMART streaming structure to camera so we can receive back the same structure
                //with .entries field populated with the maximum number of possible exposures
                if (!PVCAM.pl_get_param(m_hCam, (uint)PVCAM.PL_PARAMS.PARAM_SMART_STREAM_EXP_PARAMS, (short)PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX, unmngSsStruct))
                {
                    Marshal.FreeHGlobal(unmngSsStruct);
                    throw new PvcamException("Failed to read current SMART streaming parameters", PVCAM.pl_error_code());
                }
                else
                {
                    //read the structure from unmanaged environment
                    smartStreamVals = (PVCAM.smart_stream_type)Marshal.PtrToStructure(unmngSsStruct, typeof(PVCAM.smart_stream_type));
                }

                //Limit the exposure time if needed
                smartStreamVals.entries = (short)Math.Min(smartStreamVals.entries, expTimes.Count);

                int[] ssExposures = new int[smartStreamVals.entries];
                //fill exposure list with our exposure times
                for (int i = 0; i < smartStreamVals.entries; i++)
                {
                    ssExposures[i] = expTimes[i];
                }
                IntPtr unmngExposures = Marshal.AllocHGlobal(sizeof(uint) * 4);

                Marshal.Copy(ssExposures, 0, unmngExposures, ssExposures.Length);
                smartStreamVals.parameters = unmngExposures;

                Marshal.StructureToPtr(smartStreamVals, unmngSsStruct, true);

                //send the SMART streaming structure with its values to the camera
                if (!PVCAM.pl_set_param(m_hCam, (uint)PVCAM.PL_PARAMS.PARAM_SMART_STREAM_EXP_PARAMS, unmngSsStruct))
                {
                    Marshal.FreeHGlobal(unmngExposures);
                    Marshal.FreeHGlobal(unmngSsStruct);
                    throw new PvcamException("Failed to set SMART streaming parameters", PVCAM.pl_error_code());
                }

                Marshal.FreeHGlobal(unmngExposures);
                Marshal.FreeHGlobal(unmngSsStruct);

                // Enable SMART streaming in the camera
                WriteParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_SMART_STREAM_MODE_ENABLED, true);

                m_isSmartStreamingActive = true;

                string exposureList = "";
                for (int i = 0; i < smartStreamVals.entries; i++)
                    exposureList += string.Format("{0} ", expTimes[i]);

                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, string.Format("SMART streaming enabled, exposure times set to: {0} ms", exposureList)));
            }
            else
            {
                // Disable SMAR streaming in the camera
                WriteParamBool(m_hCam, PVCAM.PL_PARAMS.PARAM_SMART_STREAM_MODE_ENABLED, false);
                m_isSmartStreamingActive = false;
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, "SMART streaming disabled"));
            }
        }

        /// <summary>
        /// Allocate buffers for the current acquisition.
        /// </summary>
        /// <param name="acqType">Acquisition type</param>
        private void SetupAcquisitionBuffers(AcquisitionType acqType)
        {
            // Release previous pixel data buffer
            if (m_recomposeBufferNative != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_recomposeBufferNative);
                m_recomposeBufferNative = IntPtr.Zero;
            }

            int outputImageSize;
            int expectedRoiCount = RegionList.Length; // Number of output ROIs expected

            // Determine final image size and allocate memory accordingly
            // if metadata is enabled full frame size returned by exposure setup includes metadata + pixel data
            if (m_frameMetadataEnabledCache)
            {
                // If doing multi-roi, use full sensor size because we will be recomposing the ROIs
                // into an image of full sensor (to maintain the ROI coordinates and to show the
                // image as they would 'appear' on the sensor)
                if (RegionList.Length > 1)
                {
                    // We expect the binning to be the same for all ROIs
                    OutputImageSize = new Size(SensorSizeX / m_regions[0].sbin, SensorSizeY / m_regions[0].pbin);
                }
                else
                {
                    // If we are acquiring just one ROI, then we will display this one ROI as an image. No recomposition.
                    OutputImageSize = new Size(
                        (m_regions[0].s2 - m_regions[0].s1 + 1) / m_regions[0].sbin,
                        (m_regions[0].p2 - m_regions[0].p1 + 1) / m_regions[0].pbin);
                }

                int bytesPerPixel = PvcamUtils.GetImageFormatBytesPerPixel(ImageFormat);
                outputImageSize = (OutputImageSize.Width * OutputImageSize.Height * bytesPerPixel);

                if (m_centroidsEnabledCache)
                    expectedRoiCount = m_centroidsCountCache;

                // Allocate the pixel data buffer only if metadata is enabled, without Metadata full frame is pixel data
                m_recomposeBufferNative = Marshal.AllocHGlobal(outputImageSize);
                LatestFrameMetadata = new FrameMetadata(expectedRoiCount);
            }
            else
            {
                // No metadata is enabled, the size returned by pl_exp_setup functions
                // is the final pixel data size, output image size is the same as input ROI.
                outputImageSize = (int)m_setupBufferSize;
                OutputImageSize = new Size(
                    (m_regions[0].s2 - m_regions[0].s1 + 1) / m_regions[0].sbin,
                    (m_regions[0].p2 - m_regions[0].p1 + 1) / m_regions[0].pbin);
                LatestFrameMetadata = null;
            }

            // Release previous frame buffer
            if (m_frameBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_frameBuffer);
                m_frameBuffer = IntPtr.Zero;
            }

            if (acqType == AcquisitionType.Sequence)
            {
                // Even though PVCAM returns an unsigned integer for sequence sizes,
                // we are limited by 'int' range in .net because of the AllocHGlobal
                if (m_setupBufferSize > int.MaxValue)
                    throw new Exception(string.Format("Sequence size of {0} bytes is too large for current platform", m_setupBufferSize));

                // Allocate the sequence buffer
                m_frameBuffSize = (int)(m_setupBufferSize);
                m_frameBuffer = Marshal.AllocHGlobal(m_frameBuffSize);
            }
            else if (acqType == AcquisitionType.Continuous)
            {

                // We could use our own circular buffer size but for now, get the recommended
                // circular buffer size from PVCAM.
                ulong recommendedBufSize = ReadParamUInt64(m_hCam,
                    PVCAM.PL_PARAMS.PARAM_FRAME_BUFFER_SIZE, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_DEFAULT);
                // Trust but verify, the recommended size should be a multiple of one frame size
                if (recommendedBufSize % m_setupBufferSize != 0)
                    throw new Exception(string.Format("Unexpected circular buffer size {0} mod {1} = {2}",
                        recommendedBufSize, m_setupBufferSize, recommendedBufSize % m_setupBufferSize));
                // We are still limited by AllocHGlobal accepting 'int' only
                if (recommendedBufSize > int.MaxValue)
                    recommendedBufSize = (int.MaxValue / m_setupBufferSize) * m_setupBufferSize;
                m_frameBuffSize = (int)(recommendedBufSize);
                // Allocate the circular buffer
                m_frameBuffer = Marshal.AllocHGlobal(m_frameBuffSize);
            }
            else
            {
                throw new Exception("Unsupported acquisition type");
            }

            // Allocate the output pixel buffer for the managed environment. This is where we will
            // marshal the native data.
            m_outputPixelData = new byte[outputImageSize];

            // If Multi-ROI or Centroiding acquisition is active, zero out the values of the output
            // pixel buffer for recomposing the frame. We want the buffer to be 'black filled'.
            if ((RegionList.Length > 1) || (m_centroidsEnabledCache))
            {
                Utils.NativeMemset(m_recomposeBufferNative, 0, outputImageSize);
            }
        }

        //Extract frame data from the MD structure and 
        private void ExtractFrameHeader()
        {
            m_mdFrameHeader = (PVCAM.md_frame_header)Marshal.PtrToStructure(m_mdFrame.header, typeof(PVCAM.md_frame_header));

            // Populate selected metadata
            LatestFrameMetadata.FrameNr = m_mdFrameHeader.frameNr;
            LatestFrameMetadata.RoiCount = m_mdFrameHeader.roiCount;

            if (m_mdFrameHeader.version < 3)
            {
                // Old version
                LatestFrameMetadata.TimeStampBOF = (m_mdFrameHeader.timeStampBOF) * (ulong)(m_mdFrameHeader.timeStampResNs);
                LatestFrameMetadata.TimeStampEOF = (m_mdFrameHeader.timeStampEOF) * (ulong)(m_mdFrameHeader.timeStampResNs);
                LatestFrameMetadata.ExpTime = (m_mdFrameHeader.exposureTime) * (ulong)(m_mdFrameHeader.exposureTimeResNs);
            }
            else
            {
                // New version
                m_mdFrameHeaderV3 = (PVCAM.md_frame_header_v3)Marshal.PtrToStructure(m_mdFrame.header, typeof(PVCAM.md_frame_header_v3));
                LatestFrameMetadata.TimeStampBOF = m_mdFrameHeaderV3.timeStampBOF;
                LatestFrameMetadata.TimeStampEOF = m_mdFrameHeaderV3.timeStampEOF;
                LatestFrameMetadata.ExpTime = m_mdFrameHeaderV3.exposureTime;
            }
        }

        //Extract frame data from the MD structure and 
        private void ExtractROIHeader(IntPtr roiHeaderPtr, ushort roiIndex)
        {
            // Marshal the native header to managed counterpart
            PVCAM.md_frame_roi_header roiHeader = (PVCAM.md_frame_roi_header)Marshal.PtrToStructure(roiHeaderPtr, typeof(PVCAM.md_frame_roi_header));
            // Save some of the most important metadata to our own structure
            LatestFrameMetadata.RoiMetadata[roiIndex].RoiNr = roiHeader.roiNr;
            LatestFrameMetadata.RoiMetadata[roiIndex].S1 = roiHeader.roi.s1;
            LatestFrameMetadata.RoiMetadata[roiIndex].S2 = roiHeader.roi.s2;
            LatestFrameMetadata.RoiMetadata[roiIndex].P1 = roiHeader.roi.p1;
            LatestFrameMetadata.RoiMetadata[roiIndex].P2 = roiHeader.roi.p2;
        }

        /// <summary>
        /// Cache the camera port/speed/gain table.
        /// </summary>
        /// <param name="hCam"></param>
        /// <returns></returns>
        private void ReadPortTable()
        {
            short hCam = m_hCam;
            // Store the current camera settings, we will restore it after we are done iterating. We simply
            // do that to keep the "startup" camera configuration unchanged.
            int readoutPortOld = ReadParamInt32(
                hCam, PVCAM.PL_PARAMS.PARAM_READOUT_PORT, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            short spdTabIndexOld = ReadParamInt16(
                hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
            short gainIndexOld = ReadParamInt16(
                hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

            // Read the list of ports available to this camera
            List<Tuple<int, string>> portEnum = ReadParamEnumList(hCam, PVCAM.PL_PARAMS.PARAM_READOUT_PORT);

            // Prepare a list of Ports to fill in
            List<PortOption> portList = new List<PortOption>();

            // Set each port and find number of readout speeds on that port. Port is an enumerable type,
            // we need to discover the actual port *number* under each *index*.
            foreach (Tuple<int, string> portTuple in portEnum)
            {
                int portNum = portTuple.Item1;

                WriteParamInt32(hCam, PVCAM.PL_PARAMS.PARAM_READOUT_PORT, portNum);

                // Get number of readout *speeds* on this *port*
                short speedCount = (short)ReadParamUInt32(
                    hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_COUNT);

                // Prepare a list of Speeds to fill in
                List<SpeedOption> speedList = new List<SpeedOption>();

                // Iterate over available speed indexes
                for (short spdIdx = 0; spdIdx < speedCount; spdIdx++)
                {
                    // Set speed index
                    WriteParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_INDEX, spdIdx);

                    // Get readout frequency (pixel time) of this speed
                    ushort pixTimeNs = ReadParamUInt16(hCam,
                        PVCAM.PL_PARAMS.PARAM_PIX_TIME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);

                    // Get the Speed name, if supported
                    string spdName = "";
                    if (IsParamAvailable(m_hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_NAME))
                    {
                        spdName = ReadParamString(
                            m_hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_SPDTAB_NAME_LEN);
                    }
                    // Get number of gains available on this speed
                    short gainMin = ReadParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MIN);
                    short gainMax = ReadParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_MAX);
                    short gainInc = ReadParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_INCREMENT);

                    // Prepare a list of gains to fill in
                    List<GainOption> gainList = new List<GainOption>();

                    // Iterate over gain indexes
                    for (short gainIdx = gainMin; gainIdx <= gainMax; gainIdx += gainInc)
                    {
                        // Set the Gain
                        WriteParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, gainIdx);
                        // Get bit depth for this gain
                        short bitDepth = ReadParamInt16(hCam,
                            PVCAM.PL_PARAMS.PARAM_BIT_DEPTH, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT);
                        // Get the gain name, if supported
                        string name = string.Format("Gain {0}", gainIdx);
                        if (IsParamAvailable(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_NAME))
                            name = ReadParamString(hCam,
                                PVCAM.PL_PARAMS.PARAM_GAIN_NAME, PVCAM.PL_PARAM_ATTRIBUTES.ATTR_CURRENT, PVCAM.MAX_GAIN_NAME_LEN);
                        // Add to the list of gains
                        gainList.Add(new GainOption(gainIdx, name, bitDepth));
                    }
                    speedList.Add(new SpeedOption(spdIdx, pixTimeNs, spdName, gainList));
                }
                portList.Add(new PortOption(portNum, portTuple.Item2, speedList));
            }

            // Restore the camera settings
            WriteParamInt32(hCam, PVCAM.PL_PARAMS.PARAM_READOUT_PORT, readoutPortOld);
            WriteParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_SPDTAB_INDEX, spdTabIndexOld);
            WriteParamInt16(hCam, PVCAM.PL_PARAMS.PARAM_GAIN_INDEX, gainIndexOld);

            PortList = portList;
            Port = PortList.Find(p => p.Number == readoutPortOld);
            //m_portCurrent = PortList.Find(p => p.Number == readoutPortOld);
            //m_speedCurrent = m_portCurrent.SpeedList.Find(s => s.Index == spdTabIndexOld);
            //m_gainCurrent = m_speedCurrent.GainList.Find(g => g.Index == gainIndexOld);

            // Print the entire structure
            OnCameraNotification(this, new CameraNotificationEventArgs(
                CameraNotificationType.CameraStatusMessage, "Port Table"));
            foreach (PortOption p in portList)
            {
                OnCameraNotification(this, new CameraNotificationEventArgs(
                    CameraNotificationType.CameraStatusMessage, string.Format("- Port {0}: '{1}'", p.Number, p.Name)));
                foreach (SpeedOption s in p.SpeedList)
                {
                    OnCameraNotification(this, new CameraNotificationEventArgs(
                        CameraNotificationType.CameraStatusMessage, string.Format("-- Speed {0}: {1:00}MHz", s.Index, s.RateMHz)));
                    foreach (GainOption g in s.GainList)
                    {
                        OnCameraNotification(this, new CameraNotificationEventArgs(
                            CameraNotificationType.CameraStatusMessage, string.Format("--- Gain {0}: '{1}' ({2}-bit)", g.Index, g.Name, g.BitDepth)));
                    }
                }
            }
        }

        /// <summary>
        /// PVCAM EOF callback. Called by PVCAM when a new frame is available in the buffer.
        /// Please note that recent PVCAM versions are queueing the callbacks in a queue that
        /// is as long as is the length of the circular/sequence buffer. Calling pl_exp_get_latest_frame
        /// inside the callback is recommended for best results. If the application momentarily holds
        /// the callback for longer than frame period, next callback will be called immediately as soon
        /// as the application exits the callback. This way, latest frames are "queued" up to the size
        /// of the PVCAM buffer. However, if the application cannot generally process frames fast enough,
        /// the PVCAM queue will eventually overflow and frames will start to be dropped from the front of
        /// the queue.
        /// </summary>
        /// <param name="pFrameInfo">A reference to the FRAME_INFO structure</param>
        /// <param name="context">Optional user-defined context</param>
        private void PvcamCallbackEof(ref PVCAM.FRAME_INFO pFrameInfo, IntPtr context)
        {
            bool abort = false;
            lock (m_acqLock)
            {
                // Ignore any incoming callbacks if we are not acquiring, this may happen when
                // aborting continous acquisition. Before the abort command is sent to the camera,
                // thre might still be few callbacks queued on the PVCAM side.
                if (!IsAcquiring)
                    return;

                m_fpsCounter++;
                m_fpsDuration = (int)(m_fpsStopwatch.ElapsedTicks / (TimeSpan.TicksPerMillisecond / 1000));
                if (m_fpsDuration >= 1000000)
                {
                    FrameRate = (m_fpsCounter / (m_fpsDuration / 1000000.0));
                    m_fpsCounter = 0;
                    m_fpsDuration = 0;
                    m_fpsStopwatch.Restart();
                }

                CallbackContext acqParams = (CallbackContext)Marshal.PtrToStructure(context, typeof(CallbackContext));

                // Use the pl_exp_get_latest_frame_ex() function to get the address of the latest frame in the
                // circular buffer and additional information pertaining to this image in m_frameInfoLatest, see
                // PVCAM.FRAME_INFO type for further details.
                if (!PVCAM.pl_exp_get_latest_frame_ex(m_hCam, out IntPtr latestFramePtr, out m_frameInfoLatest))
                {
                    // We cannot throw here, it's a PVCAM callback.
                    PvcamException exc = new PvcamException("Failed to obtain latest frame", PVCAM.pl_error_code());
                    OnCameraNotification(this, new CameraNotificationEventArgs(
                        CameraNotificationType.CameraErrorMessage, exc.Message));
                    return;
                }
                else
                {
                    if (m_frameMetadataEnabledCache)
                    {
                        Marshal.StructureToPtr(m_mdFrame, m_mdFramePtr, false);
                        if (!PVCAM.pl_md_frame_decode(m_mdFramePtr, latestFramePtr, m_setupBufferSize))
                        {
                            PvcamException exc = new PvcamException("Failed to decode frame", PVCAM.pl_error_code());
                            OnCameraNotification(this, new CameraNotificationEventArgs(
                                CameraNotificationType.CameraErrorMessage, exc.Message));
                            return;
                        }
                        else
                        {
                            m_mdFrame = (PVCAM.md_frame)Marshal.PtrToStructure(m_mdFramePtr, typeof(PVCAM.md_frame));

                            // Extract Frame metadata
                            ExtractFrameHeader();

                            // How many ROIs are present in the Frame? There might be 0 actually in some cases
                            // like Centroiding or Particle Tracking if the camera cannot find any signal. The
                            // number may also change per frame, like when Centroiding is enabled.
                            ushort roiCount = m_mdFrame.roiCount;

                            for (ushort i = 0; i < roiCount; i++)
                            {
                                // Calculate pointer to every ROI descriptor in the Frame descriptor
                                IntPtr ptr = (IntPtr)(m_mdFrame.roiArray).ToInt64() + m_mdFrameRoiStructSize * i;
                                // Marshal the ROI descriptor into oure preallocated managed structure
                                m_mdFrameRoiStructs[i] = (PVCAM.md_frame_roi)Marshal.PtrToStructure(ptr, typeof(PVCAM.md_frame_roi));
                                // Frame Roi Header can be extracted from above if required.
                                ExtractROIHeader(m_mdFrameRoiStructs[i].header, i);
                            }

                            // For single-ROI acquisition, copy the only ROI data to the final managed array
                            bool isMultiROI = (RegionList.Length > 1) || (m_centroidsEnabledCache);
                            if (!isMultiROI)
                            {
                                if (m_paralelThreads > 1 && m_mdFrameRoiStructs[0].dataSize >= m_parallelMinBufSize)
                                    Utils.ParallelMarshalCopy(m_mdFrameRoiStructs[0].data, m_outputPixelData, 0, (int)m_mdFrameRoiStructs[0].dataSize, m_paralelThreads);
                                else
                                    Marshal.Copy(m_mdFrameRoiStructs[0].data, m_outputPixelData, 0, (int)m_mdFrameRoiStructs[0].dataSize);
                            }
                            else
                            {
                                // For multiple-ROI acquisition, recompose the multiple-ROIs into a black-filled image

                                // If centroiding is enabled, black-fill the buffer before *every* recompose because the
                                // centroids regions are moving in the frame and we need to make sure no residual data
                                // are left from previous recompositions.
                                // This is not needed for general, user-defined multi-ROIs because the ROIs are not
                                // moving their positions in between the frames. We black-fill only once in that case.
                                if (m_centroidsEnabledCache)
                                    Utils.NativeMemset(m_recomposeBufferNative, 0, m_outputPixelData.Length);

                                if (!PVCAM.pl_md_frame_recompose(m_recomposeBufferNative, m_mdFrame.impliedRoi.s1, m_mdFrame.impliedRoi.p1,
                                    (ushort)OutputImageSize.Width, (ushort)OutputImageSize.Height, ref m_mdFrame))
                                {
                                    PvcamException exc = new PvcamException("Failed to recompose frame", PVCAM.pl_error_code());
                                    OnCameraNotification(this, new CameraNotificationEventArgs(
                                        CameraNotificationType.CameraErrorMessage, exc.Message));
                                    IsAcquiring = false;
                                    return;
                                }
                                // Copy recomposed frame to managed array
                                if (m_paralelThreads > 1 && m_outputPixelData.Length >= m_parallelMinBufSize)
                                    Utils.ParallelMarshalCopy(m_recomposeBufferNative, m_outputPixelData, 0, m_outputPixelData.Length, m_paralelThreads);
                                else
                                    Marshal.Copy(m_recomposeBufferNative, m_outputPixelData, 0, m_outputPixelData.Length);
                            }
                        }
                    }
                    else
                    {
                        // No metadata is enabled, the buffer is a simple pixel array.
                        if (m_paralelThreads > 1 && m_outputPixelData.Length > m_parallelMinBufSize)
                            Utils.ParallelMarshalCopy(latestFramePtr, m_outputPixelData, 0, m_outputPixelData.Length, m_paralelThreads);
                        else
                            Marshal.Copy(latestFramePtr, m_outputPixelData, 0, m_outputPixelData.Length);
                    }

                    // For now use the FrameNr as an indicator of how many frames the camera acquired.
                    // This number may not necessarily represent the number of callbacks the camera sent
                    // because if we are slow with processing frames, the PVCAM will start skipping callbacks.
                    FramesAcquired = m_frameInfoLatest.FrameNr;
                    FrameSequenceNumber++;

                    // Currently, we do not use these per-frame events. With cameras reaching 10000+ FPS it is better
                    // if the GUI (user of this class) polls this class for the "Latest" frame and displays only
                    // the latest frame every 30-40 msecs or so. If streaming to RAM or Disk is needed, it should be done
                    // right here in the callback method (i.e. copy the incoming frame to a large RAM buffer or
                    // send them do disk directly right here, and notify GUI only about the progress).
                    OnCameraNotification(this, new CameraNotificationEventArgs(CameraNotificationType.FrameAcquired));

                    // If the acquisition has acquired all the requested frames, exit
                    if (FramesAcquired >= FramesToAcquire && FramesToAcquire != RUN_UNTIL_STOPPED)
                    {
                        IsAcquiring = false;
                        abort = true;
                    }
                }
                if (abort)
                {
                    m_commandThreadCommand = Command.Abort;
                    Monitor.Pulse(m_acqLock);
                }
            }
        }

        public bool AllowParallelProcessing
        {
            get { return m_paralelThreads > 0; }
            set
            {
                if (value)
                    m_paralelThreads = Math.Min(Environment.ProcessorCount, 8);
                else
                    m_paralelThreads = 0;
            }
        }

        // Private fields

        /// <summary>
        /// Camera native handle, uniquely identifies opened camera in PVCAM.
        /// </summary>
        short m_hCam;
        /// <summary>
        /// Buffer size as reported by pl_exp_setup functions. For sequence acquistions, this value
        /// will be reported as the entier buffer size to fit all the frames in the sequence. For
        /// continous acquisition, this value corresponds to one frame in the circular buffer.
        /// </summary>
        uint m_setupBufferSize = 0;
        /// <summary>
        /// For recomposing, we need another native buffer to recompose into.
        /// </summary>
        IntPtr m_recomposeBufferNative = IntPtr.Zero;
        /// <summary>
        /// PVCAM's native frame buffer, used for both sequences and circular buffer acquisition.
        /// </summary>
        IntPtr m_frameBuffer = IntPtr.Zero;
        /// <summary>
        /// PVCAM's native frame buffer size.
        /// </summary>
        int m_frameBuffSize = 0;

        /// <summary>
        /// Final output image pixel data.
        /// Output buffer size may be 1:1 to the PVCAM's setup buffer size but if we are recomposing
        /// multi-ROI frames into black-filled image, the output buffer size will correspond to full frame size,
        /// as we will be recomposing ROIs into the 'sensor' area.
        /// </summary>
        byte[] m_outputPixelData = null;

        //ROI 
        PVCAM.rgn_type[] m_regions;       //Array of region tyoes

        bool m_isFrameTransferCapable = false;

        PVCAM.PL_EXPOSURE_MODES m_triggerMode = PVCAM.PL_EXPOSURE_MODES.TIMED_MODE;
        PVCAM.PL_EXPOSE_OUT_MODES m_exposeOutMode = 0;

        PVCAM.PMCallBack_Ex3 m_eofCallbackDelegate;
        CallbackContext m_eofCallbackContext;
        GCHandle m_eofCallbackContextHandle;

        PVCAM.FRAME_INFO m_frameInfoLatest;

        bool m_isSmartStreamingActive = false;

        PVCAM.md_frame m_mdFrame; // Managed md_frame
        IntPtr m_mdFramePtr = IntPtr.Zero;  // A pointer to md_frame, required for pl_md_frame_decode
        PVCAM.md_frame_header m_mdFrameHeader;   // Embedded metadata header V1-V2
        PVCAM.md_frame_header_v3 m_mdFrameHeaderV3; // Embedded metadata header V3+

        PVCAM.md_frame_roi[] m_mdFrameRoiStructs = null;
        int m_mdFrameRoiStructSize = 0;

        Stopwatch m_fpsStopwatch = new Stopwatch();
        int m_fpsCounter = 0;
        int m_fpsDuration = 0;

        int m_paralelThreads = 0;

        /// <summary>
        /// 并行最小缓存区大小
        /// </summary>
        const int m_parallelMinBufSize = 256 * 1024;

        /// <summary>
        /// Command thread allows us to schedule a PVCAM call when needed, for now
        /// we use it to abort acquisition from outside of the PVCAM callback method.
        /// </summary>
        Thread m_commandThread;
        enum Command
        {
            None,
            Exit,
            Abort
        }
        Command m_commandThreadCommand = Command.None;

        object m_acqLock = new object();

    }
}
