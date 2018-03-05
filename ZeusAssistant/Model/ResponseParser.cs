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

        public static IMessage Parse (string content)
        {

            var parsedContent = JObject.Parse(content);
            var _text = (string)parsedContent["_text"];
            MessageIntent _createdIntent = null;
            string _createdLocation = null;
            string _createdTime = null;
            try
            {
                var intent = (string)parsedContent["entities"]["intent"][0]["value"];
                var intentConfidence = (string)parsedContent["entities"]["intent"][0]["confidence"];
                _createdIntent = CreateIntent(intent, intentConfidence);
            }
            catch (Exception)
            {}
            try
            {
                var location = (string)parsedContent["entities"]["location"][0]["value"];
                var locationConfidence = (string)parsedContent["entities"]["location"][0]["confidence"];
                _createdLocation = CreateLocation(location, locationConfidence);
            }
            catch (Exception)
            {}
            try
            {
                var time = (string)parsedContent["entities"]["datetime"][0]["value"];
                var timeConfidence = (string)parsedContent["entities"]["datetime"][0]["confidence"];
                _createdTime = CreateTime(time, timeConfidence);
            }
            catch (Exception)
            {}


            if (_createdIntent != null)
            {
                var createdMessage = new MessageWeather(content);
                createdMessage.Text = _text;
                if (_createdLocation != null)
                    createdMessage.Location = _createdLocation;
                if (_createdTime != null)
                    createdMessage.When = _createdTime;
                var messageString = createdMessage.ToString();
                logger.Info(createdMessage.ToString());
                return createdMessage;
            }
            return null;               
 
        }

        private static IMessage CreateMessage(MessageIntent intent, string content)
        {
            switch (intent.Intent)
            {
                case IntentEnum.Weather:
                    return new MessageWeather(content,intent.Confidence);
                case IntentEnum.Time:
                    return new MessageTime(content, intent.Confidence);
                case IntentEnum.Alarm:
                    return new MessageAlarm(content, intent.Confidence);
                case IntentEnum.Note:
                    return new MessageNote(content, intent.Confidence);
                default:
                    return new MessageNote(content, intent.Confidence);
            }
        }

        private static MessageIntent CreateIntent(string intent, string confidence)
        {
            if (string.IsNullOrEmpty(intent) || string.IsNullOrEmpty(confidence))
                return null;
            var _confidence = Single.Parse(confidence, new CultureInfo("en-US"));
            if (_confidence < 0.85)
                return null;

            switch (intent)
            {
                case "weather":
                    return new MessageIntent(IntentEnum.Weather, _confidence);
                case "time":
                    return new MessageIntent(IntentEnum.Time, _confidence);
                case "alarm":
                    return new MessageIntent(IntentEnum.Alarm, _confidence);
                case "note":
                    return new MessageIntent(IntentEnum.Note, _confidence);
                default:
                    return null;
            }
        }

        private static string CreateLocation(string location, string confidence)
        {
            if (string.IsNullOrEmpty(location) || string.IsNullOrEmpty(confidence))
                return null;
            var _confidence = Single.Parse(confidence, new CultureInfo("en-US"));
            if (_confidence < 0.85)
                return null;
            return location;
        }

        private static string CreateTime(string time, string confidence)
        {
            if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(confidence))
                return null;
            var _confidence = Single.Parse(confidence, new CultureInfo("en-US"));
            if (_confidence < 0.85)
                return null;
            return time;
        }
    }
}
