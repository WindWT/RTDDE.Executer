using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using RTDDE.Executer.Util;
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
            CurrentSystemLanguage.Text = string.Format(Utility.GetUiText("Config_CurrentSystemLanguage"),
                Thread.CurrentThread.CurrentCulture.ToString());
        }
        private void ConfigTab_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ImportMsgPackButton.SetResourceReference(Button.ContentProperty, "Config_ImportMDBS");
            ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportLDBS");
            SaveSettingsButton.SetResourceReference(Button.ContentProperty, "Config_SaveSettings");
        }

        private void ImportMsgPackButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog {
                CheckFileExists = false,
                ValidateNames = false,
                FileName = Utility.GetUiText("Config_ImportSelectFolder"),
                Title = Utility.GetUiText("Config_ImportSelectFolder")
            };
            if (ofd.ShowDialog() == true) {
                ImportMsgPackButton.SetResourceReference(Button.ContentProperty, "Config_ImportingMDBS");
                string path = Path.GetDirectoryName(ofd.FileName);
                var stopwatch = new Stopwatch();
                long importTime = 0, backupTime = 0;
                stopwatch.Start();
                Task taskBackup = Task.Run(() => {
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
                        File.Copy("RTD.db",
                            backupFolderInfo.Name + "\\RTD_backup_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") +
                            ".db");
                    }
                    stopwatch.Stop();
                    backupTime = stopwatch.ElapsedMilliseconds;
                });
                Task taskImport = Task.Run(() => {
                    taskBackup.Wait();
                    stopwatch.Restart();
                    foreach (string filepath in Directory.GetFiles(path)) {
                        string filename = Path.GetFileName(filepath);
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
                            var list = methodToList.Invoke(null, new object[] {sr.BaseStream});
                            //Drop table, 
                            DAL.DropTable(enumName);
                            methodToDB.Invoke(null, new object[] {list});
                        }
                    }
                    stopwatch.Stop();
                    importTime = stopwatch.ElapsedMilliseconds;
                });
                taskImport.ContinueWith(t => {
                    if (t.Exception != null) {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportMsgPackButton.SetResourceReference(Button.ContentProperty, "Config_ImportMDBSFail");
                        return;
                    }
                    if (Settings.Config.Database.AutoBackup == false) {
                        ImportMsgPackButton.Content =
                            new Run(string.Format(Utility.GetUiText("Config_ImportMDBSSuccess"), importTime));
                    }
                    else {
                        ImportMsgPackButton.Content =
                            new Run(string.Format(Utility.GetUiText("Config_ImportMDBSSuccessWithBackup"), importTime,
                                backupTime));
                    }
                    Utility.RefreshTabs();
                }, MainWindow.UiTaskScheduler);
            }
        }

        private void ImportLdbsButton_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = Utility.GetUiText("Config_LDBSFilter") + "|LDBS*_Msg.bytes";
            if (ofd.ShowDialog() == true) {
                ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportingLDBS");
                string filename = ofd.FileName;
                Task task = Task.Run(() => {
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
                task.ContinueWith(t => {
                    if (t.Exception != null) {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportLDBSFail");
                    }
                    else {
                        ImportLdbsButton.SetResourceReference(Button.ContentProperty, "Config_ImportLDBSSuccess");
                        Utility.RefreshTabs();
                    }
                }, MainWindow.UiTaskScheduler);
            }
        }

        async private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try {
                Settings.Save();
                SaveSettingsButton.SetResourceReference(Button.ContentProperty, "Config_SaveSettingsSuccess");
                Task task = Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                });
                await task;
                SaveSettingsButton.SetResourceReference(Button.ContentProperty, "Config_SaveSettings");
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
                SaveSettingsButton.SetResourceReference(Button.ContentProperty, "Config_SaveSettingsFail");
            }
        }
        private void ExpValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Utility.IsOnlyNumber(e.Text) == false;
        }
        private void PtValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Utility.IsOnlyNumber(e.Text) == false;
        }
        private void SaleValueTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Utility.IsOnlyNumber(e.Text) == false;
        }

        private void ResetDropSettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            Settings.Config.Map.Reset();
        }

        async private void ValidateCustomDropSettingButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Config.Map.CustomDropColors == null) {
                ValidateCustomDropSettingButton.SetResourceReference(Button.ContentProperty, "Config_ValidateFail");
            }
            else {
                ValidateCustomDropSettingButton.SetResourceReference(Button.ContentProperty, "Config_ValidateSuccess");
            }
            Task task = Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);
            });
            await task;
            ValidateCustomDropSettingButton.SetResourceReference(Button.ContentProperty, "Config_Validate");
        }
    }
}
