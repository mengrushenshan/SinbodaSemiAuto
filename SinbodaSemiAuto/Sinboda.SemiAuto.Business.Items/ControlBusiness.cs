using sin_mole_flu_analyzer.Models.Command;
using Sinboda.Framework.Common.Log;
using Sinboda.Framework.Core.AbstractClass;
using Sinboda.SemiAuto.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.SemiAuto.Business.Items
{
    public class ControlBusiness : BusinessBase<ControlBusiness>
    {
        public bool LightEnableCtrl(int enable, int output)
        {
            bool result = false;
            CmdLightEnable cmdLightEnable = new CmdLightEnable()
            {
                Enable = enable,
                Output = output
            };

            if (cmdLightEnable.Execute())
            {
                result = true;
            }
            else
            {
                Response response = cmdLightEnable.GetResponse() as Response;
                LogHelper.logSoftWare.Error("LightEnableCtrl Run Error ErrorCode:" + response.ErrorCode);
                result = false;
            }

            return result;
        }

        public bool LightEnableCtrl(int enable, double voltage, int output)
        {
            bool result = false;
            CmdLightEnable cmdLightEnable = new CmdLightEnable()
            {
                Enable = enable,
                Voltage = voltage,
                Output = output
            };

            if (cmdLightEnable.Execute())
            {
                result = true;
            }
            else
            {
                Response response = cmdLightEnable.GetResponse() as Response;
                LogHelper.logSoftWare.Error("LightEnableCtrl Run Error ErrorCode:" + response.ErrorCode);
                result = false;
            }

            return result;
        }
    }
}
