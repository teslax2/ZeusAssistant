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
        /// <summary>
        /// Http Get Request to OpenWeatherApi
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        private async Task GetAsync (string City)
        {
            try
            {
<<<<<<< HEAD
                var fullPath = _path + City + "&lang=pl&units=metric&APPID=" + _token;
                _httpClient.DefaultRequestHeaders.Clear();
                var response = await _httpClient.GetAsync(fullPath);
                var content = await response.Content.ReadAsStringAsync();
                System.IO.File.WriteAllText("weather.tmp", content);
                Weather = JsonConvert.DeserializeObject<WeatherResponse>(content);
=======
                if (TimeSinceLastRefresh.Hours < 1 || true)
                {
                    if (System.IO.File.Exists("weather.tmp"))
                    {
                        var weatherSaved = System.IO.File.ReadAllText("weather.tmp",Encoding.UTF8);
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
                
>>>>>>> master4
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, "Failed to get weather");
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
            if (string.IsNullOrEmpty(City)) return string.Empty;

            var weatherTmpPath = "weather.tmp";
            try
            {
                if (System.IO.File.Exists(weatherTmpPath))
                {
                    var creationTime = System.IO.File.GetLastWriteTime(weatherTmpPath);
                    var timeFromCreation = DateTime.Now - creationTime;
                    if (timeFromCreation < TimeSpan.FromHours(3) || !Utilities.CheckForInternetConnection())
                    {
                        var serializedContent = System.IO.File.ReadAllText(weatherTmpPath);
                        Weather = JsonConvert.DeserializeObject<WeatherResponse>(serializedContent);
                    }
                    else
                        await GetAsync(City);
                }
                else
                    await GetAsync(City);

            }
            catch (Exception ex)
            {

                logger.Error(ex, "Failed to acquire weather");
            }

            if (Weather == null)
                return "";
            string forecastString;

            //no hour specified
            if (date.Hour == 0)
            {
                forecastString = GetAverageForDay(date);
            }
            //hour specified
            else
            {
                int hour = 0;
                if (date.Hour > 21) hour = 21;
                else hour = date.Hour;
                forecastString = GetForSpecificHour(date, hour);
            }
            logger.Info(forecastString);
            return forecastString;
        }

        private string GetForSpecificHour(DateTime date, int hour)
        {
            var forecast = Weather.Forecast.Where(x => x.DtTxt.Day == date.Day && x.DtTxt.Hour >= hour).FirstOrDefault();
            if (forecast == null) return string.Empty;
            var forecastString = string.Format("{0} będzie {1}, temperatura {2} stopni, wilgotność {3} procent, zachmurzenie {4} procent",
                GetDay(date), forecast.Weather[0].Description, forecast.Main.Temp, forecast.Main.Humidity, forecast.Clouds.All);
            return forecastString;
        }

        private string GetAverageForDay(DateTime date)
        {
            try
            {
                var forecasts = Weather.Forecast.Where(x => x.DtTxt.Day == date.Day && x.DtTxt.Hour >= 9 && x.DtTxt.Hour <= 18).ToList();
                if (forecasts.Count > 0)
                {
                    var maxTemp =(int) forecasts.Max(x => x.Main.TempMax);
                    var minTemp =(int) forecasts.Min(x => x.Main.TempMin);
                    //list with hours 9 to 18, so [1] will be 12:00
                    var description = forecasts[1].Weather[0].Description;
                    var humidity = forecasts[1].Main.Humidity;
                    var clouds = forecasts[1].Clouds.All;
                    return string.Format("{0} bedzie {1}, temperatura minimalna {2} stopni, maksymalna {3} stopni, wilgotnosc {4} procent, zachmurzenie {5} procent",
                        GetDay(date), description, minTemp, maxTemp, humidity, clouds);
                }
                else
                    return string.Empty;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get average weather");
                return string.Empty;
            }
        }

        private string GetDay(DateTimeOffset date)
        {
            var today = DateTimeOffset.Now;
            var timeSpan = date.Day - today.Day;
            switch (timeSpan)
            {
                case 0:
                    {
                        if (date.Hour == 0) return "dzisiaj";
                        else return "dzisiaj o " + date.Hour.ToString() + "-ej";
                    }  
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
