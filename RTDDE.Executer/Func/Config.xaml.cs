using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
            this.DataContext = Settings.Config;
            InitializeComponent();
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
                        Utility.RefreshTabs();
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
                    Utility.RefreshTabs();
                }, MainWindow.UiTaskScheduler);
                taskBackup.Start();
                taskImport.Start();
            }
        }

        async private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                Settings.Save();
                SaveSettingsButton.Content = new Run("SAVED");
                Task task = Task.Run(()=>
                {
                    System.Threading.Thread.Sleep(5000);
                });
                await task;
                SaveSettingsButton.Content = new Run("Save Settings");
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
                SaveSettingsButton.Content = new Run("FAILED, click to retry.");
            }
        }

        private static readonly Regex CheckOnlyNumberRegex = new Regex("[^0-9]+", RegexOptions.Compiled); //regex that matches disallowed text
        private bool IsTextAllowed(string text)
        {
            return (CheckOnlyNumberRegex.IsMatch(text) == false);
        }
        private void ExpValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text) == false;
        }
        private void PtValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text) == false;
        }
        private void SaleValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text) == false;
        }

        private void ResetDropSettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Config.Map.Reset();
        }
    }
}
