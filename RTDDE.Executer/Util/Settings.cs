using System;
using System.IO;
using System.Text;
using RTDDE.Provider;
using System.Xml;
using System.Xml.Serialization;

namespace RTDDE.Executer.Util
{
    public static class Settings
    {
        private static readonly string CONFIG_FILENAME = "config.xml";
        public static ConfigData Config;
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigData));
        /// <summary>
        /// Init Settings
        /// </summary>
        public static void Init()
        {
            if (File.Exists(CONFIG_FILENAME) == false) {
                Config = new ConfigData();
                Settings.Save();
                return;
            }
            try {
                using (TextReader tr = new StreamReader(CONFIG_FILENAME)) {
                    Config = (ConfigData)Serializer.Deserialize(tr);
                }
            }
            catch (Exception) {
                //config file read error
                Config = new ConfigData();
                Settings.Save();
                Utility.ShowMessage("Config ERROR, use default config.");
            }
        }
        public static void Save()
        {
            using (TextWriter tw = new StreamWriter(CONFIG_FILENAME, false)) {
                Serializer.Serialize(tw, Config);
            }
        }
    }
}
