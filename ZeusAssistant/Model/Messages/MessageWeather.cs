using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeusAssistant.Model.Messages
{
    class MessageWeather : IMessage
    {
        public MessageIntent MessageIntent { get; set; }
        public string RawMessage { get; set; }
        public DateTime Created { get; set; }
        public string Location { get; set; } = string.Empty;
        public string When { get; set; } = string.Empty;
        public string Text { get; set; }

        public MessageWeather(string content, double confidence)
        {
            MessageIntent = new MessageIntent(IntentEnum.Weather, 1);
            RawMessage = content;
            Created = DateTime.Now;
        }
        public override string ToString()
        {
            return MessageIntent.ToString() + Environment.NewLine + Location + Environment.NewLine + When + Environment.NewLine + Text;
        }
    }
}
