using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace RTDDataExecuter
{
    public sealed class Utility : UtilityBase
    {
        public static FlowDocument parseTextToDocument(string text)
        {
            var flowDoc = new FlowDocument();
            //string[] textParas = text.Split(new string[] { "\\n" }, StringSplitOptions.None);
            text = text.Replace(@"\n", "\n");
            Paragraph pr = new Paragraph(); //prprpr
            pr.Margin = new Thickness(0);
            Regex rSplit = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            Regex rColor = new Regex(@"(\[[a-zA-Z0-9]{6}\])");
            var textParts = rSplit.Split(text);
            var nowFontColor = Brushes.Black;
            foreach (string textPart in textParts)
            {
                Span span = new Span();
                if (rColor.Match(textPart).Success)
                {
                    string color = textPart.Trim(new char[] { '[', ']' });
                    nowFontColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#" + color));
                    continue;
                }
                if (textPart == "[-]")
                {
                    nowFontColor = Brushes.Black;
                    continue;
                }
                span.Inlines.Add(new Run(textPart));
                span.Foreground = nowFontColor;
                pr.Inlines.Add(span);
            }
            flowDoc.Blocks.Add(pr);
            return flowDoc;
        }
        public static void ShowException(string message)
        {
            var w = (MainWindow)Application.Current.MainWindow;
            ((TextBox)w.FindName("StatusBarExceptionMessage")).Text = message;
        }
    }
}