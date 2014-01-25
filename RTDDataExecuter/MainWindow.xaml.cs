using RTDDataProvider;
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

namespace RTDDataExecuter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitSettings();
            new WindowResizer(this,
                new WindowBorder(BorderPosition.TopLeft, topLeft),
                new WindowBorder(BorderPosition.Top, top),
                new WindowBorder(BorderPosition.TopRight, topRight),
                new WindowBorder(BorderPosition.Right, right),
                new WindowBorder(BorderPosition.BottomRight, bottomRight),
                new WindowBorder(BorderPosition.Bottom, bottom),
                new WindowBorder(BorderPosition.BottomLeft, bottomLeft),
                new WindowBorder(BorderPosition.Left, left));
        }
        private TaskScheduler uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void TabStrip_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedTabNumber = 0;
            foreach (var children in TabGrid.Children)
            {
                if (children is ToggleButton)
                {
                    if (((ToggleButton)children).IsChecked == true)
                    {
                        checkedTabNumber++;
                    }
                }
            }
            if (checkedTabNumber == 0)
            {
                ((ToggleButton)sender).IsChecked = true;
            }
        }
        private void TabStrip_Checked(object sender, RoutedEventArgs e)
        {
            ChangeTab(((ToggleButton)sender).Name.Replace("_TabStrip", String.Empty));
        }
        private void ChangeTab(string name)
        {
            if (MainGrid == null)
            {
                return;
            }
            foreach (var children in MainGrid.Children)
            {
                if (children is Grid)
                {
                    if (((Grid)children).Name != name + "Tab")
                    {
                        ((Grid)children).Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        ((Grid)children).Visibility = Visibility.Visible;
                    }
                }
            }
            foreach (var children in TabGrid.Children)
            {
                if (children is ToggleButton)
                {
                    if (((ToggleButton)children).Name != name + "_TabStrip")
                    {
                        ((ToggleButton)children).IsChecked = false;
                    }
                    else
                    {
                        if (((ToggleButton)children).IsChecked == false)
                        {
                            ((ToggleButton)children).IsChecked = true;
                        }
                    }
                }
            }
        }

        

        //异常信息显示5秒之后消失。
        private DispatcherTimer dispatcherTimer = null;
        private void StatusBarExceptionMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StatusBarExceptionMessage.Text))
            {
                StatusBarExceptionMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                StatusBarExceptionMessage.Visibility = Visibility.Visible;
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                EventHandler eh = null;
                eh = (a,b) =>
                {
                    dispatcherTimer.Tick -= eh;
                    dispatcherTimer.Stop();
                    StatusBarExceptionMessage.Text = String.Empty;
                };
                dispatcherTimer.Tick += eh;
                dispatcherTimer.Start();
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
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                this.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                this.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void MinimizedButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
