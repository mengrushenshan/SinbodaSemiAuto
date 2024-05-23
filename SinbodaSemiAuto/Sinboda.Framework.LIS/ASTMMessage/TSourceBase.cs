using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public abstract class TSourceBase
    {
        private string _value = "";
        protected TSourceCollectionBase _sourceCollection;
        protected string delim = "";

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual int MaxCount
        {
            get { return _sourceCollection.Count; }
            set { }
        }

        public virtual string Value
        {
            get
            {
                if (_sourceCollection.Count != 0)
                {
                    _value = "";
                    int i = 0;
                    foreach (TSourceBase source in _sourceCollection)
                    {
                        _value += source.Value;
                        if (i != _sourceCollection.Count - 1)
                        { _value += delim; }
                        i++;
                    }
                }
                return _value;
            }
            set
            {
                _value = value;

                if (delim != "")
                {
                    string[] arr = Regex.Split(value, @"\" + delim);
                    if (arr.Count() > 1)
                    {
                        this.MaxCount = arr.Count();
                        int i = 0;
                        foreach (TSourceBase source in _sourceCollection)
                        {
                            if (i < arr.Count())
                            {
                                source.Value = arr[i];
                            }
                            i++;
                        }
                    }
                }



                //if (_sourceCollection.Count != 0 && sep != "")
                //{
                //    string[] arr = Regex.Split(value, "\\" + sep);
                //    int i = 0;
                //    foreach (TSourceBase source in _sourceCollection)
                //    {
                //        if (i < arr.Count())
                //        {
                //            source.Value = arr[i];
                //        }
                //        i++;
                //    }
                //}
            }
        }
    }
}
