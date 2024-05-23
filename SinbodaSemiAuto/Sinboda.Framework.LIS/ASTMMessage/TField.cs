using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class TField : TSourceBase
    {

        public TField()
        {
            delim = "^";
            this._sourceCollection = new TComponents();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TComponents Components
        {
            get
            {
                return (TComponents)this._sourceCollection;
            }
            set
            {
                this._sourceCollection = (TComponents)value;
            }
        }


        [Browsable(true)]
        public override int MaxCount
        {
            get { return base.MaxCount; }
            set
            {
                if (base.MaxCount == value) return;
                if (value == 0)
                {
                    this._sourceCollection.Clear();
                }
                else
                {
                    this._sourceCollection.Clear();

                    for (int i = 0; i < value; i++)
                    {
                        TComponent f = new TComponent();
                        this.Components.Add(f);
                    }
                }
            }
        }
    }
}
