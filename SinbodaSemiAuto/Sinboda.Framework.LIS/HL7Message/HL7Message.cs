using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace Sinboda.Framework.LIS.SinHL7
{

    public class HL7Message : Component
    {
        public HL7Message()
        {

        }

        //private bool _active;
        private TSegments _segments = new TSegments();
        private byte _startBlockChar = 0x0b;

        public string StartBlockChar
        {
            get { return "0x" + _startBlockChar.ToString("x2"); }
            set { _startBlockChar = System.Convert.ToByte(value, 16); }
        }

        private byte _endBlockChar = 0x1c;
        public string EndBlockChar
        {
            get { return "0x" + _endBlockChar.ToString("x2"); ; }
            set { _endBlockChar = System.Convert.ToByte(value, 16); }
        }

        [Browsable(false)]
        public THL7Data HL7
        {
            get { return new THL7Data(this); }
        }

        //private string _encodingCharacters = @"^~\&";
        //public string EncodingCharacters
        //{
        //    get { return _encodingCharacters; }
        //    set { _encodingCharacters = value; }
        //}

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TSegments Segments
        {
            get { return _segments; }
            set { _segments = value; }
        }

        //public bool Active
        //{
        //    get { return _active; }
        //    set { _active = value; }
        //}

        //private string _fieldSeparator = "|";

        //public string FieldSeparator
        //{
        //    get { return _fieldSeparator; }
        //    set { _fieldSeparator = value; }
        //}

        //protected string GetHL7Text()
        //{
        //    if (this.Segments == null) return null;
        //    string segs = "";
        //    foreach (TSegment segment in this.Segments)
        //    {
        //        segs += segment.Value + (char)0x0d;
        //    }

        //    string s = (char)Convert.ToByte(this.StartBlockChar, 16) + segs +
        //        (char)Convert.ToByte(this.EndBlockChar, 16) + (char)0x0d;

        //    return s;
        //}


        //public Stream GetHL7Stream()
        //{
        //    return ConvertHelper.StrToStream(this.GetHL7Text());
        //}
    }
}
