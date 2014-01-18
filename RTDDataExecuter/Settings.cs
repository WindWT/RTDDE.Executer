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
    public partial class MainWindow : Window
    {
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
                        StatusBarExceptionMessage.Text = ex.Message;
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
                        StatusBarExceptionMessage.Text = ex.Message;
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
                        StatusBarExceptionMessage.Text = ex.Message;
                    }
                }
            }
        }

        private bool isShowDropInfo = Properties.Settings.Default.IsShowDropInfo;
        public bool IsShowDropInfo
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
        private bool isShowBoxInfo = Properties.Settings.Default.IsShowBoxInfo;
        public bool IsShowBoxInfo
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
        private void InitSettings()
        {
            IsShowDropInfo = Properties.Settings.Default.IsShowDropInfo;
            IsShowBoxInfo = Properties.Settings.Default.IsShowBoxInfo;
            IsShowDropInfoCheckBox.IsChecked = IsShowDropInfo;
            IsShowBoxInfoCheckBox.IsChecked = IsShowBoxInfo;
        }
        private void IsShowDropInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsShowDropInfo = true;
        }
        private void IsShowDropInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsShowDropInfo = false;
        }
        private void IsShowBoxInfoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsShowBoxInfo = true;
        }
        private void IsShowBoxInfoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsShowBoxInfo = false;
        }
    }
}
