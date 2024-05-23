using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace Sinboda.Framework.LIS.SinHL7
{
    public class TSubcomponent : TSourceBase
    {
        public TSubcomponent()
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
