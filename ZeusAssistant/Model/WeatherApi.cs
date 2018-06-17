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
        private readonly string  _path;
        private readonly string _token;
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
        /// <summary>
        /// Http Get Request to OpenWeatherApi
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        private async Task<WeatherResponse> GetAsync(string City)
        {
            try
            {
                if (TimeSinceLastRefresh.Hours < 1 || true)
                {
                    if (System.IO.File.Exists("weather.tmp"))
                    {
                        var weatherSaved = System.IO.File.ReadAllText("weather.tmp", Encoding.UTF8);
                        if (!string.IsNullOrEmpty(weatherSaved))
                            Weather = JsonConvert.DeserializeObject<WeatherResponse>(weatherSaved);
                    }
                }
                else
                {
                    var fullPath = _path + City + "&lang=pl&units=metric&APPID=" + _token;
                    _httpClient.DefaultRequestHeaders.Clear();
                    var response = await _httpClient.GetAsync(fullPath);
                    var content = await response.Content.ReadAsStringAsync();
                    System.IO.File.WriteAllText("weather.tmp", content, Encoding.UTF8);
                    Weather = JsonConvert.DeserializeObject<WeatherResponse>(content);
                    LastRefresh = DateTime.Now;
                }
                return Weather;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Failed to get weather");
                return null;
            }
        }
        /// <summary>
        /// Returns weather forecast for choosen city and time
        /// </summary>
        /// <param name="City"></param>
        /// <param name="date"></param>
        /// <returns>"{0} będą {1}, temperatura {2}, wilgotność {3}, zachmurzenie {4} procent"</returns>
        public async Task<string> GetForecastAsync(string City, DateTime date)
        {
            var _weather = await GetAsync(City);

            if (_weather == null)
                return "";
            int hour = 0;
            if (date.Hour == 0) hour = 12;
            else hour = date.Hour;

            var forecast = _weather.Forecast.Where(x => x.DtTxt.Day == date.Day && x.DtTxt.Hour >= hour).FirstOrDefault();
            var forecastString = string.Format("{0} będzie {1}, temperatura {2}, wilgotność {3}, zachmurzenie {4} procent",
                GetDay(date), forecast.Weather[0].Description, forecast.Main.Temp, forecast.Main.Humidity, forecast.Clouds.All);
            logger.Info(forecastString);
            return forecastString;
        }

        private string GetDay(DateTimeOffset date)
        {
            var today = DateTimeOffset.Now;
            var timeSpan = date.Day - today.Day;
            switch (timeSpan)
            {
                case 0:
                    return "dzisiaj o " + date.Hour.ToString();
                case 1:
                    return "jutro";
                case 2:
                    return "pojutrze";
                default:
                    return "za " + timeSpan + " dni";
            }
        }
    }
}