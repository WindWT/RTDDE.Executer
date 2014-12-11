using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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

        private void TableSelectComboBox_Initialized(object sender, EventArgs e)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add(string.Empty, string.Empty);
            foreach (MASTERDB type in Enum.GetValues(typeof(MASTERDB)))
            {
                dict.Add(type.ToString(), type.ToString());
            }
            TableSelectComboBox.ItemsSource = dict;
        }

        private void TableSelectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tableName = (string)TableSelectComboBox.SelectedValue;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return;
            }
            string tableNameOld = tableName + "_old";

            Task<int> task = new Task<int>(() =>
            {
                return DAL.Get<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name IN ('" + tableName + "','" + tableNameOld + "')");
            });
            Task<DataSet> taskDataSet = new Task<DataSet>(() =>
            {
                Task.WaitAll(task);
                if (task.Exception != null)
                {
                    Utility.ShowException(task.Exception.InnerException.Message);
                    return null;
                }
                if (task.Result == 2)   //both table exists
                {
                    DataTable oldTable = DAL.GetDataTable("SELECT * FROM " + tableNameOld);
                    DataTable newTable = DAL.GetDataTable("SELECT * FROM " + tableName);
                    DataTable oldDiffTable, newDiffTable;
                    var oldDiff = oldTable.AsEnumerable().Except(newTable.AsEnumerable(), DataRowComparer.Default);
                    var newDiff = newTable.AsEnumerable().Except(oldTable.AsEnumerable(), DataRowComparer.Default);
                    if (oldDiff.Count() != 0)
                    {
                        oldDiffTable = oldDiff.CopyToDataTable();
                        oldDiffTable.TableName = "old";
                    }
                    else
                    {
                        oldDiffTable = new DataTable("old");
                    }
                    if (newDiff.Count() != 0)
                    {
                        newDiffTable = newDiff.CopyToDataTable();
                        newDiffTable.TableName = "new";
                    }
                    else
                    {
                        newDiffTable = new DataTable("new");
                    }
                    foreach (DataRow dr in oldDiffTable.Rows)
                    {
                        string id = dr["id"].ToString();
                        DataRow[] newDiffHasRow = newDiffTable.Select("id=" + id);
                        if (newDiffHasRow.Count() == 0)    //old table only row, add empty row to new table
                        {
                            DataRow newDr = newDiffTable.NewRow();
                            newDr["id"] = id;
                            newDiffTable.Rows.Add(newDr);
                        }
                    }
                    foreach (DataRow dr in newDiffTable.Rows)
                    {
                        string id = dr["id"].ToString();
                        DataRow[] oldDiffHasRow = oldDiffTable.Select("id=" + id);
                        if (oldDiffHasRow.Count() == 0)    //new table only row, add empty row to old table
                        {
                            DataRow oldDr = oldDiffTable.NewRow();
                            oldDr["id"] = id;
                            oldDiffTable.Rows.Add(oldDr);
                        }
                    }
                    DataSet ds = new DataSet();
                    ds.Tables.Add(oldDiffTable);
                    ds.Tables.Add(newDiffTable);
                    return ds;
                }
                else
                {
                    DataSet ds = new DataSet();
                    ds.Tables.Add(new DataTable("old"));
                    ds.Tables.Add(new DataTable("new"));
                    return ds;
                }
            });
            taskDataSet.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (t.Result == null)
                {
                    return;
                }
                var oldDV = t.Result.Tables["old"].DefaultView;
                if (t.Result.Tables["old"].Columns.Contains("id"))
                {
                    oldDV.Sort = "id";
                }
                var newDV = t.Result.Tables["new"].DefaultView;
                if (t.Result.Tables["new"].Columns.Contains("id"))
                {
                    newDV.Sort = "id";
                }
                OldTableDataGrid.ItemsSource = oldDV;
                NewTableDataGrid.ItemsSource = newDV;

                ScrollViewer svOld = Utility.GetVisualChild<ScrollViewer>(OldTableDataGrid);
                ScrollViewer svNew = Utility.GetVisualChild<ScrollViewer>(NewTableDataGrid);
                svOld.ScrollChanged += new ScrollChangedEventHandler(svOld_ScrollChanged);
                svNew.ScrollChanged += new ScrollChangedEventHandler(svNew_ScrollChanged);
            }, MainWindow.uiTaskScheduler);
            task.Start();
            taskDataSet.Start();
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
    }
}
