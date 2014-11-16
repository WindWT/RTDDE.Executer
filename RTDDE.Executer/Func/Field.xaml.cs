using RTDDE.Provider;
using RTDDE.Provider.MasterData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTDDE.Executer
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
                    Utility.ShowException(t.Exception.InnerException.Message);
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
                            Text = qfm.name,
                            TextWrapping = TextWrapping.Wrap
                        },
                    };
                    btn.Click += (sender, e) =>
                    {
                        Area.LoadArea((int)qfm.id);
                    };
                    btn.SetValue(Canvas.LeftProperty, (double)qfm.icon_pos_x * SCALE_PARAMETER + LEFT_OFFSET);
                    btn.SetValue(Canvas.TopProperty, (double)qfm.icon_pos_y * SCALE_PARAMETER + TOP_OFFSET);
                    FieldCanvas.Children.Add(btn);
                    //Add line between field
                    if (qfm.arrow_type > 0)
                    {
                        var line = new Line()
                        {
                            X1 = qfm.arrow_pos_x * SCALE_PARAMETER + LEFT_OFFSET,
                            Y1 = qfm.arrow_pos_y * SCALE_PARAMETER + TOP_OFFSET,
                            X2 = qfm.icon_pos_x * SCALE_PARAMETER + LEFT_OFFSET,
                            Y2 = qfm.icon_pos_y * SCALE_PARAMETER + TOP_OFFSET,
                            Stroke = new SolidColorBrush(Colors.Red),
                            StrokeThickness = 4,
                            Fill = Brushes.Black
                        };
                        FieldCanvas.Children.Add(line);
                    }
                }
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
