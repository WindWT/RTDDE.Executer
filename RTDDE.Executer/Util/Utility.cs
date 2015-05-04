using RTDDE.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using RTDDE.Executer.Func;

namespace RTDDE.Executer
{
    public sealed class Utility : RTDDE.Provider.Utility
    {
        private static readonly Regex RegColor = new Regex(@"(\[[a-zA-Z0-9]{6}\])", RegexOptions.Compiled);
        public static FlowDocument ParseTextToDocument(string text)
        {
            var flowDoc = new FlowDocument();
            text = text.Replace("\n", @"\n");   //fix split issue
            Paragraph pr = new Paragraph { Margin = new Thickness(0) }; //prprpr
            var textParts = RegColor.Split(text);
            var currentTextColor = Brushes.Black;
            foreach (string textPart in textParts) {
                if (RegColor.Match(textPart).Success) {
                    string color = textPart.Trim(new char[] { '[', ']' });
                    var convertFromString = ColorConverter.ConvertFromString("#" + color);
                    if (convertFromString != null) {
                        currentTextColor = new SolidColorBrush((Color)convertFromString);
                    }
                }
                else {
                    foreach (string part in textPart.Split(new[] { @"[-]" }, StringSplitOptions.None)) {
                        Span span = new Span { Foreground = currentTextColor };
                        span.Inlines.Add(new Run(part.Replace(@"\n", "\n")));
                        pr.Inlines.Add(span);
                        currentTextColor = Brushes.Black;
                    }
                }
            }
            flowDoc.Blocks.Add(pr);
            return flowDoc;
        }
        public static void ShowException(string message)
        {
            var w = (MainWindow)Application.Current.MainWindow;
            w.StatusBarExceptionMessage.Text = message;
        }
        public static void ChangeTab(string tabName)
        {
            var w = (MainWindow)Application.Current.MainWindow;
            w.ChangeTab(tabName);
        }
        async public static Task<UserControl> GetTabByName(string tabName)
        {
            var w = (MainWindow)Application.Current.MainWindow;
            return await w.GetTabByName(tabName);
        }
        public static void BindData(DataGrid dg, string sql, List<SQLiteParameter> paras = null)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                return DAL.GetDataTable(sql, paras);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                dg.ItemsSource = t.Result.DefaultView;
                ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(dg);
                if (scrollViewer != null) {
                    scrollViewer.ScrollToTop();
                }
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++) {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null) {
                    child = GetVisualChild<T>(v);
                }
                else {
                    break;
                }
            }
            return child;
        }
        public static T GetLogicalChild<T>(DependencyObject parent) where T : DependencyObject
        {
            T child = default(T);
            foreach (DependencyObject v in LogicalTreeHelper.GetChildren(parent)) {
                child = v as T;
                if (child == null) {
                    child = GetLogicalChild<T>(v);
                }
                else {
                    break;
                }
            }
            return child;
        }
        public static void RefreshTabs(string tabName = null)
        {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if (string.IsNullOrEmpty(tabName) && (child is Config) == false) {
                    w.MainGrid.Children.Remove(child);
                }
                else if (string.Compare(child.GetType().Name, tabName, StringComparison.OrdinalIgnoreCase) == 0) {
                    w.MainGrid.Children.Remove(child);
                    break;
                }
            }
        }
        async public static void GoToItemById(string tabName, int firstId, int lastId = -1)
        {
            GoToItemById(tabName, null, firstId, lastId);
        }
        async public static void GoToItemById(string tabName, string dataGridName, int firstId, int lastId = -1)
        {
            RefreshTabs(tabName);
            UserControl tab = await Utility.GetTabByName(tabName);
            DataGrid dataGrid;
            if (string.IsNullOrEmpty(dataGridName)) {
                dataGrid = GetLogicalChild<DataGrid>(tab);
            }
            else {
                dataGrid = tab.FindName(dataGridName) as DataGrid;
            }
            if (dataGrid == null) {
                return;
            }
            ChangeTab(tabName);
            foreach (DataRowView item in dataGrid.ItemsSource) {
                if (item == null) {
                    continue;
                }
                int itemId = Convert.ToInt32(item["id"]);
                if (lastId != -1 && itemId == lastId) {
                    //this first, last>first
                    dataGrid.ScrollIntoView(item);
                    dataGrid.SelectedItem = item;
                }
                else if (itemId == firstId) {
                    dataGrid.ScrollIntoView(item);
                    dataGrid.SelectedItem = item;
                    break;
                }
            }
        }
    }
}