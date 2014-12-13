using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    public partial class World : UserControl, IRefreshable
    {
        public World()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            Task<List<QuestWorldMaster>> task = new Task<List<QuestWorldMaster>>(() => DAL.ToAllList<QuestWorldMaster>("id"));
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
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            Field.Refresh();
        }

        private void World_Initialized(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
