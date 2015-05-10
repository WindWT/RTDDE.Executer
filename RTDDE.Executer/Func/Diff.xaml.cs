using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.Enums;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Diff.xaml 的交互逻辑
    /// </summary>
    public partial class Diff : UserControl
    {
        public Diff()
        {
            InitializeComponent();
        }

        //private void TableSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    string tableName = (string)TableSelectComboBox.SelectedValue;
        //    if (string.IsNullOrWhiteSpace(tableName)) {
        //        return;
        //    }
        //    string tableNameOld = tableName + "_old";

        //    Task<int> task = new Task<int>(() =>
        //    {
        //        return DAL.Get<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name IN ('" + tableName + "','" + tableNameOld + "')");
        //    });
        //    Task<DataSet> taskDataSet = new Task<DataSet>(() =>
        //    {
        //        Task.WaitAll(task);
        //        if (task.Exception != null) {
        //            Utility.ShowException(task.Exception.InnerException.Message);
        //            return null;
        //        }
        //        if (task.Result == 2)   //both table exists
        //        {
        //            DataTable oldTable = DAL.GetDataTable("SELECT * FROM " + tableNameOld);
        //            DataTable newTable = DAL.GetDataTable("SELECT * FROM " + tableName);
        //            DataTable oldDiffTable, newDiffTable;
        //            var oldDiff = oldTable.AsEnumerable().Except(newTable.AsEnumerable(), DataRowComparer.Default);
        //            var newDiff = newTable.AsEnumerable().Except(oldTable.AsEnumerable(), DataRowComparer.Default);
        //            if (oldDiff.Count() != 0) {
        //                oldDiffTable = oldDiff.CopyToDataTable();
        //                oldDiffTable.TableName = "old";
        //            }
        //            else {
        //                oldDiffTable = new DataTable("old");
        //            }
        //            if (newDiff.Count() != 0) {
        //                newDiffTable = newDiff.CopyToDataTable();
        //                newDiffTable.TableName = "new";
        //            }
        //            else {
        //                newDiffTable = new DataTable("new");
        //            }
        //            foreach (DataRow dr in oldDiffTable.Rows) {
        //                string id = dr["id"].ToString();
        //                DataRow[] newDiffHasRow = newDiffTable.Select("id=" + id);
        //                if (newDiffHasRow.Count() == 0)    //old table only row, add empty row to new table
        //                {
        //                    DataRow newDr = newDiffTable.NewRow();
        //                    newDr["id"] = id;
        //                    newDiffTable.Rows.Add(newDr);
        //                }
        //            }
        //            foreach (DataRow dr in newDiffTable.Rows) {
        //                string id = dr["id"].ToString();
        //                DataRow[] oldDiffHasRow = oldDiffTable.Select("id=" + id);
        //                if (oldDiffHasRow.Count() == 0)    //new table only row, add empty row to old table
        //                {
        //                    DataRow oldDr = oldDiffTable.NewRow();
        //                    oldDr["id"] = id;
        //                    oldDiffTable.Rows.Add(oldDr);
        //                }
        //            }
        //            DataSet ds = new DataSet();
        //            ds.Tables.Add(oldDiffTable);
        //            ds.Tables.Add(newDiffTable);
        //            return ds;
        //        }
        //        else {
        //            DataSet ds = new DataSet();
        //            ds.Tables.Add(new DataTable("old"));
        //            ds.Tables.Add(new DataTable("new"));
        //            return ds;
        //        }
        //    });
        //    taskDataSet.ContinueWith(t =>
        //    {
        //        if (t.Exception != null) {
        //            Utility.ShowException(t.Exception.InnerException.Message);
        //            return;
        //        }
        //        if (t.Result == null) {
        //            return;
        //        }
        //        var oldDV = t.Result.Tables["old"].DefaultView;
        //        if (t.Result.Tables["old"].Columns.Contains("id")) {
        //            oldDV.Sort = "id";
        //        }
        //        var newDV = t.Result.Tables["new"].DefaultView;
        //        if (t.Result.Tables["new"].Columns.Contains("id")) {
        //            newDV.Sort = "id";
        //        }
        //        OldTableDataGrid.ItemsSource = oldDV;
        //        NewTableDataGrid.ItemsSource = newDV;

        //        ScrollViewer svOld = Utility.GetVisualChild<ScrollViewer>(OldTableDataGrid);
        //        ScrollViewer svNew = Utility.GetVisualChild<ScrollViewer>(NewTableDataGrid);
        //        svOld.ScrollChanged += new ScrollChangedEventHandler(svOld_ScrollChanged);
        //        svNew.ScrollChanged += new ScrollChangedEventHandler(svNew_ScrollChanged);
        //    }, MainWindow.UiTaskScheduler);
        //    task.Start();
        //    taskDataSet.Start();
        //}
        private void CompareButton_OnClick(object sender, RoutedEventArgs e)
        {
            //todo async with task
            Dictionary<string, string> diffTableDictionary = new Dictionary<string, string>();
            diffTableDictionary.Add("-----", string.Empty);
            const string existSql = @"select count(*) from (
SELECT name FROM main.sqlite_master WHERE type='table' AND name='{0}'
UNION ALL
SELECT name FROM old.sqlite_master WHERE type='table' AND name='{0}'
)";
            const string compareSql = @"SELECT * FROM main.{0} EXCEPT SELECT * FROM old.{0}
UNION ALL
SELECT * FROM old.{0} EXCEPT SELECT * FROM main.{0}";
            using (SQLiteConnection connection = new SQLiteConnection(DAL.ConnectionString)) {
                connection.Open();
                //连接两个库
                string attachSql = "ATTACH '" + OldFilePathTextBox.Text + "' AS old";
                SQLiteCommand attachCommand = new SQLiteCommand(attachSql, connection);
                attachCommand.ExecuteNonQuery();
                //循环每个表
                foreach (MASTERDB type in Enum.GetValues(typeof(MASTERDB))) {
                    //检查表存在性
                    SQLiteCommand existCommand = new SQLiteCommand(string.Format(existSql, type.ToString()), connection);
                    int existCount = Convert.ToInt32(existCommand.ExecuteScalar());
                    if (existCount == 1) {
                        //只有一边存在
                        diffTableDictionary.Add("[New]"+type.ToString(), type.ToString());
                    }
                    else if (existCount == 2) {
                        //检查数据是否存在差异
                        SQLiteCommand compareCommand = new SQLiteCommand(string.Format(compareSql, type.ToString()), connection);
                        if (compareCommand.ExecuteScalar() != null) {
                            diffTableDictionary.Add("[Diff]" + type.ToString(), type.ToString());
                        }
                    }
                }
            }
            TableSelectComboBox.ItemsSource = diffTableDictionary;
        }

        private void svOld_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer svOld = Utility.GetVisualChild<ScrollViewer>(OldTableDataGrid);
            ScrollViewer svNew = Utility.GetVisualChild<ScrollViewer>(NewTableDataGrid);
            svNew.ScrollToHorizontalOffset(svOld.HorizontalOffset);
            svNew.ScrollToVerticalOffset(svOld.VerticalOffset);
        }
        private void svNew_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer svOld = Utility.GetVisualChild<ScrollViewer>(OldTableDataGrid);
            ScrollViewer svNew = Utility.GetVisualChild<ScrollViewer>(NewTableDataGrid);
            svOld.ScrollToHorizontalOffset(svNew.HorizontalOffset);
            svOld.ScrollToVerticalOffset(svNew.VerticalOffset);
        }

        private void SelectOldFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Database File|*.db";
            ofd.InitialDirectory = Environment.CurrentDirectory;
            if (ofd.ShowDialog() == true) {
                OldFilePathTextBox.Text = ofd.FileName;
            }
        }
        private void AutoOldFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            OldFilePathTextBox.Text = string.Empty;
            if (Directory.Exists("backup") == false) {
                return;
            }
            DirectoryInfo backupFolderInfo = new DirectoryInfo("backup");
            FileInfo lastBackupFile = backupFolderInfo.GetFiles().OrderByDescending(f => f.CreationTime).FirstOrDefault();
            if (lastBackupFile == null) {
                return;
            }
            OldFilePathTextBox.Text = lastBackupFile.FullName;
        }
        private void SelectNewFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Database File|*.db";
            ofd.InitialDirectory = Environment.CurrentDirectory;
            if (ofd.ShowDialog() == true) {
                NewFilePathTextBox.Text = ofd.FileName;
            }
        }
        private void AutoNewFileButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewFilePathTextBox.Text = string.Empty;
            if (File.Exists("RTD.db")) {
                NewFilePathTextBox.Text = new FileInfo("RTD.db").FullName;
            }
        }
    }
}
