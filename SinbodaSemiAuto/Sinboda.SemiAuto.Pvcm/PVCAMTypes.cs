using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    public static partial class PVCAM
    {
        // C# note: This file contains c# definitions for native C types from master.h and pvcam.h

        #region master.h defines

        [Obsolete("Obsolete, use PMCallBack_Ex3.")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PMCallBack();

        [Obsolete("Obsolete, use PMCallBack_Ex3.")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PMCallBack_Ex(IntPtr context);

        [Obsolete("Obsolete, use PMCallBack_Ex3.")]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PMCallBack_Ex2(ref FRAME_INFO frameInfo);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PMCallBack_Ex3(ref FRAME_INFO frameInfo, IntPtr context);

        public enum ReturnValue
        {
            PV_FAIL = 0,
            PV_OK
        }

        public enum BoolValue : ushort
        {
            FALSE = 0,
            TRUE
        }
        #endregion

        /******************************************************************************/
        /* Constants                                                                  */
        /******************************************************************************/

        /** Maximum number of cameras on this system. */
        public const int MAX_CAM = 16;

        /******************************************************************************/
        /* Name/ID sizes                                                              */
        /******************************************************************************/

        /** Maximum length of a camera name including space for null terminator. */
        /** @see #pl_cam_get_name for details. */
        public const int CAM_NAME_LEN = 32;
        /** @deprecated Use #MAX_PP_NAME_LEN instead. */
        [Obsolete("Obsolete, MAX_PP_NAME_LEN")]
        public const int PARAM_NAME_LEN = 32;
        /** Maximum length of an error message including space for null terminator. */
        /** @see #pl_error_message for details. */
        public const int ERROR_MSG_LEN = 255;
        /** Maximum length of a sensor chip name including space for null terminator. */
        /** @see #PARAM_CHIP_NAME for details. */
        public const int CCD_NAME_LEN = 17;
        /** Maximum length of a camera serial number string including space for null
            terminator. */
        /** @see #PARAM_HEAD_SER_NUM_ALPHA for details. */
        public const int MAX_ALPHA_SER_NUM_LEN = 32;
        /** Maximum length of a post-processing parameter/feature name including space
            for null terminator. */
        /** @see #PARAM_PP_FEAT_NAME and #PARAM_PP_PARAM_NAME for details. */
        public const int MAX_PP_NAME_LEN = 32;
        /** Maximum length of a system name including space for null terminator. */
        /** @see #PARAM_SYSTEM_NAME for details. */
        public const int MAX_SYSTEM_NAME_LEN = 32;
        /** Maximum length of a vendor name. including space for null terminator */
        /** @see #PARAM_VENDOR_NAME for details. */
        public const int MAX_VENDOR_NAME_LEN = 32;
        /** Maximum length of a product name including space for null terminator. */
        /** @see #PARAM_PRODUCT_NAME for details. */
        public const int MAX_PRODUCT_NAME_LEN = 32;
        /** Maximum length of a camera part number including space for null terminator. */
        /** @see #PARAM_CAMERA_PART_NUMBER for details. */
        public const int MAX_CAM_PART_NUM_LEN = 32;
        /** Maximum length of a gain name including space for null terminator. */
        /** @see #PARAM_GAIN_NAME for details. */
        public const int MAX_GAIN_NAME_LEN = 32;
        /** Maximum length of a speed name including space for null terminator. */
        /** @see #PARAM_SPDTAB_NAME for details. */
        public const int MAX_SPDTAB_NAME_LEN = 32;
        /** Maximum length of a gain name including space for null terminator. */
        /** @see #PARAM_CAM_SYSTEMS_INFO for details. */
        public const int MAX_CAM_SYSTEMS_INFO_LEN = 1024;

        /******************************************************************************/
        /* Data types                                                                 */
        /******************************************************************************/

        /**
        GUID for #FRAME_INFO structure.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct PVCAM_FRAME_INFO_GUID
        {
            [MarshalAs(UnmanagedType.U4)]
            public uint f1;
            [MarshalAs(UnmanagedType.U2)]
            public ushort f2;
            [MarshalAs(UnmanagedType.U2)]
            public ushort f3;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] f4;
        }

        /**
        Structure used to uniquely identify frames in the camera.

        Please note that this information is generated by the low level device driver.
        While the information is accurate for slower acquisitions and most legacy CCD cameras,
        the timestamps are still obtained from the operating systems. For that reason the
        timestamps may not always represent the time of actual, in-camera acquisition.
        This is especially true for fast CMOS cameras that implement an internal frame buffer.
        Such cameras often report both the BOF and EOF timestamps as identical numbers with the
        readout time reported as 0.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct FRAME_INFO
        {
            PVCAM_FRAME_INFO_GUID FrameInfoGUID;
            [MarshalAs(UnmanagedType.I2)]
            public short hCam;
            [MarshalAs(UnmanagedType.I4)]
            public int FrameNr;
            [MarshalAs(UnmanagedType.I8)]
            public long TimeStamp;
            [MarshalAs(UnmanagedType.I4)]
            public int ReadoutTime;
            [MarshalAs(UnmanagedType.I8)]
            public long TimeStampBOF;
        }

        /**
        The modes under which the camera can be open.
        Used with the function #pl_cam_open.
        Treated as @c #int16 type.
        */
        public enum PL_OPEN_MODES
        {
            /** Default camera open mode. The camera is opened exclusively 
            for the calling process. */
            OPEN_EXCLUSIVE
        }

        /**
        Used with the #PARAM_COOLING_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_COOL_MODES
        {
            /**
            This is a thermo-electrically (TE)-cooled camera with air or liquid assisted
            cooling.
            */
            NORMAL_COOL,
            /**
            The camera is cryogenically cooled.
            */
            CRYO_COOL,
            /**
            The camera has no cooling or the cooling is optional and should be provided
            by the end user.
            */
            NO_COOL
        }

        /**
        Used with the #PARAM_MPP_CAPABLE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_MPP_MODES
        {
            MPP_UNKNOWN,
            MPP_ALWAYS_OFF,
            MPP_ALWAYS_ON,
            MPP_SELECTABLE
        }

        /**
        Used with the #PARAM_SHTR_STATUS parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_SHTR_MODES
        {
            SHTR_FAULT,
            SHTR_OPENING,
            SHTR_OPEN,
            SHTR_CLOSING,
            SHTR_CLOSED,
            SHTR_UNKNOWN
        }

        /**
        Used with the #PARAM_PMODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_PMODES
        {
            PMODE_NORMAL,
            PMODE_FT,
            PMODE_MPP,
            PMODE_FT_MPP,
            PMODE_ALT_NORMAL,
            PMODE_ALT_FT,
            PMODE_ALT_MPP,
            PMODE_ALT_FT_MPP
        }

        /**
        Used with the #PARAM_COLOR_MODE parameter ID.
        Treated as @c #int32 type (but should not exceed a value of 255 due to
        #md_frame_header.colorMask)
        */
        public enum PL_COLOR_MODES
        {
            COLOR_NONE = 0, /**< Monochrome camera, no color mask. */
            COLOR_RESERVED = 1, /**< Reserved, do not use. */
            COLOR_RGGB = 2, /**< Color camera with RGGB color mask. */
            COLOR_GRBG,         /**< Color camera with GRBG color mask. */
            COLOR_GBRG,         /**< Color camera with GBRG color mask. */
            COLOR_BGGR          /**< Color camera with BGGR color mask. */
        }

        /**
        Image format specifies the buffer format in which the pixels are
        transferred. The format should be used together with #PARAM_BIT_DEPTH
        because it specifies only the format of the pixel container, not the
        actual bit depth of the pixel it contains.
        Used with the #PARAM_IMAGE_FORMAT and #PARAM_IMAGE_FORMAT_HOST parameter IDs.
        Treated as @c #int32 type (but should not exceed a value of 255 due to
        #md_frame_header.imageFormat field).
        */
        public enum PL_IMAGE_FORMATS
        {
            PL_IMAGE_FORMAT_MONO16 = 0, /**< 16bit mono, 2 bytes per pixel. */
            PL_IMAGE_FORMAT_BAYER16,    /**< 16bit bayer masked image, 2 bytes per pixel. See also #PL_COLOR_MODES. */
            PL_IMAGE_FORMAT_MONO8,      /**< 8bit mono, 1 byte per pixel. */
            PL_IMAGE_FORMAT_BAYER8,     /**< 8bit bayer masked image, 1 byte per pixel. See also #PL_COLOR_MODES. */
            PL_IMAGE_FORMAT_MONO24,     /**< 24bit mono, 3 bytes per pixel. */
            PL_IMAGE_FORMAT_BAYER24,    /**< 24bit bayer masked image, 3 bytes per pixel. See also #PL_COLOR_MODES. */
            PL_IMAGE_FORMAT_RGB24,      /**< 8bit RGB, 1 byte per sample, 3 bytes per pixel. */
            PL_IMAGE_FORMAT_RGB48,      /**< 16bit RGB, 2 bytes per sample, 6 bytes per pixel. */
            PL_IMAGE_FORMAT_RGB72,      /**< 24bit RGB, 3 bytes per sample, 9 bytes per pixel. */
            PL_IMAGE_FORMAT_MONO32,     /**< 32bit mono, 4 bytes per pixel. */
            PL_IMAGE_FORMAT_BAYER32,    /**< 32bit bayer masked image, 4 bytes per pixel. See also #PL_COLOR_MODES. */
            PL_IMAGE_FORMAT_RGB96       /**< 32bit RGB, 4 bytes per sample, 12 bytes per pixel. */
        }

        /**
        Image compression used to reduce the size of the pixel data. Once the
        image is decompressed, the pixels can be accessed according to #PL_IMAGE_FORMATS type.
        Used with the #PARAM_IMAGE_COMPRESSION parameter ID.
        Treated as @c #int32 type (but should not exceed a value of 255 due to
        #md_frame_header.imageCompression field).
        */
        public enum PL_IMAGE_COMPRESSIONS
        {
            PL_IMAGE_COMPRESSION_NONE = 0,     /**< No compression, the pixels can be accessed according to #PL_IMAGE_FORMATS */
            /* Bit-packing compression modes */
            PL_IMAGE_COMPRESSION_RESERVED8 = 8,
            PL_IMAGE_COMPRESSION_BITPACK9,        /**<  9-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK10,       /**< 10-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK11,       /**< 11-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK12,       /**< 12-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK13,       /**< 13-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK14,       /**< 14-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_BITPACK15,       /**< 15-bit packing in 16bit format */
            PL_IMAGE_COMPRESSION_RESERVED16 = 16,
            PL_IMAGE_COMPRESSION_BITPACK17,       /**< 17-bit packing in 24bit format */
            PL_IMAGE_COMPRESSION_BITPACK18,       /**< 18-bit packing in 24bit format */
            PL_IMAGE_COMPRESSION_RESERVED24 = 24,
            PL_IMAGE_COMPRESSION_RESERVED32 = 32,
            /* Other compression modes below */
        }

        /**
        Frame rotation modes. Used with the #PARAM_HOST_FRAME_ROTATE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_FRAME_ROTATE_MODES
        {
            PL_FRAME_ROTATE_MODE_NONE = 0, /**< No rotation */
            PL_FRAME_ROTATE_MODE_90CW,     /**< 90 degrees clockwise */
            PL_FRAME_ROTATE_MODE_180CW,    /**< 180 degrees clockwise */
            PL_FRAME_ROTATE_MODE_270CW     /**< 270 degrees clockwise */
        }

        /**
        Frame flip modes. Used with the #PARAM_HOST_FRAME_FLIP parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_FRAME_FLIP_MODES
        {
            PL_FRAME_FLIP_MODE_NONE = 0, /**< No flip */
            PL_FRAME_FLIP_MODE_X,        /**< Horizontal flip, mirroring along x-axis */
            PL_FRAME_FLIP_MODE_Y,        /**< Vertical flip, mirroring along y-axis */
            PL_FRAME_FLIP_MODE_XY        /**< Horizontal and vertical flip */
        }

        /**
        Frame summing formats. Used with the #PARAM_HOST_FRAME_SUMMING_FORMAT parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_FRAME_SUMMING_FORMATS
        {
            PL_FRAME_SUMMING_FORMAT_16_BIT = 0,
            PL_FRAME_SUMMING_FORMAT_24_BIT,
            PL_FRAME_SUMMING_FORMAT_32_BIT
        }

        /**
        Used with the function #pl_get_param.
        Treated as @c #int16 type.
        */
        public enum PL_PARAM_ATTRIBUTES
        {
            /**
            Current value.
            For the enumerated type the value returned here is the value
            assigned to current enum item, not the item index.
            The data type for this attribute is defined by #ATTR_TYPE.
            */
            ATTR_CURRENT,
            /**
            Number of possible values for enumerated and array data types.
            If the data type returned by #ATTR_TYPE is #TYPE_CHAR_PTR (and not an
            enumerated type), then the #ATTR_COUNT is the number of characters in the
            string including a space for NULL terminator. If 0 or 1 is returned,
            #ATTR_COUNT is a scalar (single element) of the following data types:
            #TYPE_INT8, #TYPE_UNS8, #TYPE_INT16, #TYPE_UNS16, #TYPE_INT32, #TYPE_UNS32,
            #TYPE_INT64, #TYPE_UNS64, #TYPE_FLT64 and #TYPE_BOOLEAN.
            The data type for this attribute is #TYPE_UNS32.
            */
            ATTR_COUNT,
            /**
            Data type of parameter.
            Data types used by #pl_get_param with attribute type (#ATTR_TYPE) are:

            |            Value           |       Type         |
            |----------------------------|--------------------|
            | TYPE_BOOLEAN               | rs_bool            |
            | TYPE_INT8                  | int8               |
            | TYPE_UNS8                  | uns8               |
            | TYPE_INT16                 | int16              |
            | TYPE_UNS16                 | uns16              |
            | TYPE_INT32                 | int32              |
            | TYPE_UNS32                 | uns32              |
            | TYPE_INT64                 | long64             |
            | TYPE_UNS64                 | ulong64            |
            | TYPE_FLT32                 | flt32              |
            | TYPE_FLT64                 | flt64              |
            | TYPE_ENUM                  | int32              |
            | TYPE_CHAR_PTR              | char*              |
            | TYPE_VOID_PTR              | void*              |
            | TYPE_VOID_PTR_PTR          | void**             |
            | TYPE_SMART_STREAM_TYPE     | smart_stream_type  |
            | TYPE_SMART_STREAM_TYPE_PTR | smart_stream_type* |
            | TYPE_RGN_TYPE              | rgn_type           |
            | TYPE_RGN_TYPE_PTR          | rgn_type*          |
            | TYPE_RGN_LIST_TYPE         | reserved           |
            | TYPE_RGN_LIST_TYPE_PTR     | reserved           |

            The data type for this attribute is #TYPE_UNS16.
            */
            ATTR_TYPE,
            /**
            Minimum value. The value is only valid for the following data types:
            #TYPE_INT8, #TYPE_UNS8, #TYPE_INT16, #TYPE_UNS16, #TYPE_INT32, #TYPE_UNS32,
            #TYPE_INT64, #TYPE_UNS64, #TYPE_FLT64 and #TYPE_BOOLEAN.
            The data type for this attribute is defined by #ATTR_TYPE.
            */
            ATTR_MIN,
            /**
            Maximum value. The value is only valid for the following data types:
            #TYPE_INT8, #TYPE_UNS8, #TYPE_INT16, #TYPE_UNS16, #TYPE_INT32, #TYPE_UNS32,
            #TYPE_INT64, #TYPE_UNS64, #TYPE_FLT64 and #TYPE_BOOLEAN.
            The data type for this attribute is defined by #ATTR_TYPE.
            */
            ATTR_MAX,
            /**
            Default value. This value should be equal to the current value loaded
            by camera on power up. For the enumerated type, the value returned here
            is the value assigned to the current enum item, not the item index.
            The data type for this attribute is defined by #ATTR_TYPE.
            */
            ATTR_DEFAULT,
            /**
            Step size for values (zero if non-linear or without increment).
            The value is only valid for the following data types:
            #TYPE_INT8, #TYPE_UNS8, #TYPE_INT16, #TYPE_UNS16, #TYPE_INT32, #TYPE_UNS32,
            #TYPE_INT64, #TYPE_UNS64 and #TYPE_FLT64. The value for this attribute
            is never negative. If the value is non-zero, valid values can be easily
            calculated. The first valid value is the value reported for the attribute #ATTR_MIN,
            the second value is the minimum value plus increment (#ATTR_INCREMENT),
            and so on up to the maximum value (#ATTR_MAX).
            The data type for this attribute is defined by #ATTR_TYPE.
            */
            ATTR_INCREMENT,
            /**
            Reports if the parameter with ID param_id can be written to and/or read from or
            in case it cannot be written to and/or read, it tells whether a feature exists.
            If the param_id can be either written to or read from, the next step is to determine
            its data type.
            The access types are enumerated:
            #ACC_EXIST_CHECK_ONLY #ACC_READ_ONLY
            #ACC_WRITE_ONLY #ACC_READ_WRITE
            The data type for this attribute is #TYPE_UNS16.
            */
            ATTR_ACCESS,
            /**
            Feature available with attached hardware and software.
            The data type for this attribute is #TYPE_BOOLEAN.
            */
            ATTR_AVAIL,
            /**
            Reports if the parameter can be accessed during active acquisition.
            The data type for this attribute is #TYPE_BOOLEAN.
            */
            ATTR_LIVE
        }

        /**
        Used with the function #pl_get_param and #ATTR_ACCESS.
        Treated as @c #uns16 type.
        */
        public enum PL_PARAM_ACCESS
        {
            ACC_ERROR = 0,
            /** Parameter is read only, #pl_set_param for such parameter will fail. */
            ACC_READ_ONLY = 1,
            /** Parameter can be read and written. */
            ACC_READ_WRITE,
            /** Only parameter availability can be checked. */
            ACC_EXIST_CHECK_ONLY,
            /** Parameter can only be written. */
            ACC_WRITE_ONLY
        }

        /**
        Used with the #PARAM_IO_TYPE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_IO_TYPE
        {
            IO_TYPE_TTL, /**< The bit pattern written to this address.*/
            IO_TYPE_DAC /**< The value of the desired analog output written to the DAC on this address.*/
        }

        /**
        Used with the #PARAM_IO_DIRECTION parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_IO_DIRECTION
        {
            IO_DIR_INPUT,           /**< The port configured as input. */
            IO_DIR_OUTPUT,          /**< The port configured as output. */
            IO_DIR_INPUT_OUTPUT     /**< The port configured as bi-directional. */
        }

        /**
        Used with the #PARAM_READOUT_PORT parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_READOUT_PORTS
        {
            READOUT_PORT_0 = 0,
            READOUT_PORT_1,
            READOUT_PORT_2,
            READOUT_PORT_3
        }

        /**
        Used with the #PARAM_CLEAR_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_CLEAR_MODES
        {
            /**
            Don't ever clear the sensor. Useful for performing a readout after an
            exposure has been aborted.
            */
            CLEAR_NEVER,
            /**
            Clear the sensor automatically. Modern cameras control the sensor clearing
            as needed. Some cameras will report only this clearing mode. For backward
            compatibility with existing applications, the 'auto' clearing mode is reported
            under the same value as the 'clear never' mode.
            */
            CLEAR_AUTO = CLEAR_NEVER,
            /**
            Before each exposure, clears the sensor the number of times specified by the
            @c clear_cycles variable. This mode can be used in a sequence. It is most
            useful when there is a considerable amount of time between exposures.
            */
            CLEAR_PRE_EXPOSURE,
            /**
            Before each sequence, clears the sensor the number of times specified by the
            @c clear_cycles variable. If no sequence is set up, this mode behaves as if
            the sequence had one exposure. The result is the same as when using
            #CLEAR_PRE_EXPOSURE.
            */
            CLEAR_PRE_SEQUENCE,
            /**
            Clears continuously after the sequence ends. The camera continues clearing
            until a new exposure is set up or started, the abort command is sent, the
            speed entry number is changed, or the camera is reset.
            */
            CLEAR_POST_SEQUENCE,
            /**
            Clears @c clear_cycles times before each sequence and clears continuously
            after the sequence ends. The camera continues clearing until a new exposure
            is set up or started, the abort command is sent, the speed entry number is
            changed, or the camera is reset.
            */
            CLEAR_PRE_POST_SEQUENCE,
            /**
            Clears @c clear_cycles times before each exposure and clears continuously
            after the sequence ends. The camera continues clearing until a new exposure
            is set up or started, the abort command is sent, the speed entry number is
            changed, or the camera is reset.
            */
            CLEAR_PRE_EXPOSURE_POST_SEQ,

            /* Should be the last and never used value. */
            MAX_CLEAR_MODE
        }

        /**
        Used with the #PARAM_SHTR_OPEN_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_SHTR_OPEN_MODES
        {
            /**
            The shutter closes before the exposure and stays closed during the exposure.
            */
            OPEN_NEVER,
            /**
            Opens the shutter with each exposure. Normal mode.
            */
            OPEN_PRE_EXPOSURE,
            /**
            Opens the shutter at the start of each sequence. Useful for frame transfer
            and external strobe devices.
            */
            OPEN_PRE_SEQUENCE,
            /**
            If using a triggered mode, this function causes the shutter to open before
            the external trigger is armed. If using a non-triggered mode, this function
            operates identically to #OPEN_PRE_EXPOSURE.
            */
            OPEN_PRE_TRIGGER,
            /**
            Sends no signals to open or close the shutter. Useful for frame transfer
            when you want to open the shutter and leave it open (see #pl_exp_abort).
            */
            OPEN_NO_CHANGE
        }

        /**
        Used with the #PARAM_EXPOSURE_MODE parameter ID.
        Treated as @c #int32 type.

        Used with the functions #pl_exp_setup_cont and #pl_exp_setup_seq.
        Treated as @c #int16 type.
        */
        public enum PL_EXPOSURE_MODES
        {
            /**
            Causes the exposure to use the internal timer to control
            the exposure duration.
            */
            TIMED_MODE,
            /**
            Accepts the external trigger as exposure start signal.
            The exposure duration is still controlled by the internal timer.
            */
            STROBED_MODE,
            /**
            Accepts the external trigger as exposure start and duration control 
            signal.
            */
            BULB_MODE,
            /**
            Accepts the external trigger as acquisition sequence start signal.
            Each exposure in the sequence then uses the internal timer to control the
            exposure duration. The software will look for one trigger only and then
            proceed with the sequence.
            */
            TRIGGER_FIRST_MODE,
            /** @warning Deprecated. Not supported by any modern camera. */
            [Obsolete("Obsolete, not supported by any modern camera.")]
            FLASH_MODE,
            /**
            The duration of the exposure can be controlled by software dynamically
            without requiring the acquisition to be reconfigured with #pl_exp_setup_seq.
            Use #pl_set_param with #PARAM_EXP_TIME to set new exposure time before calling
            #pl_exp_start_seq.
            */
            VARIABLE_TIMED_MODE,
            /** @warning Deprecated. Not supported by any modern camera. */
            [Obsolete("Obsolete, not supported by any modern camera.")]
            INT_STROBE_MODE,
            MAX_EXPOSE_MODE = 7,

            /*
            Extended EXPOSURE modes used with #PARAM_EXPOSURE_MODE when
            camera dynamically reports its capabilities.
            The "7" in each of these calculations comes from the previous
            definition of #MAX_EXPOSE_MODE when this file was defined.
            */

            /**
            Internal camera trigger, camera controls the start and the duration
            of the exposure. This mode is similar to the legacy #TIMED_MODE.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_INTERNAL = (7 + 0) << 8,
            /**
            Trigger controls the start of first exposure. Subsequent exposures are
            controlled by the camera internal timer. This mode is similar to the legacy
            #TRIGGER_FIRST_MODE.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_TRIG_FIRST = (7 + 1) << 8,
            /**
            Trigger controls the start of each exposure. This mode is similar to
            the legacy #STROBED_MODE.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_EDGE_RISING = (7 + 2) << 8,
            /**
            Trigger controls the start and duration of each exposure. This mode is similar to
            the legacy #BULB_MODE.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_LEVEL = (7 + 3) << 8,
            /**
            Exposure is triggered with software trigger using the #pl_exp_trigger call.
            Similarly to #EXT_TRIG_TRIG_FIRST, the trigger starts the first exposure only.
            Subsequent exposures are controlled by the camera internal timer.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_SOFTWARE_FIRST = (7 + 4) << 8,
            /**
            Exposure is triggered with software trigger using the #pl_exp_trigger call.
            Similarly to #EXT_TRIG_EDGE_RISING, each call to #pl_exp_trigger triggers one exposure.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_SOFTWARE_EDGE = (7 + 5) << 8,
            /**
            Trigger controls the start and duration of each exposure. This mode allows
            overlapping exposure and readout.
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_LEVEL_OVERLAP = (7 + 6) << 8,
            /**
            A special case of a level trigger where the first trigger pulse starts
            the exposure and the next trigger pulse starts the readout of that exposure.
            For N frames specified in the software, there must be N+1 trigger pulses in the
            series with individual exposure times being equal to the time interval between
            the trigger pulses. 
            This value allows the exposure mode to be "ORed" with #PL_EXPOSE_OUT_MODES
            for #pl_exp_setup_seq and #pl_exp_setup_cont functions.
            */
            EXT_TRIG_LEVEL_PULSED = (7 + 7) << 8
        }

        /**
        Used with #pl_exp_trigger function.
        Treated as @c #uns32 type.
        */
        public enum PL_SW_TRIG_STATUSES
        {
            /**
            The camera has accepted the trigger signal and started exposing.
            */
            PL_SW_TRIG_STATUS_TRIGGERED = 0,
            /**
            The camera was unable to accept the trigger due to an ongoing exposure.
            */
            PL_SW_TRIG_STATUS_IGNORED
        }

        /**
        Used with the #PARAM_EXPOSE_OUT_MODE parameter ID.
        Build the values for the Expose Out modes that are "ORed" with the trigger
        modes when setting up the script.
        Treated as @c #int32 type.
        */
        public enum PL_EXPOSE_OUT_MODES
        {
            /**
            Expose Out high when first row is exposed (from first row begin to first row end)
            */
            EXPOSE_OUT_FIRST_ROW = 0,
            /**
            Expose Out high when all rows are exposed simultaneously (from last row begin to first row end).
            The duration of the signal equals the exposure value entered which means the actual exposure
            time is longer - use this mode with triggered light source only.
            */
            EXPOSE_OUT_ALL_ROWS,
            /**
            Expose Out high when any row is exposed (from first row begin to last row end)
            */
            EXPOSE_OUT_ANY_ROW,
            /**
            Similar to ALL_ROWS. Actual exposure duration matches the value entered, but the Expose Out is
            only high while all rows are exposing simultaneously. 
            If the exposure time entered is shorter than the readout time, the Expose Out signal will
            remain at low level.
            */
            EXPOSE_OUT_ROLLING_SHUTTER,
            /**
            Expose Out signal pulses for every line readout as configured via the #PARAM_SCAN_MODE
            and related parameters. If #PARAM_SCAN_MODE is not available or enabled, the Expose Out
            behaves as #EXPOSE_OUT_ANY_ROW.
            */
            EXPOSE_OUT_LINE_TRIGGER,
            /**
            Similar to EXPOSE_OUT_ROLLING_SHUTTER but using sensor's global shutter mode.
            */
            EXPOSE_OUT_GLOBAL_SHUTTER,

            /* Should be the last and never used value. */
            MAX_EXPOSE_OUT_MODE
        }

        /**
        Used with the #PARAM_FAN_SPEED_SETPOINT parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_FAN_SPEEDS
        {
            FAN_SPEED_HIGH,   /**< Full fan speed, the default value for most cameras. */
            FAN_SPEED_MEDIUM, /**< Medium fan speed. */
            FAN_SPEED_LOW,    /**< Low fan speed. */
            FAN_SPEED_OFF     /**< Fan is turned off. */
        }

        /**
        Used with the #PARAM_TRIGTAB_SIGNAL parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_TRIGTAB_SIGNALS
        {
            PL_TRIGTAB_SIGNAL_EXPOSE_OUT /**< Control the expose out hardware signal multiplexing */
        }

        /**
        Used with the #PARAM_FRAME_DELIVERY_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_FRAME_DELIVERY_MODES
        {
            /**
            Camera will sample and deliver the frames as fast as possible while considering
            the interface bandwidth. This mode may result in uneven frame intervals, but
            possibly higher frame rates.
            */
            PL_FRAME_DELIVERY_MODE_MAX_FPS = 0,
            /**
            Camera will sample and deliver the frames in constant intervals. In this mode the
            camera samples the sensor in constant time intervals but if the interface is busy and
            all frame buffers are full, the frame will be skipped and the sensor will be sampled
            in the next interval (unlike the MAX_FPS mode where the sensor would be sampled immediately
            once the buffer/interface is unclogged). This results in even frame intervals, but
            there might be gaps	in between the frame deliveries and thus reduced frame rate.
            */
            PL_FRAME_DELIVERY_MODE_CONSTANT_INTERVALS
        }

        /**
        Used with the #PARAM_CAM_INTERFACE_TYPE parameter ID.

        32-bit enum where:
        - Upper 24 bits are interface classes, flags, 1bit = one class, 24 possible classes.
        - Lower 8 bits are interface revisions with 254 possible revisions per each interface class.

        Usage:
        @code{.cpp}
         if (attrCurrent & PL_CAM_IFC_TYPE_USB)
            // The camera is running on USB, any USB
         if (attrCurrent & PL_CAM_IFC_TYPE_USB && type >= PL_CAM_IFC_TYPE_USB_3_0)
            // The camera is running on USB, the camera is running on at least USB 3.0
         if (attrCurrent == PL_CAM_IFC_TYPE_USB_3_1)
            // The camera is running exactly on USB 3.1
        @endcode

        Treated as @c #int32 type.
        */
        public enum PL_CAM_INTERFACE_TYPES
        {
            PL_CAM_IFC_TYPE_UNKNOWN = 0,        /**< Unrecognized type. */

            PL_CAM_IFC_TYPE_1394 = 0x100,    /**< A generic 1394 in case we cannot identify the sub type. */
            PL_CAM_IFC_TYPE_1394_A,                 /**< FireWire 400. */
            PL_CAM_IFC_TYPE_1394_B,                 /**< FireWire 800. */

            PL_CAM_IFC_TYPE_USB = 0x200,    /**< A generic USB in case we cannot identify the sub type. */
            PL_CAM_IFC_TYPE_USB_1_1,                /**< FullSpeed (12 Mbit/s). */
            PL_CAM_IFC_TYPE_USB_2_0,                /**< HighSpeed (480 Mbit/s). */
            PL_CAM_IFC_TYPE_USB_3_0,                /**< SuperSpeed (5 Gbit/s). */
            PL_CAM_IFC_TYPE_USB_3_1,                /**< SuperSpeed+ (10 Gbit/s). */

            PL_CAM_IFC_TYPE_PCI = 0x400,    /**< A generic PCI interface. */
            PL_CAM_IFC_TYPE_PCI_LVDS,               /**< LVDS PCI interface. */

            PL_CAM_IFC_TYPE_PCIE = 0x800,    /**< A generic PCIe interface. */
            PL_CAM_IFC_TYPE_PCIE_LVDS,              /**< LVDS PCIe interface. */
            PL_CAM_IFC_TYPE_PCIE_X1,                /**< Single channel PCIe interface. */
            PL_CAM_IFC_TYPE_PCIE_X4,                /**< 4 channel PCIe interface. */
            PL_CAM_IFC_TYPE_PCIE_X8,                /**< 8 channel PCIe interface. */

            PL_CAM_IFC_TYPE_VIRTUAL = 0x1000,   /**< Base for all Virtual camera interfaces. */

            PL_CAM_IFC_TYPE_ETHERNET = 0x2000    /**< Base for all Ethernet-based cameras. */
        }

        /**
        Used with the #PARAM_CAM_INTERFACE_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_CAM_INTERFACE_MODES
        {
            PL_CAM_IFC_MODE_UNSUPPORTED = 0, /**< Interface is not supported. */
            PL_CAM_IFC_MODE_CONTROL_ONLY,    /**< Control commands only. */
            PL_CAM_IFC_MODE_IMAGING          /**< Full imaging. */
        }

        /**
        Used with the #PARAM_CENTROIDS_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_CENTROIDS_MODES
        {
            PL_CENTROIDS_MODE_LOCATE = 0, /**< Locate mode (PrimeLocate) */
            PL_CENTROIDS_MODE_TRACK,      /**< Particle Tracking mode */
            PL_CENTROIDS_MODE_BLOB        /**< Blob Detection mode */
        }

        /**
        Used with the #PARAM_SCAN_MODE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_SCAN_MODES
        {
            /** Normal camera imaging and the default mode of operation, the FPGA
            reads each row in succession without inserting additional delays between rows */
            PL_SCAN_MODE_AUTO = 0,
            /** This mode allows the user to configure the #PARAM_SCAN_LINE_DELAY.
            The #PARAM_SCAN_WIDTH will become read-only and its value will be
            auto-calculated and reported by the camera */
            PL_SCAN_MODE_PROGRAMMABLE_LINE_DELAY,
            /** This mode allows the user to configure the #PARAM_SCAN_WIDTH.
            The #PARAM_SCAN_LINE_DELAY will become read-only and its value will be
            auto-calculated and reported by the camera */
            PL_SCAN_MODE_PROGRAMMABLE_SCAN_WIDTH
        }

        /**
        Used with the #PARAM_SCAN_DIRECTION parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_SCAN_DIRECTIONS
        {
            /** Default mode. The camera starts reading from top to bottom */
            PL_SCAN_DIRECTION_DOWN = 0,
            /** Camera starts reading from bottom to top */
            PL_SCAN_DIRECTION_UP,
            /** Camera initially starts reading from top to bottom
            and then switches automatically to read out from bottom to top.
            The direction alternates between frames down-up-down-up and so on.
            */
            PL_SCAN_DIRECTION_DOWN_UP
        }

        /**
        Used with the #PARAM_PP_FEAT_ID parameter ID.
        Treated as @c #uns16 type.
        */
        public enum PP_FEATURE_IDS
        {
            PP_FEATURE_RING_FUNCTION,
            PP_FEATURE_BIAS,
            PP_FEATURE_BERT, /**< Background Event Reduction Technology */
            PP_FEATURE_QUANT_VIEW,
            PP_FEATURE_BLACK_LOCK,
            PP_FEATURE_TOP_LOCK,
            PP_FEATURE_VARI_BIT,
            PP_FEATURE_RESERVED,            /**< Should not be used at any time moving forward. */
            PP_FEATURE_DESPECKLE_BRIGHT_HIGH,
            PP_FEATURE_DESPECKLE_DARK_LOW,
            PP_FEATURE_DEFECTIVE_PIXEL_CORRECTION,
            PP_FEATURE_DYNAMIC_DARK_FRAME_CORRECTION,
            PP_FEATURE_HIGH_DYNAMIC_RANGE,
            PP_FEATURE_DESPECKLE_BRIGHT_LOW,
            PP_FEATURE_DENOISING, /**< PrimeEnhance feature */
            PP_FEATURE_DESPECKLE_DARK_HIGH,
            PP_FEATURE_ENHANCED_DYNAMIC_RANGE,
            PP_FEATURE_FRAME_SUMMING,
            PP_FEATURE_LARGE_CLUSTER_CORRECTION,
            PP_FEATURE_FRAME_AVERAGING,
            PP_FEATURE_MAX
        }

        /**
        Used with the #PARAM_PP_PARAM_ID parameter ID.
        */
        public const int PP_MAX_PARAMETERS_PER_FEATURE = 10;

        /**
        Used with the #PARAM_PP_PARAM_ID parameter ID.
        Treated as @c #uns16 type.
        */
        public enum PP_PARAMETER_IDS
        {
            PP_PARAMETER_RF_FUNCTION = (PP_FEATURE_IDS.PP_FEATURE_RING_FUNCTION * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_BIAS_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_BIAS * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_BIAS_LEVEL,
            PP_FEATURE_BERT_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_BERT * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_BERT_THRESHOLD,
            PP_FEATURE_QUANT_VIEW_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_QUANT_VIEW * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_QUANT_VIEW_E,
            PP_FEATURE_BLACK_LOCK_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_BLACK_LOCK * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_BLACK_LOCK_BLACK_CLIP,
            PP_FEATURE_TOP_LOCK_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_TOP_LOCK * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_TOP_LOCK_WHITE_CLIP,
            PP_FEATURE_VARI_BIT_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_VARI_BIT * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_VARI_BIT_BIT_DEPTH,
            PP_FEATURE_DESPECKLE_BRIGHT_HIGH_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DESPECKLE_BRIGHT_HIGH * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DESPECKLE_BRIGHT_HIGH_THRESHOLD,
            PP_FEATURE_DESPECKLE_BRIGHT_HIGH_MIN_ADU_AFFECTED,
            PP_FEATURE_DESPECKLE_DARK_LOW_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DESPECKLE_DARK_LOW * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DESPECKLE_DARK_LOW_THRESHOLD,
            PP_FEATURE_DESPECKLE_DARK_LOW_MAX_ADU_AFFECTED,
            PP_FEATURE_DEFECTIVE_PIXEL_CORRECTION_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DEFECTIVE_PIXEL_CORRECTION * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DYNAMIC_DARK_FRAME_CORRECTION_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DYNAMIC_DARK_FRAME_CORRECTION * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_HIGH_DYNAMIC_RANGE_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_HIGH_DYNAMIC_RANGE * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DESPECKLE_BRIGHT_LOW_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DESPECKLE_BRIGHT_LOW * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DESPECKLE_BRIGHT_LOW_THRESHOLD,
            PP_FEATURE_DESPECKLE_BRIGHT_LOW_MAX_ADU_AFFECTED,
            PP_FEATURE_DENOISING_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DENOISING * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DENOISING_NO_OF_ITERATIONS,
            PP_FEATURE_DENOISING_GAIN,
            PP_FEATURE_DENOISING_OFFSET,
            PP_FEATURE_DENOISING_LAMBDA,
            PP_FEATURE_DESPECKLE_DARK_HIGH_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_DESPECKLE_DARK_HIGH * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_DESPECKLE_DARK_HIGH_THRESHOLD,
            PP_FEATURE_DESPECKLE_DARK_HIGH_MIN_ADU_AFFECTED,
            PP_FEATURE_ENHANCED_DYNAMIC_RANGE_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_ENHANCED_DYNAMIC_RANGE * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_FRAME_SUMMING_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_FRAME_SUMMING * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_FRAME_SUMMING_COUNT,
            PP_FEATURE_FRAME_SUMMING_32_BIT_MODE,
            PP_FEATURE_LARGE_CLUSTER_CORRECTION_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_LARGE_CLUSTER_CORRECTION * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_FRAME_AVERAGING_ENABLED = (PP_FEATURE_IDS.PP_FEATURE_FRAME_AVERAGING * PP_MAX_PARAMETERS_PER_FEATURE),
            PP_FEATURE_FRAME_AVERAGING_COUNT_FACTOR,
            PP_PARAMETER_ID_MAX
        }

#pragma warning disable IDE1006 // Naming rule violation: These words must begin with upper case...

        /**
        Used with the #PARAM_SMART_STREAM_EXP_PARAMS and #PARAM_SMART_STREAM_DLY_PARAMS
        parameter IDs and #pl_create_smart_stream_struct and
        #pl_release_smart_stream_struct functions.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct smart_stream_type
        {
            [MarshalAs(UnmanagedType.U2)]
            public short entries;     /**< The number of entries in the array. */
            public IntPtr parameters; /**< The actual S.M.A.R.T. stream parameters. */
        }

        /**
        Used with the #PARAM_SMART_STREAM_MODE parameter ID.
        Treated as @c #uns16 type.
        */
        public enum PL_SMT_MODES
        {
            SMTMODE_ARBITRARY_ALL = 0, /**< Smart streaming values can be arbitrary. */
            SMTMODE_MAX
        }

        /**
        Used with the functions #pl_exp_check_status, #pl_exp_check_cont_status and
        #pl_exp_check_cont_status_ex.
        Treated as @c #int16 type.
        */
        public enum PL_IMAGE_STATUSES
        {
            /** The system is @b idle, no data is expected. If any data arrives, it will be discarded. */
            READOUT_NOT_ACTIVE,
            /**
            The data collection routines are @b active. They are waiting for data to arrive,
            but none has arrived yet.
            */
            EXPOSURE_IN_PROGRESS,
            /** The data collection routines are @b active. The data has started to arrive. */
            READOUT_IN_PROGRESS,
            /** Frame has been read out and is available in acquisition buffer. */
            READOUT_COMPLETE,
            /** New camera status indicating at least one frame is available. */
            FRAME_AVAILABLE = READOUT_COMPLETE,
            /** Something went wrong. The function returns a #PV_FAIL and #pl_error_code is set. */
            READOUT_FAILED,
            /** @warning Deprecated. Not used by PVCAM. */
            [Obsolete("Obsolete, not by PVCAM.")]
            ACQUISITION_IN_PROGRESS,

            /* Should be the last and never used value. */
            MAX_CAMERA_STATUS
        }

        /**
        Used with the #pl_exp_abort function. The CCS (Camera Control Subsystem) modes 
        are applicable with CCD cameras only. With latest sCMOS cameras use the
        #CCS_HALT mode.

        Treated as @c #int16 type.
        */
        public enum PL_CCS_ABORT_MODES
        {
            /**
            Do not alter the current state of the CCS. Use only if instructed
            by the camera vendor.
            */
            CCS_NO_CHANGE = 0,
            /**
            Halt all CCS activity and put the CCS into the idle state.
            Recommended with sCMOS cameras and with most acquisitions with CCD
            sensors when using pre-sequence clearing.
            */
            CCS_HALT,
            /**
            Close the shutter, then halt all CCS activity and put the CCS
            into the idle state.
            */
            CCS_HALT_CLOSE_SHTR,
            /**
            Put the CCS into the continuous clearing state. Recommended for
            CCD sensors where continuous clearing is required.
            */
            CCS_CLEAR,
            /**
            Close the shutter, then put the CCS into the continuous clearing state.
            */
            CCS_CLEAR_CLOSE_SHTR,
            /**
            Open the shutter, then halt all CCS activity and put the CCS into
            the idle state.
            */
            CCS_OPEN_SHTR,
            /**
            Open the shutter, then put the CCS into the continuous clearing state.
            */
            CCS_CLEAR_OPEN_SHTR
        }

        /**
        Used with the #PARAM_BOF_EOF_ENABLE parameter ID.
        Treated as @c #int32 type.
        */
        public enum PL_IRQ_MODES
        {
            NO_FRAME_IRQS = 0,      /**< Both counters are disabled. */
            BEGIN_FRAME_IRQS,       /**< Counts BOF events. */
            END_FRAME_IRQS,         /**< Counts EOF events. */
            BEGIN_END_FRAME_IRQS    /**< Provides a sum of BOF and EOF event. */
        }

        /**
        Used with the function #pl_exp_setup_cont.
        Treated as @c #int16 type.
        */
        public enum PL_CIRC_MODES : short
        {
            /**
            Used internally by PVCAM for sequence acquisitions only.
            @internal
            */
            CIRC_NONE = 0,
            /**
            In this circular buffer mode the oldest frame in buffer is automatically
            replaced with new frame. No error is indicated by any function when this
            occurs.
            */
            CIRC_OVERWRITE,
            /**
            In non-overwrite mode the oldest frame in circular buffer is also replaced
            with new frame as in #CIRC_OVERWRITE mode (e.g. via DMA write from camera),
            but PVCAM recognizes it and returns a buffer overrun error in the next
            call of #pl_exp_get_oldest_frame or #pl_exp_get_latest_frame_ex.
            */
            CIRC_NO_OVERWRITE
        }

        /**
        Used with the #PARAM_EXP_RES parameter ID.
        The resolution defines units in which the exposure time is passed to #pl_exp_setup_seq
        and #pl_exp_setup_cont calls and for the Variable Timed Mode (#PARAM_EXP_TIME).
        Treated as @c #int32 type.
        */
        public enum PL_EXP_RES_MODES
        {
            EXP_RES_ONE_MILLISEC = 0, /**< Exposure value is defined in milli-seconds. */
            EXP_RES_ONE_MICROSEC,     /**< Exposure value is defined in micro-seconds. */
            EXP_RES_ONE_SEC           /**< Exposure value is defined in seconds. */
        }

        /**
        Used with the function #pl_io_script_control.
        Treated as @c #uns32 type.
        */
        public enum PL_SRC_MODES
        {
            SCR_PRE_OPEN_SHTR = 0,
            SCR_POST_OPEN_SHTR,
            SCR_PRE_FLASH,
            SCR_POST_FLASH,
            SCR_PRE_INTEGRATE,
            SCR_POST_INTEGRATE,
            SCR_PRE_READOUT,
            SCR_POST_READOUT,
            SCR_PRE_CLOSE_SHTR,
            SCR_POST_CLOSE_SHTR
        }

        /**
        Used with the functions pl_cam_register_callback* and #pl_cam_deregister_callback.

        Please note that callbacks are generated based on actual frame data transfer.
        This means that the BOF and EOF callbacks may not always correspond to actual
        readout of given frame from the camera sensor. This is especially true for fast
        cameras where frames are buffered on the camera side and BOF/EOF callbacks are
        generated when the frame is actually received on the device driver side.
        It is recommended to use the camera hardware signals for accurate frame
        synchronization.

        Treated as @c #int32 type.
        */
        public enum PL_CALLBACK_EVENT
        {
            /**
            Beginning of frame callback. Occurs when the frame transfer begins.
            */
            PL_CALLBACK_BOF = 0,
            /**
            End of frame callback. Occurs when the frame is fully transferred
            and ready in the frame buffer.
            */
            PL_CALLBACK_EOF,
            /**
            List of connected cameras has changed. This feature is currently not
            supported. The list of cameras is refreshed only during #pl_pvcam_init.
            */
            PL_CALLBACK_CHECK_CAMS,
            /**
            A camera has been removed from the system.
            */
            PL_CALLBACK_CAM_REMOVED,
            /**
            A camera previously removed is available again. This feature is not
            currently supported.
            */
            PL_CALLBACK_CAM_RESUMED,
            PL_CALLBACK_MAX
        }

        /**
        Defines the acquisition region.
        Used in #pl_exp_setup_seq and #pl_exp_setup_cont.
        In most cases, the <tt>s1, s2</tt> coordinates correspond to <tt>x1, x2</tt> coordinates.
        The sensor region width is then calculated as <tt>s2 - s1 + 1</tt>. The resulting
        image width would be <tt>(s2 - s1 + 1) / sbin</tt>.
        Similarly, the <tt>p1, p2</tt> coordinates correspond to <tt>y1, y2</tt> coordinates.
        The sensor region height is then calculated as <tt>(p2 - p1 + 1)</tt>. The resulting
        image height would be <tt>(p2 - p1 + 1) / pbin</tt>.
        */
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct rgn_type
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort s1;   /**< First pixel in the serial register. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort s2;   /**< Last pixel in the serial register. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort sbin; /**< Serial binning for this region. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort p1;   /**< First row in the parallel register. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort p2;   /**< Last row in the parallel register. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort pbin; /**< Parallel binning for this region. */
        }

        /******************************************************************************/
        /* Start of Frame Metadata Types                                              */
        /******************************************************************************/

        /******************************************************************************/
        /* Data headers and camera shared types                                       */

        /**
        Used in #md_frame_header structure.
        Treated as @c #uns8 type.
        */
        public enum PL_MD_FRAME_FLAGS
        {
            /** Check this bit before using the timestampBOR and timestampEOR. */
            PL_MD_FRAME_FLAG_ROI_TS_SUPPORTED = 0x01,
            PL_MD_FRAME_FLAG_UNUSED_2 = 0x02,
            PL_MD_FRAME_FLAG_UNUSED_3 = 0x04,
            PL_MD_FRAME_FLAG_UNUSED_4 = 0x10,
            PL_MD_FRAME_FLAG_UNUSED_5 = 0x20,
            PL_MD_FRAME_FLAG_UNUSED_6 = 0x40,
            PL_MD_FRAME_FLAG_UNUSED_7 = 0x80
        }

        /**
        Used in #md_frame_roi_header structure.
        Treated as @c #uns8 type.
        */
        public enum PL_MD_ROI_FLAGS
        {
            /**
            This flag is used by #pl_md_frame_decode to discard invalid ROIs.
            Any ROI with this flag will not be included in the #md_frame ROI array.
            */
            PL_MD_ROI_FLAG_INVALID = 0x01,
            /**
            This flag is used to report an ROI that contains no pixel data. Such
            ROI is used to only mark a location within the frame.
            */
            PL_MD_ROI_FLAG_HEADER_ONLY = 0x02,
            PL_MD_ROI_FLAG_UNUSED_3 = 0x04,
            PL_MD_ROI_FLAG_UNUSED_4 = 0x10,
            PL_MD_ROI_FLAG_UNUSED_5 = 0x20,
            PL_MD_ROI_FLAG_UNUSED_6 = 0x40,
            PL_MD_ROI_FLAG_UNUSED_7 = 0x80
        }

        /**
        The signature is located in the first 4 bytes of the frame header. The signature
        is checked before any metadata-related operations are executed on the buffer.
        */
        public const int PL_MD_FRAME_SIGNATURE = 5328208;

        /**
        This is a frame header that is located before each frame. The size of this
        structure must remain constant. The structure is generated by the camera
        and should be 16-byte aligned.
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct md_frame_header
        {
            /* TOTAL: 48 bytes */
            [MarshalAs(UnmanagedType.U4)]
            public uint signature;         /**< 4B - Equal to #PL_MD_FRAME_SIGNATURE. */
            [MarshalAs(UnmanagedType.U1)]
            public byte version;           /**< 1B - Must be 1 in the first release. */
            [MarshalAs(UnmanagedType.U4)]
            public uint frameNr;           /**< 4B - 1-based, reset with each acquisition. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort roiCount;        /**< 2B - Number of ROIs in the frame, at least 1. */

            /** The final timestamp = timestampBOF * timestampResNs (in nanoseconds). */
            [MarshalAs(UnmanagedType.U4)]
            public uint timeStampBOF;      /**< 4B - Depends on resolution. */
            [MarshalAs(UnmanagedType.U4)]
            public uint timeStampEOF;      /**< 4B - Depends on resolution. */
            [MarshalAs(UnmanagedType.U4)]
            public uint timeStampResNs;    /**< 4B - 1=1ns, 1000=1us, 5000000=5ms, ... */

            /** The final exposure time = exposureTime * exposureTimeResNs (nanoseconds). */
            [MarshalAs(UnmanagedType.U4)]
            public uint exposureTime;      /**< 4B - Depends on resolution. */
            [MarshalAs(UnmanagedType.U4)]
            public uint exposureTimeResNs; /**< 4B - 1=1ns, 1000=1us, 5000000=5ms, ... */

            /** ROI timestamp resolution is stored here, no need to transfer with each ROI. */
            [MarshalAs(UnmanagedType.U4)]
            public uint roiTimestampResNs; /**< 4B - ROI timestamps resolution. */

            [MarshalAs(UnmanagedType.U1)]
            public byte bitDepth;          /**< 1B - Must be 10, 13, 14, 16, etc. */
            [MarshalAs(UnmanagedType.U1)]
            public byte colorMask;         /**< 1B - Corresponds to #PL_COLOR_MODES. */
            [MarshalAs(UnmanagedType.U1)]
            public byte flags;             /**< 1B - Frame flags, see #PL_MD_FRAME_FLAGS. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort extendedMdSize;  /**< 2B - Must be 0 or actual ext md data size. */

            /** Version 2 header additions. The following entries are available only
            when the md_frame_header.version is reported as 2 or higher. */
            [MarshalAs(UnmanagedType.U1)]
            public byte imageFormat;       /**< 1B - Image data format, see #PL_IMAGE_FORMATS */
            [MarshalAs(UnmanagedType.U1)]
            public byte imageCompression;  /**< 1B - Image pixel data compression, see #PL_IMAGE_COMPRESSIONS */

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] reserved;
        }

        /**
        Version 3 of the frame header with improved timestamp and exposure time accuracy.
        When the md_frame_header.version is reported as 3, users are expected to use reinterpret_cast
        to cast the md_frame.header pointer to md_frame_header_v3 type.
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct md_frame_header_v3
        {
            /* TOTAL: 48 bytes */
            [MarshalAs(UnmanagedType.U4)]
            public uint signature;         /**< 4B - Equal to #PL_MD_FRAME_SIGNATURE. */
            [MarshalAs(UnmanagedType.U1)]
            public byte version;           /**< 1B - Must be 1 in the first release. */
            [MarshalAs(UnmanagedType.U4)]
            public uint frameNr;           /**< 4B - 1-based, reset with each acquisition. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort roiCount;        /**< 2B - Number of ROIs in the frame, at least 1. */

            [MarshalAs(UnmanagedType.U8)]
            public ulong timeStampBOF;     /**< 8B - Beginning of frame timestamp, in picoseconds. */
            [MarshalAs(UnmanagedType.U8)]
            public ulong timeStampEOF;     /**< 8B - End of frame timestamp, in picoseconds. */
            [MarshalAs(UnmanagedType.U8)]
            public ulong exposureTime;     /**< 8B - Exposure time, in picoseconds. */

            [MarshalAs(UnmanagedType.U1)]
            public byte bitDepth;          /**< 1B - Must be 10, 13, 14, 16, etc. */
            [MarshalAs(UnmanagedType.U1)]
            public byte colorMask;         /**< 1B - Corresponds to #PL_COLOR_MODES. */
            [MarshalAs(UnmanagedType.U1)]
            public byte flags;             /**< 1B - Frame flags, see #PL_MD_FRAME_FLAGS. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort extendedMdSize;  /**< 2B - Must be 0 or actual ext md data size. */
            [MarshalAs(UnmanagedType.U1)]
            public byte imageFormat;       /**< 1B - Image data format, see #PL_IMAGE_FORMATS */
            [MarshalAs(UnmanagedType.U1)]
            public byte imageCompression;  /**< 1B - Image pixel data compression, see #PL_IMAGE_COMPRESSIONS */

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] reserved;
        }

        /**
        This is a ROI header that is located before every ROI data. The size of this
        structure must remain constant. The structure is generated by the camera
        and should be 16-byte aligned.
        */
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct md_frame_roi_header
        {                                 /* TOTAL: 32 bytes */
            [MarshalAs(UnmanagedType.U2)]
            public ushort roiNr;          /**< 2B - 1-based, reset with each frame. */

            /** ROI timestamps. Currently unused. */
            [MarshalAs(UnmanagedType.U4)]
            public uint timestampBOR;     /**< 4B - Beginning of ROI readout timestamp. */
            [MarshalAs(UnmanagedType.U4)]
            public uint timestampEOR;     /**< 4B - End of ROI readout timestamp. */

            public rgn_type roi;          /**< 12B - ROI coordinates and binning. */

            [MarshalAs(UnmanagedType.U1)]
            public byte flags;            /**< 1B - ROI flags, see #PL_MD_ROI_FLAGS. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort extendedMdSize; /**< 2B - Must be 0 or actual ext metadata size in bytes. */

            /** Version 2 header additions */
            [MarshalAs(UnmanagedType.U4)]
            public uint roiDataSize;      /**< 4B - ROI image data size in bytes. */

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] reserved;
        }

        /******************************************************************************/
        /* Extended metadata related structures                                       */

        /**
        Maximum number of extended metadata tags supported.
        */
        public const int PL_MD_EXT_TAGS_MAX_SUPPORTED = 255;

        /**
        Available extended metadata tags.
        Used in #md_ext_item_info structure.
        Used directly as an enum type without casting to any integral type.
        */
        public enum PL_MD_EXT_TAGS
        {
            PL_MD_EXT_TAG_PARTICLE_ID = 0, /**< Particle ID */
            PL_MD_EXT_TAG_PARTICLE_M0,     /**< Particle M0 vector */
            PL_MD_EXT_TAG_PARTICLE_M2,     /**< Particle M2 vector */
            PL_MD_EXT_TAG_MAX
        }

        /**
        This structure describes the extended metadata TAG. This information is
        retrieved from an internal table. User needs this to correctly read and
        display the extended metadata value.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct md_ext_item_info
        {
            public PL_MD_EXT_TAGS tag;   /**< Tag ID */
            [MarshalAs(UnmanagedType.U2)]
            public ushort type;  /**< Tag type, corresponds to #ATTR_TYPE */
            [MarshalAs(UnmanagedType.U2)]
            public ushort size;  /**< Tag value size */
            public IntPtr name;  /**< Tag friendly name */
        }

        /**
        An extended metadata item together with its value. The user will retrieve a
        collection of these items.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct md_ext_item
        {
            public IntPtr tagInfo;  /**< Tag information */
            public IntPtr tagValue; /**< Tag value */
        }

        /**
        A collection of decoded extended metadata.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct md_ext_item_collection
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PL_MD_EXT_TAGS_MAX_SUPPORTED)]
            public md_ext_item[] list;  /**< List of extended metadata tags */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = PL_MD_EXT_TAGS_MAX_SUPPORTED)]
            public IntPtr[] map;   /**< Map of extended metadata tags */
            public ushort count; /**< Number of valid tags in the arrays above */
        }

        /**
        This is a helper structure that is used to decode the md_frame_roi_header. Since
        the header cannot contain any pointers, PVCAM will calculate all information
        using offsets from frame & ROI headers.

        The structure must be created using the #pl_md_create_frame_struct function.
        Please note the structure keeps only pointers to data residing in the image
        buffer. Once the buffer is deleted, the contents of the structure become invalid.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct md_frame_roi
        {
            public IntPtr header;         /**< Points directly to the header within the buffer. */
            public IntPtr data;           /**< Points to the ROI image data. */
            [MarshalAs(UnmanagedType.U4)]
            public uint dataSize;       /**< Size of the ROI image data in bytes. */
            public IntPtr extMdData;      /**< Points directly to ext. metadata data within the buffer. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort extMdDataSize;  /**< Size of the extended metadata buffer. */
        }

        /**
        This is a helper structure that is used to decode the md_frame_header. Since
        the header cannot contain any pointers, we need to calculate all information
        using offsets only.

        Please note the structure keeps only pointers to data residing in the image
        buffer. Once the buffer is deleted, the contents of the structure become invalid.
        */
        [StructLayout(LayoutKind.Sequential)]
        public struct md_frame
        {
            public IntPtr header;        /**< Points directly to the header within the buffer. */
            public IntPtr extMdData;     /**< Points directly to ext. metadata within the buffer. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort extMdDataSize; /**< Size of the ext. metadata buffer in bytes. */
            public rgn_type impliedRoi;/**< Implied ROI calculated during decoding. */
            public IntPtr roiArray;      /**< An array of ROI descriptors. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort roiCapacity;   /**< Number of ROIs the structure can hold. */
            [MarshalAs(UnmanagedType.U2)]
            public ushort roiCount;      /**< Number of ROIs found during decoding. */
        }

        /******************************************************************************/
        /*End of Frame Metadata Types                                                 */
        /******************************************************************************/

#pragma warning restore IDE1006

        /**
        Data type used by #pl_get_param with #ATTR_TYPE.
        @{
        */
        private const int TYPE_INT16 = 1;
        private const int TYPE_INT32 = 2;
        private const int TYPE_FLT64 = 4;
        private const int TYPE_UNS8 = 5;
        private const int TYPE_UNS16 = 6;
        private const int TYPE_UNS32 = 7;
        private const int TYPE_UNS64 = 8;
        private const int TYPE_ENUM = 9;
        private const int TYPE_BOOLEAN = 11;
        private const int TYPE_INT8 = 12;
        private const int TYPE_CHAR_PTR = 13;
        private const int TYPE_VOID_PTR = 14;
        private const int TYPE_VOID_PTR_PTR = 15;
        private const int TYPE_INT64 = 16;
        private const int TYPE_SMART_STREAM_TYPE = 17;
        private const int TYPE_SMART_STREAM_TYPE_PTR = 18;
        private const int TYPE_FLT32 = 19;
        private const int TYPE_RGN_TYPE = 20;
        private const int TYPE_RGN_TYPE_PTR = 21;
        private const int TYPE_RGN_LIST_TYPE = 22;
        private const int TYPE_RGN_LIST_TYPE_PTR = 23;
        /** @} */

        /*
        Defines for classes.
        */
        private const int CLASS0 = 0;          /* Camera Communications */
        private const int CLASS2 = 2;          /* Configuration/Setup */
        private const int CLASS3 = 3;          /* Data Acquisition */

        /******************************************************************************/
        /* Start of parameter ID definitions.                                         */
        /* Format: TTCCxxxx, where TT = Data type, CC = Class, xxxx = ID number       */
        /* Please note that some data types encoded in the parameter IDs do not       */
        /* correspond to the actual parameter data type. Please always check the      */
        /* ATTR_TYPE before using the parameter.                                      */
        /******************************************************************************/

        /* CAMERA COMMUNICATION PARAMETERS */

        /**
        Camera parameters. Native C header does not wrap the parameters in an enum.
        However, for .NET, we add the "PL_PARAM" enum here to make it easier to list
        all parameters with IntelliSense and to use ToString() on an enum.
        */
        public enum PL_PARAMS : uint
        {

            /**
            @brief Returns the length of an information message for each device driver.

            Some devices have no message. In other words, they return a value of 0 for bytes.

            Datatype: @c #int16

            @note The availability is camera dependent.
            */
            PARAM_DD_INFO_LENGTH = ((CLASS0 << 16) + (TYPE_INT16 << 24) + 1),
            /**
            @brief Returns a version number for the device driver used to access the camera.

            The version number is for information only. Application does not need to check
            the version or make decisions based on its value. Every PVCAM release is bundled
            with a specific set of device driver versions. PVCAM binaries and device driver
            binaries are usually tightly coupled and cannot be interchanged.

            The version is a formatted hexadecimal number of the style:

            |  High byte    |      Low      |      byte       |
            |:-------------:|--------------:|:----------------|
            |               | High nibble   |   Low nibble    |
            | Major version | Minor version | Trivial version |

            For example, the number 0xB1C0 indicates major release 177, minor release 12,
            and trivial change 0.

            Open the camera before calling this parameter. Note that different cameras in
            the same system may use different drivers. Thus, each camera can have its own
            driver and its own driver version.

            Datatype: @c #uns16
            */
            PARAM_DD_VERSION = ((CLASS0 << 16) + (TYPE_UNS16 << 24) + 2),
            /**
            @brief Reads/sets the maximum number of command retransmission attempts that are
                allowed.

            When a command or status transmission is garbled, the system signals for a
            re-transmission. After a certain number of failed transmissions (an initial
            attempt + max_retries), the system abandons the attempt and concludes that the
            communication link has failed. The camera will not close, but the command or
            status read returns with an error. The maximum number of retries is initially
            set by the device driver, and is matched to the communication link, hardware
            platform, and operating system. It may also be reset by the user.

            Datatype: @c #uns16

            @note The availability is camera-dependent.

            @remarks This parameter is deprecated.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_DD_RETRIES = ((CLASS0 << 16) + (TYPE_UNS16 << 24) + 3),
            /**
            @brief Reads/sets the maximum time the driver waits for acknowledgment.

            I.e., the slowest allowable response speed from the camera. This is a crucial
            factor used in the device driver for communication control. If the driver sends
            a command to the camera and does not receive acknowledgment within the timeout
            period, the driver times out and returns an error. Unless reset by the user,
            this timeout is a default setting that is contained in the device driver and is
            matched to the communication link, hardware platform, and operating system.

            Datatype: @c #uns16

            @note The availability is camera-dependent.

            @remarks This parameter is deprecated.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_DD_TIMEOUT = ((CLASS0 << 16) + (TYPE_UNS16 << 24) + 4),
            /**
            @brief Returns an information message for each device.

            Some devices have no message. The user is responsible for allocating enough
            memory to hold the message string. Required number of bytes can be obtained via
            parameter #PARAM_DD_INFO_LENGTH.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_DD_INFO = ((CLASS0 << 16) + (TYPE_CHAR_PTR << 24) + 5),

            /**
            @brief Returns a list of camera communication interfaces.

            For example it can be USB 3.0, PCIe, etc. Use the #ATTR_CURRENT to retrieve the
            currently selected interface. Use the #pl_get_enum_param function to retrieve
            all camera supported interfaces.

            Datatype: @c enum (@c #int32)

            @see #PL_CAM_INTERFACE_TYPES
            @note The availability is camera-dependent.
            */
            PARAM_CAM_INTERFACE_TYPE = ((CLASS0 << 16) + (TYPE_ENUM << 24) + 10),
            /**
            @brief Returns a list of camera communication interface modes.

            This for example returns whether the interface is fully capable of imaging or if it
            has limitations. Use the #ATTR_CURRENT to retrieve the mode of the currently
            selected interface. Use the #pl_get_enum_param function to retrieve the list of
            modes for all the camera supported interfaces.

            The number of items reported by this parameter corresponds to the number of
            items reported by the #PARAM_CAM_INTERFACE_TYPE. Using the #pl_get_enum_param
            call one can build a table of camera interfaces and their modes, simply iterate
            through both parameters and build the table, for example:

            | Enum index | Type id | Type string | Mode id | Mode string |
            |:----------:|:-------:|:-----------:|:-------:|:-----------:|
            |      0     |    514  |  "USB 2.0"  |    1    |  "Control"  |
            |      1     |    515  |  "USB 3.0"  |    2    |  "Imaging"  |
            |      2     |   2051  |  "PCIe x4"  |    2    |  "Imaging"  |

            Datatype: @c enum (@c #int32)

            @see #PL_CAM_INTERFACE_MODES
            @note The availability is camera-dependent.
            */
            PARAM_CAM_INTERFACE_MODE = ((CLASS0 << 16) + (TYPE_ENUM << 24) + 11),

            /* CONFIGURATION AND SETUP PARAMETERS */

            /**
            @brief Bias offset voltage.

            The units do not correspond to the output pixel values in any simple fashion
            (the conversion rate should be linear, but may differ from system to system), but
            a lower offset voltage will yield a lower value for all output pixels. Pixels
            brought below zero by this method will be clipped at zero. Pixels raised above
            saturation will be clipped at saturation. Before you can change the offset
            level, you must read the current offset level. The default offset level will
            also vary from system to system and may change with each speed and gain setting.

            Datatype: @c #int16

            @warning THIS VALUE IS SET AT THE FACTORY AND SHOULD NOT BE CHANGED!
            If you would like to change this value, please contact customer service before
            doing so.

            @note The availability is camera-dependent.
            */
            PARAM_ADC_OFFSET = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 195),
            /**
            @brief The name of the sensor.

            The name is a null-terminated text string. The user must pass in a character
            array that is at least #CCD_NAME_LEN elements long.

            Datatype: @c char*
            */
            PARAM_CHIP_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 129),
            /**
            @brief The name of the system.

            The name is a null-terminated text string. The user must pass in a character
            array that is at least #MAX_SYSTEM_NAME_LEN elements long. It is meant to
            replace #PARAM_CHIP_NAME behavior on some cameras which were
            reporting their friendly product name with this parameter, and in turn help
            future cameras go back to reporting the name of the sensor with
            #PARAM_CHIP_NAME.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_SYSTEM_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 130),
            /**
            @brief The name of the vendor.

            The name is a null-terminated text string. The user must pass in a character
            array that is at least #MAX_VENDOR_NAME_LEN elements long. This is meant to
            differentiate between "QImaging" and "Photometrics" products moving forward.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_VENDOR_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 131),
            /**
            @brief The name of the product.

            The name is a null-terminated text string. The user must pass in a character
            array that is at least #MAX_PRODUCT_NAME_LEN elements long. This is meant to
            report camera name such as "Prime 95B" or "Retiga R6". OEMs should also consider
            using this for branding their cameras.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_PRODUCT_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 132),
            /**
            @brief The part number of the camera.

            The part number is a null-terminated text string. The user must pass in a
            character array that is at least #MAX_CAM_PART_NUM_LEN elements long.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_CAMERA_PART_NUMBER = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 133),

            /**
            @brief This is the type of cooling used by the current camera.

            See #PL_COOL_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)
            */
            PARAM_COOLING_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 214),
            /**
            @brief The number of milliseconds required for the sensor output preamp to
            stabilize, after it is turned on.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_PREAMP_DELAY = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 502),
            /**
            @brief The color mode of the sensor.

            This enum parameter provides a list of all possible color masks defined in
            #PL_COLOR_MODES type. The real mask applied on sensor is reported as current
            value (#ATTR_CURRENT). Take into account that for mono cameras this parameter is
            usually not available (for #ATTR_AVAIL it returns @c FALSE) instead of reporting
            #COLOR_NONE value.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_COLOR_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 504),
            /**
            @brief Indicates whether this sensor runs in MPP mode.

            See #PL_MPP_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_MPP_CAPABLE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 224),
            /**
            @brief The exposure time limit in milliseconds above which the preamp is turned
            off during exposure.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            */
            PARAM_PREAMP_OFF_CONTROL = ((CLASS2 << 16) + (TYPE_UNS32 << 24) + 507),

            /* Sensor dimensions and physical characteristics */

            /**
            @brief The number of masked lines at the near end of the parallel register.

            It is next to the serial register. 0 means no mask (no normal mask). If the
            pre-mask is equal to par_size, this probably indicates a frame transfer device
            with an ordinary mask. Accordingly, the sensor should probably be run in frame
            transfer mode.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_PREMASK = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 53),
            /**
            @brief The number of pixels discarded from the serial register before the first
            real data pixel.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_PRESCAN = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 55),
            /**
            @brief The number of masked lines at the far end of the parallel register.

            It's away from the serial register. This is the number of additional parallel
            shifts that need to be done after readout to clear the parallel register.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_POSTMASK = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 54),
            /**
            @brief The number of pixels to discard from the serial register after the last
            real data pixel.

            These must be read or discarded to clear the serial register.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_POSTSCAN = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 56),
            /**
            @brief This is the center-to-center distance between pixels in the parallel
            direction.

            It is measured in nanometers. This is identical to #PARAM_PIX_PAR_SIZE if there
            are no inter-pixel dead areas.

            Datatype: @c #uns16
            */
            PARAM_PIX_PAR_DIST = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 500),
            /**
            @brief This is the size of the active area of a pixel, in the parallel direction.

            Reported in nanometers.

            Datatype: @c #uns16
            */
            PARAM_PIX_PAR_SIZE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 63),
            /**
            @brief This is the center-to-center distance between pixels in the serial
            direction.

            Reported in nanometers. This is identical to #PARAM_PIX_SER_SIZE, if there
            are no dead areas.

            Datatype: @c #uns16
            */
            PARAM_PIX_SER_DIST = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 501),
            /**
            @brief This is the size of the active area of a pixel in the serial direction.

            Reported in nanometers.

            Datatype: @c #uns16
            */
            PARAM_PIX_SER_SIZE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 62),
            /**
            @brief Checks to see if the summing well exists.

            When a @c TRUE is returned for #ATTR_AVAIL, the summing well exists.

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            */
            PARAM_SUMMING_WELL = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 505),
            /**
            @brief Gets the sensor full-well capacity, measured in electrons.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            */
            PARAM_FWELL_CAPACITY = ((CLASS2 << 16) + (TYPE_UNS32 << 24) + 506),
            /**
            @brief The parallel size of the sensor chip, in active rows.

            In most cases this parameter reports the height of the sensor imaging area or
            the number of rows in sCMOS type sensor.

            The full size of the parallel register is actually (par_size + premask + postmask).

            Datatype: @c #uns16
            */
            PARAM_PAR_SIZE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 57),
            /**
            @brief The serial size of the sensor chip, in active columns.

            In most cases this parameter reports the width of the sensor imaging area or
            the number of columns in sCMOS type sensor.

            Datatype: @c #uns16
            */
            PARAM_SER_SIZE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 58),
            /**
            @brief Returns @c TRUE for #ATTR_AVAIL if the camera has accumulation capability.

            Accumulation functionality is provided with the FF plug-in.

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            */
            PARAM_ACCUM_CAPABLE = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 538),
            /**
            @brief Reports if the camera supports firmware download.

            This parameter is for Teledyne Photometrics internal use only. It is largely unused
            because we simply do it on all cameras now, thus it can become deprecated in
            future.

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_FLASH_DWNLD_CAPABLE = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 539),

            /* General parameters */

            /**
            @brief Time it will take to read out the image from the sensor with the current
            camera settings, in microseconds.

            Settings have to be applied with #pl_exp_setup_seq or #pl_exp_setup_cont before
            the camera will calculate the readout time for the new settings.

            @warning: as with all other parameters please do not access this parameter
            while the camera is imaging.
            
            Datatype: @c #uns32

            @note The availability is camera-dependent.
            @note The parameter type is incorrectly defined. The actual type is TYPE_UNS32.
            */
            PARAM_READOUT_TIME = ((CLASS2 << 16) + (TYPE_FLT64 << 24) + 179),
            /**
            @brief This parameter reports the time needed to clear the sensor.

            The time is reported in nano seconds. This delay is incurred once prior to the
            acquisition when pre-sequence clearing mode is chosen by the application. The
            delay is incurred prior to every frame when the imaging application chooses
            pre-exposure clearing mode.

            The value is valid only after #pl_exp_setup_seq or #pl_exp_setup_cont call.

            Datatype: @c #long64

            @note The availability is camera-dependent.
            */
            PARAM_CLEARING_TIME = ((CLASS2 << 16) + (TYPE_INT64 << 24) + 180),
            /**
            @brief Post-trigger delay, in nanoseconds.

            In addition to the #PARAM_CLEARING_TIME, there might be a delay between an
            internal or external trigger and the transition event (low to high) for the
            Expose Out signal. This happens, for example, in global All Rows Expose Out mode
            in which case the value is equal to the readout time.

            The value is valid only after #pl_exp_setup_seq or #pl_exp_setup_cont call.

            Datatype: @c #long64

            @note The availability is camera-dependent.
            */
            PARAM_POST_TRIGGER_DELAY = ((CLASS2 << 16) + (TYPE_INT64 << 24) + 181),
            /**
            @brief Pre-trigger delay, in nanoseconds.

            For pre-exposure clearing mode and the first frame in pre-sequence clearing mode,
            the frame cycle time is the sum of #PARAM_EXPOSURE_TIME, #PARAM_PRE_TRIGGER_DELAY,
            #PARAM_POST_TRIGGER_DELAY and #PARAM_CLEARING_TIME.

            For second and subsequent frames in pre-sequence clearing mode (most typical
            scenario) the frame cycle time is the sum of #PARAM_EXPOSURE_TIME,
            #PARAM_PRE_TRIGGER_DELAY and #PARAM_POST_TRIGGER_DELAY.

            Frame cycle time is defined as the interval between start of exposure for one
            frame and start of exposure for the next frame when the camera is in internal
            triggered (timed) mode and set up for continuous (circular buffer) acquisition.

            The value is valid only after #pl_exp_setup_seq or #pl_exp_setup_cont call.

            Datatype: @c #long64

            @note The availability is camera-dependent.
            */
            PARAM_PRE_TRIGGER_DELAY = ((CLASS2 << 16) + (TYPE_INT64 << 24) + 182),

            /* CAMERA PARAMETERS */

            /**
            @brief Number of clear cycles.

            This is the number of times the sensor must be cleared to completely remove
            charge from the parallel register. The value is ignored in case the clearing
            mode is set to #CLEAR_NEVER or #CLEAR_AUTO via #PARAM_CLEAR_MODE parameter.

            Datatype: @c #uns16
            */
            PARAM_CLEAR_CYCLES = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 97),
            /**
            @brief Defines when clearing takes place.

            All the possible clearing modes are defined in #PL_CLEAR_MODES enum. But keep in
            mind that some cameras might not support all the modes. Use #pl_get_enum_param
            function to enumerate all the supported modes.

            Datatype: @c enum (@c #int32)

            @see @ref ClearModes
            @note The availability is camera-dependent.
            */
            PARAM_CLEAR_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 523),
            /**
            @brief Reports frame transfer capability.

            Returns @c TRUE for #ATTR_AVAIL if this camera can run in frame transfer mode
            (set through #PARAM_PMODE).

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            */
            PARAM_FRAME_CAPABLE = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 509),
            /**
            @brief Parallel clocking method.

            See #PL_PMODES enum for all possible values.
            The @c "_FT" in mode name indicates frame transfer mode, @c "_FT_MPP" indicates
            both frame transfer and MPP mode. @c "_ALT" indicates that custom parameters may
            be loaded.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_PMODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 524),

            /* Temperature parameters for the sensor. */

            /**
            @brief Returns the current measured temperature of the sensor in hundredths of
            degrees Celsius.

            For example, a temperature of minus 35°C would be read as -3500.

            @warning: Unless the #ATTR_LIVE attribute for this parameter is reported as #TRUE,
            this parameter must not be accessed while the camera is imaging.

            Datatype: @c #int16

            @note The availability is camera-dependent.
            */
            PARAM_TEMP = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 525),
            /**
            @brief Sets the desired sensor temperature in hundredths of degrees Celsius.

            E.g. -35°C is represented as -3500. The hardware attempts to heat or cool the
            sensor to this temperature. The min/max allowable temperatures are given by
            #ATTR_MIN and #ATTR_MAX. Settings outside this range are ignored. Note that this
            function only sets the desired temperature. Even if the desired temperature is
            within a legal range, it still may be impossible to achieve. If the ambient
            temperature is too high, it is difficult to get sufficient cooling on an air-cooled
            camera.

            Datatype: @c #int16

            @note The availability is camera-dependent.
            */
            PARAM_TEMP_SETPOINT = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 526),

            /* Parameters used for firmware version retrieval. */

            /**
            @brief Returns the firmware version of the camera, as a hexadecimal number.

            The form is @c MMmm, where @c MM is the major version and @c mm is the minor
            version. For example, 0x0814 corresponds to version 8.20.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_CAM_FW_VERSION = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 532),
            /**
            @brief Returns the alphanumeric serial number for the camera head.

            The serial number for Teledyne Photometrics-branded cameras has a maximum length of
            #MAX_ALPHA_SER_NUM_LEN.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_HEAD_SER_NUM_ALPHA = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 533),
            /**
            @brief Returns the version number of the PCI firmware.

            This number is a single 16-bit unsigned value.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_PCI_FW_VERSION = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 534),

            /**
            @brief Sets and gets the desired fan speed.

            Note that the camera can automatically adjust the fan speed to higher level due to
            sensor overheating or completely shut down power to the sensor board to protect
            camera from damage. The default fan speed is supposed to be changed only
            temporarily during experiments to reduce sound noise or vibrations.

            Datatype: @c enum (@c #int32)

            @warning Use this parameter with caution.
            @note The availability is camera-dependent.
            */
            PARAM_FAN_SPEED_SETPOINT = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 710),
            /**
            @brief Returns description of all camera nodes.

            The text is a multi-line and null-terminated string. The user must pass in a
            character array that is at least #MAX_CAM_SYSTEMS_INFO_LEN elements long or
            dynamically allocated array to size returned for #ATTR_COUNT attribute.

            The format is the same as can be seen in output of @c VersionInformation tool.

            Datatype: @c char*

            @warning Extra caution should be taken while accessing this parameter. It is 
            known to hang some cameras when used together with other parameters e.g. while 
            scanning camera capabilities after opening (especially with Retiga R1/R3/R6 cameras). 
            To work around this limitation ensure there is no communication with camera one 
            second before and one second after this parameter is used!
            @note The availability is camera-dependent.
            */
            PARAM_CAM_SYSTEMS_INFO = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 536),

            /**
            @brief The exposure/triggering mode.

            This parameter cannot be set, but its value can be retrieved. The mode is set as
            a value combined with Expose Out mode via #pl_exp_setup_seq or
            #pl_exp_setup_cont function.
            See #PL_EXPOSURE_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @see @ref ExposureModes, @ref secTriggerModes, @ref secExtTriggerModes
            */
            PARAM_EXPOSURE_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 535),
            /**
            @brief The Expose Out mode.

            This parameter cannot be set, but its value can be retrieved. The mode is set as
            a value combined with extended exposure mode via #pl_exp_setup_seq or
            #pl_exp_setup_cont function.
            See #PL_EXPOSE_OUT_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @see @ref secExpOutModes, @ref secExtTriggerModes
            @note The availability is camera-dependent.
            */
            PARAM_EXPOSE_OUT_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 560),

            /* SPEED TABLE PARAMETERS */

            /**
            @brief *Native* pixel sample range in number of bits for the currently configured acquisition.

            The image bit depth may depend on currently selected #PARAM_READOUT_PORT, #PARAM_SPDTAB_INDEX,
            #PARAM_GAIN_INDEX and other acquisition parameters. Please, make sure to check this parameter
            before starting the acquisition, ideally after calling #pl_exp_setup_seq or #pl_exp_setup_cont.

            Most Teledyne Photometrics cameras transfer pixels in 16-bit words (#uns16 type). However, some
            cameras are capable of transferring 8-bit data (#uns8 type). Make sure to also check the
            #PARAM_IMAGE_FORMAT parameter to discover the current camera pixel format.

            This parameter indicates the number of valid bits within the transferred word or byte.

            Datatype: @c #int16

            @see @ref SpeedTable
            */
            PARAM_BIT_DEPTH = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 511),

            /**
            @brief Pixel sample range of the image outputted to the *host*.

            This parameter differs from the *native* #PARAM_BIT_DEPTH in a way that it reports the bit depth
            of the *output* frame - a frame that is delivered to the *host*. Since PVCAM supports various *host*
            side post processing features, the *host* bit depth may differ from the *native* camera bit depth,
            depending on what *host*-side post processing features are active.

            For example, the camera *native* bit depth may be reported as 12-bit for current port/speed/gain
            selection. However, when #PARAM_HOST_FRAME_SUMMING_ENABLED is enabled, the *host* frame bit depth may
            be reported as 16 or 32-bit.

            As a general rule, the application should always rely on the '_HOST'-specific parameters when
            identifying the output data format. The *native* parameters should be used only for informational
            purposes, e.g. to show the camera native format in the GUI.

            Data type: @c #int16

            @see @ref SpeedTable, #PARAM_HOST_FRAME_SUMMING_ENABLED, #PARAM_HOST_FRAME_SUMMING_FORMAT.
            */
            PARAM_BIT_DEPTH_HOST = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 551),
            /**
            @brief *Native* image format of the camera pixel stream.

            This parameter is used to retrieve the list of camera-supported image formats. The
            image format may depend on current camera configuration. It is advised to check
            the image format before starting the acquisition. Some cameras allow the native
            format to be selected. Use the #ATTR_ACCESS to check the write permissions.

            Datatype: @c enum (@c #int32)

            @see #PL_IMAGE_FORMATS enum for all possible values.
            */
            PARAM_IMAGE_FORMAT = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 248),

            /**
            @brief Image format of the pixel stream that is outputted to the *host*.

            This parameter differs from the *native* #PARAM_IMAGE_FORMAT in a way that it reports the format
            of the *output* frame - a frame that is delivered to the *host*. Since PVCAM supports various *host*
            side post processing features, the *host* image format may differ from the *native* camera bit depth,
            depending on what *host*-side post processing features are active.

            For example, the camera *native* image format may be reported as 16-bit MONO for the current
            port/speed/gain selection. However, when #PARAM_HOST_FRAME_SUMMING_ENABLED is enabled, the *host* image
            format may need to be upgraded to a wider, 32-bit MONO format.

            As a general rule, the application should always rely on the '_HOST'-specific parameters when
            identifying the output data format. The *native* parameters should be used only for informational
            purposes, e.g. to show the camera native format in the GUI.

            Data type: @c enum (@c #int32)

            @see #PL_IMAGE_FORMATS enum for all possible values and related #PARAM_HOST_FRAME_SUMMING_ENABLED,
                #PARAM_HOST_FRAME_SUMMING_FORMAT parameters.
            */
            PARAM_IMAGE_FORMAT_HOST = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 552),
            /**
            @brief *Native* image compression of the camera pixel stream.

            This parameter is used to retrieve the list of camera-supported image compression modes.
            The compression mode may depend on currently selected port and speed combination.
            It is advised to check the compression mode before starting the acquisition. Some
            cameras allow the native compression to be selected. Use the #ATTR_ACCESS to check
            the write permissions.

            Datatype: @c enum (@c #int32)

            @see #PL_IMAGE_COMPRESSIONS enum for all possible values.
            */
            PARAM_IMAGE_COMPRESSION = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 249),

            /**
            @brief Image compression of the pixel stream outputted to the host.

            This parameter differs from the *native* #PARAM_IMAGE_COMPRESSION in a way that it reports the
            compression of the *output* frame - a frame that is delivered to the *host*. For some camera
            models, various compression methods may be used to increase the interface bandwidth.

            In general, applications do not have to support any of the compression-related parameters because
            decompression is automatically and seamlessly done by PVCAM. However, in specific cases the
            automatic decompression may be disabled.

            Data type: @c enum (@c #int32)

            @see #PL_IMAGE_COMPRESSIONS enum for all possible values and related
                #PARAM_HOST_FRAME_DECOMPRESSION_ENABLED.
            */
            PARAM_IMAGE_COMPRESSION_HOST = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 253),
            /**
            @brief Scan mode of the camera.

            This parameter is used to retrieve and control the camera-supported scan modes.
            Please note that the #PARAM_SCAN_LINE_DELAY and #PARAM_SCAN_WIDTH access mode
            depends on the scan mode selection.

            If the mode is set to #PL_SCAN_MODE_PROGRAMMABLE_SCAN_WIDTH, then the
            #PARAM_SCAN_WIDTH can be controlled and the #PARAM_SCAN_LINE_DELAY becomes read-only.
            If the mode is set to #PL_SCAN_MODE_PROGRAMMABLE_LINE_DELAY, then the
            #PARAM_SCAN_LINE_DELAY can be controlled and the #PARAM_SCAN_WIDTH becomes read-only.
            In both cases the #PARAM_SCAN_LINE_TIME reports the total time it takes to read
            one sensor line (row), including any delay added with the parameters above.

            Datatype: @c enum (@c #int32)

            @see #PL_SCAN_MODES enum for all possible values.
            */
            PARAM_SCAN_MODE = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 250),
            /**
            @brief Scan direction of the camera.

            This parameter is used to retrieve the list of camera-supported scan directions.

            Datatype: @c enum (@c #int32)

            @see #PL_SCAN_DIRECTIONS enum for all possible values.
            */
            PARAM_SCAN_DIRECTION = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 251),
            /**
            @brief Scan direction reset state.

            This parameter is used to retrieve scan direction reset state of camera.
            The parameter is used with alternate scan directions (down-up) to reset the
            direction with every acquisition.

            Datatype: @c boolean (@c #rs_bool)
            */
            PARAM_SCAN_DIRECTION_RESET = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 252),
            /**
            @brief Scan line delay value of the camera.

            This parameter is used to retrieve or set the scan line delay. The parameter
            access mode depends on the #PARAM_SCAN_MODE selection.

            Datatype: @c uns16 (@c #uns16)
            */
            PARAM_SCAN_LINE_DELAY = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 253),
            /**
            @brief Scan line time of the camera.

            This parameter is used to retrieve scan line time of camera. The time is reported
            in nanoseconds.

            Please note the parameter value should be retrieved after the call to
            #pl_exp_setup_seq or #pl_exp_setup_cont because the camera needs to know the
            actual acquisition configuration in order to calculate the value.

            Datatype: @c long64 (@c #long64)
            */
            PARAM_SCAN_LINE_TIME = ((CLASS3 << 16) + (TYPE_INT64 << 24) + 254),
            /**
            @brief Scan width in number of lines.

            This parameter is used to retrieve scan width of camera.

            Datatype: @c uns16 (@c #uns16)
            */
            PARAM_SCAN_WIDTH = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 255),
            /**
            @brief Virtual frame rotation mode.

            This parameter controls PVCAM-internal frame rotation module. When enabled,
            PVCAM will automatically rotate incoming frames based on the parameter setting.
            Please note that the frame rotation setting affects other parameters:
            #PARAM_SER_SIZE, #PARAM_PAR_SIZE, #PARAM_BINNING_SER, #PARAM_BINNING_PAR and
            #PARAM_COLOR_MODE. For example, if a 90-degree rotation is requested the affected
            parameters will return appropriately swapped or rotated values. Application should
            re-read the affected parameters after every change in #PARAM_HOST_FRAME_ROTATE.

            If #PARAM_METADATA_ENABLED is enabled, the frame-embedded metadata is also adjusted
            automatically if needed. This applies to the metadata-reported ROI and color mask.

            Legacy applications can set a system environment variable "PVCAM_FRAME_ROTATE_MODE" to
            one of the following values: 90, 180, 270 or 0. If set, sensors of all PVCAM cameras in the
            system will become virtually rotated and all sensor-related parameters will be
            reported appropriately rotated as well.

            A multi-threaded block-rotate algorithm is used for image rotation. The delay introduced
            by the algorithm depends on CPU performance. An Intel Xeon W-2123 processor can rotate
            a 2048x2048 16-bit image in approximately 1.5ms.

            This parameter can be used together with #PARAM_HOST_FRAME_FLIP. When both parameters
            are enabled, PVCAM uses an algorithm that processes the rotation/flipping combination
            using only one pass, keeping the performance withing 1-2ms on the same CPU.

            Datatype: @c enum (@c #int32)

            @see #PL_FRAME_ROTATE_MODES enum for all possible values.
            */
            PARAM_HOST_FRAME_ROTATE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 256),
            /** @warning Deprecated. Use the #PARAM_HOST_FRAME_ROTATE */
            [Obsolete("This parameter is deprecated. Use PARAM_HOST_FRAME_ROTATE")]
            PARAM_FRAME_ROTATE = (PARAM_HOST_FRAME_ROTATE),
            /**
            @brief Virtual frame flipping mode.

            This parameter controls PVCAM-internal frame flipping module. When enabled,
            PVCAM will automatically flip incoming frames based on the parameter setting.
            Please note that the frame flip setting affects the sensor reported color mask
            (#PARAM_COLOR_MODE). For example, if flip-X is requested, the sensor mask is
            also reported as flipped: an RGGB sensor mask would be reported as GRBG mask.

            If #PARAM_METADATA_ENABLED is enabled, the frame-embedded metadata is also adjusted
            automatically if needed. This applies to the metadata-reported color mask value.

            Legacy applications can set a system environment variable "PVCAM_FRAME_FLIP_MODE" to
            one of the following values: X, Y, XY or NONE. If set, sensors of all PVCAM cameras
            in the system will become virtually flipped and all sensor-related parameters will be
            reported appropriately flipped as well.

            A multi-threaded algorithm is used for image flipping. The delay introduced
            by the algorithm depends on CPU performance. An Intel Xeon W-2123 processor can flip
            a 2048x2048 16-bit image in approximately 1ms.

            This parameter can be used together with #PARAM_HOST_FRAME_ROTATE. When both parameters
            are enabled, PVCAM uses an algorithm that processes the rotation/flipping combination
            using only one pass, keeping the performance withing 1-2ms on the same CPU.

            Datatype: @c enum (@c #int32)

            @see #PL_FRAME_FLIP_MODES enum for all possible values.
            */
            PARAM_HOST_FRAME_FLIP = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 257),
            /** @warning Deprecated. Use the #PARAM_HOST_FRAME_FLIP */
            [Obsolete("This parameter is deprecated. Use PARAM_HOST_FRAME_FLIP")]
            PARAM_FRAME_FLIP = (PARAM_HOST_FRAME_FLIP),
            /**
            @brief This parameter is used to enable or disable the host frame summing feature.

            Use the #PARAM_HOST_FRAME_SUMMING_COUNT to set the number of frames to sum and the
            #PARAM_HOST_FRAME_SUMMING_FORMAT to discover and set the desired frame output
            format. After setting up the acquisition with #pl_exp_setup_seq or #pl_exp_setup_cont,
            check the #PARAM_BIT_DEPTH_HOST and #PARAM_IMAGE_FORMAT_HOST to discover the
            correct output frame format. This format may differ from the native #PARAM_BIT_DEPTH
            and #PARAM_IMAGE_FORMAT.

            By enabling this feature, PVCAM will start summing incoming frames and provide
            an output frame for every N-th frame (defined by #PARAM_HOST_FRAME_SUMMING_COUNT).
            Please note that when sequences are used (#pl_exp_setup_seq), the number of frames in
            sequence may be limited. When M-frames are requested in #pl_exp_setup_seq function and
            N-frames are configured for summing, PVCAM has to internally acquire a total of MxN
            frames to deliver correct amount of frames to the host. The camera hardware signals
            will also correspond to the MxN count because the frame summing is done on the host
            side without any knowledge of the camera.

            Data type: @c #rs_bool

            @see #PARAM_BIT_DEPTH_HOST, #PARAM_IMAGE_FORMAT_HOST
            */
            PARAM_HOST_FRAME_SUMMING_ENABLED = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 258),
            /**
            @brief This parameter is used to set the number of frames to sum.

            @see #PARAM_HOST_FRAME_SUMMING_ENABLED for details.

            Data type: @c #uns32
            */
            PARAM_HOST_FRAME_SUMMING_COUNT = ((CLASS2 << 16) + (TYPE_UNS32 << 24) + 259),
            /**
            @brief This parameter is used to set the desired output format for the summed frame.

            When host frame summing feature is enabled, this parameter can be used to set the
            desired output frame format. Depending on the average image intensity and dynamic
            range requirements, the output format can be widened as needed.
            For example, when summing 12-bit images, the output format may be set to 16-bit width,
            allowing to sum up to 16 saturated images into one. When wider dynamic range is required,
            the output format can be switched to 32-bit mode, allowing to sum up to 1M 12-bit frames
            without reaching the 32-bit saturation level.

            Data type: @c enum (@c #int32)

            @see #PARAM_HOST_FRAME_SUMMING_ENABLED and #PL_FRAME_SUMMING_FORMATS for details.
            */
            PARAM_HOST_FRAME_SUMMING_FORMAT = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 260),
            /**
            @brief This parameter is used to enable or disable the automated frame decompression
            feature.

            In general, applications do not have to support any of the compression-related parameters because
            decompression is automatically and seamlessly done by PVCAM. However, in specific cases the
            automatic decompression may be disabled. In such case, the application is expected to provide
            the frame decompression.

            Please note that when this parameter is disabled for cameras that require frame
            decompression, other *host* post processing parameters such as #PARAM_HOST_FRAME_ROTATE,
            #PARAM_HOST_FRAME_FLIP, #PARAM_HOST_FRAME_SUMMING_ENABLED and similar cannot be enabled because
            they require uncompressed pixels to work properly. The #pl_exp_setup_seq and #pl_exp_setup_cont
            functions will fail.

            @see #PARAM_IMAGE_COMPRESSION, #PARAM_IMAGE_COMPRESSION_HOST

            Data type: @c #rs_bool
            */
            PARAM_HOST_FRAME_DECOMPRESSION_ENABLED = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 261),
            /**
            @brief Gain setting for the current speed choice.

            The valid range for a gain setting is reported via #ATTR_MIN and #ATTR_MAX,
            where the min. gain is usually 1 and the max. gain may be as high as 16. Values
            outside this range will be ignored. Note that gain setting may not be linear!
            Values 1-16 may not correspond to 1x-16x, and there are gaps between the
            values. However, when the camera is initialized and every time a new speed is
            selected, the system will always reset to run at a gain of 1x.

            After setting the parameter, the #PARAM_GAIN_NAME can be used to retrieve user-
            friendly description of the selected gain (if supported by camera).

            Datatype: @c #int16

            @see @ref SpeedTable
            */
            PARAM_GAIN_INDEX = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 512),
            /**
            @brief Selects the sensor readout speed from a table of available choices.

            Entries are 0-based and the range of possible values is 0 to @c max_entries,
            where @c max_entries can be determined using #ATTR_MAX attribute. This setting
            relates to other speed table values, including #PARAM_BIT_DEPTH,
            #PARAM_PIX_TIME, #PARAM_READOUT_PORT, #PARAM_GAIN_INDEX and #PARAM_GAIN_NAME.
            After setting #PARAM_SPDTAB_INDEX, the gain setting is always reset to a value
            corresponding to 1x gain. To use a different gain setting, call #pl_set_param
            with #PARAM_GAIN_INDEX after setting the speed table index.

            After setting the parameter the #PARAM_SPDTAB_NAME can be used to retrieve user
            friendly description of the selected speed (if supported by camera).

            Datatype: @c #int16

            @see @ref SpeedTable
            @note The availability is camera-dependent.
            */
            PARAM_SPDTAB_INDEX = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 513),
            /**
            @brief Name of the currently selected Gain (via #PARAM_GAIN_INDEX).

            Use #ATTR_AVAIL to check for the availability. The gain name has a maximum
            length of #MAX_GAIN_NAME_LEN and can be retrieved with the #ATTR_CURRENT
            attribute.

            Datatype: @c char*

            @see @ref SpeedTable
            @note The availability is camera-dependent.
            */
            PARAM_GAIN_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 514),
            /**
            @brief Name of the currently selected Speed (via #PARAM_SPDTAB_INDEX).

            Use #ATTR_AVAIL to check for the availability. The speed name has a maximum
            length of #MAX_SPDTAB_NAME_LEN and can be retrieved with the #ATTR_CURRENT
            attribute.

            Data type: @c char*

            @see @ref SpeedTable
            @note The availability is camera-dependent.
            */
            PARAM_SPDTAB_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 515),
            /**
            @brief Sensor readout port being used by the currently selected speed.

            Different readout ports (used for alternate speeds) flip the image in serial,
            parallel, or both.

            Datatype: @c enum (@c #int32)

            @see @ref SpeedTable
            @note The availability is camera-dependent.
            */
            PARAM_READOUT_PORT = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 247),
            /**
            @brief This is the actual speed for the currently selected speed choice.

            It returns the time for each pixel conversion, in nanoseconds. This value may
            change as other speed choices are selected.

            Datatype: @c #uns16

            @see @ref SpeedTable
            @note The availability is camera-dependent.
            */
            PARAM_PIX_TIME = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 516),

            /* SHUTTER PARAMETERS */

            /**
            @brief The shutter close delay.

            This is the number of milliseconds required for the shutter to close. Since physical
            shutters are no longer shipped or used with modern Teledyne Photometrics cameras, this
            signal can be used for controlling other devices such as light sources.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_SHTR_CLOSE_DELAY = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 519),
            /**
            @brief The shutter open delay.

            This is the number of milliseconds required for the shutter to open. Since physical
            shutters are no longer shipped or used with modern Teledyne Photometrics cameras, this
            signal can be used for controlling other devices such as light sources.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_SHTR_OPEN_DELAY = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 520),
            /**
            @brief The shutter opening condition.

            See #PL_SHTR_OPEN_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @see @ref ExposureLoops
            @note The availability is camera-dependent.
            */
            PARAM_SHTR_OPEN_MODE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 521),
            /**
            @brief The state of the camera shutter.

            This is a legacy parameter not used with modern Teledyne Photometrics cameras.
            If the shutter is run too fast, it will overheat and trigger #SHTR_FAULT. The
            shutter electronics will disconnect until the temperature returns to a suitable
            range. Note that although the electronics have reset the voltages to open or
            close the shutter, there is a lag time for the physical mechanism to respond.
            See also #PARAM_SHTR_OPEN_DELAY and #PARAM_SHTR_CLOSE_DELAY.
            See #PL_SHTR_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_SHTR_STATUS = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 522),

            /* I/O PARAMETERS */

            /**
            @brief Sets and gets the currently active I/O address.

            The number of available I/O addresses can be obtained using the #ATTR_COUNT.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_IO_ADDR = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 527),
            /**
            @brief Gets the type of I/O available at the current address.

            See #PL_IO_TYPE enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_IO_TYPE = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 528),
            /**
            @brief Gets the direction of the signal at the current address.

            See #PL_IO_DIRECTION enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_IO_DIRECTION = ((CLASS2 << 16) + (TYPE_ENUM << 24) + 529),
            /**
            @brief Sets and gets the state of the currently active I/O signal.

            The new or return value when setting or getting the value respectively has 
            different meanings depending on the I/O type:
            - #IO_TYPE_TTL - A bit pattern, indicating the current state (0 or 1) of each of
              the control lines (bit 0 indicates line 0 state, etc.).
            - #IO_TYPE_DAC - The value of the desired analog output (only applies to
              #pl_set_param).

            The minimum and maximum range for the signal can be obtained using the #ATTR_MIN
            and #ATTR_MAX attributes, respectively.

            When outputting signals, the state is the desired output. For example, when
            setting the output of a 12-bit DAC with a range of 0-5V to half-scale, the state
            should be 2.5 (volts), not 1024 (bits).

            Datatype: @c #flt64

            @note The availability is camera-dependent.
            */
            PARAM_IO_STATE = ((CLASS2 << 16) + (TYPE_FLT64 << 24) + 530),
            /**
            @brief Gets the bit depth for the signal at the current address.

            The bit depth has different meanings, depending on the I/O Type:
            - #IO_TYPE_TTL - The number of bits read or written at this address.
            - #IO_TYPE_DAC - The number of bits written to the DAC.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_IO_BITDEPTH = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 531),

            /* GAIN MULTIPLIER PARAMETERS */

            /**
            @brief Gain multiplication factor for cameras with multiplication gain
            functionality.

            The valid range is reported via #ATTR_MIN and #ATTR_MAX.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_GAIN_MULT_FACTOR = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 537),
            /**
            @brief Gain multiplier on/off indicator for cameras with the multiplication gain
            functionality.

            This parameter is read-only and its value is always @c TRUE if it is available on the 
            camera.

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            */
            PARAM_GAIN_MULT_ENABLE = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 541),

            /* POST PROCESSING PARAMETERS */

            /**
            @brief This returns the name of the currently selected post-processing feature.

            The name is a null-terminated text string. User is responsible for buffer
            allocation with at least #MAX_PP_NAME_LEN bytes.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_PP_FEAT_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 542),
            /**
            @brief This selects the current post-processing feature from a table of
            available choices.

            The entries are 0-based and the range of possible values is 0 to @c max_entries.
            @c max_entries can be determined with the #ATTR_MAX attribute. This setting
            relates to other post-processing table values, including #PARAM_PP_FEAT_NAME,
            #PARAM_PP_FEAT_ID and #PARAM_PP_PARAM_INDEX.

            Datatype: @c #int16

            @note The availability is camera-dependent.
            */
            PARAM_PP_INDEX = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 543),
            /**
            @brief Gets the actual e/ADU for the current gain setting.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_ACTUAL_GAIN = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 544),
            /**
            @brief This selects the current post-processing parameter from a table of
            available choices.

            The entries are 0-based and the range of possible values is 0 to @c max_entries.
            @c max_entries can be determined with the #ATTR_MAX attribute. This setting
            relates to other post-processing table values, including #PARAM_PP_PARAM_NAME,
            #PARAM_PP_PARAM_ID and #PARAM_PP_PARAM.

            Datatype: @c #int16

            @note The availability is camera-dependent.
            */
            PARAM_PP_PARAM_INDEX = ((CLASS2 << 16) + (TYPE_INT16 << 24) + 545),
            /**
            @brief Gets the name of the currently selected post-processing parameter for the
            currently selected post-processing feature.

            The name is a null-terminated text string. User is responsible for buffer
            allocation with at least #MAX_PP_NAME_LEN bytes.

            Datatype: @c char*

            @note The availability is camera-dependent.
            */
            PARAM_PP_PARAM_NAME = ((CLASS2 << 16) + (TYPE_CHAR_PTR << 24) + 546),
            /**
            @brief This gets or sets the post-processing parameter for the currently selected
            post-processing parameter at the index.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            */
            PARAM_PP_PARAM = ((CLASS2 << 16) + (TYPE_UNS32 << 24) + 547),
            /**
            @brief Gets the read noise for the current speed.

            Datatype: @c #uns16

            @note The availability is camera-dependent.
            */
            PARAM_READ_NOISE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 548),
            /**
            @brief This returns the ID of the currently-selected post-processing feature.

            This maps a specific post-processing module across cameras to help applications
            filter for camera features they need to expose and those that they don't. It
            helps to identify similarities between camera post-processing features.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            @note The parameter type is incorrectly defined. The actual type is TYPE_UNS32.
            */
            PARAM_PP_FEAT_ID = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 549),
            /**
            @brief This returns the ID of the currently selected post-processing parameter.

            This maps a specific post-processing parameter across cameras to help
            applications filter for camera features they need to expose and those that they
            don't. It helps to identify similarities between camera post-processing features.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            @note The parameter type is incorrectly defined. The actual type is TYPE_UNS32.
            */
            PARAM_PP_PARAM_ID = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 550),

            /* S.M.A.R.T. STREAMING PARAMETERS */

            /**
            @brief This parameter allows the user to retrieve or set the state of the
            S.M.A.R.T. streaming mode.

            When a @c TRUE is returned by the camera, S.M.A.R.T. streaming is enabled.

            Datatype: @c #rs_bool

            @see @ref SmartStreaming
            @note The availability is camera-dependent.
            */
            PARAM_SMART_STREAM_MODE_ENABLED = ((CLASS2 << 16) + (TYPE_BOOLEAN << 24) + 700),
            /**
            @brief This parameter allows the user to select between available S.M.A.R.T.
            streaming modes.

            See #PL_SMT_MODES enum for all possible values.
            Currently the only available mode is #SMTMODE_ARBITRARY_ALL.

            Datatype: @c #uns16

            @see @ref SmartStreaming
            @note The availability is camera-dependent.
            */
            PARAM_SMART_STREAM_MODE = ((CLASS2 << 16) + (TYPE_UNS16 << 24) + 701),
            /**
            @brief This parameter allows to set or read the current exposure parameters for
            S.M.A.R.T. streaming.

            Datatype: @c #smart_stream_type*

            @see @ref SmartStreaming
            @note The availability is camera-dependent.
            @note The parameter type is incorrectly defined. The actual type is TYPE_SMART_STREAM_TYPE_PTR.
            */
            PARAM_SMART_STREAM_EXP_PARAMS = ((CLASS2 << 16) + (TYPE_VOID_PTR << 24) + 702),
            /**
            @brief This parameter allows to set or read the current delays between exposures
            in S.M.A.R.T. streaming.

            Datatype: @c #smart_stream_type*

            @see @ref SmartStreaming
            @note The availability is camera-dependent.
            @note This parameter is currently not supported and #ATTR_AVAIL always returns
            @c FALSE.
            @note The parameter type is incorrectly defined. The actual type is TYPE_SMART_STREAM_TYPE_PTR.
            */
            PARAM_SMART_STREAM_DLY_PARAMS = ((CLASS2 << 16) + (TYPE_VOID_PTR << 24) + 703),

            /* ACQUISITION PARAMETERS */

            /**
            @brief Used to examine and change the exposure time in VTM only.

            VTM is active if exposure mode is set to #VARIABLE_TIMED_MODE. This value is
            limited to 16-bit. For higher exposure times separate single frame acquisitions,
            or SMART streaming (if available), have to be used.

            Datatype: @c #uns16
            */
            PARAM_EXP_TIME = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 1),
            /**
            @brief Gets the exposure resolution for the current resolution index.

            This parameter does exactly the same as #PARAM_EXP_RES_INDEX, but additionally
            provides human-readable string for each exposure resolution.

            For some older cameras this parameter might not be available (#ATTR_AVAIL
            returns @c FALSE). In this case camera uses #EXP_RES_ONE_MILLISEC resolution.

            Datatype: @c enum (@c #int32)
            */
            PARAM_EXP_RES = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 2),
            /**
            @brief Gets and sets the index into the exposure resolution table.

            The table contains the resolutions supported by the camera. The value at this
            index is an enumerated type, representing different resolutions. The number of
            supported resolutions can be obtained using the #ATTR_COUNT attribute.
            See #PL_EXP_RES_MODES enum for all possible values.

            Datatype: @c #uns16
            */
            PARAM_EXP_RES_INDEX = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 4),
            /**
            @brief Used to examine current exposure time and range of valid values.

            The minimum and maximum value could be limited by camera hardware. Use #ATTR_MIN
            and #ATTR_MAX to retrieve it. This parameter is always available, but for older
            cameras not reporting their limits, the min. value is set to 0 and max. value is
            set to max. 32-bit range for backward compatibility. This means the range is not
            known, which does not rule out exposure limits. In such case limits may be specified
            e.g. in camera manual. Please note the reported value unit depends on
            currently selected exposure resolution (#PARAM_EXP_RES).

            Datatype: @c #ulong64
            */
            PARAM_EXPOSURE_TIME = ((CLASS3 << 16) + (TYPE_UNS64 << 24) + 8),

            /* PARAMETERS FOR  BEGIN and END of FRAME Interrupts */

            /**
            @brief Enables and configures the BOF/EOF interrupts.

            See #PL_IRQ_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            @warning This parameter is deprecated and should not be accessed. Modern cameras
            use callback acquisition where BOF/EOF is controlled by the hardware.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_BOF_EOF_ENABLE = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 5),
            /**
            @brief Returns the Begin-Of-Frame and/or End-Of-Frame count.

            BOF/EOF counting is enabled and configured with #PARAM_BOF_EOF_ENABLE.

            Datatype: @c #uns32

            @note The availability is camera-dependent.
            @warning This parameter is deprecated and should not be accessed. Modern cameras
            use callback acquisition where BOF/EOF is controlled by the hardware.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_BOF_EOF_COUNT = ((CLASS3 << 16) + (TYPE_UNS32 << 24) + 6),
            /**
            @brief Clears the BOF/EOF count when a #pl_set_param is performed.

            This is a write-only parameter.

            Datatype: @c #rs_bool

            @note The availability is camera-dependent.
            @warning This parameter is deprecated and should not be accessed. Modern cameras
            use callback acquisition where BOF/EOF is controlled by the hardware.
            */
            [Obsolete("This parameter is deprecated.")]
            PARAM_BOF_EOF_CLR = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 7),

            /**
            @brief Tests to see if the hardware/software can perform circular buffer.

            When a @c TRUE is returned for #ATTR_AVAIL attribute, the circular buffer
            function can be used.

            Datatype: @c #rs_bool
            */
            PARAM_CIRC_BUFFER = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 299),
            /**
            @brief Retrieves the min, max, current and recommended (default) buffer size in
            bytes.

            Applies to currently configured acquisition. This parameter becomes
            available only after calling the #pl_exp_setup_seq or #pl_exp_setup_cont.
            For sequence acquisition, the attribute always reports the full sequence size in
            bytes. For circular buffer acquisition, use the #ATTR_DEFAULT to retrieve the
            recommended buffer size.

            Datatype: @c #ulong64
            */
            PARAM_FRAME_BUFFER_SIZE = ((CLASS3 << 16) + (TYPE_UNS64 << 24) + 300),

            /* Binning reported by camera */

            /**
            @brief Used to obtain serial part of serial x parallel binning factors
            permutations.

            It has to be always used in pair with #PARAM_BINNING_PAR parameter.

            Datatype: @c enum (@c #int32)

            @see @ref BinningDiscovery
            @note The availability is camera-dependent.
            */
            PARAM_BINNING_SER = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 165),
            /**
            @brief Used to obtain parallel part of serial x parallel binning factors
            permutations.

            It has to be always used in pair with #PARAM_BINNING_SER parameter.

            Datatype: @c enum (@c #int32)

            @see @ref BinningDiscovery
            @note The availability is camera-dependent.
            */
            PARAM_BINNING_PAR = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 166),

            /* Parameters related to multiple ROIs and Centroids */

            /**
            @brief Parameter used to enable or disable the embedded frame metadata feature.

            Once enabled, the acquired frames will contain additional information describing
            the frame.

            Datatype: @c #rs_bool

            @see @ref EmbeddedFrameMetadata
            @note The availability is camera-dependent.
            */
            PARAM_METADATA_ENABLED = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 168),
            /**
            @brief Resets the camera-generated metadata timestamp.

            In normal operation, the timestamp is reset upon camera power up. Use this parameter
            to reset the timestamp when needed. This is a write-only paremeter, use #pl_set_param
            with @c TRUE value to reset the timestamp.

            Data type: @c #rs_bool

            @see @ref EmbeddedFrameMetadata
            @note The availability is camera-dependent.
            */
            PARAM_METADATA_RESET_TIMESTAMP = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 176),
            /**
            @brief The number of currently configured ROIs.

            The #ATTR_CURRENT returns the currently configured number of ROIs set via
            #pl_exp_setup_seq or #pl_exp_setup_cont functions. The #ATTR_MAX can be used to
            retrieve the maximum number of ROIs the camera supports.

            Datatype: @c #uns16
            */
            PARAM_ROI_COUNT = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 169),
            /**
            @brief Gets or sets the current ROI.

            This parameter returns the current ROI that was configured through the
            #pl_exp_setup_seq or #pl_exp_setup_cont functions. If multiple ROIs were
            configured, this parameter returns the first one.
            If the camera supports live ROI feature, the parameter is used to send the live
            ROI to the camera.

            Data type: @c #rgn_type
            */
            PARAM_ROI = ((CLASS3 << 16) + (TYPE_RGN_TYPE << 24) + 1),
            /**
            @brief This parameter is used to enable or disable the Centroids/Object detection
            feature.

            Use the #PARAM_CENTROIDS_MODE to retrieve a list of camera supported
            object detection modes.

            Datatype: @c #rs_bool

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_ENABLED = ((CLASS3 << 16) + (TYPE_BOOLEAN << 24) + 170),
            /**
            @brief This parameter is used to set the Centroid radius.

            This read-write parameter is used to obtain the range of Centroids radii the
            camera supports. Use the #ATTR_MIN and #ATTR_MAX to retrieve the range.

            The radius defines the distance from the center pixel. For example, if the camera
            reports the radius range between 1 and 5, it means that the resulting ROIs can be
            configured to the following sizes: 1=3x3, 2=5x5, 3=7x7, 4=9x9, 5=11x11.

            The parameter interpretation also depends on the selected Centroid/Object detection
            mode. In some modes, the radius is used to set the size of the 'object' to be detected.

            Use #pl_set_param to set the desired Centroids radius. Once set, make sure to
            reconfigure the acquisition with #pl_exp_setup_seq or #pl_exp_setup_cont
            functions.

            Datatype: @c #uns16

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_RADIUS = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 171),
            /**
            @brief This parameter is used to control the number of Centroids.

            This read-write parameter is used to obtain the maximum supported number of
            Centroids and set the desired number of Centroids to the camera.

            The camera usually supports a limited number of Centroids/objects to be detected.
            Use this parameter to limit the number of objects to be detected. For example, in
            particle tracking mode, the parameter will limit number of particles to be tracked
            - if the camera finds more particles than the current limit, the remaining particles
            will be ignored.

            Datatype: @c #uns16

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_COUNT = ((CLASS3 << 16) + (TYPE_UNS16 << 24) + 172),
            /**
            @brief This parameter is used to retrieve and control the camera Centroid/Object
            detection modes.

            In case the camera supports centroids, but reports this parameter as not
            available, only the #PL_CENTROIDS_MODE_LOCATE feature is supported.
            See #PL_CENTROIDS_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_MODE = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 173),
            /**
            @brief Supported Centroids background frame processing count.

            This allows the camera to report supported selections for number of frames for
            dynamic background removal. The processing is optimized only for the selected set
            of frames. Thus there is no related enumeration type with supported values for this
            parameter.

            Datatype: @c enum (@c #int32)

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_BG_COUNT = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 174),
            /**
            @brief Used to configure threshold multiplier for specific object detection modes.

            For 'particle tracking' mode, the value is a fixed-point real number in Q8.4 format.
            E.g. the value 1234 (0x4D2) means 77.2 (0x4D hex = 77 dec).

            Datatype: @c #uns32

            @see @ref Centroids
            @note The availability is camera-dependent.
            */
            PARAM_CENTROIDS_THRESHOLD = ((CLASS3 << 16) + (TYPE_UNS32 << 24) + 175),

            /* Parameters related to triggering table */

            /**
            @brief Used to select the output signal to be configured.

            The configuration of number of active outputs is done via
            #PARAM_LAST_MUXED_SIGNAL parameter.
            See #PL_TRIGTAB_SIGNALS enum for all possible values.

            Datatype: @c enum (@c #int32)

            @see @ref TriggeringTable
            @note The availability is camera-dependent.
            */
            PARAM_TRIGTAB_SIGNAL = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 180),
            /**
            @brief Used to set the number of active output signals.

            The set number of signals is used by multiplexer for the signal selected
            by #PARAM_TRIGTAB_SIGNAL parameter.

            Datatype: @c #uns8

            @see @ref TriggeringTable
            @note The availability is camera-dependent.
            */
            PARAM_LAST_MUXED_SIGNAL = ((CLASS3 << 16) + (TYPE_UNS8 << 24) + 181),
            /**
            @brief Deals with frame delivery modes.

            This parameter allows to switch among various frame delivery modes.
            If not available, camera runs in #PL_FRAME_DELIVERY_MODE_MAX_FPS mode.
            See #PL_FRAME_DELIVERY_MODES enum for all possible values.

            Datatype: @c enum (@c #int32)

            @note The availability is camera-dependent.
            */
            PARAM_FRAME_DELIVERY_MODE = ((CLASS3 << 16) + (TYPE_ENUM << 24) + 400),
        }

        /******************************************************************************/
        /* End of parameter ID definitions.                                           */
        /******************************************************************************/
    }
}
