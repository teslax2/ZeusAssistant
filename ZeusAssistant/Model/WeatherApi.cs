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
        }

        private async Task GetAsync (string City)
        {
            try
            {
                var fullPath = _path + City + "&lang=pl&units=metric&APPID=" + _token;
                _httpClient.DefaultRequestHeaders.Clear();
                var response = await _httpClient.GetAsync(fullPath);
                var content = await response.Content.ReadAsStringAsync();
                System.IO.File.WriteAllText("weather.tmp", content);
                Weather = JsonConvert.DeserializeObject<WeatherResponse>(content);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get weather");
            }
        }
        /// <summary>
        /// Returns weather forecast for choosen city and time
        /// </summary>
        /// <param name="City"></param>
        /// <param name="date"></param>
        /// <returns>"{0} będą {1}, temperatura {2}, wilgotność {3}, zachmurzenie {4} procent"</returns>
        public async Task<string> GetForecastAsync (string City, DateTime date)
        {
            var weatherTmpPath = "weather.tmp";
            if (System.IO.File.Exists(weatherTmpPath))
            {
                var creationTime = System.IO.File.GetLastWriteTime(weatherTmpPath);
                var timeFromCreation = DateTime.Now - creationTime;
                if (timeFromCreation.Hours <=3 || !Utilities.CheckForInternetConnection())
                {
                    var serializedContent = System.IO.File.ReadAllText(weatherTmpPath);
                    Weather = JsonConvert.DeserializeObject<WeatherResponse>(serializedContent);
                }
                else
                    await GetAsync(City);
            }
            else
                await GetAsync(City);

            if (Weather == null)
                return "";
            int hour = 0;
            if (date.Hour == 0) hour = 12;
            else hour = date.Hour;

            var forecast = Weather.Forecast.Where(x => x.DtTxt.Day == date.Day && x.DtTxt.Hour >= hour).FirstOrDefault();
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
