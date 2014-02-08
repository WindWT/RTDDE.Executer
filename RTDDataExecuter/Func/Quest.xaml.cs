using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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
    /// Quest.xaml 的交互逻辑
    /// </summary>
    public partial class Quest : UserControl
    {
        public Quest()
        {
            InitializeComponent();
        }
        private void QuestDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string eqInfo_id = ((DataRowView)QuestDataGrid.SelectedItem).Row["id"].ToString();
            QuestInfo_id.Text = eqInfo_id;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"SELECT id,name,pt_num,difficulty,stamina,soul,
reward_money,reward_exp,reward_soul,category,
(select name from quest_category_master where quest_category_master.id=category) as category_name,
(select text from quest_category_master where quest_category_master.id=category) as category_text,
open_type_1,open_param_1,
open_type_2,open_param_2,
open_type_3,open_param_3,
open_type_4,open_param_4,
open_type_5,open_param_5,
open_type_6,open_param_6,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.sp_event_id) as sp_event_name,
sp_event_id,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.open_sp_event_id) as open_sp_event_name,open_sp_event_point,
bonus_type,bonus_start,bonus_end,
present_type,present_param,present_param_1,
(case when present_type=4 then (select name from unit_master where unit_master.id=quest_master.present_param) else present_param end) as present_param_name ,
panel_sword,panel_lance,panel_archer,panel_cane,panel_heart,panel_sp,
bgm_f,bgm_b,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_0) as challenge0,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_1) as challenge1,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_2) as challenge2
  FROM quest_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, eqInfo_id));
            });
            Task<List<Dictionary<string, string>>> taskParse = new Task<List<Dictionary<string, string>>>(() =>
            {
                List<Dictionary<string, string>> opentypeList = new List<Dictionary<string, string>>();
                Task.WaitAll(task);
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return null;
                }
                DataRow dr = task.Result.Rows[0];
                opentypeList.Add(Utility.parseOpentype(dr["open_type_1"].ToString(), dr["open_param_1"].ToString()));
                opentypeList.Add(Utility.parseOpentype(dr["open_type_2"].ToString(), dr["open_param_2"].ToString()));
                opentypeList.Add(Utility.parseOpentype(dr["open_type_3"].ToString(), dr["open_param_3"].ToString()));
                opentypeList.Add(Utility.parseOpentype(dr["open_type_4"].ToString(), dr["open_param_4"].ToString()));
                opentypeList.Add(Utility.parseOpentype(dr["open_type_5"].ToString(), dr["open_param_5"].ToString()));
                opentypeList.Add(Utility.parseOpentype(dr["open_type_6"].ToString(), dr["open_param_6"].ToString()));
                return opentypeList;
            }
            );
            taskParse.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return;
                }
                DataRow dr = task.Result.Rows[0];
                QuestInfo_name.Text = dr["name"].ToString();
                QuestInfo_pt.Text = dr["pt_num"].ToString();
                QuestInfo_difficulty.Text = dr["difficulty"].ToString();
                QuestInfo_stamina.Text = dr["stamina"].ToString();
                QuestInfo_category.Text = dr["category"].ToString();
                QuestInfo_category_name.Text = dr["category_name"].ToString();
                QuestInfo_text.Text = Utility.parseText(dr["category_text"].ToString());
                QuestInfo_reward_money.Text = dr["reward_money"].ToString();
                QuestInfo_reward_exp.Text = dr["reward_exp"].ToString();
                QuestInfo_init_soul.Text = dr["soul"].ToString();
                QuestInfo_bgm_f.Text = Utility.parseBgmFileName(Convert.ToInt32(dr["bgm_f"]));
                QuestInfo_bgm_b.Text = Utility.parseBgmFileName(Convert.ToInt32(dr["bgm_b"]));

                QuestInfo_opentype1_name.Text = t.Result[0]["opentype"];
                QuestInfo_opentype1_value.Text = t.Result[0]["opentypeParam"];
                QuestInfo_opentype2_name.Text = t.Result[1]["opentype"];
                QuestInfo_opentype2_value.Text = t.Result[1]["opentypeParam"];
                QuestInfo_opentype3_name.Text = t.Result[2]["opentype"];
                QuestInfo_opentype3_value.Text = t.Result[2]["opentypeParam"];
                QuestInfo_opentype4_name.Text = t.Result[3]["opentype"];
                QuestInfo_opentype4_value.Text = t.Result[3]["opentypeParam"];
                QuestInfo_opentype5_name.Text = t.Result[4]["opentype"];
                QuestInfo_opentype5_value.Text = t.Result[4]["opentypeParam"];
                QuestInfo_opentype6_name.Text = t.Result[5]["opentype"];
                QuestInfo_opentype6_value.Text = t.Result[5]["opentypeParam"];
                if (string.IsNullOrWhiteSpace(dr["open_sp_event_name"].ToString()))
                {
                    QuestInfo_open_sp_event_name.Visibility = Visibility.Collapsed;
                    QuestInfo_open_sp_event_point.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestInfo_open_sp_event_name.Visibility = Visibility.Visible;
                    QuestInfo_open_sp_event_point.Visibility = Visibility.Visible;
                    QuestInfo_open_sp_event_name.Text = dr["open_sp_event_name"].ToString();
                    QuestInfo_open_sp_event_point.Text = dr["open_sp_event_point"].ToString();
                }

                if (string.IsNullOrWhiteSpace(dr["sp_event_id"].ToString())
                    || dr["sp_event_id"].ToString() == "0")
                {
                    QuestInfo_sp_event.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestInfo_sp_event.Visibility = Visibility.Visible;
                    QuestInfo_sp_event_id.Text = dr["sp_event_id"].ToString();
                    QuestInfo_sp_event_name.Text = dr["sp_event_name"].ToString();
                }

                QuestInfo_bonus.Text = Utility.parseBonustype(dr["bonus_type"].ToString());
                QuestInfo_bonus_start.Text = Utility.parseRTDDate(dr["bonus_start"].ToString());
                QuestInfo_bonus_end.Text = Utility.parseRTDDate(dr["bonus_end"].ToString());

                QuestInfo_panel_sword.Text = dr["panel_sword"].ToString();
                QuestInfo_panel_lance.Text = dr["panel_lance"].ToString();
                QuestInfo_panel_archer.Text = dr["panel_archer"].ToString();
                QuestInfo_panel_cane.Text = dr["panel_cane"].ToString();
                QuestInfo_panel_heart.Text = dr["panel_heart"].ToString();
                QuestInfo_panel_sp.Text = dr["panel_sp"].ToString();

                if (string.IsNullOrWhiteSpace(dr["challenge0"].ToString())
                    && string.IsNullOrWhiteSpace(dr["challenge1"].ToString())
                    && string.IsNullOrWhiteSpace(dr["challenge2"].ToString()))
                {
                    QuestInfo_challenge.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestInfo_challenge.Visibility = Visibility.Visible;
                    QuestInfo_challenge_gold.Document = Utility.parseTextToDocument(dr["challenge2"].ToString());
                    QuestInfo_challenge_silver.Document = Utility.parseTextToDocument(dr["challenge1"].ToString());
                    QuestInfo_challenge_bronze.Document = Utility.parseTextToDocument(dr["challenge0"].ToString());
                }

                QuestInfo_present_type.Text = Utility.parsePresenttype(dr["present_type"].ToString());
                QuestInfo_present_param.Text = dr["present_param_name"].ToString();
                QuestInfo_present_param_1.Text = dr["present_param_1"].ToString();
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskParse.Start();
        }

        private void QuestTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end]
  FROM quest_master
 ORDER BY start DESC,end DESC,id DESC;";
            QuestDataGrid_BindData(sql);
        }

        private void QuestTypeRadio_Daily_Checked(object sender, RoutedEventArgs e)
        {
            string today = DateTime.Today.AddHours(1).ToString("yyyyMMddHH");
            string sql = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
       ( CASE
                WHEN open_type_1 = 1 THEN open_param_1 
                WHEN open_type_2 = 1 THEN open_param_2 
                WHEN open_type_3 = 1 THEN open_param_3 
                WHEN open_type_4 = 1 THEN open_param_4 
                WHEN open_type_5 = 1 THEN open_param_5 
                WHEN open_type_6 = 1 THEN open_param_6 
                ELSE -1
       END ) AS DayOfWeek,
       ( CASE
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                ELSE 0 
       END ) AS [end],
       ( CASE
                WHEN open_type_1 = 6 THEN open_param_1 
                WHEN open_type_2 = 6 THEN open_param_2 
                WHEN open_type_3 = 6 THEN open_param_3 
                WHEN open_type_4 = 6 THEN open_param_4 
                WHEN open_type_5 = 6 THEN open_param_5 
                WHEN open_type_6 = 6 THEN open_param_6 
                ELSE 0 
       END ) AS isDisabled
FROM QUEST_MASTER
WHERE DayOfWeek>=0
AND isDisabled=0
AND ([end]>" + today + @" OR [end]=0)
ORDER BY DayOfWeek,id DESC";
            QuestDataGrid_BindData(sql);
        }

        private void QuestTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category
FROM QUEST_MASTER
WHERE category<1000
ORDER BY id DESC";
            QuestDataGrid_BindData(sql);
        }

        private void QuestSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuestTypeRadio_Event.IsChecked = false;
            QuestTypeRadio_Daily.IsChecked = false;
            QuestTypeRadio_Main.IsChecked = false;

            string sql = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category_name
FROM QUEST_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(QuestSearch_id.Text) == false)
            {
                sql += "id=" + QuestSearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_name.Text) == false)
            {
                sql += "name LIKE '%" + QuestSearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_category.Text) == false)
            {
                sql += "category=" + QuestSearch_category.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_category_name.Text) == false)
            {
                sql += "category_name LIKE '%" + QuestSearch_category_name.Text.Trim() + "%' AND ";
            }
            sql += " 1=1 ORDER BY id DESC";
            QuestDataGrid_BindData(sql);
        }

        private void QuestSearchClear_Click(object sender, RoutedEventArgs e)
        {
            QuestSearch_id.Text = String.Empty;
            QuestSearch_name.Text = String.Empty;
            QuestSearch_category.Text = String.Empty;
            QuestSearch_category_name.Text = String.Empty;
            QuestTypeRadio_Event.IsChecked = true;
        }

        private void QuestDataGrid_BindData(string sql)
        {
            QuestDataGrid_BindData(sql, null);
        }
        private void QuestDataGrid_BindData(string sql, List<SQLiteParameter> paras)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(sql, paras);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                QuestDataGrid.ItemsSource = t.Result.DefaultView;
            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
