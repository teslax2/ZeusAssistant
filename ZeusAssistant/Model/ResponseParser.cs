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
            DateTimeOffset Time = DateTimeOffset.Now;
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
                else
                {
                    Location = "Cork";
                    LocationConfidence = 2;
                }
                if (parsedContent.entities.datetime != null && parsedContent.entities.datetime != null)
                {
                    Time = (DateTimeOffset) parsedContent.entities.datetime[0].value;
                    var text = (string)parsedContent.entities.datetime[0].confidence;
                    double.TryParse(text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out TimeConfidence);
                }
                else
                {
                    Time = DateTimeOffset.Now;
                    TimeConfidence = 2;
                }

                Message = CreateMessage(Intent, Confidence, Location, LocationConfidence, Time.DateTime, TimeConfidence);
                return Message;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Parsing error");
                throw new ApplicationException("Failed to parse", ex);
            }
        }

        public static Message CreateMessage(string intent, double confidence, string location, double locationConfidence, DateTime time, double timeConfidence)
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
