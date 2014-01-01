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
        }
        private TaskScheduler uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string SQL = SQLTextBox.Text;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(SQL);
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

        private void MainTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                if (QuestViewerTabItem.IsSelected)
                {
                    QuestViewerDataGrid_BindData();
                }
                else if (MapViewerTabItem.IsSelected)
                {
                    if (!string.IsNullOrWhiteSpace(QuestInfo_id.Text))
                    {
                        InitMap(QuestInfo_id.Text);
                    }
                }
                else if (UnitViewerTabItem.IsSelected)
                {
                    Task<DataTable> task = new Task<DataTable>(() =>
                    {
                        DB db = new DB();
                        return db.GetData("SELECT id,g_id,name FROM UNIT_MASTER order by g_id");
                    });
                    task.ContinueWith(t =>
                    {
                        if (t.Exception != null)
                        {
                            StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                            return;
                        }
                        UnitViewerDataGrid.ItemsSource = t.Result.DefaultView;
                    }, uiTaskScheduler);    //this Task work on ui thread
                    task.Start();
                }
                else if (CommonViewerTabItem.IsSelected)
                {
                    Task<DataTable> task = new Task<DataTable>(() =>
                    {
                        DB db = new DB();
                        return db.GetData("SELECT * FROM USER_RANK_MASTER");
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
            }
        }
        #region settings
        private void ImportMDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "MDBS File|MDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlMDB = sr.ReadToEnd();
                    try
                    {
                        DataSet ds = XMLParser.ParseMDB(xmlMDB);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);
                    }
                    catch (Exception ex)
                    {
                        StatusBarExceptionMessage.Text = ex.Message;
                    }
                }
            }
        }
        private void ImportLDBSButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "LDBS File|LDBS.xml";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string xmlLDB = sr.ReadToEnd();
                    try
                    {
                        DataTable dt = XMLParser.ParseLDB(xmlLDB);
                        DataSet lds = new DataSet("LDB");
                        lds.Tables.Add(dt);
                        DB db = new DB();
                        db.ImportDataSet(lds, false);
                    }
                    catch (Exception ex)
                    {
                        StatusBarExceptionMessage.Text = ex.Message;
                    }
                }
            }
        }
        private void ImportplistButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".plist";
            ofd.Filter = "plist File|*.plist";
            if (ofd.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    try
                    {
                        DataSet ds = XMLParser.ParsePlistMDB(sr.BaseStream);
                        DB db = new DB();
                        db.ImportDataSet(ds, true);

                        sr.BaseStream.Position = 0;

                        DataTable dt = XMLParser.ParsePlistLDB(sr.BaseStream);
                        DataSet lds = new DataSet("LDB");
                        lds.Tables.Add(dt);
                        db.ImportDataSet(lds, false);
                    }
                    catch (Exception ex)
                    {
                        StatusBarExceptionMessage.Text = ex.Message;
                    }
                }
            }
        }
        #endregion
    }
}
