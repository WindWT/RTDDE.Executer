using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
        private const string ExistSql = @"select count(*) from (
SELECT name FROM new.sqlite_master WHERE type='table' AND name='{0}'
UNION ALL
SELECT name FROM old.sqlite_master WHERE type='table' AND name='{0}'
)";
        private const string ExistInNewSql = @"SELECT * FROM new.{0} EXCEPT SELECT * FROM old.{0}";
        private const string ExistInOldSql = @"SELECT * FROM old.{0} EXCEPT SELECT * FROM new.{0}";
        private string OldFile { get; set; }
        private string NewFile { get; set; }
        async private void CompareButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (File.Exists(OldFilePathTextBox.Text) == false || File.Exists(NewFilePathTextBox.Text) == false) {
                return;
            }
            OldFile = new FileInfo(OldFilePathTextBox.Text).FullName;
            NewFile = new FileInfo(NewFilePathTextBox.Text).FullName;
            CompareButton.Content = new Run("Comparing...");
            Task<Dictionary<string, string>> fastDiffTask = Task.Run(() =>
            {
                Dictionary<string, string> diffTableDictionary = new Dictionary<string, string>();
                //diffTableDictionary.Add("-----", string.Empty);
                using (SQLiteConnection connection = new SQLiteConnection(DAL.ConnectionString)) {
                    connection.Open();
                    //连接两个库
                    string attachSql = "ATTACH '" + OldFile + "' AS old;ATTACH '" + NewFile + "' AS new;";
                    SQLiteCommand attachCommand = new SQLiteCommand(attachSql, connection);
                    attachCommand.ExecuteNonQuery();
                    //循环每个表
                    foreach (MASTERDB type in Enum.GetValues(typeof(MASTERDB))) {
                        if (type == MASTERDB.LEVEL_DATA_MASTER) {
                            //地图表没啥比对价值
                            continue;
                        }
                        //检查表存在性
                        SQLiteCommand existCommand = new SQLiteCommand(string.Format(ExistSql, type.ToString()), connection);
                        int existCount = Convert.ToInt32(existCommand.ExecuteScalar());
                        if (existCount == 1) {
                            //只有一边存在
                            diffTableDictionary.Add("[New]" + type.ToString(), type.ToString());
                        }
                        else if (existCount == 2) {
                            //检查数据是否存在差异
                            int oldHasNew = 0, newHasNew = 0;
                            string hasDiffMark = string.Empty;
                            existCommand.CommandText = string.Format(ExistInNewSql, type.ToString());
                            if (existCommand.ExecuteScalar() != null) {
                                newHasNew = 1;
                            }
                            existCommand.CommandText = string.Format(ExistInOldSql, type.ToString());
                            if (existCommand.ExecuteScalar() != null) {
                                oldHasNew = 1;
                            }
                            switch ((oldHasNew << 1) | newHasNew) {
                                case 3: { hasDiffMark = "←→"; break; }
                                case 2: { hasDiffMark = "←　"; break; }
                                case 1: { hasDiffMark = "　→"; break; }
                                default: break;
                            }

                            if (string.IsNullOrEmpty(hasDiffMark) == false) {
                                diffTableDictionary.Add("[" + hasDiffMark + "]" + type.ToString(), type.ToString());
                            }
                        }
                    }
                }
                //trick to add to dict first
                var reverseDict = diffTableDictionary.Reverse().ToDictionary(pair => pair.Key, pair => pair.Value);
                reverseDict.Add("Results:" + diffTableDictionary.Count, "");
                return reverseDict.Reverse().ToDictionary(pair => pair.Key, pair => pair.Value);
            });

            TableSelectComboBox.ItemsSource = await fastDiffTask;
            CompareButton.Content = new Run("Compare");
        }

        async private void TableSelectComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string tableName = (string)TableSelectComboBox.SelectedValue;
            if (string.IsNullOrWhiteSpace(tableName)) {
                return;
            }
            MASTERDB tableEnum;
            if (Enum.TryParse(tableName, true, out tableEnum) == false) {
                return;
            }
            string pkName = DAL.GetColumnPKName(Converter.Enum2Type(tableEnum));
            Task<DataSet> getDiffResult = Task.Run(() =>
            {
                using (SQLiteConnection connection = new SQLiteConnection(DAL.ConnectionString)) {
                    connection.Open();
                    //连接两个库
                    string attachSql = "ATTACH '" + OldFile + "' AS old;ATTACH '" + NewFile + "' AS new;";
                    SQLiteCommand command = new SQLiteCommand(attachSql, connection);
                    command.ExecuteNonQuery();
                    //检查表存在性
                    command.CommandText = string.Format(ExistSql, tableName);
                    int existCount = Convert.ToInt32(command.ExecuteScalar());
                    if (existCount == 2) {
                        //两个表都存在，可以开工比对了
                        command.CommandText = string.Format(ExistInOldSql, tableName);
                        DataTable oldDiffTable = new DataTable("old");
                        oldDiffTable.Load(command.ExecuteReader());
                        command.CommandText = string.Format(ExistInNewSql, tableName);
                        DataTable newDiffTable = new DataTable("new");
                        newDiffTable.Load(command.ExecuteReader());
                        foreach (DataRow dr in oldDiffTable.Rows) {
                            string id = dr[pkName].ToString();
                            DataRow[] newDiffHasRow = newDiffTable.Select(pkName + "=" + id);
                            if (newDiffHasRow.Any() == false) {    //old table only row, add empty row to new table
                                DataRow newDr = newDiffTable.NewRow();
                                newDr[pkName] = id;
                                newDiffTable.Rows.Add(newDr);
                            }
                        }
                        foreach (DataRow dr in newDiffTable.Rows) {
                            string id = dr[pkName].ToString();
                            DataRow[] oldDiffHasRow = oldDiffTable.Select("id=" + id);
                            if (oldDiffHasRow.Any() == false) {     //new table only row, add empty row to old table
                                DataRow oldDr = oldDiffTable.NewRow();
                                oldDr[pkName] = id;
                                oldDiffTable.Rows.Add(oldDr);
                            }
                        }
                        DataSet ds = new DataSet();
                        ds.Tables.Add(oldDiffTable);
                        ds.Tables.Add(newDiffTable);
                        return ds;
                    }
                    return new DataSet();
                }
            });
            DataSet result = await getDiffResult;
            if (result.Tables.Contains("old")) {
                var oldDV = result.Tables["old"].DefaultView;
                if (result.Tables["old"].Columns.Contains(pkName)) {
                    oldDV.Sort = pkName;
                }
                OldTableDataGrid.ItemsSource = oldDV;
            }
            if (result.Tables.Contains("new")) {
                var newDV = result.Tables["new"].DefaultView;
                if (result.Tables["new"].Columns.Contains(pkName)) {
                    newDV.Sort = pkName;
                }
                NewTableDataGrid.ItemsSource = newDV;
            }

            ScrollViewer svOld = Utility.GetVisualChild<ScrollViewer>(OldTableDataGrid);
            ScrollViewer svNew = Utility.GetVisualChild<ScrollViewer>(NewTableDataGrid);
            svOld.ScrollChanged -= svOld_ScrollChanged;
            svOld.ScrollChanged += svOld_ScrollChanged;
            svNew.ScrollChanged -= svNew_ScrollChanged;
            svNew.ScrollChanged += svNew_ScrollChanged;
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
