using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ZeusAssistant.Model
{
    class Creditentials
    {
        public string WitAiPath { get; set; } = string.Empty;
        public string WitAiToken { get; set; } = string.Empty;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public Creditentials()
        {
        }

        public void Load()
        {
            try
            {
                var serializedObject = File.ReadAllText("credits.xxx");
                var creds = JsonConvert.DeserializeObject<Creditentials>(serializedObject);
                WitAiPath = creds.WitAiPath;
                WitAiToken = creds.WitAiToken;
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
}
