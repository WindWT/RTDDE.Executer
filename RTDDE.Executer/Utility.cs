using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using RTDDE.Executer.Func;
using RTDDE.Executer.Util;
using RTDDE.Provider;

namespace RTDDE.Executer
{
    public sealed class Utility : RTDDE.Provider.Utility
    {
        private static readonly Regex RegColor = new Regex(@"(\[[a-zA-Z0-9]{6}\])", RegexOptions.Compiled);

        public static FlowDocument ParseTextToDocument(string text, int forceWrapCharCount = 0) {
            var flowDoc = new FlowDocument();
            text = text.Replace(@"\n", "\n");
            Paragraph pr = new Paragraph { Margin = new Thickness(0) }; //prprpr
            var textParts = RegColor.Split(text);
            Stack<SolidColorBrush> brushStack = new Stack<SolidColorBrush>();
            brushStack.Push(Brushes.Black);
            int charsInLine = 0;
            foreach (string textPart in textParts) {
                if (RegColor.Match(textPart).Success) {
                    //color part, ex:[FFFFFF]
                    string color = textPart.Trim(new char[] { '[', ']' });
                    var convertFromString = ColorConverter.ConvertFromString("#" + color);
                    if (convertFromString != null) {
                        brushStack.Push(new SolidColorBrush((Color) convertFromString));
                    }
                }
                else {
                    //text part, may with color end part [-]
                    bool colorEnd = false;
                    foreach (string part in textPart.Split(new[] { @"[-]" }, StringSplitOptions.None)) {
                        string splitedPart = part;
                        if (forceWrapCharCount > 0) {
                            //force wrap by given length
                            for (int i = 0; i < splitedPart.Length; i++) {
                                if (charsInLine >= forceWrapCharCount && splitedPart[i] != '\n') {
                                    splitedPart = splitedPart.Insert(i, "\n");
                                    charsInLine = 0;
                                }
                                else if (splitedPart[i] == '\n') {
                                    charsInLine = 0;
                                }
                                else if((int)splitedPart[i]<128) {
                                    charsInLine++;   //deal with alphabet
                                }
                                else {
                                    charsInLine += 2;
                                }
                            }
                        }
                        if (colorEnd && brushStack.Count > 1) {
                            brushStack.Pop();
                        }
                        SolidColorBrush foreground = brushStack.Peek();
                        FontWeight fontWeight = foreground.Color == Brushes.Black.Color
                            ? FontWeights.Normal
                            : Settings.Config.General.IsShowColorTextAsBold ? FontWeights.Bold : FontWeights.Normal;
                        Span span = new Span { Foreground = foreground, FontWeight = fontWeight };
                        span.Inlines.Add(new Run(splitedPart));
                        pr.Inlines.Add(span);
                        colorEnd = true;
                    }
                }
            }
            flowDoc.Blocks.Add(pr);
            return flowDoc;
        }

        public static readonly Regex CheckOnlyNumberRegex = new Regex("[^0-9]+", RegexOptions.Compiled); //regex that matches disallowed text

        public static bool IsOnlyNumber(string text) {
            return (CheckOnlyNumberRegex.IsMatch(text) == false);
        }

        public static void ShowException(Exception ex) {
            while (ex.InnerException != null) {
                ex = ex.InnerException;
            }
            var w = (MainWindow)Application.Current.MainWindow;
            w.StatusBarExceptionMessage.Text = ex.Message;
        }

        public static void ShowMessage(string message) {
            var w = (MainWindow) Application.Current.MainWindow;
            w.StatusBarExceptionMessage.Text = message;
        }

        public static string GetUiText(string key) {
            return Application.Current.Resources[key].ToString();
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
                Utility.ShowException(ex);
                return;
            }
            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(dg);
            scrollViewer?.ScrollToTop();
            System.Diagnostics.Trace.WriteLine("Data binded");
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
        public static void RefreshTabs() {
            var w = (MainWindow)Application.Current.MainWindow;
            for (int i = 0; i < w.MainGrid.Children.Count; i++) {
                var child = w.MainGrid.Children[i];
                if ((child is Config) == false) {
                    System.Diagnostics.Trace.WriteLine($"{child.GetType().Name} Tab has removed.");
                    w.MainGrid.Children.Remove(child);
                    i--;
                }
            }
        }

        public static void RefreshTabs<T>() where T : UserControl
        {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if (child is T) {
                    System.Diagnostics.Trace.WriteLine($"{child.GetType().Name} Tab has removed.");
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