using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Core.Pvcam
{
    public class PvcamStrings
    {
        public static string AttrName(PVCAM.PL_PARAM_ATTRIBUTES attrId)
        {
            return attrId.ToString();
        }
        public static string ParamName(PVCAM.PL_PARAMS paramId)
        {
            return paramId.ToString();
        }
    }
}
