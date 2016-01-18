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
    public partial class Field : UserControl
    {
        public Field()
        {
            InitializeComponent();
        }
        private static readonly double SCALE_PARAMETER = 0.5d;
        private static readonly double LEFT_OFFSET = -50d;
        private static readonly double TOP_OFFSET = -150d;
        public void LoadField(int worldId)
        {
            Task<List<QuestFieldMaster>> task = new Task<List<QuestFieldMaster>>(() =>
            {
                return DAL.ToList<QuestFieldMaster>(string.Format("SELECT * FROM QUEST_FIELD_MASTER WHERE parent_world_id={0} ORDER BY ID", worldId));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception);
                    return;
                }
                FieldCanvas.Children.Clear();
                foreach (var qfm in t.Result)
                {
                    //Add field
                    var btn = new Button()
                    {
                        Width = qfm.icon_col_w * SCALE_PARAMETER,
                        Height = qfm.icon_col_h * SCALE_PARAMETER,
                        Content = new TextBlock()
                        {
                            Text = qfm.name_short,
                            TextWrapping = TextWrapping.Wrap
                        },
                    };
                    btn.Click += (sender, e) =>
                    {
                        Area.LoadArea((int)qfm.id);
                        LoadFieldInfo(qfm);
                    };
                    btn.SetValue(Canvas.LeftProperty, (double)qfm.icon_pos_x * SCALE_PARAMETER + LEFT_OFFSET);
                    btn.SetValue(Canvas.TopProperty, (double)qfm.icon_pos_y * SCALE_PARAMETER + TOP_OFFSET);
                    FieldCanvas.Children.Add(btn);
                    //Add line between field
                    if (qfm.arrow_type > 0)
                    {
                        var lineCanvas = GetArrowCanvas(qfm.arrow_type, qfm.arrow_rotate, qfm.arrow_reverse);
                        lineCanvas.SetValue(Canvas.LeftProperty, qfm.arrow_pos_x * SCALE_PARAMETER + LEFT_OFFSET + 25);   //use magic number 25, hope won't break in 17 years
                        lineCanvas.SetValue(Canvas.TopProperty, qfm.arrow_pos_y * SCALE_PARAMETER + TOP_OFFSET);
                        lineCanvas.SetValue(Grid.ZIndexProperty, 128);
                        FieldCanvas.Children.Add(lineCanvas);
                    }
                }
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }

        private void LoadFieldInfo(QuestFieldMaster qfm) {
            FieldInfo_id.Text = qfm.id.ToString();
            FieldInfo_name.Text = qfm.name;
            FieldInfo_name_short.Text = qfm.name_short;
        }
        private Canvas GetArrowCanvas(int type = 1, int rotation = 0, byte reverse = 0)
        {
            var canvas = new Canvas();
            Brush strokeBrush = new SolidColorBrush(Color.FromRgb(104, 245, 215));
            strokeBrush.Freeze();
            switch (type)
            {
                case 1:
                    {
                        var line1 = new Line()
                        {
                            X1 = 45 * SCALE_PARAMETER,
                            Y1 = 5 * SCALE_PARAMETER,
                            X2 = 5 * SCALE_PARAMETER,
                            Y2 = 75 * SCALE_PARAMETER,
                            Stroke = strokeBrush,
                            StrokeThickness = 5
                        };
                        var line2 = new Line()
                        {
                            X1 = 45 * SCALE_PARAMETER,
                            Y1 = 105 * SCALE_PARAMETER,
                            X2 = 5 * SCALE_PARAMETER,
                            Y2 = 75 * SCALE_PARAMETER,
                            Stroke = strokeBrush,
                            StrokeThickness = 5
                        };
                        canvas.Children.Add(line1);
                        canvas.Children.Add(line2);
                        var transform = new TransformGroup();
                        transform.Children.Add(new RotateTransform(-rotation, 25 * SCALE_PARAMETER, 55 * SCALE_PARAMETER));
                        if (reverse == 1)
                        {
                            transform.Children.Add(new ScaleTransform(-1, 1, 25 * SCALE_PARAMETER, 55 * SCALE_PARAMETER));
                        }
                        canvas.RenderTransform = transform;
                        break;
                    }
                case 2:
                    {
                        var line1 = new Line()
                        {
                            X1 = 120 * SCALE_PARAMETER,
                            Y1 = 70 * SCALE_PARAMETER,
                            X2 = 45 * SCALE_PARAMETER,
                            Y2 = 57 * SCALE_PARAMETER,
                            Stroke = strokeBrush,
                            StrokeThickness = 5
                        };
                        var line2 = new Line()
                        {
                            X1 = 45 * SCALE_PARAMETER,
                            Y1 = 57 * SCALE_PARAMETER,
                            X2 = 5 * SCALE_PARAMETER,
                            Y2 = 68 * SCALE_PARAMETER,
                            Stroke = strokeBrush,
                            StrokeThickness = 5
                        };
                        canvas.Children.Add(line1);
                        canvas.Children.Add(line2);
                        var transform = new TransformGroup();
                        transform.Children.Add(new RotateTransform(-rotation, 55 * SCALE_PARAMETER, 40 * SCALE_PARAMETER));
                        if (reverse == 1)
                        {
                            transform.Children.Add(new ScaleTransform(-1, 1, 55 * SCALE_PARAMETER, 40 * SCALE_PARAMETER));
                        }
                        canvas.RenderTransform = transform;
                        break;
                    }
                default: break;
            }
            return canvas;
        }
    }
}
