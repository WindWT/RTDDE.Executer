using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Configuration;
using System.ComponentModel;
using System.Linq.Expressions;

namespace RTDDataExecuter
{
    public static class Settings
    {
        private static bool isShowDropInfo = Properties.Settings.Default.IsShowDropInfo;
        public static bool IsShowDropInfo
        {
            get
            {
                return isShowDropInfo;
            }
            set
            {
                isShowDropInfo = value;
                Properties.Settings.Default.IsShowDropInfo = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool isShowBoxInfo = Properties.Settings.Default.IsShowBoxInfo;
        public static bool IsShowBoxInfo
        {
            get
            {
                return isShowBoxInfo;
            }
            set
            {
                isShowBoxInfo = value;
                Properties.Settings.Default.IsShowBoxInfo = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool isEnableLevelLimiter = Properties.Settings.Default.IsEnableLevelLimiter;
        public static bool IsEnableLevelLimiter
        {
            get
            {
                return isEnableLevelLimiter;
            }
            set
            {
                isEnableLevelLimiter = value;
                Properties.Settings.Default.IsEnableLevelLimiter = value;
                Properties.Settings.Default.Save();
            }
        }
        private static bool isDefaultLvMax = Properties.Settings.Default.IsDefaultLvMax;
        public static bool IsDefaultLvMax
        {
            get
            {
                return isDefaultLvMax;
            }
            set
            {
                isDefaultLvMax = value;
                Properties.Settings.Default.IsDefaultLvMax = value;
                Properties.Settings.Default.Save();
            }
        }
    }
}
