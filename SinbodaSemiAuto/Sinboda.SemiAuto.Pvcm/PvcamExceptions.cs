using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    public class PvcamException : Exception
    {
        public short PvcamErrorCode { get; private set; }
        public string PvcamErrorMessage { get; private set; }
        public override String Message
        {
            get
            {
                return base.Message + String.Format(", ErrCode={0}, ErrMessage='{1}'",
                    PvcamErrorCode, PvcamErrorMessage);
            }
        }
        public PvcamException() : base()
        {
        }

        /// <summary>
        /// Builds an exception based on failure from any PVCAM function that returns failure.
        /// </summary>
        /// <param name="errCode">Error code, use pl_error_code() rigth after the pl_get_param() returns FALSE</param>
        public PvcamException(string message, short errCode) : base(message)
        {
            PvcamErrorCode = errCode;
            StringBuilder sb = new StringBuilder(PVCAM.ERROR_MSG_LEN);
            if (!PVCAM.pl_error_message(errCode, sb))
                PvcamErrorMessage = "[UNKNOWN: pl_error_message() failed]";
            else
                PvcamErrorMessage = sb.ToString();
        }
    }
    public class PvcamGetParamException : PvcamException
    {
        public PVCAM.PL_PARAMS PvcamParameter { get; private set; }
        public PVCAM.PL_PARAM_ATTRIBUTES PvcamAttribute { get; private set; }
        public override String Message
        {
            get
            {
                return base.Message + String.Format(", Parameter={0}, Attribute={1}",
                    PvcamStrings.ParamName(PvcamParameter), PvcamStrings.AttrName(PvcamAttribute));
            }
        }
        public PvcamGetParamException() : base()
        {
        }

        /// <summary>
        /// Builds an exception based on failure from pl_get_param() function.
        /// </summary>
        /// <param name="paramId">Parameter ID used in the function</param>
        /// <param name="attr">Attribute ID used int the function</param>
        /// <param name="errCode">Error code, use pl_error_code() rigth after the pl_get_param() returns FALSE</param>
        public PvcamGetParamException(string message, PVCAM.PL_PARAMS paramId, PVCAM.PL_PARAM_ATTRIBUTES attr, short errCode) : base(message, errCode)
        {
            PvcamParameter = paramId;
            PvcamAttribute = attr;
        }
    }
    public class PvcamSetParamException : PvcamException
    {
        public PVCAM.PL_PARAMS PvcamParameterId { get; private set; }
        public PVCAM.PL_PARAM_ATTRIBUTES PvcamAttribute { get; private set; }
        public override String Message
        {
            get
            {
                return base.Message + String.Format(", Parameter={0}",
                    PvcamStrings.ParamName(PvcamParameterId));
            }
        }
        public PvcamSetParamException() : base()
        {
        }

        /// <summary>
        /// Builds an exception based on failure from pl_set_param() function.
        /// </summary>
        /// <param name="paramId">Parameter ID used in the function</param>
        /// <param name="errCode">Error code, use pl_error_code() rigth after the pl_set_param() returns FALSE</param>
        public PvcamSetParamException(string message, PVCAM.PL_PARAMS paramId, short errCode) : base(message, errCode)
        {
            PvcamParameterId = paramId;
        }
    }
}
