using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class TComponent : TSourceBase
    {
        public TComponent()
        {
            delim = "";
            this._sourceCollection = new TSourceCollectionBase();
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int MaxCount
        {
            get { return base.MaxCount; }
            set { base.MaxCount = value; }
        }
    }
}
