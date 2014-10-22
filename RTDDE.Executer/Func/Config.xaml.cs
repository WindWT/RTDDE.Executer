using RTDDE.Provider;
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

namespace RTDDE.Executer
{
    /// <summary>
    /// Config.xaml 的交互逻辑
    /// </summary>
    public partial class Config : UserControl
    {
        public Config()
        {
            InitializeComponent();
            InitSettings();
        }
        private void ConfigTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ImportAndroidGAMEButton.Content = new Run("Import GAME(MAP Data)");
            ImportiOSGAMEButton.Content = new Run("Import GAME(MAP Data)");
            ImportMsgPackButton.Content = new Run("Import MDBS MsgPack");
            SaveSettingsButton.Content = new Run("Save Settings");
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
                            var game = new MapData(jsonGAME);
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.UnitTalkMaster>(game.UTM));
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.LevelDataMaster>(game.LDM));
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.EnemyTableMaster>(game.ETM));
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
                    //using (StreamReader sr = new StreamReader(ofd.FileName))
                    //{
                    //    DataTable dt = FileParser.ParsePlistFileLDB(sr.BaseStream);
                    //    db.ImportDataTable(dt, "level_data_id", false);
                    //}
                    using (StreamReader sr = new StreamReader(ofd.FileName))
                    {
                        List<MapData> game = MapData.FromPlist(sr.BaseStream);
                        foreach (MapData data in game)
                        {
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.UnitTalkMaster>(data.UTM));
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.LevelDataMaster>(data.LDM));
                            DAL.FromSingle(JSON.ToSingle<RTDDE.Provider.MasterData.EnemyTableMaster>(data.ETM));
                        }
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
        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.IsShowDropInfo = IsShowDropInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.IsShowBoxInfo = IsShowBoxInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.IsEnableLevelLimiter = IsEnableLevelLimiterCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.IsDefaultLvMax = IsDefaultLvMaxCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.IsUseLocalTime = IsUseLocalTimeCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.DisunityPath = DisunityPathTextBox.Text;
            Settings.AdbPath = AdbPathTextBox.Text;
            try
            {
                Settings.Save();
                SaveSettingsButton.Content = new Run("SAVED");
            }
            catch (Exception ex)
            {
                Utility.ShowException(ex.Message);
                SaveSettingsButton.Content = new Run("FAILED, click to retry.");
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
        private void InitSettings()
        {
            //fake for designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this) == false)
            {
                IsShowDropInfoCheckBox.IsChecked = Settings.IsShowDropInfo;
                IsShowBoxInfoCheckBox.IsChecked = Settings.IsShowBoxInfo;
                IsEnableLevelLimiterCheckBox.IsChecked = Settings.IsEnableLevelLimiter;
                IsDefaultLvMaxCheckBox.IsChecked = Settings.IsDefaultLvMax;
                IsUseLocalTimeCheckBox.IsChecked = Settings.IsUseLocalTime;
                DisunityPathTextBox.Text = Settings.DisunityPath;
                AdbPathTextBox.Text = Settings.AdbPath;
            }
        }

        private void SelectDisunityPathButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".bat";
            ofd.Filter = "Disunity Bat|disunity.bat";
            if (ofd.ShowDialog() == true)
            {
                DisunityPathTextBox.Text = ofd.FileName;
            }
        }

        private void SelectAdbPathButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".exe";
            ofd.Filter = "ADB|ADB.exe";
            if (ofd.ShowDialog() == true)
            {
                AdbPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
