using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZeusAssistant.Model.Messages;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ZeusAssistant.Model
{
    public static class ResponseParser
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static Message Parse (string content)
        {

            dynamic parsedContent = JObject.Parse(content);
            var _text = (string)parsedContent["_text"];
            Message Message = null;
            string Location = string.Empty;
            DateTime Time = DateTime.Now;
            string Intent = string.Empty;
            double Confidence = 0;
            double LocationConfidence = 0;
            double TimeConfidence = 0;

            try
            {
                if (parsedContent.entities.intent[0] != null && parsedContent.entities.intent[0] != null)
                {
                    Intent = (string)parsedContent.entities.intent[0].value;
                    var text = (string)parsedContent.entities.intent[0].confidence;
                    double.TryParse(text.Replace(',','.'),NumberStyles.Any,CultureInfo.InvariantCulture, out Confidence);
                }        
                if (parsedContent.entities.location!= null && parsedContent.entities.location != null)
                {
                    Location = (string)parsedContent.entities.location[0].value;
                    var text = (string)parsedContent.entities.location[0].confidence;
                    double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out LocationConfidence);
                }
                if (parsedContent.entities.datetime != null && parsedContent.entities.datetime != null)
                {
                    DateTime.TryParse((string)parsedContent.entities.datetime[0].value, out Time);
                    var text = (string)parsedContent.entities.datetime[0].confidence;
                    double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out TimeConfidence);
                }

                Message = CreateMessage(Intent, Confidence, Location, LocationConfidence, Time, TimeConfidence);
                return Message;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Parsing error");
                return null;
            }
        }

        private static Message CreateMessage(string intent, double confidence, string location, double locationConfidence, DateTime time, double timeConfidence)
        {
            if (string.IsNullOrEmpty(intent) || confidence < 0.85)
                return null;

            switch (intent)
            {
                case "weather":
                    return new MessageWeather(IntentEnum.Weather, confidence, location,locationConfidence,time,timeConfidence);
                case "time":
                    return new MessageTime(IntentEnum.Time, confidence);
                case "alarm":
                    return new MessageAlarm(IntentEnum.Alarm, confidence,time,timeConfidence);
                case "note":
                    return new MessageNote(IntentEnum.Note, confidence);
                default:
                    return null;
            }
        }
    }
}
