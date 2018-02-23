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

namespace ZeusAssistant.Model
{
    class ApiHandler
    {
        private static HttpClient _client;
        public ApiHandler()
        {
            _client = new HttpClient();
        }

        public async Task<string> Post(byte[] data)
        {
            _client.DefaultRequestHeaders.Clear();
            var path = "https://api.wit.ai/speech?v=20170307";
            ByteArrayContent sound = new ByteArrayContent(data);
            sound.Headers.Clear();
            sound.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg3");
            _client.DefaultRequestHeaders.Add("Transfer-encoding", "chunked");
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer IGIZT37VHBNUUQVNL6R6Z3MNZTMOPAKW");
            var start = DateTime.Now;
            var response = await _client.PostAsync(path, sound);
            var responseString = await response.Content.ReadAsStringAsync();
            var obj = JObject.Parse(responseString);
            var stop = DateTime.Now;
            var duration = stop - start;
            return (string)obj["_text"] + " .. "+ duration.ToString();
        }

        public string PostChunked()
        {
            var path = "https://api.wit.ai/speech?v=20170307";
            var _client = new WebClient();
            _client.Headers.Add("Authorization", "Bearer IGIZT37VHBNUUQVNL6R6Z3MNZTMOPAKW");
            //_client.Headers.Add(HttpRequestHeader.TransferEncoding, "chunked");
            _client.Headers.Add("Content-Type", "audio/mpeg3");
            var response = _client.UploadFile(path, @"C:\Users\wiesi_000\Desktop\sample.mp3");
            return Encoding.UTF8.GetString(response);
        }

        public string PostChunked2()
        {
            var path = "https://api.wit.ai/speech?v=20170307";
            var _client = WebRequest.Create(path);
            _client.Headers.Add("Authorization", "Bearer IGIZT37VHBNUUQVNL6R6Z3MNZTMOPAKW");
            //_client.Headers.Add(HttpRequestHeader.TransferEncoding, "chunked");
            _client.Method = "POST";
            _client.ContentType = "audio/mpeg3";
            Stream dataStream = _client.GetRequestStream();
            var start = DateTime.Now;

            var file = File.ReadAllBytes(@"C:\Users\wiesi_000\Desktop\sample.mp3");
            var size = file.Length;
            int i = 0;
            byte[] dupa = new byte[5120];
            int bufferSize = 5120;
            while (bufferSize > 0)
            {
                Buffer.BlockCopy(file, i, dupa, 0, bufferSize);
                i += bufferSize;
                if (i + bufferSize > size)
                    bufferSize = size - i;
                dataStream.Write(dupa,0,dupa.Length);
            }
            var response = _client.GetResponse();
            var responseStream = response.GetResponseStream();
            var streamReader = new StreamReader(responseStream);
            var responseString = streamReader.ReadToEnd();
            dataStream.Close();
            streamReader.Close();
            response.Close();
            return responseString;
        }

        public string PostChunked22()
        {
            var path = "https://api.wit.ai/speech?v=20170307";
            var _client = WebRequest.Create(path);
            _client.Headers.Add("Authorization", "Bearer IGIZT37VHBNUUQVNL6R6Z3MNZTMOPAKW");
            //_client.Headers.Add(HttpRequestHeader.TransferEncoding, "chunked");
            _client.Method = "POST";
            _client.ContentType = "audio/mpeg3";
            Stream dataStream = _client.GetRequestStream();
            var file = File.ReadAllBytes(@"C:\Users\wiesi_000\Desktop\sample.mp3");
            var start = DateTime.Now;
            dataStream.Write(file, 0, file.Length);
            var response = _client.GetResponse();
            var responseStream = response.GetResponseStream();
            var streamReader = new StreamReader(responseStream);
            var responseString = streamReader.ReadToEnd();
            dataStream.Close();
            streamReader.Close();
            response.Close();
            return responseString;
        }
    }
}
