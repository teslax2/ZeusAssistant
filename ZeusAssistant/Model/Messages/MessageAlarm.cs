using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public class MessageAlarm : Message
    {
        public DateTime Time { get; set; }
        public double TimeConfidence { get; set; }
        public int DaysEnabled { get; set; }

        public MessageAlarm(IntentEnum intent, double confidence) : base(intent, confidence)
        {
        }
        public MessageAlarm(IntentEnum intent, double confidence,DateTime time, double timeConfidence, int days, string message) : this(intent, confidence)
        {
            Time = time;
            TimeConfidence = timeConfidence;
            DaysEnabled = days;
            Note = message;
        }

        public MessageAlarm(IntentEnum intent, double confidence, DateTime time, double timeConfidence) : this(intent, confidence)
        {
            Time = time;
            TimeConfidence = timeConfidence;
        }
    }
}
