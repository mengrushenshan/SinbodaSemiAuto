using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    internal static class PVCAM64
    {

        #region PVCAM CLASS 0

        [DllImport("Pvcam64.dll", EntryPoint = "pl_pvcam_init",
        ExactSpelling = false)]
        public static extern bool pl_pvcam_init();

        [DllImport("Pvcam64.dll", EntryPoint = "pl_pvcam_uninit",
        ExactSpelling = false)]
        public static extern bool pl_pvcam_uninit();

        [DllImport("Pvcam64.dll", EntryPoint = "pl_pvcam_get_ver",
        ExactSpelling = false)]
        public static extern bool pl_pvcam_get_ver(out ushort version);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_close",
        ExactSpelling = false)]
        public static extern bool pl_cam_close(short hCam);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_get_name", CharSet = CharSet.Ansi,
        ExactSpelling = false)]
        public static extern bool pl_cam_get_name(short cameraNumber, StringBuilder cameraName);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_get_total",
        ExactSpelling = false)]
        public static extern bool pl_cam_get_total(out short hCam);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_open",
        ExactSpelling = false)]
        public static extern bool pl_cam_open(StringBuilder cameraName, out short hCam,
                                              PVCAM.PL_OPEN_MODES o_mode);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_register_callback_ex3",
        ExactSpelling = false)]
        public static extern bool pl_cam_register_callback_ex3(short hCam, PVCAM.PL_CALLBACK_EVENT callBackEvent,
                                                               PVCAM.PMCallBack_Ex3 callback, IntPtr context);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_cam_deregister_callback",
        ExactSpelling = false)]
        public static extern bool pl_cam_deregister_callback(short hCam, PVCAM.PL_CALLBACK_EVENT callBackEvent);

        #endregion

        #region PVCAM CLASS 1

        [DllImport("Pvcam64.dll", EntryPoint = "pl_error_code",
        ExactSpelling = false)]
        public static extern short pl_error_code();

        [DllImport("Pvcam64.dll", EntryPoint = "pl_error_message",
        ExactSpelling = false)]
        public static extern bool pl_error_message(short err_code, StringBuilder msg);

        #endregion

        #region PVCAM CLASS 2

        [DllImport("Pvcam64.dll", EntryPoint = "pl_get_param",
        ExactSpelling = false)]
        public static extern bool pl_get_param(short hcam, uint param_id,
                                               short param_attrib, IntPtr param_value);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_set_param",
        ExactSpelling = false)]
        public static extern bool pl_set_param(short hcam, uint param_id,
                                               IntPtr param_value);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_get_enum_param",
        ExactSpelling = false)]
        public static extern bool pl_get_enum_param(short hcam, uint param_id,
                                                    uint index, out int value,
                                                    StringBuilder desc, uint length);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_enum_str_length",
        ExactSpelling = false)]
        public static extern bool pl_enum_str_length(short hcam, uint param_id,
                                                     uint index, out uint length);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_pp_reset",
        ExactSpelling = false)]
        public static extern bool pl_pp_reset(short hcam);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_create_smart_stream_struct",
        ExactSpelling = false)]
        public static extern bool pl_create_smart_stream_struct(out PVCAM.smart_stream_type smtStreamStruct,
                                                                ushort entries);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_release_smart_stream_struct",
        ExactSpelling = false)]
        public static extern bool pl_release_smart_stream_struct(out PVCAM.smart_stream_type smtStreamStruct);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_create_frame_info_struct",
        ExactSpelling = false)]
        public static extern bool pl_create_frame_info_struct(out PVCAM.FRAME_INFO new_frame);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_release_frame_info_struct",
        ExactSpelling = false)]
        public static extern bool pl_release_frame_info_struct(PVCAM.FRAME_INFO frame_to_delete);

        #endregion

        #region PVCAM CLASS 3

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_setup_seq",
        ExactSpelling = false)]
        public static extern bool pl_exp_setup_seq(short hcam, ushort exp_total,
                                                   ushort rgn_total, ref PVCAM.rgn_type rgn_array,
                                                   short mode, uint exposure_time,
                                                   out uint stream_size);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_start_seq",
        ExactSpelling = false)]
        public static extern bool pl_exp_start_seq(short hcam, IntPtr pixel_stream_ptr);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_finish_seq",
        ExactSpelling = false)]
        public static extern bool pl_exp_finish_seq(short hcam, IntPtr pixel_stream_ptr, short hbuf);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_abort",
        ExactSpelling = false)]
        public static extern bool pl_exp_abort(short hcam, short cam_state);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_start_cont",
        ExactSpelling = false)]
        public static extern bool pl_exp_start_cont(short hcam, IntPtr pixel_stream, uint size);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_setup_cont",
        ExactSpelling = false)]
        public static extern bool pl_exp_setup_cont(short hcam, ushort rgn_total,
                                                    ref PVCAM.rgn_type rgn_array,
                                                    short mode, uint exposure_time,
                                                    out uint stream_size, short circ_mode);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_trigger",
        ExactSpelling = false)]
        public static extern bool pl_exp_trigger(short hcam, ref uint flags, uint val);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_get_latest_frame",
        ExactSpelling = false)]
        public static extern bool pl_exp_get_latest_frame(short hcam, out IntPtr frame);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_get_latest_frame_ex",
        ExactSpelling = false)]
        public static extern bool pl_exp_get_latest_frame_ex(short hcam, out IntPtr frame,
                                                             out PVCAM.FRAME_INFO pFrameInfo);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_get_oldest_frame",
        ExactSpelling = false)]
        public static extern bool pl_exp_get_oldest_frame(short hcam, out IntPtr frame);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_get_oldest_frame_ex",
        ExactSpelling = false)]
        public static extern bool pl_exp_get_oldest_frame_ex(short hcam, out IntPtr frame,
                                                             out PVCAM.FRAME_INFO pFrameInfo);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_stop_cont",
        ExactSpelling = false)]
        public static extern bool pl_exp_stop_cont(short hcam, short cam_state);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_exp_unlock_oldest_frame",
        ExactSpelling = false)]
        public static extern bool pl_exp_unlock_oldest_frame(short hcam);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_md_frame_decode",
        ExactSpelling = false)]
        public static extern bool pl_md_frame_decode(IntPtr pDstFrame, IntPtr pSrcBuf, uint srcBufSize);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_md_frame_recompose",
        ExactSpelling = false)]
        public static extern bool pl_md_frame_recompose(IntPtr pDstBuf, ushort offX, ushort offY,
                                                        ushort dstWidth, ushort dstHeight,
                                                        ref PVCAM.md_frame pSrcFrame);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_md_create_frame_struct_cont",
        ExactSpelling = false)]
        public static extern bool pl_md_create_frame_struct_cont(ref IntPtr pFrame, ushort roiCount);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_md_create_frame_struct",
        ExactSpelling = false)]
        public static extern bool pl_md_create_frame_struct(ref IntPtr pFrame, IntPtr pSrcBuf, uint srcBufSize);

        [DllImport("Pvcam64.dll", EntryPoint = "pl_md_release_frame_struct",
        ExactSpelling = false)]
        public static extern bool pl_md_release_frame_struct(IntPtr pFrame);

        #endregion

    }
}
