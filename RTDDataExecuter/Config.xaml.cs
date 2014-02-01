using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDDataExecuter
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config : UserControl
    {
        public Config()
        {
            InitializeComponent();
        }
        private void ImportMDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "MDBS File|MDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlMDB = sr.ReadToEnd();
                    try
                    {
                        DataSet ds = XMLParser.ParseMDB(xmlMDB);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);
                    }
                    catch (Exception ex)
                    {
                        Utility.LogException(ex.Message);
                    }
                }
            }
        }
        private void ImportLDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "LDBS File|LDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlLDB = sr.ReadToEnd();
                    try
                    {
                        DataTable dt = XMLParser.ParseLDB(xmlLDB);
                        DataSet lds = new DataSet("LDB");
                        lds.Tables.Add(dt);
                        DB db = new DB();
                        db.ImportDataSet(lds, false);
                    }
                    catch (Exception ex)
                    {
                        Utility.LogException(ex.Message);
                    }
                }
            }
        }
        private void ImportplistButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".plist";
            ofd.Filter = "plist File|*.plist";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    try
                    {
                        DataSet ds = XMLParser.ParsePlistMDB(sr.BaseStream);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);

                        sr.BaseStream.Position = 0;

                        DataTable dt = XMLParser.ParsePlistLDB(sr.BaseStream);
                        DataSet lds = new DataSet("LDB");
                        lds.Tables.Add(dt);
                        db.ImportDataSet(lds, false);
                    }
                    catch (Exception ex)
                    {
                        Utility.LogException(ex.Message);
                    }
                }
            }
        }
        public void InitSettings()
        {
            Settings.IsShowDropInfo = Properties.Settings.Default.IsShowDropInfo;
            Settings.IsShowBoxInfo = Properties.Settings.Default.IsShowBoxInfo;
            Settings.IsEnableLevelLimiter = Properties.Settings.Default.IsEnableLevelLimiter;
            Settings.IsDefaultLvMax = Properties.Settings.Default.IsDefaultLvMax;
            IsShowDropInfoCheckBox.IsChecked = Settings.IsShowDropInfo;
            IsShowBoxInfoCheckBox.IsChecked = Settings.IsShowBoxInfo;
            IsEnableLevelLimiterCheckBox.IsChecked = Settings.IsEnableLevelLimiter;
            IsDefaultLvMaxCheckBox.IsChecked = Settings.IsDefaultLvMax;
        }
        private void IsShowDropInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsShowDropInfo = true;
        }
        private void IsShowDropInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsShowDropInfo = false;
        }
        private void IsShowBoxInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsShowBoxInfo = true;
        }
        private void IsShowBoxInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsShowBoxInfo = false;
        }
        private void IsEnableLevelLimiterCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsEnableLevelLimiter = true;
        }
        private void IsEnableLevelLimiterCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsEnableLevelLimiter = false;
        }
        private void IsDefaultLvMaxCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Settings.IsDefaultLvMax = true;
        }
        private void IsDefaultLvMaxCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Settings.IsDefaultLvMax = false;
        }
    }
}
