﻿using RTDDE.Provider;
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
        public static TaskScheduler uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            Button tb = sender as Button;
            ChangeTab(tb.Name.Replace("MenuItem_", String.Empty));
        }

        public void ChangeTab(string tabName)
        {
            foreach (UserControl child in MainGrid.Children)
            {
                if (child.Name == tabName)
                {
                    child.Visibility = Visibility.Visible;
                }
                else
                {
                    child.Visibility = Visibility.Collapsed;
                }
            }
            MenuButton.IsChecked = false;
        }
        
        //异常信息显示15秒之后消失。
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
                dispatcherTimer.Interval = new TimeSpan(0, 0, 15);
                EventHandler eh = null;
                eh = (a, b) =>
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