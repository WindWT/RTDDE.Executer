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
        }
        private TaskScheduler uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private void CommonViewerTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM USER_RANK_MASTER";
            CommonViewerSQLTextBox.Text = sql;
            CommonViewerDataGrid_BindData(sql);
        }
        private void CommonViewerRunSQL_Click(object sender, RoutedEventArgs e)
        {
            string sql = CommonViewerSQLTextBox.Text;
            CommonViewerDataGrid_BindData(sql);
        }
        private void CommonViewerDataGrid_BindData(string sql)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(sql);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                CommonViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
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
                dispatcherTimer.Tick += new EventHandler((a, b) => { StatusBarExceptionMessage.Visibility = Visibility.Collapsed; });
                dispatcherTimer.Start();
            }
        }


    }
}
