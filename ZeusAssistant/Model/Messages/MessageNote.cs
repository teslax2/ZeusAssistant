using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    class MessageNote : Message
    {
        public MessageNote(IntentEnum intent, double confidence) : base(intent, confidence)
        {
        }
    }
}
