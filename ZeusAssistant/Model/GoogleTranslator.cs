using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace ZeusAssistant.Model
{
    public class GoogleTranslator
    {
        private HttpClient _client;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static readonly string path = @"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q=brawo dla tego pana";


        public GoogleTranslator(HttpClient client)
        {
            _client = client;
        }
        public static async Task<string> TranslateAync(HttpClient client, string phrase, string fromLang, string toLang)
        {
            if (client == null)
            {
                logger.Error("Client is null");
                return "";
            }
            try
            {
                client.DefaultRequestHeaders.Clear();
                var url = await client.GetAsync(GetPath(phrase, fromLang, toLang));
                var content = await url.Content.ReadAsStringAsync();
                var response = JObject.Parse(content);
                var textResponse = (string)response[0][0][0];
                return textResponse;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get translation");
                return "";
            }
        }

        public async Task<string> TranslateAsync(string phrase, string fromLang, string toLang)
        {
            if (_client == null)
            {
                logger.Error("Client is null");
                return "";
            }
            try
            {
                _client.DefaultRequestHeaders.Clear();
                var url = await _client.GetAsync(GetPath(phrase, fromLang, toLang));
                var content = await url.Content.ReadAsStringAsync();
                var response = JObject.Parse(content);
                var textResponse = (string)response[0][0][0];
                return textResponse;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get translation");
                return "";
            }
        }
        private static string GetPath(string phrase, string fromLang, string toLang)
        {
            return string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", fromLang, toLang, phrase);
        }
    }
}
