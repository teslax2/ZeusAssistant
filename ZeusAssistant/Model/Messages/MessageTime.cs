using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public class MessageTime : Message
    {
        public MessageTime(IntentEnum intent, double confidence) : base(intent, confidence)
        {
        }
    }
}
