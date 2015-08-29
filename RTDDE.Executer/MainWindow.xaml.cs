using RTDDE.Provider;
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
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Windows.Shell;
using RTDDE.Executer.Func;
using RTDDE.Executer.Util;

namespace RTDDE.Executer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Settings.Init();    //init settings here
            InitializeComponent();
            InitLanguageDictionary();
            ChangeTabByName("Quest");
        }

        private void InitLanguageDictionary()
        {
            //load external language file
            if (Settings.Config.General.IsForceEnglish) {
                return;
            }
            string filepath = $"Lang\\{Thread.CurrentThread.CurrentCulture.ToString()}.xaml";
            if (File.Exists(filepath) == false) {
                return;
            }
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                var dic = (ResourceDictionary)XamlReader.Load(fs);
                Application.Current.Resources.MergedDictionaries.Add(dic);
            }
        }
        [Obsolete("use async and await instead")]
        public static readonly TaskScheduler UiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private string LastTabName { get; set; }
        private void MenuButton_OnChecked(object sender, RoutedEventArgs e)
        {
            LastTabName = MenuButton.Content.ToString();
            MenuButton.Content = "MENU";
        }
        private void MenuButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            MenuButton.Content = string.IsNullOrEmpty(LastTabName) ? "MENU" : LastTabName;
        }
        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Button tb = sender as Button;
            if (tb != null) {
                ChangeTabByName(tb.Name.Replace("MenuItem_", String.Empty));
            }
        }

        public void ChangeTabByName(string tabName)
        {
            foreach (UserControl child in MainGrid.Children) {
                child.Visibility = Visibility.Collapsed;
            }
            var tab = GetTabByName(tabName);
            if (tab != null) {
                tab.Visibility = Visibility.Visible;
                LastTabName = tab.GetType().Name;
                MenuButton.Content = LastTabName;
            }
            MenuButton.IsChecked = false;
        }
        public UserControl GetTabByName(string tabName, bool disableAutoLoad = false)
        {
            foreach (UserControl child in MainGrid.Children) {
                if (string.Compare(child.GetType().Name, tabName, StringComparison.OrdinalIgnoreCase) == 0) {
                    return child;
                }
            }
            //该Tab尚未创建，尝试创建
            string tabFullName = string.Format("RTDDE.Executer.Func.{0}", tabName);
            var tabType = Type.GetType(tabFullName);
            if (tabType != null) {
                UserControl tab = (UserControl)Activator.CreateInstance(tabType,
                    tabType.GetConstructor(new Type[] { typeof(bool) }) == null ? null : new object[] { disableAutoLoad });
                if (tab != null) {
                    MainGrid.Children.Add(tab);
                    return tab;
                }
            }
            return null;
        }

        private void SB_ShowMenu_Completed(object sender, EventArgs e)
        {
            MenuMask.IsHitTestVisible = true;
        }

        private void SB_HideMenu_Completed(object sender, EventArgs e)
        {
            MenuMask.IsHitTestVisible = false;
        }

        private void MenuMask_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MenuButton.IsChecked = false;
        }

        //异常信息显示15秒之后消失。
        private DispatcherTimer _dispatcherTimer = null;
        private void StatusBarExceptionMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusBarExceptionMessage.Text)) {
                StatusBarExceptionMessage.Visibility = Visibility.Collapsed;
            }
            else {
                StatusBarExceptionMessage.Visibility = Visibility.Visible;
                _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 15) };
                EventHandler eh = null;
                eh = (a, b) =>
                {
                    _dispatcherTimer.Tick -= eh;
                    _dispatcherTimer.Stop();
                    StatusBarExceptionMessage.Text = String.Empty;
                };
                _dispatcherTimer.Tick += eh;
                _dispatcherTimer.Start();
            }
        }

        private void MoveBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MoveBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void MinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
