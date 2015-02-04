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

namespace RTDDE.Executer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            new WindowResizer(this,
                new WindowBorder(BorderPosition.TopLeft, topLeft),
                new WindowBorder(BorderPosition.Top, top),
                new WindowBorder(BorderPosition.TopRight, topRight),
                new WindowBorder(BorderPosition.Right, right),
                new WindowBorder(BorderPosition.BottomRight, bottomRight),
                new WindowBorder(BorderPosition.Bottom, bottom),
                new WindowBorder(BorderPosition.BottomLeft, bottomLeft),
                new WindowBorder(BorderPosition.Left, left));
            ChangeTab("Quest");
        }
        public static readonly TaskScheduler UiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Button tb = sender as Button;
            if (tb != null)
            {
                ChangeTab(tb.Name.Replace("MenuItem_", String.Empty));
            }
        }

        public void ChangeTab(string tabName)
        {
            foreach (UserControl child in MainGrid.Children)
            {
                child.Visibility = child.Name == tabName ? Visibility.Visible : Visibility.Collapsed;
            }
            MenuButton.IsChecked = false;
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
            if (string.IsNullOrWhiteSpace(StatusBarExceptionMessage.Text))
            {
                StatusBarExceptionMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
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
