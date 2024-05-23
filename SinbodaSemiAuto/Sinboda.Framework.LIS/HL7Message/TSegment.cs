using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;


namespace Sinboda.Framework.LIS.SinHL7
{
    public class TSegment : TSourceBase
    {
        private string _name = ""; //名称。
        private int _tag = 0;

        /// <summary>
        /// 获取或设置 消息段 的名称。
        /// </summary> 
        [Browsable(true), Category("Data")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name == value) return;
                _name = value;
                if (this.Fields != null)
                {
                    if (this._sourceCollection.Count > 0)
                    {
                        this.Fields[0].Value = _name;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置 消息段下标
        /// </summary> 
        [Browsable(true), Category("Data")]
        public int Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
            }
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TFields Fields
        {
            get
            {
                return (TFields)this._sourceCollection;
            }
            set
            {
                this._sourceCollection = (TFields)value;
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
                        TField f = new TField();
                        this.Fields.Add(f);
                    }
                    if (this.Fields.Count > 0 && _name != null)
                    {
                        this.Fields[0].Value = _name;
                    }
                }

            }
        }

        public TSegment()
        {
            delim = "|";
            this._sourceCollection = new TFields();
        }
    }
}
