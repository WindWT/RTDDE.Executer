using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml;
using RTDDE.Provider;
using RTDDE.Provider.Enums;

namespace RTDDE.Executer.Func
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
            ImportMsgPackButton.Content = new Run("Import MDBS MsgPack");
            ImportLdbsButton.Content = new Run("Import MAP");
            SaveSettingsButton.Content = new Run("Save Settings");
        }
        private void ImportLdbsButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "LDBS File|LDBS*_Msg.bytes";
            if (ofd.ShowDialog() == true) {
                ImportLdbsButton.Content = new Run("Importing MAP Data...");
                string filename = ofd.FileName;
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(filename)) {
                        var game = new MapData(sr.BaseStream);
                        DAL.FromSingle(game.LDM);
                        DAL.FromSingle(game.LDM.enemy_table_master);
                        DAL.FromSingle(game.LDM.unit_talk_master);
                        //foreach (var ecm in game.LDM.event_cutin_master)
                        //{
                        //    DAL.FromSingle(ecm);
                        //}
                    }

                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null) {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportLdbsButton.Content = new Run("MAP Data Import Failed.");
                    }
                    else {
                        ImportLdbsButton.Content = new Run("MAP Data Successfully Imported.");
                        RefreshTabs();
                    }
                }, MainWindow.UiTaskScheduler);
                task.Start();
            }
        }
        private void ImportMsgPackButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = "Ignore me",
                Title = "Browse Folder Then Press Open"
            };
            if (ofd.ShowDialog() == true) {
                ImportMsgPackButton.Content = new Run("Importing MDBS MsgPack...");
                string path = System.IO.Path.GetDirectoryName(ofd.FileName);
                var stopwatch = new Stopwatch();
                long importTime = 0, backupTime = 0;
                stopwatch.Start();
                Task taskBackup = new Task(() =>
                {
                    if (Settings.Config.Database.AutoBackup == false) {
                        return;
                    }
                    stopwatch.Restart();
                    DirectoryInfo backupFolderInfo;
                    if (Directory.Exists("backup") == false) {
                        backupFolderInfo = Directory.CreateDirectory("backup");
                    }
                    else {
                        backupFolderInfo = new DirectoryInfo("backup");
                    }
                    if (File.Exists("RTD.db")) {
                        File.Copy("RTD.db", backupFolderInfo.Name + "\\RTD_backup_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".db");
                    }
                    stopwatch.Stop();
                    backupTime = stopwatch.ElapsedMilliseconds;
                });
                Task taskImport = new Task(() =>
                {
                    taskBackup.Wait();
                    stopwatch.Restart();
                    foreach (string filepath in System.IO.Directory.GetFiles(path)) {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename != null && filename.EndsWith("_Msg.bytes") == false) {
                            continue;
                        }
                        string enumName = filename.Replace("_Msg.bytes", string.Empty);
                        MASTERDB mdbEnum;
                        if (Enum.TryParse<MASTERDB>(enumName, true, out mdbEnum) == false) {
                            //type not exist, skip
                            continue;
                        }
                        using (StreamReader sr = new StreamReader(filepath)) {
                            //Dynamic type from enum
                            Type currentType = Converter.Enum2Type(mdbEnum);
                            //Generate Method
                            MethodInfo methodToList =
                                typeof(MsgBytes).GetMethod("ToList").MakeGenericMethod(currentType);
                            MethodInfo methodToDB = typeof(DAL).GetMethod("FromList").MakeGenericMethod(currentType);
                            //Invoke
                            var list = methodToList.Invoke(null, new object[] { sr.BaseStream });
                            //Drop table, 
                            DAL.DropTable(enumName);
                            methodToDB.Invoke(null, new object[] { list });
                        }
                    }
                    stopwatch.Stop();
                    importTime = stopwatch.ElapsedMilliseconds;
                });
                taskImport.ContinueWith(t =>
                {
                    if (t.Exception != null) {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportMsgPackButton.Content = new Run("MDBS MsgPack Import Failed.");
                        return;
                    }
                    if (Settings.Config.Database.AutoBackup == false) {
                        ImportMsgPackButton.Content = new Run(string.Format("MDBS MsgPack Successfully Imported. ({0}ms)", importTime));
                    }
                    else {
                        ImportMsgPackButton.Content = new Run(string.Format("MDBS MsgPack Successfully Backuped & Imported . (B:{1}ms I:{0}ms)", importTime, backupTime));
                    }
                    RefreshTabs();
                }, MainWindow.UiTaskScheduler);
                taskBackup.Start();
                taskImport.Start();
            }
        }
        private void RefreshTabs()
        {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if ((child is Config) == false) {
                    w.MainGrid.Children.Remove(child);
                }
            }
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //map
            Settings.Config.Map.IsShowDropInfo = IsShowDropInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.Map.IsShowBoxInfo = IsShowBoxInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.Map.IsShowEnemyAttribute = IsShowEnemyAttributeCheckBox.IsChecked.GetValueOrDefault(false);
            //general
            Settings.Config.General.IsEnableLevelLimiter = IsEnableLevelLimiterCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsDefaultLvMax = IsDefaultLvMaxCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsUseLocalTime = IsUseLocalTimeCheckBox.IsChecked.GetValueOrDefault(false);
            //db
            Settings.Config.Database.AutoBackup = AutoBackupCheckBox.IsChecked.GetValueOrDefault(false);
            //model
            Settings.Config.Model.DisunityPath = DisunityPathTextBox.Text;
            try {
                Settings.Save();
                SaveSettingsButton.Content = new Run("SAVED");
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
                SaveSettingsButton.Content = new Run("FAILED, click to retry.");
            }
        }
        private void InitSettings()
        {
            //fake for designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            //map
            IsShowDropInfoCheckBox.IsChecked = Settings.Config.Map.IsShowDropInfo;
            IsShowBoxInfoCheckBox.IsChecked = Settings.Config.Map.IsShowBoxInfo;
            IsShowEnemyAttributeCheckBox.IsChecked = Settings.Config.Map.IsShowEnemyAttribute;
            //general
            IsEnableLevelLimiterCheckBox.IsChecked = Settings.Config.General.IsEnableLevelLimiter;
            IsDefaultLvMaxCheckBox.IsChecked = Settings.Config.General.IsDefaultLvMax;
            IsUseLocalTimeCheckBox.IsChecked = Settings.Config.General.IsUseLocalTime;
            //db
            AutoBackupCheckBox.IsChecked = Settings.Config.Database.AutoBackup;
            //model
            DisunityPathTextBox.Text = Settings.Config.Model.DisunityPath;
        }
        private void SelectDisunityPathButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".bat",
                Filter = "Disunity Bat|disunity.bat"
            };
            if (ofd.ShowDialog() == true) {
                DisunityPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
