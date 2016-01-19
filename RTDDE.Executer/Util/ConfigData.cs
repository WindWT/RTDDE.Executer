using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Media;
using RTDDE.Provider;
using System.Xml;
using System.Xml.Serialization;

namespace RTDDE.Executer.Util
{
    public class ConfigData : INotifyPropertyChanged
    {
        private GeneralClass _general;
        private MapClass _map;
        private DatabaseClass _database;

        public class GeneralClass : INotifyPropertyChanged
        {
            private bool _isEnableLevelLimiter;
            private bool _isDefaultLvMax;
            private bool _isUseLocalTime;
            private bool _isForceEnglish;
            private bool _isForceWrapInStory;
            private bool _isShowColorTextAsBold;

            public bool IsEnableLevelLimiter {
                get { return _isEnableLevelLimiter; }
                set {
                    _isEnableLevelLimiter = value;
                    OnPropertyChanged("IsEnableLevelLimiter");
                }
            }

            public bool IsDefaultLvMax {
                get { return _isDefaultLvMax; }
                set {
                    _isDefaultLvMax = value;
                    OnPropertyChanged("IsDefaultLvMax");
                }
            }

            public bool IsUseLocalTime {
                get { return _isUseLocalTime; }
                set {
                    _isUseLocalTime = value;
                    Utility.UseLocalTime = _isUseLocalTime;
                    OnPropertyChanged("IsUseLocalTime");
                }
            }

            public bool IsForceEnglish {
                get { return _isForceEnglish; }
                set {
                    _isForceEnglish = value;
                    OnPropertyChanged("IsForceEnglish");
                }
            }

            public bool IsForceWrapInStory {
                get { return _isForceWrapInStory; }
                set {
                    _isForceWrapInStory = value;
                    OnPropertyChanged("IsForceWrapInStory");
                }
            }

            public bool IsShowColorTextAsBold {
                get { return _isShowColorTextAsBold; }
                set {
                    _isShowColorTextAsBold = value;
                    OnPropertyChanged("IsShowColorTextAsBold");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class MapClass : INotifyPropertyChanged
        {
            private bool _isShowDropInfo;
            private bool _isForceShowDropInfo;
            private bool _isShowBoxInfo;
            private bool _isShowEnemyAttribute;
            private int _expValue;
            private string _expColorValue;
            private int _ptValue;
            private string _ptColorValue;
            private int _saleValue;
            private string _saleColorValue;
            private string _customDrop;

            public bool IsShowDropInfo {
                get { return _isShowDropInfo; }
                set {
                    _isShowDropInfo = value;
                    OnPropertyChanged("IsShowDropInfo");
                }
            }

            public bool IsForceShowDropInfo
            {
                get { return _isForceShowDropInfo; }
                set
                {
                    _isForceShowDropInfo = value;
                    OnPropertyChanged("IsForceShowDropInfo");
                }
            }

            public bool IsShowBoxInfo {
                get { return _isShowBoxInfo; }
                set {
                    _isShowBoxInfo = value;
                    OnPropertyChanged("IsShowBoxInfo");
                }
            }

            public bool IsShowEnemyAttribute {
                get { return _isShowEnemyAttribute; }
                set {
                    _isShowEnemyAttribute = value;
                    OnPropertyChanged("IsShowEnemyAttribute");
                }
            }

            public int ExpValue {
                get { return _expValue; }
                set {
                    _expValue = value;
                    OnPropertyChanged("ExpValue");
                }
            }

            public string ExpColorValue {
                get { return _expColorValue; }
                set {
                    _expColorValue = value;
                    OnPropertyChanged("ExpColorValue");
                }
            }

            public Color ExpColor {
                get { return (Color) (ColorConverter.ConvertFromString(ExpColorValue) ?? Colors.Transparent); }
            }

            public int PtValue {
                get { return _ptValue; }
                set {
                    _ptValue = value;
                    OnPropertyChanged("PtValue");
                }
            }

            public string PtColorValue {
                get { return _ptColorValue; }
                set {
                    _ptColorValue = value;
                    OnPropertyChanged("PtColorValue");
                }
            }

            public Color PtColor {
                get { return (Color) (ColorConverter.ConvertFromString(PtColorValue) ?? Colors.Transparent); }
            }

            public int SaleValue {
                get { return _saleValue; }
                set {
                    _saleValue = value;
                    OnPropertyChanged("SaleValue");
                }
            }

            public string SaleColorValue {
                get { return _saleColorValue; }
                set {
                    _saleColorValue = value;
                    OnPropertyChanged("SaleColorValue");
                }
            }

            public Color SaleColor {
                get { return (Color) (ColorConverter.ConvertFromString(SaleColorValue) ?? Colors.Transparent); }
            }

            public string CustomDrop {
                get { return _customDrop; }
                set {
                    _customDrop = value;
                    OnPropertyChanged("CustomDrop");
                }
            }

            [XmlIgnore]
            public Dictionary<int, Color> CustomDropColors {
                get {
                    Dictionary<int, Color> customDropColors = new Dictionary<int, Color>();
                    string[] customs = CustomDrop.Split(';');
                    foreach (var custom in customs) {
                        string[] split = custom.Split(':');
                        if (split.Length != 2) {
                            //异常数据
                            return null;
                        }
                        string colorString = split[1];
                        object colorObj;
                        try {
                            colorObj = ColorConverter.ConvertFromString(colorString);
                        }
                        catch (Exception) {
                            return null;
                        }
                        Color color = (Color) colorObj;
                        foreach (string idString in split[0].Split(',')) {
                            int id;
                            if (int.TryParse(idString, out id) == false) {
                                return null;
                            }
                            customDropColors.Add(id, color);
                        }
                    }
                    return customDropColors;
                }
            }

            public void Reset() {
                ExpValue = 30000;
                ExpColorValue = "#FF9898";
                PtValue = 250;
                PtColorValue = "#9898FF";
                SaleValue = 20000;
                SaleColorValue = "Silver";
                CustomDrop = "15022,16027:Black";
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class DatabaseClass : INotifyPropertyChanged
        {
            private bool _autoBackup;

            public bool AutoBackup {
                get { return _autoBackup; }
                set {
                    _autoBackup = value;
                    OnPropertyChanged("AutoBackup");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public ConfigData() {
            General = new GeneralClass() {
                IsDefaultLvMax = true,
                IsEnableLevelLimiter = true,
                IsUseLocalTime = false,
                IsForceWrapInStory = true,
                IsShowColorTextAsBold = true,
                IsForceEnglish = false
            };
            Map = new MapClass() {
                IsShowDropInfo = false,
                IsForceShowDropInfo = false,
                IsShowBoxInfo = true,
                IsShowEnemyAttribute = true,
                ExpValue = 30000,
                ExpColorValue = "#FF9898",
                PtValue = 250,
                PtColorValue = "#9898FF",
                SaleValue = 20000,
                SaleColorValue = "#989898",
                CustomDrop = "15022,16027:#222222;15026:#009800"
            };
            Database = new DatabaseClass() {
                AutoBackup = false
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
