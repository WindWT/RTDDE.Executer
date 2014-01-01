using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RTDDataExecuter
{
    public partial class MainWindow : Window
    {
        private static string QuestViewerSQL = @"SELECT id,
       name,stamina,
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

        private void QuestViewerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestViewerDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string eqInfo_id = ((DataRowView)QuestViewerDataGrid.SelectedItem).Row["id"].ToString();
            QuestInfo_id.Text = eqInfo_id;
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"SELECT id,name,pt_num,difficulty,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
(select text from quest_category_master where quest_category_master.id=category) as text,
open_type_1,open_param_1,
open_type_2,open_param_2,
open_type_3,open_param_3,
open_type_4,open_param_4,
open_type_5,open_param_5,
open_type_6,open_param_6,
bonus_type,bonus_start,bonus_end,
present_type,present_param,present_param_1,
panel_sword,panel_lance,panel_archer,panel_cane,panel_heart,panel_sp,
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
                DataRow dr = task.Result.Rows[0];
                if (dr == null || dr.ItemArray.Length == 0)
                {
                    return null;
                }
                opentypeList.Add(parseOpentype(dr["open_type_1"].ToString(), dr["open_param_1"].ToString()));
                opentypeList.Add(parseOpentype(dr["open_type_2"].ToString(), dr["open_param_2"].ToString()));
                opentypeList.Add(parseOpentype(dr["open_type_3"].ToString(), dr["open_param_3"].ToString()));
                opentypeList.Add(parseOpentype(dr["open_type_4"].ToString(), dr["open_param_4"].ToString()));
                opentypeList.Add(parseOpentype(dr["open_type_5"].ToString(), dr["open_param_5"].ToString()));
                opentypeList.Add(parseOpentype(dr["open_type_6"].ToString(), dr["open_param_6"].ToString()));
                opentypeList.Add(parsePresenttype(dr["present_type"].ToString(), dr["present_param"].ToString()));
                return opentypeList;
            }
            );
            taskParse.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                DataRow dr = task.Result.Rows[0];
                if (dr == null || dr.ItemArray.Length == 0)
                {
                    return;
                }
                QuestInfo_name.Text = dr["name"].ToString();
                QuestInfo_pt.Text = dr["pt_num"].ToString();
                QuestInfo_difficulty.Text = dr["difficulty"].ToString();
                QuestInfo_stamina.Text = dr["stamina"].ToString();
                QuestInfo_category.Text = dr["category"].ToString();
                QuestInfo_text.Text = parseText(dr["text"].ToString());

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

                QuestInfo_bonus.Text = parseBonustype(dr["bonus_type"].ToString());
                QuestInfo_bonus_start.Text = parseRTDDate(dr["bonus_start"].ToString());
                QuestInfo_bonus_end.Text = parseRTDDate(dr["bonus_end"].ToString());

                QuestInfo_panel_sword.Text = dr["panel_sword"].ToString();
                QuestInfo_panel_lance.Text = dr["panel_lance"].ToString();
                QuestInfo_panel_archer.Text = dr["panel_archer"].ToString();
                QuestInfo_panel_cane.Text = dr["panel_cane"].ToString();
                QuestInfo_panel_heart.Text = dr["panel_heart"].ToString();
                QuestInfo_panel_sp.Text = dr["panel_sp"].ToString();

                QuestInfo_challenge0.Document = parseTextToDocument(dr["challenge0"].ToString());
                QuestInfo_challenge1.Document = parseTextToDocument(dr["challenge1"].ToString());
                QuestInfo_challenge2.Document = parseTextToDocument(dr["challenge2"].ToString());

                QuestInfo_present_type.Text = t.Result[6]["presenttype"];
                QuestInfo_present_param.Text = t.Result[6]["presentParam"];
                QuestInfo_present_param_1.Text = dr["present_param_1"].ToString();
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskParse.Start();
        }

        private void QuestTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            QuestViewerSQL = @"SELECT id,name,stamina,
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
            QuestViewerDataGrid_BindData();
        }

        private void QuestTypeRadio_Daily_Checked(object sender, RoutedEventArgs e)
        {
            string today = DateTime.Today.AddHours(1).ToString("yyyyMMddHH");
            QuestViewerSQL = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category,
(select text from quest_category_master where quest_category_master.id=category) as text,
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
            QuestViewerDataGrid_BindData();
        }

        private void QuestTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            QuestViewerSQL = @"SELECT id,name,stamina,
(select name from quest_category_master where quest_category_master.id=category) as category
FROM QUEST_MASTER
WHERE category<1000
ORDER BY id DESC";
            QuestViewerDataGrid_BindData();
        }

        private void QuestViewerDataGrid_BindData()
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData(QuestViewerSQL);
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                QuestViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
    }
}
