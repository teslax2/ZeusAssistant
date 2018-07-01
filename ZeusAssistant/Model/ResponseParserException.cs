using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model
{
    public class ResponseParserException:Exception
    {
        public ResponseParserException() { }
        public ResponseParserException(string message) : base(message) { }
        public ResponseParserException(string message, Exception innerException) : base(message, innerException) { }
    }
}
