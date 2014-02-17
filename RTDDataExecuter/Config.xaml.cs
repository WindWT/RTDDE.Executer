using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
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
        private void ConfigTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ImportMDBSButton.Content = new Run("Import MDBS");
            ImportLDBSButton.Content = new Run("Import LDBS");
            ImportplistButton.Content = new Run("Import plist");
            ImportAndroidDirectoryButton.Content = new Run("Import Android Directory(LDBS not included)");
            ImportiOSDirectoryButton.Content = new Run("Import iOS Directory");
        }
        private void ImportMDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "MDBS File|MDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                ImportMDBSButton.Content = new Run("Importing MDBS...");
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        string xmlMDB = sr.ReadToEnd();
                        DataSet ds = XMLParser.ParseMDB(xmlMDB);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);
                    }
                });
                task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            Utility.ShowException(t.Exception.InnerException.Message);
                            ImportMDBSButton.Content = new Run("MDBS Import Failed.");
                        }
                        else
                        {
                            ImportMDBSButton.Content = new Run("MDBS Successfully Imported.");
                            var w = (MainWindow)Application.Current.MainWindow;
                            w.Unit.UnitSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                            w.Quest.QuestSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        }
                    }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportLDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "LDBS File|LDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                ImportLDBSButton.Content = new Run("Importing LDBS...");
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        string xmlLDB = sr.ReadToEnd();
                        DataTable dt = XMLParser.ParseLDB(xmlLDB);
                        DataSet lds = new DataSet("LDB");
                        lds.Tables.Add(dt);
                        DB db = new DB();
                        db.ImportDataSet(lds, false);
                    }
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportLDBSButton.Content = new Run("LDBS Import Failed.");
                    }
                    else
                    {
                        ImportLDBSButton.Content = new Run("LDBS Successfully Imported.");
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportplistButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".plist";
            ofd.Filter = "plist File|*.plist";
            if (ofd.ShowDialog() == true)
            {
                ImportplistButton.Content = new Run("Importing plist...");
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
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
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportplistButton.Content = new Run("plist Import Failed.");
                    }
                    else
                    {
                        ImportplistButton.Content = new Run("plist Successfully Imported.");
                        var w = (MainWindow)Application.Current.MainWindow;
                        w.Unit.UnitSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        w.Quest.QuestSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportAndroidDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.ValidateNames = false;
            ofd.FileName = "Ignore me";
            ofd.Title = "Browse Folder Then Press Open";
            if (ofd.ShowDialog() == true)
            {
                ImportAndroidDirectoryButton.Content = new Run("Importing MDBS...");
                string path = System.IO.Path.GetDirectoryName(ofd.FileName);
                //MessageBox.Show(path);
                Task task = new Task(() =>
                {
                    DataSet ds = new DataSet("MDB");
                    DB db = new DB();
                    foreach (string filepath in System.IO.Directory.GetFiles(path))
                    {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename.StartsWith("MDBS") == false)
                        {
                            continue;
                        }
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            string jsonMDB = sr.ReadToEnd();
                            string enumName = filename.Substring("MDBS".Length);
                            DataTable dt = JSON.ParseJSONMDB(jsonMDB, (MASTERDB)Enum.Parse(typeof(MASTERDB), enumName, true));
                            ds.Tables.Add(dt);
                        }
                    }
                    db.ImportDataSet(ds, true);
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportAndroidDirectoryButton.Content = new Run("MDBS Import Failed.");
                    }
                    else
                    {
                        ImportAndroidDirectoryButton.Content = new Run("MDBS Successfully Imported.");
                        var w = (MainWindow)Application.Current.MainWindow;
                        w.Unit.UnitSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        w.Quest.QuestSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportiOSDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.ValidateNames = false;
            ofd.FileName = "Ignore me";
            ofd.Title = "Browse Folder Then Press Open";
            if (ofd.ShowDialog() == true)
            {
                ImportiOSDirectoryButton.Content = new Run("Importing iOS...");
                string path = System.IO.Path.GetDirectoryName(ofd.FileName);
                //MessageBox.Show(path);
                Task task = new Task(() =>
                {
                    DataSet ds = new DataSet("MDB");
                    DB db = new DB();
                    foreach (string filepath in System.IO.Directory.GetFiles(path))
                    {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename.StartsWith("MDBS") == false)
                        {
                            continue;
                        }
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            string jsonMDB = sr.ReadToEnd();
                            string enumName = filename.Substring("MDBS_".Length) + "_MASTER";   //diff from android
                            DataTable dt = JSON.ParseJSONMDB(jsonMDB, (MASTERDB)Enum.Parse(typeof(MASTERDB), enumName, true));
                            ds.Tables.Add(dt);
                        }
                    }
                    db.ImportDataSet(ds, true);
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportiOSDirectoryButton.Content = new Run("iOS Import Failed.");
                    }
                    else
                    {
                        ImportiOSDirectoryButton.Content = new Run("iOS Successfully Imported.");
                        var w = (MainWindow)Application.Current.MainWindow;
                        w.Unit.UnitSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        w.Quest.QuestSearchClear.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
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
