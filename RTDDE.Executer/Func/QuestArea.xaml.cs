using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    public partial class QuestArea : UserControl
    {
        public QuestArea()
        {
            InitializeComponent();
        }
        private void QuestAreaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestAreaDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string qcInfo_id = ((DataRowView)QuestAreaDataGrid.SelectedItem).Row["id"].ToString();
            QuestAreaInfo_id.Text = qcInfo_id;
            Task<QuestAreaMaster> task = new Task<QuestAreaMaster>(() =>
            {
                string sql = "SELECT * FROM quest_area_master WHERE id={0}";
                return DAL.ToSingle<QuestAreaMaster>(String.Format(sql, qcInfo_id));
            });
            Task<List<QuestMaster>> taskQuest = new Task<List<QuestMaster>>(() =>
            {
                string sql = "SELECT * FROM quest_master WHERE parent_area_id={0} order by display_order DESC";
                return DAL.ToList<QuestMaster>(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskReward = new Task<DataTable>(() =>
            {
                string sql = @"select *,
(case when present_type=4 
then (select name from unit_master where unit_master.id=quest_challenge_reward_master.present_param_0) 
else present_param_0 end) as present_param_name 
from quest_challenge_reward_master WHERE parent_area_id={0}
order by point";
                return DAL.GetDataTable(String.Format(sql, qcInfo_id));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (t.Result == null)
                {
                    return;
                }
                QuestAreaMaster qam = t.Result;
                Task.WaitAll(taskQuest, taskReward);
                if (taskQuest.Exception != null)
                {
                    Utility.ShowException(taskQuest.Exception.InnerException.Message);
                    return;
                }
                if (taskReward.Exception != null)
                {
                    Utility.ShowException(taskReward.Exception.InnerException.Message);
                    return;
                }

                QuestAreaInfo_name.Text = qam.name;
                QuestAreaInfo_display_order.Text = qam.display_order.ToString();
                QuestAreaInfo_icon_texture.Text = qam.icon_texture;
                QuestAreaInfo_text.Text = Utility.ParseText(qam.text);

                List<QuestMaster> listQM = taskQuest.Result;
                if (listQM == null||listQM.Count==0 )
                {
                    QuestAreaInfo_quest.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestAreaInfo_quest.Children.Clear();
                    QuestAreaInfo_quest.Visibility = Visibility.Visible;
                    foreach (QuestMaster qm in listQM)
                    {
                        QuestAreaInfo_quest.Children.Add(new TextBlock()
                        {
                            Text = qm.id.ToString(),
                            Width = 50
                        });
                        QuestAreaInfo_quest.Children.Add(new TextBox()
                        {
                            Text = qm.name,
                            Width = 250
                        });
                    }
                    QuestAreaInfo_quest.Children.Add(new Separator() { Width = 300 });
                }
                DataTable dtReward = taskReward.Result;
                if (dtReward == null || dtReward.Rows.Count == 0)
                {
                    QuestAreaInfo_reward.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestAreaInfo_reward.Children.Clear();
                    QuestAreaInfo_reward.Visibility = Visibility.Visible;
                    foreach (DataRow drReward in dtReward.Rows)
                    {
                        QuestAreaInfo_reward.Children.Add(new TextBlock()
                        {
                            Text = drReward["point"].ToString(),
                            Width = 25
                        });
                        QuestAreaInfo_reward.Children.Add(new TextBox()
                        {
                            Text = Utility.ParsePresenttype(drReward["present_type"].ToString()),
                            Width = 50
                        });
                        QuestAreaInfo_reward.Children.Add(new TextBox()
                        {
                            Text = drReward["present_param_name"].ToString(),
                            Width = 175
                        });
                        QuestAreaInfo_reward.Children.Add(new TextBox()
                        {
                            Text = drReward["present_param_1"].ToString(),
                            Width = 50
                        });
                    }
                    QuestAreaInfo_reward.Children.Add(new Separator() { Width = 300 });
                }

            }, MainWindow.UiTaskScheduler);
            task.Start();
            taskQuest.Start();
            taskReward.Start();
        }
        private void QuestAreaDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                string id = ((DataRowView)dgr.Item).Row["id"].ToString();
                string name = ((DataRowView)dgr.Item).Row["name"].ToString();
                var quest = (Quest)Utility.GetTabByName("Quest");
                quest.QuestSearch_parent_area_id.Text = id;
                quest.QuestSearch_parent_area_name.Text = name;
                quest.QuestSearchExpander.IsExpanded = true;
                Utility.ChangeTab("Quest");
            }
        }

        private void QuestAreaTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT id,name,text FROM quest_area_master WHERE parent_field_id>0 order by id DESC";
            Utility.BindData(QuestAreaDataGrid, sql);
        }
        private void QuestAreaTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT id,name,text FROM quest_area_master WHERE parent_field_id=0 order by id DESC";
            Utility.BindData(QuestAreaDataGrid, sql);
        }
    }
}
