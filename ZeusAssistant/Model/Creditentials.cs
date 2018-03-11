using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZeusAssistant.Model
{
    public class Creditentials
    {
        public List<Credit> Credits { get; set; }
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Creditentials()
        {
            Credits = new List<Credit>() { new Credit() { Provider = ApiProvider.WitAi, Path = "dupa", Token = "dupa" } };
        }

        public void Load()
        {
            try
            {
                var serializedObject = File.ReadAllText("Apicredits.xxx");
                var creds = JsonConvert.DeserializeObject<Creditentials>(serializedObject);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Couldnt load creditentials");
            }
        }

        public void Save()
        {
            try
            {
                var serializedObject = JsonConvert.SerializeObject(this);
                File.WriteAllText("credits.xxx", serializedObject);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Couldnt save creditentials");
            }
        }
    }

    public class Credit
    {
        public ApiProvider Provider { get; set; }
        public string Path { get; set; }
        public string Token { get; set; }
    }

    public enum ApiProvider
    {
        WitAi,
        OpenWeatherMap,
    }
}
