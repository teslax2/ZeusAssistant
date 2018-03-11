using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZeusAssistant.Model.Weather
{
    public class WeatherApi
    {
        public DateTime LastRefresh { get; set; }
        public TimeSpan TimeSinceLastRefresh { get { return DateTime.Now - LastRefresh; } }
        private HttpClient _httpClient;
        public WeatherResponse Weather { get; set; }
        private string _path;
        private string _token;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        //http://api.openweathermap.org/data/2.5/forecast?q=Cork&lang=pl&units=metric&APPID=
        public WeatherApi(HttpClient httpClient, string path, string token)
        {
            Weather = new WeatherResponse();
            _httpClient = httpClient;
            _path = path;
            _token = token;
            LastRefresh = new DateTime(2000, 01, 01);
        }

        public async Task GetWeather (string City)
        {
            if (TimeSinceLastRefresh.Hours < 1)
                return;
            try
            {
                var fullPath = _path + City + "&lang=pl&units=metric&APPID=" + _token;
                _httpClient.DefaultRequestHeaders.Clear();
                var response = await _httpClient.GetAsync(fullPath);
                var content = await response.Content.ReadAsStringAsync();
                Weather = JsonConvert.DeserializeObject<WeatherResponse>(content);
                LastRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get weather");
            }
        }

        public 
    }
}
