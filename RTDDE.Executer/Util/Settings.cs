using IniParser;
using IniParser.Model;
using System;
using System.IO;
using System.Text;

namespace RTDDE.Executer
{
    public static class Settings
    {
        public static bool IsShowDropInfo { get; set; }
        public static bool IsShowBoxInfo { get; set; }
        public static bool IsEnableLevelLimiter { get; set; }
        public static bool IsDefaultLvMax { get; set; }
        public static bool IsUseLocalTime
        {
            get
            {
                return isUseLocalTime;
            }
            set
            {
                isUseLocalTime = value;
                Utility.UseLocalTime = isUseLocalTime;
            }
        }
        private static bool isUseLocalTime = false;
        public static string DisunityPath { get; set; }
        public static string AdbPath { get; set; }
        private static IniData data = new IniData();
        public static readonly string CONFIG_FILENAME = "config.ini";
        static Settings()
        {
            if (File.Exists(CONFIG_FILENAME) == false)
            {
                data = initConfigFile();
            }
            else
            {
                FileIniDataParser parser = new FileIniDataParser();
                data = parser.ReadFile(CONFIG_FILENAME);
            }
            IsShowDropInfo = Convert.ToBoolean(data["Common"]["IsShowDropInfo"]);
            IsShowBoxInfo = Convert.ToBoolean(data["Common"]["IsShowBoxInfo"]);
            IsEnableLevelLimiter = Convert.ToBoolean(data["Common"]["IsEnableLevelLimiter"]);
            IsDefaultLvMax = Convert.ToBoolean(data["Common"]["IsDefaultLvMax"]);
            IsUseLocalTime = Convert.ToBoolean(data["Common"]["IsUseLocalTime"]);
            DisunityPath = data["Common"]["DisunityPath"];
            AdbPath = data["Common"]["AdbPath"];
        }
        public static void Save()
        {
            data["Common"]["IsShowDropInfo"] = IsShowDropInfo.ToString();
            data["Common"]["IsShowBoxInfo"] = IsShowBoxInfo.ToString();
            data["Common"]["IsEnableLevelLimiter"] = IsEnableLevelLimiter.ToString();
            data["Common"]["IsDefaultLvMax"] = IsDefaultLvMax.ToString();
            data["Common"]["IsUseLocalTime"] = IsUseLocalTime.ToString();
            data["Common"]["DisunityPath"] = DisunityPath;
            data["Common"]["AdbPath"] = AdbPath;
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(CONFIG_FILENAME, data);
        }
        private static IniData initConfigFile()
        {
            IniData initData = new IniData();
            initData.Sections.AddSection("Common");
            initData["Common"].AddKey("IsShowDropInfo", "false");
            initData["Common"].AddKey("IsShowBoxInfo", "true");
            initData["Common"].AddKey("IsEnableLevelLimiter", "true");
            initData["Common"].AddKey("IsDefaultLvMax", "true");
            initData["Common"].AddKey("IsUseLocalTime", "false");
            initData["Common"].AddKey("DisunityPath", "");
            initData["Common"].AddKey("AdbPath", "");
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(CONFIG_FILENAME, initData);
            return initData;
        }
    }
}
