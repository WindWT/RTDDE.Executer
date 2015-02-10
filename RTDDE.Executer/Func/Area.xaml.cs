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
        private static readonly double LEFT_OFFSET = -75d;
        private static readonly double TOP_OFFSET = -150d;
        public void LoadArea(int fieldId)
        {
            Task<List<QuestAreaMaster>> task = new Task<List<QuestAreaMaster>>(() =>
            {
                return DAL.ToList<QuestAreaMaster>(string.Format("SELECT * FROM QUEST_AREA_MASTER WHERE parent_field_id={0} ORDER BY ID", fieldId));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                AreaCanvas.Children.Clear();
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
                    btn.SetValue(Canvas.LeftProperty, (double)qam.icon_pos_x * SCALE_PARAMETER + LEFT_OFFSET);
                    btn.SetValue(Canvas.TopProperty, (double)qam.icon_pos_y * SCALE_PARAMETER + TOP_OFFSET);
                    btn.SetValue(Grid.ZIndexProperty, 128);
                    btn.Click += (e, s) =>
                    {
                        if (qam.move_field_id > 0)
                        {
                            LoadArea((int)qam.move_field_id);
                        }
                        AreaInfo_Name.Text = qam.name;
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
        private Line GetAreaLine(QuestAreaMaster thisArea, QuestAreaMaster nextArea)
        {
            Line line = new Line();
            line.X1 = (thisArea.icon_pos_x + thisArea.icon_col_w / 2) * SCALE_PARAMETER + LEFT_OFFSET;
            line.Y1 = (thisArea.icon_pos_y + thisArea.icon_col_h / 2) * SCALE_PARAMETER + TOP_OFFSET;
            line.X2 = (nextArea.icon_pos_x + nextArea.icon_col_w / 2) * SCALE_PARAMETER + LEFT_OFFSET;
            line.Y2 = (nextArea.icon_pos_y + nextArea.icon_col_h / 2) * SCALE_PARAMETER + TOP_OFFSET;
            line.Stroke = new SolidColorBrush(Colors.Red);
            line.StrokeThickness = 3;
            return line;
        }
    }
}
