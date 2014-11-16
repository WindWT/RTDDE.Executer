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
    public partial class World : UserControl
    {
        public World()
        {
            InitializeComponent();
        }

        private void World_Initialized(object sender, EventArgs e)
        {
            Task<List<QuestWorldMaster>> task = new Task<List<QuestWorldMaster>>(() =>
            {
                return DAL.ToList<QuestWorldMaster>("SELECT * FROM QUEST_WORLD_MASTER ORDER BY ID");
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                WorldButtonStackPanel.Children.Clear();
                foreach (var qwm in t.Result)
                {
                    var btn = new Button()
                    {
                        Content = qwm.name
                    };
                    btn.Click += (s, args) =>
                        {
                            Field.LoadField((int)qwm.id);
                        };
                    WorldButtonStackPanel.Children.Add(btn);
                }
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
