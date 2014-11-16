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
                    AreaCanvas.Children.Add(btn);
                }
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
