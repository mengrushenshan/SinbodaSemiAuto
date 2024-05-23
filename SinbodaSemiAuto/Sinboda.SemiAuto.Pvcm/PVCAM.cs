using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    /// <summary>
    /// Partial class containing the PVCAM API exports.
    /// See also PVCAMTypes.cs
    /// </summary>
    public static partial class PVCAM
    {
        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="pvcam_version"></param>
        /// <returns></returns>
        public static bool pl_pvcam_get_ver(out ushort pvcam_version)
        {
            return PVCAM64.pl_pvcam_get_ver(out pvcam_version);
        }

        /// <summary>
        /// 初始化相机模块
        /// </summary>
        /// <returns></returns>
        public static bool pl_pvcam_init()
        {
            return PVCAM64.pl_pvcam_init();
        }

        /// <summary>
        /// 释放相机模块
        /// </summary>
        /// <returns></returns>
        public static bool pl_pvcam_uninit()
        {
            return PVCAM64.pl_pvcam_uninit();
        }

        /// <summary>
        /// 关闭相机
        /// </summary>
        /// <param name="hcam"></param>
        /// <returns></returns>
        public static bool pl_cam_close(short hcam)
        {
            return PVCAM64.pl_cam_close(hcam);
        }

        /// <summary>
        /// 获取指定id的相机名称
        /// </summary>
        /// <param name="cam_num"></param>
        /// <param name="camera_name"></param>
        /// <returns></returns>
        public static bool pl_cam_get_name(short cam_num, StringBuilder camera_name)
        {
            return PVCAM64.pl_cam_get_name(cam_num, camera_name);
        }

        /// <summary>
        /// 获取所有相机
        /// </summary>
        /// <param name="totl_cams"></param>
        /// <returns></returns>
        public static bool pl_cam_get_total(out short totl_cams)
        {
            return PVCAM64.pl_cam_get_total(out totl_cams);
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        /// <param name="camera_name"></param>
        /// <param name="hcam"></param>
        /// <param name="o_mode"></param>
        /// <returns></returns>
        public static bool pl_cam_open(StringBuilder camera_name, out short hcam, PVCAM.PL_OPEN_MODES o_mode)
        {
            return PVCAM64.pl_cam_open(camera_name, out hcam, o_mode);
        }

        /// <summary>
        /// 相机事件回调设置
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="callback_event"></param>
        /// <param name="callback"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool pl_cam_register_callback_ex3(short hcam, PVCAM.PL_CALLBACK_EVENT callback_event,
                                                        PVCAM.PMCallBack_Ex3 callback, IntPtr context)
        {
            return PVCAM64.pl_cam_register_callback_ex3(hcam, callback_event, callback, context);
        }
        /// <summary>
        /// 回调事件取消
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="callback_event"></param>
        /// <returns></returns>
        public static bool pl_cam_deregister_callback(short hcam, PVCAM.PL_CALLBACK_EVENT callback_event)
        {
            return PVCAM64.pl_cam_deregister_callback(hcam, callback_event);
        }

        /// <summary>
        /// 获取错误代码
        /// </summary>
        /// <returns></returns>
        public static short pl_error_code()
        {
            return PVCAM64.pl_error_code();
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <param name="err_code"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool pl_error_message(short err_code, StringBuilder msg)
        {
            return PVCAM64.pl_error_message(err_code, msg);
        }

        /// <summary>
        /// 获取相机参数
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="param_id"></param>
        /// <param name="param_attribute"></param>
        /// <param name="param_value"></param>
        /// <returns></returns>
        public static bool pl_get_param(short hcam, uint param_id, short param_attribute, IntPtr param_value)
        {
            return PVCAM64.pl_get_param(hcam, param_id, param_attribute, param_value);
        }

        /// <summary>
        /// 设置相机参数
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="param_id"></param>
        /// <param name="param_value"></param>
        /// <returns></returns>
        public static bool pl_set_param(short hcam, uint param_id, IntPtr param_value)
        {
            return PVCAM64.pl_set_param(hcam, param_id, param_value);
        }

        /// <summary>
        /// 获取枚举成员
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="param_id"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="desc"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool pl_get_enum_param(short hcam, uint param_id, uint index, out int value,
                                             StringBuilder desc, uint length)
        {
            return PVCAM64.pl_get_enum_param(hcam, param_id, index, out value, desc, length);
        }

        /// <summary>
        /// 枚举字符串长度
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="param_id"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool pl_enum_str_length(short hcam, uint param_id, uint index, out uint length)
        {
            return PVCAM64.pl_enum_str_length(hcam, param_id, index, out length);
        }

        /// <summary>
        /// 相机复位
        /// </summary>
        /// <param name="hcam"></param>
        /// <returns></returns>
        public static bool pl_pp_reset(short hcam)
        {
            return PVCAM64.pl_pp_reset(hcam);
        }

        /**
        @brief Creates and allocates variable of smart_stream_type type with the number of
        entries passed in via the entries parameter and returns pointer to it.

        This function will create a variable of smart_stream_type type and return a pointer
        to access it. The entries parameter passed by the user determines how many entries
        the structure will contain.

        @param[out] array   Created struct to be returned.
        @param[in]  entries Number of entries to be in smart stream struct.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @see pl_release_smart_stream_struct
        */
        public static bool pl_create_smart_stream_struct(out PVCAM.smart_stream_type array, ushort entries)
        {
            return PVCAM64.pl_create_smart_stream_struct(out array, entries);
        }

        /**
        @brief Frees the space previously allocated by the #pl_create_smart_stream_struct function.

        This function will deallocate a smart_stream_type variable created by
        #pl_create_smart_stream_struct.

        @param[in]  array   Struct to be released.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @see pl_create_smart_stream_struct
        */
        public static bool pl_release_smart_stream_struct(out PVCAM.smart_stream_type array)
        {
            return PVCAM64.pl_release_smart_stream_struct(out array);
        }

        /**
        @brief Creates and allocates variable of #FRAME_INFO type and returns pointer to it.

        This function will create a variable of #FRAME_INFO type and
        return a pointer to access it. The GUID field of the #FRAME_INFO structure is assigned
        by this function. Other fields are updated by PVCAM at the time of frame reception.

        @param[out] new_frame   Frame info struct to be returned.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @see pl_release_frame_info_struct, pl_exp_get_latest_frame_ex, pl_exp_get_oldest_frame_ex,
        pl_exp_check_cont_status_ex, pl_cam_register_callback_ex2, pl_cam_register_callback_ex3
        */
        public static bool pl_create_frame_info_struct(out PVCAM.FRAME_INFO new_frame)
        {
            return PVCAM64.pl_create_frame_info_struct(out new_frame);
        }

        /// <summary>
        /// 设置序列采集
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="exp_total"></param>
        /// <param name="rgn_total"></param>
        /// <param name="rgn_array"></param>
        /// <param name="mode"></param>
        /// <param name="exposure_time"></param>
        /// <param name="stream_size"></param>
        /// <returns></returns>
        public static bool pl_exp_setup_seq(short hcam, ushort exp_total, ushort rgn_total,
                                            ref PVCAM.rgn_type rgn_array, short mode,
                                            uint exposure_time, out uint stream_size)
        {
            return PVCAM64.pl_exp_setup_seq(hcam, exp_total, rgn_total, ref rgn_array,
                                            mode, exposure_time, out stream_size);
        }

        /// <summary>
        /// 开始序列采集
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="pixel_stream"></param>
        /// <returns></returns>
        public static bool pl_exp_start_seq(short hcam, IntPtr pixel_stream)
        {
            return PVCAM64.pl_exp_start_seq(hcam, pixel_stream);
        }

        /// <summary>
        /// 序列采集后内部处理
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="pixel_stream"></param>
        /// <param name="hbuf"></param>
        /// <returns></returns>
        public static bool pl_exp_finish_seq(short hcam, IntPtr pixel_stream, short hbuf)
        {
            return PVCAM64.pl_exp_finish_seq(hcam, pixel_stream, hbuf);
        }

        /// <summary>
        /// 停止采集数据
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="cam_state"></param>
        /// <returns></returns>
        public static bool pl_exp_abort(short hcam, short cam_state)
        {
            return PVCAM64.pl_exp_abort(hcam, cam_state);
        }

        /// <summary>
        /// 开始连续采集
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="pixel_stream"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static bool pl_exp_start_cont(short hcam, IntPtr pixel_stream, uint size)
        {
            return PVCAM64.pl_exp_start_cont(hcam, pixel_stream, size);
        }

        /// <summary>
        /// 设置连续采集
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="rgn_total"></param>
        /// <param name="rgn_array"></param>
        /// <param name="exp_mode"></param>
        /// <param name="exposure_time"></param>
        /// <param name="exp_bytes"></param>
        /// <param name="buffer_mode"></param>
        /// <returns></returns>
        public static bool pl_exp_setup_cont(short hcam, ushort rgn_total, ref PVCAM.rgn_type rgn_array,
                                             short exp_mode, uint exposure_time, out uint exp_bytes,
                                             short buffer_mode)
        {
            return PVCAM64.pl_exp_setup_cont(hcam, rgn_total, ref rgn_array, exp_mode, exposure_time,
                                             out exp_bytes, buffer_mode);
        }
        /**
        @brief Sends a software trigger to the device.

        In order to use the software trigger, following preconditions must be met:
                     - Acquisition has been configured via a call to #pl_exp_setup_seq or #pl_exp_setup_cont
                       using one of the software triggering modes (@see PL_EXPOSURE_MODES).
                     - Acquisition has been started via #pl_exp_start_seq or #pl_exp_start_cont.

        @param[in]     hcam  Camera handle returned from #pl_cam_open.
        @param[in,out] flags Input/output flags, see remarks.
        @param[in]     value Reserved for future use. This value should be set to 0.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @remarks In current implementation the input flags should be set to 0. On output,
        the flags will contain one of the values defined in #PL_SW_TRIG_STATUSES enumeration.

        @see pl_exp_setup_seq, pl_exp_start_seq, pl_exp_setup_cont, pl_exp_start_cont,
        pl_exp_check_cont_status, pl_exp_get_oldest_frame, pl_exp_get_latest_frame,
        pl_exp_unlock_oldest_frame, pl_exp_abort
        */
        public static bool pl_exp_trigger(short hcam, ref uint flags, uint value)
        {
            return PVCAM64.pl_exp_trigger(hcam, ref flags, value);
        }
        /**
        @brief This function returns a pointer to the most recently acquired frame in the circular buffer.

        @param[in]  hcam    Camera handle returned from #pl_cam_open.
        @param[out] frame   Pointer to the most recent frame.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @see pl_exp_setup_cont, pl_exp_start_cont, pl_exp_check_cont_status, pl_exp_stop_cont

        @note If the camera in use is not able to return the latest frame for the current operating mode,
        this function will fail. For example, some cameras cannot return the latest frame in
        #CIRC_NO_OVERWRITE mode. Use the parameter ID #PARAM_CIRC_BUFFER with #pl_get_param to
        see if the system can perform circular buffer operations.
        */
        public static bool pl_exp_get_latest_frame(short hcam, out IntPtr frame)
        {
            return PVCAM64.pl_exp_get_latest_frame(hcam, out frame);
        }

        /// <summary>
        /// 获取最新帧
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="frame"></param>
        /// <param name="frame_info"></param>
        /// <returns></returns>
        public static bool pl_exp_get_latest_frame_ex(short hcam, out IntPtr frame,
                                                      out PVCAM.FRAME_INFO frame_info)
        {
            return PVCAM64.pl_exp_get_latest_frame_ex(hcam, out frame, out frame_info);
        }
        /**
        @brief This function returns pointer to the oldest unretrieved frame in the circular buffer. 

        After calling this function, #pl_exp_unlock_oldest_frame has to be called to notify PVCAM that 
        the oldest frame can be overwritten with new data.\n
        This function should be used with acquisitions running in #CIRC_NO_OVERWRITE mode.

        @param[in]  hcam  Camera handle returned from #pl_cam_open.
        @param[out] frame Pointer to the oldest unretrieved frame.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.

        @see pl_exp_setup_cont, pl_exp_start_cont, pl_exp_check_cont_status,
        pl_exp_unlock_oldest_frame, pl_exp_stop_cont, pl_exp_get_oldest_frame_ex

        @note If the camera in use is not able to return the oldest frame for the current operating mode,
        this function will fail. For example, some cameras cannot return the oldest frame
        in #CIRC_OVERWRITE mode. Use the parameter ID #PARAM_CIRC_BUFFER with #pl_get_param
        to see if the system can perform circular buffer operations.
        */
        public static bool pl_exp_get_oldest_frame(short hcam, out IntPtr frame)
        {
            return PVCAM64.pl_exp_get_oldest_frame(hcam, out frame);
        }

        /// <summary>
        /// 获取最早帧
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="frame"></param>
        /// <param name="frame_info"></param>
        /// <returns></returns>
        public static bool pl_exp_get_oldest_frame_ex(short hcam, out IntPtr frame,
                                                      out PVCAM.FRAME_INFO frame_info)
        {
            return PVCAM64.pl_exp_get_oldest_frame_ex(hcam, out frame, out frame_info);
        }

        /// <summary>
        /// 停止连续采集
        /// </summary>
        /// <param name="hcam"></param>
        /// <param name="cam_state"></param>
        /// <returns></returns>
        public static bool pl_exp_stop_cont(short hcam, short cam_state)
        {
            return PVCAM64.pl_exp_stop_cont(hcam, cam_state);
        }

        /// <summary>
        /// 允许最早的帧被覆盖
        /// </summary>
        /// <param name="hcam"></param>
        /// <returns></returns>
        public static bool pl_exp_unlock_oldest_frame(short hcam)
        {
            return PVCAM64.pl_exp_unlock_oldest_frame(hcam);
        }

        /*********************Frame Metadata methods ********************/

        /**
        @brief Decodes a metadata-enabled frame buffer into provided frame descriptor structure.

        This function processes the input frame buffer and calculates pointers to frame metadata headers,
        ROI headers and ROI image data and stores them to previously allocated @a pDstFrame structure.
        This function does not copy any pixel data. Since the @a pDstFrame stores only pointers
        to the @a pSrcBuf memory, the @a pSrcBuf must be valid for as long as the @a pDstFrame is
        accessed.

        @param[out] pDstFrame   A pre-allocated helper structure that will be filled with
                                            information from the given raw buffer. Create this structure with
                                            #pl_md_create_frame_struct or #pl_md_create_frame_struct_cont functions.
        @param[in]  pSrcBuf     A pointer to a frame data as retrieved from PVCAM. See functions
                                            #pl_exp_get_latest_frame and #pl_exp_get_latest_frame_ex.
        @param[in]  srcBufSize  The size of the raw frame data. Size of a frame is obtained
                                            from #pl_exp_setup_seq and #pl_exp_setup_cont functions.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.
        */
        public static bool pl_md_frame_decode(IntPtr pDstFrame, IntPtr pSrcBuf, uint srcBufSize)
        {
            return PVCAM64.pl_md_frame_decode(pDstFrame, pSrcBuf, srcBufSize);
        }

        /**
        @brief Optional function that recomposes a multi-ROI frame into a displayable image buffer.

        Every ROI will be copied into its appropriate location in the provided buffer.
        Please note that the function will subtract the Implied ROI position from each ROI
        position which essentially moves the entire Implied ROI to a [0, 0] position.
        Use the Offset arguments to shift all ROIs back to desired positions if needed.
        If you use the Implied ROI position for offset arguments, the frame will be recomposed
        as it appears on the full frame.

        The caller is responsible for black-filling the input buffer. Usually, this function
        is called during live/preview mode where the destination buffer is re-used. If the
        ROIs do move during acquisition, it is essential to black-fill the destination buffer
        before calling this function. This is not needed if the ROIs do not move.
        If the ROIs move during live mode, it is also recommended to use the offset arguments
        and recompose the ROI to a full frame - with moving ROIs the implied ROI may change
        with each frame and this may cause undesired ROI "twitching" in the displayable image.

        @param[out] pDstBuf     An output buffer; the buffer must be at least the size of the implied
                                            ROI that is calculated during the frame decoding process. The buffer
                                            must be of the same type as is the input frame - e.g. if the input
                                            frame format is 8-bit, the destination buffer should be 8-bit as well.
                                            If offset is set, the buffer dimensions must be large enough to allow
                                            the entire implied ROI to be positioned in the buffer.
        @param[in]  offX        Offset in the destination buffer, in pixels. If 0, the Implied
                                            ROI will be shifted to position 0 in the target buffer.
                                            Use (ImpliedRoi.s1 / ImplierRoi.sbin) as offset value to
                                            disable the shift and keep the ROIs in their absolute positions.
        @param[in]  offY        Offset in the destination buffer, in pixels. If 0, the Implied
                                            ROI will be shifted to position 0 in the target buffer.
                                            Use (ImpliedRoi.p1 / ImplierRoi.pbin) as offset value to
                                            disable the shift and keep the ROIs in their absolute positions.
        @param[in]  dstWidth    Width, in pixels of the destination image buffer. The buffer
                                            must be large enough to hold the entire Implied ROI, including
                                            the offsets (if used).
        @param[in]  dstHeight   Height, in pixels of the destination image buffer.
        @param[in]  pSrcFrame   A helper structure, previously decoded using the frame
                                            decoding function #pl_md_frame_decode.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.
        */
        public static bool pl_md_frame_recompose(IntPtr pDstBuf, ushort offX, ushort offY,
                                                 ushort dstWidth, ushort dstHeight, ref PVCAM.md_frame pSrcFrame)
        {
            return PVCAM64.pl_md_frame_recompose(pDstBuf, offX, offY, dstWidth, dstHeight, ref pSrcFrame);
        }

        /**
        @brief This method creates an empty md_frame structure for known number of ROIs.

        Use this method to prepare and pre-allocate one structure before starting
        continuous acquisition. Once callback arrives, fill the structure with
        #pl_md_frame_decode and display the metadata.
        Release the structure when not needed.

        @param[out] pFrame      A pointer to frame helper structure address where the structure
                                            will be allocated.
        @param[in]  roiCount    Number of ROIs the structure should be prepared for.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.
        */
        public static bool pl_md_create_frame_struct_cont(ref IntPtr pFrame, ushort roiCount)
        {
            return PVCAM64.pl_md_create_frame_struct_cont(ref pFrame, roiCount);
        }

        /**
        @brief This method creates an empty md_frame structure from an existing buffer.

        Use this method when loading buffers from disk or when performance is not
        critical. Do not forget to release the structure when not needed.
        For continuous acquisition where the number or ROIs is known, it is recommended
        to use the other provided method to avoid frequent memory allocation.

        @param[out] pFrame      A pointer address where the newly created structure will be stored.
        @param[in]  pSrcBuf     A raw frame data pointer as returned from the camera.
        @param[in]  srcBufSize  Size of the raw frame data buffer.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.
        */
        public static bool pl_md_create_frame_struct(ref IntPtr pFrame, IntPtr pSrcBuf, uint srcBufSize)
        {
            return PVCAM64.pl_md_create_frame_struct(ref pFrame, pSrcBuf, srcBufSize);
        }

        /**
        @brief Releases the md_frame struct.

        @param[in]  pFrame  A pointer to the previously allocated structure.

        @return #PV_OK for success, #PV_FAIL for failure. Failure sets #pl_error_code.
        */
        public static bool pl_md_release_frame_struct(IntPtr pFrame)
        {
            return PVCAM64.pl_md_release_frame_struct(pFrame);
        }


    }
}
