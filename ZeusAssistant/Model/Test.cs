using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeusAssistant.Model.Messages;


namespace ZeusAssistant.Model
{
    public class Test
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static Message CreateMessage(string intent, double confidence, string location, double locationConfidence, DateTime time, double timeConfidence)
        {
            var response = ResponseParser.CreateMessage(intent, confidence, location, locationConfidence, time, timeConfidence);
            logger.Debug(response.ToString);
            return response;
        }
    }
}
