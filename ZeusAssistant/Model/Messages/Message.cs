using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public class Message
    {
        public IntentEnum MessageIntent { get; set; }
        public string RawMessage { get; set; }
        public DateTime Created { get; private set; }
        public string Note { get; set; }
        public double Confidence { get; set; }

        public Message(IntentEnum intent, double confidence)
        {
            MessageIntent = intent;
            Confidence = confidence;
            Created = DateTime.Now;
        }

        public override string ToString()
        {
            return MessageIntent.ToString() + Environment.NewLine + Note;
        }
    }

}
