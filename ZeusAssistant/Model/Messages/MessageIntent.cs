using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    public class MessageIntent
    {
        public IntentEnum Intent{ get; set; }
        public double Confidence { get; set; }

        public MessageIntent (IntentEnum intent, double confidence)
        {
            Intent = intent;
            Confidence = confidence;
        }
        public MessageIntent()
        {

        }

        public override string ToString()
        {
            return Intent.ToString() + Environment.NewLine + Confidence.ToString();
        }
    }
}
