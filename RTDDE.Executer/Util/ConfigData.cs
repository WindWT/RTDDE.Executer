using System;
using System.IO;
using System.Text;
using RTDDE.Provider;
using System.Xml;
using System.Xml.Serialization;

namespace RTDDE.Executer
{
    public class ConfigData
    {
        public class GeneralClass
        {
            public bool IsShowDropInfo { get; set; }
            public bool IsShowBoxInfo { get; set; }
            public bool IsEnableLevelLimiter { get; set; }
            public bool IsDefaultLvMax { get; set; }
            public bool IsUseLocalTime
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
            private bool _isUseLocalTime = false;
        }
        public class ModelClass
        {
            public string DisunityPath { get; set; }
        }
        public class DatabaseClass
        {
            public bool AutoBackup { get; set; }
        }
        public GeneralClass General;
        public ModelClass Model;
        public DatabaseClass Database;

        public ConfigData()
        {
            General = new GeneralClass()
            {
                IsShowDropInfo = false,
                IsShowBoxInfo = true,
                IsDefaultLvMax = true,
                IsEnableLevelLimiter = true,
                IsUseLocalTime = false
            };
            Model = new ModelClass()
            {
                DisunityPath = string.Empty
            };
            Database = new DatabaseClass()
            {
                AutoBackup = false
            };
        }
    }
}
