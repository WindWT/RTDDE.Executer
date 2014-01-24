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
using System.ComponentModel;

namespace RTDDataExecuter
{
    public partial class MainWindow : Window
    {
        private void CommonTab_Initialized(object sender, EventArgs e)
        {
            CommonSQLComboBox.ItemsSource = new CommonSQLList();
        }
        private void CommonSQLComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            CommonDataGrid_BindData();
        }
        private void CommonRunSQL_Click(object sender, RoutedEventArgs e)
        {
            CommonDataGrid_BindData();
        }
        private void CommonDataGrid_BindData()
        {
            string sql = CommonSQLTextBox.Text;
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
                CommonDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        public class CommonSQL
        {
            public string Name { set; get; }
            public string SQL { set; get; }
        }
        public class CommonSQLList : ObservableCollection<CommonSQL>
        {
            public CommonSQLList()
            {
                this.Add(new CommonSQL
                {
                    Name = "Rank",
                    SQL = "SELECT * FROM USER_RANK_MASTER"
                });
                this.Add(new CommonSQL
                {
                    Name = "MDB",
                    SQL = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name"
                });
                this.Add(new CommonSQL
                {
                    Name = "LoginBonus",
                    SQL = "SELECT * FROM LOGIN_BONUS_MASTER order by day"
                });
            }
        }
    }
}