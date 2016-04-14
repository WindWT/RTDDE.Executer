using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using RTDDE.Executer.Func;
using RTDDE.Executer.Util;
using RTDDE.Provider;
using RTDDE.Provider.Enums;

namespace RTDDE.Executer
{
    public sealed class Utility : RTDDE.Provider.Utility
    {
        private static readonly Application application = Application.Current;
        private static readonly Regex RegColor = new Regex(@"(\[[a-zA-Z0-9]{6}\])", RegexOptions.Compiled);
        
        public static FlowDocument ParseTextToDocument(string text, int forceWrapCharCount = 0) {
            var flowDoc = new FlowDocument();
            SolidColorBrush currentColor = Brushes.Black;
            foreach (string line in text.Split('\n')) {
                string currentLine = line.Replace(@"\n", "\n");
                Paragraph pr = new Paragraph { Margin = new Thickness(0) }; //prprpr
                var textParts = RegColor.Split(currentLine);
                Stack<SolidColorBrush> brushStack = new Stack<SolidColorBrush>();
                brushStack.Push(currentColor);
                int charsInLine = 0;
                foreach (string textPart in textParts) {
                    if (RegColor.Match(textPart).Success) {
                        //color part, ex:[FFFFFF]
                        string colorText = textPart.Trim(new char[] { '[', ']' });
                        var convertFromString = ColorConverter.ConvertFromString("#" + colorText);
                        if (convertFromString != null) {
                            brushStack.Push(new SolidColorBrush((Color)convertFromString));
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
                                    var fontGlyph = FontGlyphs.FirstOrDefault(o => o.Char == splitedPart[i]);
                                    if (charsInLine >= forceWrapCharCount && splitedPart[i] != '\n') {
                                        splitedPart = splitedPart.Insert(i, "\n");
                                        charsInLine = 0;
                                    }
                                    else if (splitedPart[i] == '\n') {
                                        charsInLine = 0;
                                    }
                                    else if (fontGlyph != null) {
                                        charsInLine += fontGlyph.advance;
                                    }
                                }
                            }
                            if (colorEnd && brushStack.Count > 1) {
                                brushStack.Pop();
                            }
                            if (string.IsNullOrEmpty(splitedPart)) {
                                continue;
                            }
                            currentColor = brushStack.Peek();
                            FontWeight fontWeight = currentColor.Color == Brushes.Black.Color
                                ? FontWeights.Normal
                                : Settings.Config.General.IsShowColorTextAsBold ? FontWeights.Bold : FontWeights.Normal;
                            Span span = new Span { Foreground = currentColor, FontWeight = fontWeight };
                            span.Inlines.Add(new Run(splitedPart));
                            pr.Inlines.Add(span);
                            colorEnd = true;
                        }
                    }
                }
                flowDoc.Blocks.Add(pr);
            }
            return flowDoc;
        }

        public static Color ParseAttributeToColor(UnitAttribute attribute) {
            switch (attribute) {
                case UnitAttribute.FIRE:
                    return (Color)(application.TryFindResource("FireColor"));
                case UnitAttribute.WATER:
                    return (Color)(application.TryFindResource("WaterColor"));
                case UnitAttribute.LIGHT:
                    return (Color)(application.TryFindResource("LightColor"));
                case UnitAttribute.DARK:
                    return (Color)(application.TryFindResource("DarkColor"));
                case UnitAttribute.NONE:
                    return (Color)(application.TryFindResource("NoneColor"));
                default:
                    return Colors.Transparent;
            }
        }

        public static Brush ParseAttributeToBrush(UnitAttribute attribute, bool isTransparent = false) {
            switch (attribute) {
                case UnitAttribute.FIRE:
                    return (Brush)(isTransparent
                        ? application.TryFindResource("FireTransBrush")
                        : application.TryFindResource("FireBrush"));
                case UnitAttribute.WATER:
                    return (Brush)(isTransparent
                        ? application.TryFindResource("WaterTransBrush")
                        : application.TryFindResource("WaterBrush"));
                case UnitAttribute.LIGHT:
                    return (Brush)(isTransparent
                        ? application.TryFindResource("LightTransBrush")
                        : application.TryFindResource("LightBrush"));
                case UnitAttribute.DARK:
                    return (Brush)(isTransparent
                        ? application.TryFindResource("DarkTransBrush")
                        : application.TryFindResource("DarkBrush"));
                case UnitAttribute.NONE:
                    return (Brush)(isTransparent
                        ? application.TryFindResource("NoneTransBrush")
                        : application.TryFindResource("NoneBrush"));
                default:
                    return Brushes.Transparent;
            }
        }

        public static Brush GetWarningBrush() {
            return (Brush) application.TryFindResource("FireTransBrush");
        }
        public static QuestOpenType GetQuestOpenType(int type, int param, int group) {
            QuestOpenType result = new QuestOpenType();
            result.Group = group;
            switch (type) {
                case 0: {
                        result.Type = string.Empty;
                        result.Param = string.Empty;
                        break;
                    }
                case 1: {
                        result.Type = "每周";
                        switch (param) {
                            case 0: result.Param = "日"; break;
                            case 1: result.Param = "一"; break;
                            case 2: result.Param = "二"; break;
                            case 3: result.Param = "三"; break;
                            case 4: result.Param = "四"; break;
                            case 5: result.Param = "五"; break;
                            case 6: result.Param = "六"; break;
                            case 7: result.Param = "日一二三四五六"; break;
                            default: break;
                        }
                        break;
                    }
                case 2: {
                        result.Type = "完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        result.Param = param + "|" + DAL.Get<string>(String.Format(sql, param));
                        break;
                    }
                case 3:   //not used
                    {
                        result.Type += "UserID";
                        result.Param += param;
                        break;
                    }
                case 4: {
                        result.Type = "开始日期";
                        result.Param = ParseRTDDate(param).ToString("yyyy-MM-dd HH:mm ddd");
                        break;
                    }
                case 5: {
                        result.Type = "结束日期";
                        result.Param = ParseRTDDate(param).ToString("yyyy-MM-dd HH:mm ddd");
                        break;
                    }
                case 6: {
                        result.Type = "是否关闭";
                        result.Param = param.ToString();
                        break;
                    }
                case 7: {
                        result.Type = "完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        result.Param = param + "|" + DAL.Get<string>(String.Format(sql, param));
                        break;
                    }
                case 8: {
                        result.Type = "支线任务";
                        result.Param = param.ToString();
                        break;
                    }
                case 9: {
                        result.Type = "不完成关卡";
                        string sql = @"SELECT name FROM quest_master WHERE id={0}";
                        result.Param = param + "|" + DAL.Get<string>(String.Format(sql, param));
                        break;
                    }
                case 10: {
                        result.Type = "自身等级大于等于";
                        result.Param = param.ToString();
                        break;
                    }
                case 11: {
                        result.Type = "自身等级小于等于";
                        result.Param = param.ToString();
                        break;
                    }
                case 12: {
                        result.Type = "教程通过";
                        result.Param = param.ToString();
                        break;
                    }
                case 13: {
                        result.Type = "教程未通过";
                        result.Param = param.ToString();
                        break;
                    }
                case 14: {
                        result.Type = "队长限定";
                        //result.OpentypeParam = opentypeParam + "|" + ParseUnitName(opentypeParam);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("角色组|" + param);
                        sb.Append(ParseUnitGroupName(param));
                        result.Param = sb.ToString();
                        break;
                    }
                case 15: {
                        result.Type = "当日限定";
                        DateTime day;
                        if (DateTime.TryParseExact(param.ToString("D8"), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out day)) {
                            result.Param = day.ToString("yyyy-MM-dd");
                        }
                        break;
                    }
                case 16: {
                        result.Type = "Everyday";
                        break;
                    }
                case 100: {
                        result.Type = "时间段内";
                        DateTime open = ParseRTDTime(param, true);
                        DateTime close = ParseRTDTime(group, true);
                        result.Param = string.Format("{0}~{1}", open.ToString("HH:mm"), close.ToString("HH:mm"));
                        result.Group = -1;
                        break;
                    }
                default: {
                        result.Type += type;
                        result.Param += param;
                        break;
                    }
            }
            return result;
        }
        private static readonly Regex CheckOnlyNumberRegex = new Regex("[^0-9]+", RegexOptions.Compiled); //regex that matches disallowed text

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
            var w = (MainWindow)Application.Current.MainWindow;
            w.StatusBarExceptionMessage.Text = message;
        }

        public static string GetUiText(string key) {
            return Application.Current.Resources[key].ToString();
        }

        public static void ChangeTab<T>() where T : UserControl {
            var w = (MainWindow)Application.Current.MainWindow;
            w.ChangeTabByName(typeof(T).Name);
        }
        public static T GetTab<T>(bool newTab = false, bool disableAutoLoad = false) where T : UserControl {
            if (newTab) {
                Utility.RefreshTabs<T>();
            }
            var w = (MainWindow)Application.Current.MainWindow;
            return (T)w.GetTabByName(typeof(T).Name, disableAutoLoad);
        }
        public static bool DisableBindData { get; set; }
        async public static void BindData(DataGrid dg, string sql, List<SQLiteParameter> paras = null) {
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
        public static T GetVisualChild<T>(DependencyObject parent) where T : DependencyObject {
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
        public static T GetLogicalChild<T>(DependencyObject parent) where T : DependencyObject {
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

        public static void RefreshTabs<T>() where T : UserControl {
            var w = (MainWindow)Application.Current.MainWindow;
            foreach (UserControl child in w.MainGrid.Children) {
                if (child is T) {
                    System.Diagnostics.Trace.WriteLine($"{child.GetType().Name} Tab has removed.");
                    w.MainGrid.Children.Remove(child);
                    break;
                }
            }
        }
        public static void GoToItemById<T>(int firstId, int lastId = -1, string type = null) where T : UserControl {
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