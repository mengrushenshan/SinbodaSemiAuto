using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Sinboda.Framework.LIS.SinHL7
{
    public class TComponent : TSourceBase
    {
        public TComponent()
        {
            delim = "&";
            this._sourceCollection = new TSubcomponents();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TSubcomponents Subcomponents
        {
            get
            {
                return (TSubcomponents)this._sourceCollection;
            }
            set
            {
                this._sourceCollection = (TSubcomponents)value;
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
                        TSubcomponent f = new TSubcomponent();
                        this.Subcomponents.Add(f);
                    }
                }
            }
        }
    }
}
