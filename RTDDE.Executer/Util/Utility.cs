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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using RTDDE.Executer.Func;
using RTDDE.Executer.Util;

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
        public static void ChangeTab<T>() where T : UserControl
        {
            var w = (MainWindow)Application.Current.MainWindow;
            w.ChangeTabByName(typeof(T).Name);
        }
        public static T GetTab<T>(bool newTab = false, bool disableAutoLoad = false) where T : UserControl
        {
            if (newTab) {
                Utility.RefreshTabs<T>();
            }
            var w = (MainWindow)Application.Current.MainWindow;
            return (T)w.GetTabByName(typeof(T).Name, disableAutoLoad);
        }
        public static bool DisableBindData { get; set; }
        async public static void BindData(DataGrid dg, string sql, List<SQLiteParameter> paras = null)
        {
            if (DisableBindData) {
                return;
            }
            Task<DataTable> task = Task.Run(() => DAL.GetDataTable(sql, paras));
            try {
                dg.ItemsSource = (await task).DefaultView;
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
                return;
            }
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(dg);
            if (scrollViewer != null) {
                scrollViewer.ScrollToTop();
            }
            //todo remove this line
            System.Diagnostics.Trace.WriteLine("bind");
            AfterBindDataEvent();
        }
        public delegate void AfterBindDataEventHandler();
        public static event AfterBindDataEventHandler AfterBindDataEvent = () => { };
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
        public static void RefreshTabs()
        {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if ((child is Config) == false) {
                    w.MainGrid.Children.Remove(child);
                }
            }
        }
        public static void RefreshTabs<T>() where T : UserControl
        {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if (child is T) {
                    w.MainGrid.Children.Remove(child);
                    break;
                }
            }
        }
        public static void GoToItemById<T>(int firstId, int lastId = -1, string type = null) where T : UserControl
        {
            T tab = GetTab<T>(true, true);
            if ((tab is IRedirectable) == false) {
                return;
            }
            DataGrid dataGrid = ((IRedirectable)tab).GetTargetDataGrid(firstId, lastId, type);
            AfterBindDataEventHandler afterBindDataEventHandler = null;
            afterBindDataEventHandler = () =>
            {
                Utility.AfterBindDataEvent -= afterBindDataEventHandler;
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
            };
            Utility.AfterBindDataEvent += afterBindDataEventHandler;
            ChangeTab<T>();
        }
    }
}