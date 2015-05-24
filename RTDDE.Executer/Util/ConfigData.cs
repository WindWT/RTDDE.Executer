using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using RTDDE.Provider;
using System.Xml;
using System.Xml.Serialization;

namespace RTDDE.Executer
{
    public class ConfigData : INotifyPropertyChanged
    {
        private GeneralClass _general;
        private MapClass _map;
        private DatabaseClass _database;

        public class GeneralClass : INotifyPropertyChanged
        {
            public bool IsEnableLevelLimiter
            {
                get { return _isEnableLevelLimiter; }
                set
                {
                    _isEnableLevelLimiter = value;
                    OnPropertyChanged("IsEnableLevelLimiter");
                }
            }

            public bool IsDefaultLvMax
            {
                get { return _isDefaultLvMax; }
                set
                {
                    _isDefaultLvMax = value;
                    OnPropertyChanged("IsDefaultLvMax");
                }
            }

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
                    OnPropertyChanged("IsUseLocalTime");
                }
            }
            private bool _isEnableLevelLimiter;
            private bool _isDefaultLvMax;
            private bool _isUseLocalTime = false;

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class MapClass : INotifyPropertyChanged
        {
            private bool _isShowDropInfo;
            private bool _isShowBoxInfo;
            private bool _isShowEnemyAttribute;
            private int _expValue;
            private string _expColor;
            private int _ptValue;
            private string _ptColor;
            private int _saleValue;
            private string _saleColor;
            private string _customDrop;

            public bool IsShowDropInfo
            {
                get { return _isShowDropInfo; }
                set
                {
                    _isShowDropInfo = value;
                    OnPropertyChanged("IsShowDropInfo");
                }
            }

            public bool IsShowBoxInfo
            {
                get { return _isShowBoxInfo; }
                set
                {
                    _isShowBoxInfo = value;
                    OnPropertyChanged("IsShowBoxInfo");
                }
            }

            public bool IsShowEnemyAttribute
            {
                get { return _isShowEnemyAttribute; }
                set
                {
                    _isShowEnemyAttribute = value;
                    OnPropertyChanged("IsShowEnemyAttribute");
                }
            }

            public int ExpValue
            {
                get { return _expValue; }
                set
                {
                    _expValue = value;
                    OnPropertyChanged("ExpValue");
                }
            }

            public string ExpColor
            {
                get { return _expColor; }
                set
                {
                    _expColor = value;
                    OnPropertyChanged("ExpColor");
                }
            }

            public int PtValue
            {
                get { return _ptValue; }
                set
                {
                    _ptValue = value;
                    OnPropertyChanged("PtValue");
                }
            }

            public string PtColor
            {
                get { return _ptColor; }
                set
                {
                    _ptColor = value;
                    OnPropertyChanged("PtColor");
                }
            }

            public int SaleValue
            {
                get { return _saleValue; }
                set
                {
                    _saleValue = value;
                    OnPropertyChanged("SaleValue");
                }
            }

            public string SaleColor
            {
                get { return _saleColor; }
                set
                {
                    _saleColor = value;
                    OnPropertyChanged("SaleColor");
                }
            }

            public string CustomDrop
            {
                get { return _customDrop; }
                set
                {
                    _customDrop = value;
                    OnPropertyChanged("CustomDrop");
                }
            }

            public void Reset()
            {
                ExpValue = 30000;
                ExpColor = "#FF9898";
                PtValue = 250;
                PtColor = "#9898FF";
                SaleValue = 20000;
                SaleColor = "Silver";
                CustomDrop = "15022,16027:Black";
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
        public class DatabaseClass : INotifyPropertyChanged
        {
            private bool _autoBackup;

            public bool AutoBackup
            {
                get { return _autoBackup; }
                set
                {
                    _autoBackup = value;
                    OnPropertyChanged("AutoBackup");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        public GeneralClass General
        {
            get { return _general; }
            set
            {
                _general = value;
                OnPropertyChanged("General");
            }
        }

        public MapClass Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged("Map");
            }
        }

        public DatabaseClass Database
        {
            get { return _database; }
            set
            {
                _database = value;
                OnPropertyChanged("Database");
            }
        }

        public ConfigData()
        {
            General = new GeneralClass()
            {
                IsDefaultLvMax = true,
                IsEnableLevelLimiter = true,
                IsUseLocalTime = false
            };
            Map = new MapClass()
            {
                IsShowDropInfo = false,
                IsShowBoxInfo = true,
                IsShowEnemyAttribute = true,
                ExpValue = 30000,
                ExpColor = "#FF9898",
                PtValue = 250,
                PtColor = "#9898FF",
                SaleValue = 20000,
                SaleColor = "Silver",
                CustomDrop = "15022,16027:Black"
            };
            Database = new DatabaseClass()
            {
                AutoBackup = false
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
