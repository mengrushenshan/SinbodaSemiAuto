using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace Sinboda.Framework.LIS.SinHL7
{
    public class THL7Data
    {
        private HL7Message hl7Message;
        public THL7Data(HL7Message parent)
        {
            hl7Message = parent;
        }

        public override string ToString()
        {
            string message = "";
            lock (hl7Message)
            {
                if (hl7Message.Segments == null) return null;

                foreach (TSegment segment in hl7Message.Segments)
                {
                    message += segment.Value + (char)0x0d;
                }
            }

            return (char)System.Convert.ToByte(hl7Message.StartBlockChar, 16) + message +
                (char)System.Convert.ToByte(hl7Message.EndBlockChar, 16) + (char)0x0d;
        }

        public Stream ToStream()
        {
            return Common.Convert.StrToStream(this.ToString());
        }

        public byte[] ToBytes()
        {
            return Common.Convert.StrToBytes(this.ToString());
        }

        public Stream CreateData
        {
            set
            {
                value.Position = 0;
                AnalyzeData(Common.Convert.HL7RemoveBlockChar(Common.Convert.StreamToStr(value),
                    hl7Message.StartBlockChar, hl7Message.EndBlockChar));
            }
        }

        public void Load(string filePath)
        {
            FileStream fileStream = new FileStream(filePath,
                 FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                this.CreateData = fileStream;
            }
            finally
            {
                fileStream.Close();
            }
        }

        public void SaveAs(string filePath)
        {
            if (hl7Message.Segments == null) return;
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            try
            {
                BinaryWriter bw = new BinaryWriter(fileStream);
                bw.Write(this.ToBytes());
                bw.Close();
            }
            finally
            {
                fileStream.Close();
            }
        }

        private void AnalyzeData(string s)
        {
            lock (hl7Message)
            {
                hl7Message.Segments.Clear();
                if (s != "")
                {
                    string[] arr = Regex.Split(s, "\r");
                    for (int i = 0; i < arr.Count(); i++)
                    {
                        hl7Message.Segments.Add(AnalyzeSegment(arr[i]));
                    }
                }
            }
        }

        private int GetTag(string name)
        {
            int tag = 1;
            foreach (TSegment segment in hl7Message.Segments)
            {
                if (name == segment.Name)
                {
                    tag += 1;
                }
            }
            return tag;
        }

        private TSegment AnalyzeSegment(string s)
        {
            TSegment segment = new TSegment();
            string[] arr = Regex.Split(s, "\\|");
            segment.Name = arr[0];
            segment.Tag = GetTag(segment.Name);
            segment.Value = s;
            return segment;
        }

        //private TField AnalyzeField(string s)
        //{
        //    TField field = new TField();
        //    string[] arr = Regex.Split(s, "\\^");
        //    for (int i = 0; i < arr.Count(); i++)
        //    {
        //        field.Components.Add(AnalyzeComponent(arr[i]));
        //    }
        //    return field;
        //}

        //private TComponent AnalyzeComponent(string s)
        //{
        //    TComponent component = new TComponent();
        //    component.Value = s;
        //    return component;
        //}


    }
}
