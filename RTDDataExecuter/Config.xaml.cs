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
            ImportMDBSButton.Content = new Run("Import MDBS(NOT available for 2.4.0.0 or higher)");
            ImportLDBSButton.Content = new Run("Import LDBS(NOT available for 2.5.0.0 or higher)");
            ImportplistButton.Content = new Run("Import plist(NOT available for 2.4.0.0 or higher)");
            ImportAndroidDirectoryButton.Content = new Run("Import Android MDBS Directory");
            ImportiOSDirectoryButton.Content = new Run("Import iOS MDBS Directory");
            ImportAndroidGAMEButton.Content = new Run("Import GAME(MAP Data)");
            ImportiOSGAMEButton.Content = new Run("Import GAME(MAP Data)");
        }
        #region Android
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
                        DataSet ds = FileParser.ParseXmlMDB(xmlMDB);
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
                            RefreshControl();
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
                        DataTable dt = FileParser.ParseXmlLDB(xmlLDB);
                        DB db = new DB();
                        db.ImportDataTable(dt, "level_data_id", false);
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
                            db.ImportDataTable(dt, true);
                        }
                    }
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
                        RefreshControl();
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportAndroidGAMEButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "GAME File|GAME.xml";
            if (ofd.ShowDialog() == true)
            {
                ImportAndroidGAMEButton.Content = new Run("Importing MAP Data...");
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        string xml = sr.ReadToEnd();
                        DataTable dt = FileParser.ParseXmlLDB(xml);
                        DB db = new DB();
                        db.ImportDataTable(dt, "level_data_id", false);
                        db.ImportDataSet(FileParser.ParseXmlGAME(xml), false);
                    }
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportAndroidGAMEButton.Content = new Run("MAP Data Import Failed.");
                    }
                    else
                    {
                        ImportAndroidGAMEButton.Content = new Run("MAP Data Successfully Imported.");
                        RefreshControl();
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        #endregion

        #region iOS
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
                        DataSet ds = FileParser.ParsePlistMDB(sr.BaseStream);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);

                        sr.BaseStream.Position = 0;

                        DataTable dt = FileParser.ParsePlistLDB(sr.BaseStream);
                        db.ImportDataTable(dt, "level_data_id", false);
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
                        RefreshControl();
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

                Task task = new Task(() =>
                {
                    DB db = new DB();
                    foreach (string filepath in System.IO.Directory.GetFiles(path))
                    {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename.StartsWith("MDBS"))
                        {
                            using (StreamReader sr = new StreamReader(filepath))
                            {
                                DataTable dt = FileParser.ParsePlistFileMDB(sr.BaseStream);
                                db.ImportDataTable(dt, true);
                            }
                        }
                        else if (filename.StartsWith("LDBS"))
                        {
                            using (StreamReader sr = new StreamReader(filepath))
                            {
                                DataTable dt = FileParser.ParsePlistFileLDB(sr.BaseStream);
                                db.ImportDataTable(dt, "level_data_id", false);
                            }
                        }
                        else
                        {
                        }
                    }
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
                        RefreshControl();
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        private void ImportiOSGAMEButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "GAME File|GAME";
            if (ofd.ShowDialog() == true)
            {
                ImportiOSGAMEButton.Content = new Run("Importing MAP Data...");
                Task task = new Task(() =>
                {
                    DB db = new DB();
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        DataTable dt = FileParser.ParsePlistFileLDB(sr.BaseStream);
                        db.ImportDataTable(dt, "level_data_id", false);
                    }
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        db.ImportDataSet(FileParser.ParsePlistFileGAME(sr.BaseStream), false);
                    }
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportiOSGAMEButton.Content = new Run("MAP Data Import Failed.");
                    }
                    else
                    {
                        ImportiOSGAMEButton.Content = new Run("MAP Data Successfully Imported.");
                        RefreshControl();
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }
        #endregion

        private void RefreshControl()
        {
            var w = (MainWindow)Application.Current.MainWindow;
            w.Quest.Refresh();
            w.QuestCategory.Refresh();
            w.Unit.Refresh();
            w.Skill.Refresh();
            w.Guide.Refresh();
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
