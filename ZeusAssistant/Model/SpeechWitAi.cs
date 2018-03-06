using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using ZeusAssistant.Model.Messages;

namespace ZeusAssistant.Model
{
    class SpeechWitAi
    {
        private static HttpClient _httpClient;
        private static HttpWebRequest _webClient;
        private string _path;
        private string _token;
        public string Path { get; set; }
        private Stream dataStream;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private DateTime start;
        private DateTime stop;

        public SpeechWitAi(HttpClient client, string path, string token)
        {
            _httpClient = client;
            _path = path;
            _token = token;
        }

        public async Task<Message> PostASync(byte[] data)
        {
            var before = DateTime.Now;
            _httpClient.DefaultRequestHeaders.Clear();
            ByteArrayContent sound = new ByteArrayContent(data);
            sound.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg3");
            _httpClient.DefaultRequestHeaders.Add("Transfer-encoding", "chunked");
            _httpClient.DefaultRequestHeaders.Add("Authorization", _token);
            var response = await _httpClient.PostAsync(_path, sound);
            var responseString = await response.Content.ReadAsStringAsync();
            return ResponseParser.Parse(responseString);
        }

        public void StartPostChunked()
        {
            _webClient = (HttpWebRequest)WebRequest.Create(_path);
            _webClient.SendChunked = true;
            _webClient.Headers.Add("Authorization", _token);
            _webClient.Method = "POST";
            _webClient.ContentType = "audio/mpeg3";
            _webClient.AllowWriteStreamBuffering = false;
            dataStream = _webClient.GetRequestStream();
        }
        public async Task StartPostChunkedAsync()
        {
            _webClient = (HttpWebRequest)WebRequest.Create(_path);
            _webClient.SendChunked = true;
            _webClient.Headers.Add("Authorization", _token);
            _webClient.Method = "POST";
            _webClient.ContentType = "audio/mpeg3";
            _webClient.AllowWriteStreamBuffering = false;
            dataStream = await _webClient.GetRequestStreamAsync();
        }

        public void SendPostChunked(byte[] data)
        {
            try
            {
                start = DateTime.Now;
                dataStream.Write(data, 0, data.Length);
                System.Diagnostics.Debug.WriteLine("Sending audio");
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in sending");
            }
        }
        public async Task SendPostChunkedAsync(byte[] data)
        {
            try
            {
                start = DateTime.Now;
                await dataStream.WriteAsync(data, 0, data.Length);
                System.Diagnostics.Debug.WriteLine("Sending audio");
            }
            catch (Exception e)
            {
                logger.Error(e, "Error in sending");
            }
        }

        public Message StopPostChunked()
        {
            try
            {
                start = DateTime.Now;
                var response = _webClient.GetResponse();
                var responseStream = response.GetResponseStream();
                var streamReader = new StreamReader(responseStream);
                var responseString = streamReader.ReadToEnd();
                dataStream.Close();
                streamReader.Close();
                response.Close();
                stop = DateTime.Now;
                var timeSpan = stop - start;
                logger.Debug(timeSpan.TotalSeconds.ToString());
                return ResponseParser.Parse(responseString);

            }
            catch (Exception e)
            {
                logger.Error(e, "Error while getting api's response");
                return null;
            }
        }
        public async Task<Message> StopPostChunkedAsync()
        {
            try
            {
                var response = await _webClient.GetResponseAsync();
                var responseStream = response.GetResponseStream();
                var streamReader = new StreamReader(responseStream);
                var responseString = streamReader.ReadToEnd();
                dataStream.Close();
                streamReader.Close();
                response.Close();
                stop = DateTime.Now;
                var timeSpan = stop-start;
                logger.Info(timeSpan.TotalSeconds.ToString());
                return ResponseParser.Parse(responseString);

            }
            catch (Exception e)
            {
                logger.Error(e, "Error while getting api's response");
                return null;
            }
        }
    }
}
