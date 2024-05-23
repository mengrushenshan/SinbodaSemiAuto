using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinboda.Framework.Common.ResourceExtensions
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding _encoding;

        public StringWriterWithEncoding()
        {
        }

        public StringWriterWithEncoding(Encoding encoding)
        {
            _encoding = encoding;
        }

        public StringWriterWithEncoding(StringBuilder sb)
        {
        }

        public StringWriterWithEncoding(IFormatProvider formatProvider)
        {
        }

        public StringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider)
        {
        }

        public override Encoding Encoding
        {
            get
            {
                return (null == _encoding) ? base.Encoding : _encoding;
            }
        }
    }
}
