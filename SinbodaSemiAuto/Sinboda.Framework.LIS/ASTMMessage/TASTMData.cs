using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sinboda.Framework.LIS.SinASTM
{
    public class TASTMData
    {
        private ASTMMessage astmMessage;

        private Object thisLock = new Object();
        public TASTMData(ASTMMessage parent)
        {
            astmMessage = parent;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            lock (thisLock)
            {
                if (astmMessage.Frames == null) return null;
                int num = 1;                
                foreach (TFrame frame in astmMessage.Frames)
                {
                    if (string.IsNullOrEmpty(frame.Value))
                        continue;
                    string frameStr = frame.Value;
                    int frameCount = frameStr.Length / 200;
                    if (frameCount * 200 < frameStr.Length)
                    {
                        frameCount++;
                    }
                    for (int i = 0; i < frameCount; i++)
                    {
                        string temp = string.Empty;
                        string endStr = string.Empty;
                        num = num % 8;
                        if (i == frameCount - 1)
                        {
                            temp = frameStr.Substring(i * 200);
                            endStr = Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar);
                        }
                        else
                        {
                            temp = frameStr.Substring(i * 200, 200);
                            endStr = Common.Convert.GetASIIString(ASTMCommand.EtbBlockChar);
                        }
                        byte[] buffer = System.Text.Encoding.Default.GetBytes(char.Parse(num.ToString()) + temp + endStr);
                        int sum = 0;
                        for (int j = 0; j < buffer.Length; j++)
                        {
                            sum = sum + buffer[j];
                        }
                        int mod = sum % 256;
                        string str = Common.Convert.GetASIIString(ASTMCommand.StartBlockChar) + char.Parse(num.ToString()) + temp + endStr + mod.ToString("X2") + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.LFBlockChar);
                        sb.Append(str);
                        num++;
                    }
                }
            }
            return sb.ToString();
        }

        public string ToNoSplitString()
        {
            StringBuilder sb = new StringBuilder();
            lock (thisLock)
            {
                if (astmMessage.Frames == null) return null;
                int num = 1;
                string frameStr = string.Empty;
                foreach (TFrame frame in astmMessage.Frames)
                {
                    if (string.IsNullOrEmpty(frame.Value))
                        continue;
                    frameStr = frameStr + frame.Value + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar);
                }
                num = num % 8;
                byte[] buffer = System.Text.Encoding.Default.GetBytes(char.Parse(num.ToString()) + frameStr + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar));
                int sum = 0;
                for (int j = 0; j < buffer.Length; j++)
                {
                    sum = sum + buffer[j];
                }
                int mod = sum % 256;
                string str = Common.Convert.GetASIIString(ASTMCommand.StartBlockChar) + char.Parse(num.ToString()) + frameStr + Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar) + mod.ToString("X2") + Common.Convert.GetASIIString(ASTMCommand.EndBlockChar) + Common.Convert.GetASIIString(ASTMCommand.LFBlockChar);
                sb.Append(str);
            }
            return sb.ToString();
        }
        public void Load(string filePath)
        {
            FileStream fileStream = new FileStream(filePath,
                 FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                this.CreateData = Common.Convert.StreamToStr(fileStream);
            }
            finally
            {
                fileStream.Close();
            }
        }
        public Stream CreateDataByStream
        {
            set
            {
                value.Position = 0;
                AnalyzeData(Common.Convert.ASTMRemoveBlockChar(Common.Convert.StreamToStr(value),
                    "0x05", "0x04"));
            }
        }
        public string CreateData
        {
            set
            {
                AnalyzeData(Common.Convert.ASTMRemoveBlockChar(value,
                    "0x05", "0x04"));
            }
        }
        private void AnalyzeData(string str)
        {
            lock (thisLock)
            {
                astmMessage.Frames.Clear();
                if (!string.IsNullOrEmpty(str))
                {
                    string[] arr = str.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    string temp = string.Empty;
                    //针对不拆分的数据包解析
                    if (arr.Count() == 1)
                    {
                        temp = GetFrameStr(str, ASTMCommand.EtxBlockChar);
                        //数据帧结束符<CR>
                        string[] fraArr = temp.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < fraArr.Count(); i++)
                        {
                            astmMessage.Frames.Add(AnalyzeFrame(fraArr[i]));
                        }
                    }
                    //针对拆分的数据包解析
                    else
                    {
                        for (int i = 0; i < arr.Count(); i++)
                        {
                            if (arr[i].Length <= 7)
                            {
                                continue;
                            }
                            //中间帧的处理ETB
                            temp = temp + GetFrameStr(arr[i], ASTMCommand.EndBlockChar);
                            if (arr[i].IndexOf(Common.Convert.GetASIIString(ASTMCommand.EtxBlockChar)) > 0)
                            {
                                astmMessage.Frames.Add(AnalyzeFrame(temp));
                                temp = string.Empty;
                            }
                        }
                    }
                }
            }
        }

        private string GetFrameStr(string str, string command)
        {
            string frameStr = string.Empty;
            int crIndex = str.IndexOf(Common.Convert.GetASIIString(command));
            if (crIndex > 0)
            {
                frameStr = str.Substring(2, crIndex - 2);
            }
            return frameStr;
        }
        private TFrame AnalyzeFrame(string frameStr)
        {
            TFrame frame = new TFrame();
            if (!string.IsNullOrEmpty(frameStr))
            {
                string[] arr = Regex.Split(frameStr, @"\|");
                frame.Name = arr[0];
                //frame.FN = Convert.ToInt16(str[1].ToString());
                frame.Tag = GetTag(frame.Name);
                frame.Value = frameStr;
            }
            return frame;
        }
        private int GetTag(string name)
        {
            int tag = 1;
            foreach (TFrame frame in astmMessage.Frames)
            {
                if (name == frame.Name)
                {
                    tag += 1;
                }
            }
            return tag;
        }
    }
}
