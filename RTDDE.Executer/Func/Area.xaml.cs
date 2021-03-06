﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Common.xaml 的交互逻辑
    /// </summary>
    public partial class Area : UserControl
    {
        public Area()
        {
            InitializeComponent();
        }
        private static readonly double SCALE_PARAMETER = 0.5d;
        private static readonly double LEFT_OFFSET = -50d;
        private static readonly double TOP_OFFSET = -125d;
        public void LoadArea(int fieldId)
        {
            AreaCanvas.Children.Clear();
            AreaInfoStackPanel.Visibility = Visibility.Collapsed;
            Task<List<QuestAreaMaster>> task = new Task<List<QuestAreaMaster>>(() =>
            {
                return DAL.ToList<QuestAreaMaster>(string.Format("SELECT * FROM QUEST_AREA_MASTER WHERE parent_field_id={0} ORDER BY ID", fieldId));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception);
                    return;
                }
                foreach (var qam in t.Result)
                {
                    var btn = new Button()
                    {
                        Width = qam.icon_col_w * SCALE_PARAMETER,
                        Height = qam.icon_col_h * SCALE_PARAMETER,
                        Content = new TextBlock()
                        {
                            Text = qam.name,
                            TextWrapping = TextWrapping.Wrap
                        },
                    };
                    btn.SetValue(Canvas.LeftProperty, (qam.icon_pos_x - qam.icon_col_w/2)*SCALE_PARAMETER + LEFT_OFFSET);
                    btn.SetValue(Canvas.TopProperty, (qam.icon_pos_y - qam.icon_col_h/2)*SCALE_PARAMETER + TOP_OFFSET);
                    btn.SetValue(Grid.ZIndexProperty, 128);
                    btn.Click += (e, s) =>
                    {
                        LoadAreaInfo(qam);
                        if (qam.move_field_id > 0)
                        {
                            LoadArea((int)qam.move_field_id);
                        }
                    };
                    AreaCanvas.Children.Add(btn);
                    QuestAreaMaster nextQam = t.Result.Find(o => o.id == qam.connect_area_id);
                    if (nextQam != null)
                    {
                        AreaCanvas.Children.Add(GetAreaLine(qam, nextQam));
                    }

                }
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        public void LoadEventMark(MapEventMaster mem) {
            int markSize = 16;
            var rec = new Rectangle {
                Width = markSize,
                Height = markSize,
                Stroke = Brushes.Red,
                StrokeThickness = 5,
                SnapsToDevicePixels = true
            };
            rec.SetValue(Canvas.LeftProperty, (mem.icon_pos_x - markSize/2)*SCALE_PARAMETER + LEFT_OFFSET);
            rec.SetValue(Canvas.TopProperty, (mem.icon_pos_y - markSize/2)*SCALE_PARAMETER + TOP_OFFSET);
            rec.SetValue(Grid.ZIndexProperty, 129);
            AreaCanvas.Children.Add(rec);
        }

        private void LoadAreaInfo(QuestAreaMaster qam) {
            AreaInfoStackPanel.Visibility = Visibility.Visible;
            AreaInfo_id.Text = qam.id.ToString();
            AreaInfo_name.Text = qam.name;
            AreaInfo_text.Document = Utility.ParseTextToDocument(qam.text);
        }
        private void AreaInfoToQuestAreaButton_OnClick(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(AreaInfo_id.Text) == false) {
                Utility.GoToItemById<QuestArea>(Convert.ToInt32(AreaInfo_id.Text));
            }
        }
        private Line GetAreaLine(QuestAreaMaster thisArea, QuestAreaMaster nextArea)
        {
            Line line = new Line();
            line.X1 = thisArea.icon_pos_x*SCALE_PARAMETER + LEFT_OFFSET;
            line.Y1 = thisArea.icon_pos_y*SCALE_PARAMETER + TOP_OFFSET;
            line.X2 = nextArea.icon_pos_x*SCALE_PARAMETER + LEFT_OFFSET;
            line.Y2 = nextArea.icon_pos_y*SCALE_PARAMETER + TOP_OFFSET;
            line.Stroke = new SolidColorBrush(Color.FromRgb(153, 255, 119));
            line.StrokeThickness = 4;
            line.StrokeDashArray = new DoubleCollection() { 0.75, 0.75 };
            return line;
        }
    }
}
