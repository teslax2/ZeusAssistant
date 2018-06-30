using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    class MessageTranslate : Message
    {
        public string Translation { get; set; }
        public MessageTranslate(IntentEnum intent, double confidence, string translate) : base(intent, confidence)
        {
            this.RawMessage = translate;
        }

        public override string ToString()
        {
            return string.Format("Translated {0} -> {1}",RawMessage,Translation);
        }
    }
}
