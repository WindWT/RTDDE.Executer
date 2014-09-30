using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Xml;

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
            ImportAndroidGAMEButton.Content = new Run("Import GAME(MAP Data)");
            ImportiOSGAMEButton.Content = new Run("Import GAME(MAP Data)");
            ImportMsgPackButton.Content = new Run("Import MDBS MsgPack");
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
                    XmlDocument xmlGame = new XmlDocument();
                    xmlGame.Load(ofd.FileName);
                    foreach (XmlNode xmlNode in xmlGame.GetElementsByTagName("string"))
                    {
                        if (xmlNode.Attributes["name"] != null && xmlNode.Attributes["name"].Value.StartsWith("LDBS"))
                        {
                            string jsonGAME = xmlNode.InnerText;
                            foreach (KeyValuePair<Type, string> kv in JSON.GetJSONFromGAME(jsonGAME))
                            {
                                Type type = kv.Key;
                                string json = kv.Value;
                                //Generate Method
                                MethodInfo methodToList = typeof(JSON).GetMethod("ToSingle").MakeGenericMethod(type);
                                MethodInfo methodToDB = typeof(DAL).GetMethod("FromSingle").MakeGenericMethod(type);
                                //Invoke
                                var data = methodToList.Invoke(null, new object[] { json });
                                methodToDB.Invoke(null, new object[] { data });
                            }
                        }
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
        private void ImportiOSGAMEButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "GAME File|GAME";
            if (ofd.ShowDialog() == true)
            {
                ImportiOSGAMEButton.Content = new Run("Importing MAP Data...");
                Task task = new Task(() =>
                {
                    //DB db = new DB();
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    //{
                    //    DataTable dt = FileParser.ParsePlistFileLDB(sr.BaseStream);
                    //    db.ImportDataTable(dt, "level_data_id", false);
                    //}
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    //{
                    //    db.ImportDataSet(FileParser.ParsePlistFileGAME(sr.BaseStream), false);
                    //}
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
        private void ImportMsgPackButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.CheckFileExists = false;
            ofd.ValidateNames = false;
            ofd.FileName = "Ignore me";
            ofd.Title = "Browse Folder Then Press Open";
            if (ofd.ShowDialog() == true)
            {
                ImportMsgPackButton.Content = new Run("Importing MDBS MsgPack...");
                string path = System.IO.Path.GetDirectoryName(ofd.FileName);
                Task task = new Task(() =>
                {
                    foreach (string filepath in System.IO.Directory.GetFiles(path))
                    {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename.EndsWith("_Msg.bytes") == false)
                        {
                            continue;
                        }
                        string enumName = filename.Replace("_Msg.bytes", string.Empty);
                        MASTERDB mdbEnum = (MASTERDB)Enum.Parse(typeof(MASTERDB), enumName, true);
                        string json = string.Empty;
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            json = MsgBytes.ToJson(sr.BaseStream);
                        }
                        //Dynamic type from enum
                        Type currentType = Utility.Enum2Type(mdbEnum);
                        //Generate Method
                        MethodInfo methodToList = typeof(JSON).GetMethod("ToList").MakeGenericMethod(currentType);
                        MethodInfo methodToDB = typeof(DAL).GetMethod("FromList").MakeGenericMethod(currentType);
                        //Invoke
                        var list = methodToList.Invoke(null, new object[] { json });
                        //Drop table, 
                        DAL.DropTable(enumName);
                        methodToDB.Invoke(null, new object[] { list });
                    }
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportMsgPackButton.Content = new Run("MDBS MsgPack Import Failed.");
                    }
                    else
                    {
                        ImportMsgPackButton.Content = new Run("MDBS MsgPack Successfully Imported.");
                        RefreshControl();
                    }
                }, MainWindow.uiTaskScheduler);
                task.Start();
            }
        }

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
