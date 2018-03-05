using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public class MessageTime : IMessage
    {
        public MessageIntent MessageIntent { get; set; }
        public string RawMessage { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string Text { get; set; }

        public MessageTime (string content, double confidence)
        {
            MessageIntent = new MessageIntent(IntentEnum.Time, confidence);
            RawMessage = content;
        }
    }
}
