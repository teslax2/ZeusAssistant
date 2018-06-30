using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    class MessageWeather : Message
    {
        public string Location { get; set; } = string.Empty;
        public double LocationConfidence { get; set; }
        public DateTime When { get; set; } = DateTime.Now;
        public double WhenConfidence { get; set; }

        public MessageWeather(IntentEnum intent, double confidence) : base(intent, confidence)
        {
        }
        public MessageWeather(IntentEnum intent, double confidence, string location, double locationConfidence, DateTime when, double whenConfidence) : this(intent,confidence)
        {
            Location = location;
            LocationConfidence = locationConfidence;
            When = when;
            WhenConfidence = whenConfidence;
        }
        public override string ToString()
        {
            return String.Format("Weather in {0} at {1}", Location, When.ToShortTimeString());
        }
    }
}
