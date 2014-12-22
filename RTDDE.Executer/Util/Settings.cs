using IniParser;
using IniParser.Model;
using System;
using System.IO;
using System.Text;
using RTDDE.Provider;

namespace RTDDE.Executer
{
    public static class Settings
    {
        #region Settings

        public static bool IsShowDropInfo { get; set; }
        public static bool IsShowBoxInfo { get; set; }
        public static bool IsEnableLevelLimiter { get; set; }
        public static bool IsDefaultLvMax { get; set; }
        public static bool IsUseLocalTime
        {
            get
            {
                return _isUseLocalTime;
            }
            set
            {
                _isUseLocalTime = value;
                Utility.UseLocalTime = _isUseLocalTime;
            }
        }
        private static bool _isUseLocalTime = false;
        public static string DisunityPath { get; set; }
        public static string DatabaseName { get; set; }
        private static string _connectionString;
        public static string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                DAL.ConnectionString = _connectionString;
            }
        }

        #endregion

        private static readonly IniData Data;
        public static readonly string CONFIG_FILENAME = "config.ini";

        static Settings()
        {
            if (File.Exists(CONFIG_FILENAME) == false)
            {
                Data = InitConfigFile();
            }
            else
            {
                FileIniDataParser parser = new FileIniDataParser();
                Data = parser.ReadFile(CONFIG_FILENAME);
            }
            try
            {
                IsShowDropInfo = Convert.ToBoolean(Data["General"]["IsShowDropInfo"]);
                IsShowBoxInfo = Convert.ToBoolean(Data["General"]["IsShowBoxInfo"]);
                IsEnableLevelLimiter = Convert.ToBoolean(Data["General"]["IsEnableLevelLimiter"]);
                IsDefaultLvMax = Convert.ToBoolean(Data["General"]["IsDefaultLvMax"]);
                IsUseLocalTime = Convert.ToBoolean(Data["General"]["IsUseLocalTime"]);
                DisunityPath = Data["Model"]["DisunityPath"];
                ConnectionString = Data["Database"]["ConnectionString"];
            }
            catch (Exception ex)
            {
                //config file read error
                Data = InitConfigFile();
                Utility.ShowException("Config ERROR, use default config.");
            }
        }
        public static void Save()
        {
            Data["General"]["IsShowDropInfo"] = IsShowDropInfo.ToString();
            Data["General"]["IsShowBoxInfo"] = IsShowBoxInfo.ToString();
            Data["General"]["IsEnableLevelLimiter"] = IsEnableLevelLimiter.ToString();
            Data["General"]["IsDefaultLvMax"] = IsDefaultLvMax.ToString();
            Data["General"]["IsUseLocalTime"] = IsUseLocalTime.ToString();
            Data["Model"]["DisunityPath"] = DisunityPath;
            Data["Database"]["ConnectionString"] = ConnectionString;
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(CONFIG_FILENAME, Data);
        }
        private static IniData InitConfigFile()
        {
            IniData initData = new IniData();
            initData.Sections.AddSection("General");
            initData["General"].AddKey("IsShowDropInfo", "false");
            initData["General"].AddKey("IsShowBoxInfo", "true");
            initData["General"].AddKey("IsEnableLevelLimiter", "true");
            initData["General"].AddKey("IsDefaultLvMax", "true");
            initData["General"].AddKey("IsUseLocalTime", "false");
            initData.Sections.AddSection("Model");
            initData["Model"].AddKey("DisunityPath", string.Empty);
            initData.Sections.AddSection("Database");
            initData["Database"].AddKey("ConnectionString", "Data Source=RTD.db");
            FileIniDataParser parser = new FileIniDataParser();
            parser.WriteFile(CONFIG_FILENAME, initData);
            return initData;
        }
    }
}
