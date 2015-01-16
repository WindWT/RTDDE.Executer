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
            ofd.Filter = "LDBS File|LDBS0_Msg.bytes";
            if (ofd.ShowDialog() == true)
            {
                ImportLdbsButton.Content = new Run("Importing MAP Data...");
                string filename = ofd.FileName;
                Task task = new Task(() =>
                {
                    using (StreamReader sr = new StreamReader(filename))
                    {
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
                    if (t.Exception != null)
                    {
                        Utility.ShowException(t.Exception.InnerException.Message);
                        ImportLdbsButton.Content = new Run("MAP Data Import Failed.");
                    }
                    else
                    {
                        ImportLdbsButton.Content = new Run("MAP Data Successfully Imported.");
                        RefreshControl();
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
            if (ofd.ShowDialog() == true)
            {
                ImportMsgPackButton.Content = new Run("Importing MDBS MsgPack...");
                string path = System.IO.Path.GetDirectoryName(ofd.FileName);
                var stopwatch = new Stopwatch();
                Task task = new Task(() =>
                {
                    stopwatch.Start();
                    foreach (string filepath in System.IO.Directory.GetFiles(path))
                    {
                        string filename = System.IO.Path.GetFileName(filepath);
                        if (filename != null && filename.EndsWith("_Msg.bytes") == false)
                        {
                            continue;
                        }
                        string enumName = filename.Replace("_Msg.bytes", string.Empty);
                        MASTERDB mdbEnum;
                        if (Enum.TryParse<MASTERDB>(enumName, true, out mdbEnum) == false)
                        {
                            //type not exist, skip
                            continue;
                        }
                        using (StreamReader sr = new StreamReader(filepath))
                        {
                            //Dynamic type from enum
                            Type currentType = Converter.Enum2Type(mdbEnum);
                            //Generate Method
                            MethodInfo methodToList =
                                typeof (MsgBytes).GetMethod("ToList").MakeGenericMethod(currentType);
                            MethodInfo methodToDB = typeof (DAL).GetMethod("FromList").MakeGenericMethod(currentType);
                            //Invoke
                            var list = methodToList.Invoke(null, new object[] {sr.BaseStream});
                            //Drop table, 
                            DAL.DropTable(enumName);
                            methodToDB.Invoke(null, new object[] {list});
                        }
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
                        stopwatch.Stop();
                        ImportMsgPackButton.Content = new Run("MDBS MsgPack Successfully Imported. ("+stopwatch.ElapsedMilliseconds+"ms)");
                        RefreshControl();
                    }
                }, MainWindow.UiTaskScheduler);
                task.Start();
            }
        }
        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Config.General.IsShowDropInfo = IsShowDropInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsShowBoxInfo = IsShowBoxInfoCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsEnableLevelLimiter = IsEnableLevelLimiterCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsDefaultLvMax = IsDefaultLvMaxCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.General.IsUseLocalTime = IsUseLocalTimeCheckBox.IsChecked.GetValueOrDefault(false);
            Settings.Config.Model.DisunityPath = DisunityPathTextBox.Text;
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
            foreach (UserControl child in w.MainGrid.Children)
            {
                if (child is IRefreshable)
                {
                    (child as IRefreshable).Refresh();
                }
            }
        }
        private void InitSettings()
        {
            //fake for designer
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            IsShowDropInfoCheckBox.IsChecked = Settings.Config.General.IsShowDropInfo;
            IsShowBoxInfoCheckBox.IsChecked = Settings.Config.General.IsShowBoxInfo;
            IsEnableLevelLimiterCheckBox.IsChecked = Settings.Config.General.IsEnableLevelLimiter;
            IsDefaultLvMaxCheckBox.IsChecked = Settings.Config.General.IsDefaultLvMax;
            IsUseLocalTimeCheckBox.IsChecked = Settings.Config.General.IsUseLocalTime;
            DisunityPathTextBox.Text = Settings.Config.Model.DisunityPath;
        }
        private void SelectDisunityPathButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".bat",
                Filter = "Disunity Bat|disunity.bat"
            };
            if (ofd.ShowDialog() == true)
            {
                DisunityPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
