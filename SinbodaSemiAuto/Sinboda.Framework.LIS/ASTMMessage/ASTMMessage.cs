using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class ASTMMessage : Component
    {

        /// <summary>
        /// <STX> FN <FRAME> <CR> <ETB> or <ETX> <CS><CR><LF>
        /// </summary>
        private TFrames _frames = new TFrames();

        


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TFrames Frames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        [Browsable(false)]
        public TASTMData ASTMData
        {
            get { return new TASTMData(this); }
        }
        public ASTMMessage()
        {

        }

    }
}
