using RTDDataProvider;
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

namespace RTDDataExecuter
{
    /// <summary>
    /// QuestCategory.xaml 的交互逻辑
    /// </summary>
    public partial class QuestCategory : UserControl
    {
        public QuestCategory()
        {
            InitializeComponent();
        }
        private void QuestCategoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestCategoryDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string qcInfo_id = ((DataRowView)QuestCategoryDataGrid.SelectedItem).Row["id"].ToString();
            QuestCategoryInfo_id.Text = qcInfo_id;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = "SELECT * FROM quest_category_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskQuest = new Task<DataTable>(() =>
            {
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return null;
                }
                DataRow dr = task.Result.Rows[0];
                string sql = "SELECT * FROM quest_master WHERE category={0} order by display_order DESC";
                DB db = new DB();
                return db.GetData(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskReward = new Task<DataTable>(() =>
            {
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return null;
                }
                DataRow dr = task.Result.Rows[0];
                string sql = @"select *,
(case when present_type=4 
then (select name from unit_master where unit_master.id=quest_challenge_reward_master.present_param_0) 
else present_param_0 end) as present_param_name 
from quest_challenge_reward_master WHERE category={0}
order by point";
                DB db = new DB();
                return db.GetData(String.Format(sql, qcInfo_id));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (t.Result == null || t.Result.Rows.Count == 0)
                {
                    return;
                }
                DataRow dr = t.Result.Rows[0];
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

                QuestCategoryInfo_name.Text = dr["name"].ToString();
                QuestCategoryInfo_order.Text = dr["display_order"].ToString();
                QuestCategoryInfo_icon.Text = dr["icon"].ToString();
                QuestCategoryInfo_kind.Text = Utility.parseQuestKind(dr["kind"].ToString());
                QuestCategoryInfo_zbtn_kind.Text = Utility.parseZBTNKind(dr["zbtn_kind"].ToString());
                QuestCategoryInfo_pt_num.Text = dr["pt_num"].ToString();
                QuestCategoryInfo_text.Text = Utility.parseText(dr["text"].ToString());

                DataTable dtQuest = taskQuest.Result;
                if (dtQuest == null || dtQuest.Rows.Count == 0)
                {
                    QuestCategoryInfo_quest.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestCategoryInfo_quest.Children.Clear();
                    QuestCategoryInfo_quest.Visibility = Visibility.Visible;
                    foreach (DataRow drQuest in dtQuest.Rows)
                    {
                        QuestCategoryInfo_quest.Children.Add(new TextBlock()
                        {
                            Text = drQuest["id"].ToString(),
                            Width = 50
                        });
                        QuestCategoryInfo_quest.Children.Add(new TextBox()
                        {
                            Text = drQuest["name"].ToString(),
                            Width = 250
                        });
                    }
                    QuestCategoryInfo_quest.Children.Add(new Separator() { Width = 300 });
                }
                DataTable dtReward = taskReward.Result;
                if (dtReward == null || dtReward.Rows.Count == 0)
                {
                    QuestCategoryInfo_reward.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestCategoryInfo_reward.Children.Clear();
                    QuestCategoryInfo_reward.Visibility = Visibility.Visible;
                    foreach (DataRow drReward in dtReward.Rows)
                    {
                        QuestCategoryInfo_reward.Children.Add(new TextBlock()
                        {
                            Text = drReward["point"].ToString(),
                            Width = 25
                        });
                        QuestCategoryInfo_reward.Children.Add(new TextBox()
                        {
                            Text = Utility.parsePresenttype(drReward["present_type"].ToString()),
                            Width = 50
                        });
                        QuestCategoryInfo_reward.Children.Add(new TextBox()
                        {
                            Text = drReward["present_param_name"].ToString(),
                            Width = 175
                        });
                        QuestCategoryInfo_reward.Children.Add(new TextBox()
                        {
                            Text = drReward["present_param_1"].ToString(),
                            Width = 50
                        });
                    }
                    QuestCategoryInfo_reward.Children.Add(new Separator() { Width = 300 });
                }

            }, MainWindow.uiTaskScheduler);
            task.Start();
            taskQuest.Start();
            taskReward.Start();
        }
        private void QuestCategoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                DataGridRow dgr = sender as DataGridRow;
                string id = ((DataRowView)dgr.Item).Row["id"].ToString();
                string name = ((DataRowView)dgr.Item).Row["name"].ToString();
                var w = (MainWindow)Application.Current.MainWindow;
                ((TextBox)w.Quest.FindName("QuestSearch_category")).Text = id;
                ((TextBox)w.Quest.FindName("QuestSearch_category_name")).Text = name;
                ((Expander)w.Quest.FindName("QuestSearchExpander")).IsExpanded = true;
                w.ChangeTab("Quest");
            }
        }

        private void QuestCategoryTypeRadio_Normal_Checked(object sender, RoutedEventArgs e)
        {
            QuestCategoryTypeRadio_Checked(0);
        }
        private void QuestCategoryTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            QuestCategoryTypeRadio_Checked(1);
        }
        private void QuesCategorytTypeRadio_LargeEvent_Checked(object sender, RoutedEventArgs e)
        {
            QuestCategoryTypeRadio_Checked(2);
        }
        private void QuestCategoryTypeRadio_Special_Checked(object sender, RoutedEventArgs e)
        {
            QuestCategoryTypeRadio_Checked(3);
        }
        private void QuestCategoryTypeRadio_Checked(int zbtn_kind)
        {
            string sql = string.Format("SELECT id,name,text FROM quest_category_master WHERE zbtn_kind={0} order by id", zbtn_kind.ToString());
            QuestCategoryDataGrid_BindData(sql);
        }
        private void QuestCategoryDataGrid_BindData(string sql)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(sql);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                QuestCategoryDataGrid.ItemsSource = t.Result.DefaultView;
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
