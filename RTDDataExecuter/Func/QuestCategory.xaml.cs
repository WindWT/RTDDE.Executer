using RTDDataProvider;
using RTDDataProvider.MasterData;
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
        public void Refresh()
        {
            QuestCategoryTypeRadio_Normal.IsChecked = false;
            QuestCategoryTypeRadio_Normal.IsChecked = true;
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
            Task<QuestCategoryMaster> task = new Task<QuestCategoryMaster>(() =>
            {
                string sql = "SELECT * FROM quest_category_master WHERE id={0}";
                return DAL.ToSingle<QuestCategoryMaster>(String.Format(sql, qcInfo_id));
            });
            Task<List<QuestMaster>> taskQuest = new Task<List<QuestMaster>>(() =>
            {
                string sql = "SELECT * FROM quest_master WHERE category={0} order by display_order DESC";
                return DAL.ToList<QuestMaster>(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskReward = new Task<DataTable>(() =>
            {
                string sql = @"select *,
(case when present_type=4 
then (select name from unit_master where unit_master.id=quest_challenge_reward_master.present_param_0) 
else present_param_0 end) as present_param_name 
from quest_challenge_reward_master WHERE category={0}
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
                QuestCategoryMaster qcm = t.Result;
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

                QuestCategoryInfo_name.Text = qcm.name;
                QuestCategoryInfo_order.Text = qcm.display_order.ToString();
                QuestCategoryInfo_icon.Text = qcm.icon;
                QuestCategoryInfo_kind.Text = Utility.ParseQuestKind(qcm.kind);
                QuestCategoryInfo_zbtn_kind.Text = Utility.ParseZBTNKind(qcm.kind);
                QuestCategoryInfo_pt_num.Text = qcm.pt_num.ToString();
                QuestCategoryInfo_text.Text = Utility.ParseText(qcm.text);

                List<QuestMaster> listQM = taskQuest.Result;
                if (listQM == null||listQM.Count==0 )
                {
                    QuestCategoryInfo_quest.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestCategoryInfo_quest.Children.Clear();
                    QuestCategoryInfo_quest.Visibility = Visibility.Visible;
                    foreach (QuestMaster qm in listQM)
                    {
                        QuestCategoryInfo_quest.Children.Add(new TextBlock()
                        {
                            Text = qm.id.ToString(),
                            Width = 50
                        });
                        QuestCategoryInfo_quest.Children.Add(new TextBox()
                        {
                            Text = qm.name,
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
                            Text = Utility.ParsePresenttype(drReward["present_type"].ToString()),
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
                w.Quest.QuestSearch_category.Text = id;
                w.Quest.QuestSearch_category_name.Text = name;
                w.Quest.QuestSearchExpander.IsExpanded = true;
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
            Utility.BindData(QuestCategoryDataGrid, sql);
        }
    }
}
