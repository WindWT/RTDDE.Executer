using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
    public partial class MainWindow : Window
    {
        private static string QuestCategoryViewerSQL = "SELECT id,name,display_order FROM quest_category_master order by id";
        private void QuestCategoryViewerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestCategoryViewerDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string qcInfo_id = ((DataRowView)QuestCategoryViewerDataGrid.SelectedItem).Row["id"].ToString();
            QuestCategoryInfo_id.Text = qcInfo_id;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = "SELECT * FROM quest_category_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskQuest = new Task<DataTable>(() =>
            {
                DataRow dr = task.Result.Rows[0];
                if (dr == null || dr.ItemArray.Length == 0)
                {
                    return null;
                }
                string sql = "SELECT * FROM quest_master WHERE category={0} order by display_order DESC";
                DB db = new DB();
                return db.GetData(String.Format(sql, qcInfo_id));
            });
            Task<DataTable> taskReward = new Task<DataTable>(() =>
            {
                DataRow dr = task.Result.Rows[0];
                if (dr == null || dr.ItemArray.Length == 0)
                {
                    return null;
                }
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
                        StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                        return;
                    }
                    DataRow dr = t.Result.Rows[0];
                    if (dr == null || dr.ItemArray.Length == 0)
                    {
                        return;
                    }
                    Task.WaitAll(taskQuest, taskReward);
                    if (taskQuest.Exception != null)
                    {
                        StatusBarExceptionMessage.Text = taskQuest.Exception.InnerException.Message;
                        return;
                    }
                    if (taskReward.Exception != null)
                    {
                        StatusBarExceptionMessage.Text = taskReward.Exception.InnerException.Message;
                        return;
                    }

                    QuestCategoryInfo_name.Text = dr["name"].ToString();
                    QuestCategoryInfo_order.Text = dr["display_order"].ToString();
                    QuestCategoryInfo_icon.Text = dr["icon"].ToString();
                    QuestCategoryInfo_kind.Text = parseQuestKind(dr["kind"].ToString());
                    QuestCategoryInfo_zbtn_kind.Text = parseZBTNKind(dr["zbtn_kind"].ToString());
                    QuestCategoryInfo_pt_num.Text = dr["pt_num"].ToString();
                    QuestCategoryInfo_text.Text = parseText(dr["text"].ToString());

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
                                Text = parsePresenttype(drReward["present_type"].ToString()),
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

                }, uiTaskScheduler);
            task.Start();
            taskQuest.Start();
            taskReward.Start();
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
            QuestCategoryViewerSQL = string.Format("SELECT id,name,text FROM quest_category_master WHERE zbtn_kind={0} order by id", zbtn_kind.ToString());
            QuestCategoryViewerDataGrid_BindData();
        }
        private void QuestCategoryViewerDataGrid_BindData()
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(QuestCategoryViewerSQL);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                QuestCategoryViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
