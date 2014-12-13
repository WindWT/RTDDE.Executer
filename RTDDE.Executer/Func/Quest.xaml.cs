﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RTDDE.Provider;

namespace RTDDE.Executer.Func
{
    public partial class Quest : UserControl, IRefreshable
    {
        public Quest()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            QuestTypeRadio_Event.IsChecked = false;
            QuestTypeRadio_Event.IsChecked = true;
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
                string sql = @"SELECT *,
(select name from quest_area_master where quest_area_master.id=category) as category_name,
(select text from quest_area_master where quest_area_master.id=category) as category_text,
(select banner_texture from quest_area_master where quest_area_master.id=category) as category_banner,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.sp_event_id) as sp_event_name,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.open_sp_event_id) as open_sp_event_name,open_sp_event_point,
(case when present_type=4 then (select name from unit_master where unit_master.id=quest_master.present_param) else present_param end) as present_param_name ,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_0) as challenge0,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_1) as challenge1,
(SELECT text from QUEST_CHALLENGE_MASTER where id=challenge_id_2) as challenge2
  FROM quest_master WHERE id={0}";
                return DAL.GetDataTable(String.Format(sql, eqInfo_id));
            });
            Task<List<OpenType>> taskParse = new Task<List<OpenType>>(() =>
            {
                List<OpenType> opentypeList = new List<OpenType>();
                Task.WaitAll(task);
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return null;
                }
                DataRow dr = task.Result.Rows[0];
                for (int i = 1; i <= 8; i++)
                {
                    string opentype = "open_type_" + i.ToString();
                    string openparam = "open_param_" + i.ToString();
                    string opengroup = "open_group_" + i.ToString();
                    opentypeList.Add(Utility.ParseOpentype(dr[opentype].ToString(), dr[openparam].ToString(), Convert.ToInt32(dr[opengroup])));
                }
                opentypeList.Add(new OpenType(dr["open_sp_event_name"].ToString(), dr["open_sp_event_point"].ToString(), 0));
                opentypeList.RemoveAll(o => string.IsNullOrEmpty(o.Type));
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
                QuestInfo_category_text.Text = Utility.ParseText(dr["category_text"].ToString());
                QuestInfo_category_banner.Text = Utility.ParseText(dr["category_banner"].ToString());
                QuestInfo_reward_money.Text = dr["reward_money"].ToString();
                QuestInfo_reward_exp.Text = dr["reward_exp"].ToString();
                QuestInfo_soul.Text = dr["soul"].ToString();
                QuestInfo_reward_money_limit.Text = dr["reward_money_limit"].ToString();
                QuestInfo_reward_exp_limit.Text = dr["reward_exp_limit"].ToString();
                //QuestInfo_bgm_f.Text = Utility.ParseBgmFileName(Convert.ToInt32(dr["bgm_f"]));
                //QuestInfo_bgm_b.Text = Utility.ParseBgmFileName(Convert.ToInt32(dr["bgm_b"]));
                QuestInfo_bgm_f.Text = dr["bgm_f"].ToString();
                QuestInfo_bgm_b.Text = dr["bgm_b"].ToString();
                QuestInfo_sp_guide_id.Text = dr["sp_guide_id"].ToString();
                QuestInfo_event_cutin_id.Text = dr["event_cutin_id"].ToString();

                QuestInfo_banner.Text = Utility.ParseText(dr["banner"].ToString());

                QuestInfo_h_id.Text = dr["h_id"].ToString();
                QuestInfo_h_name.Text = Utility.ParseUnitName(dr["h_id"].ToString());
                QuestInfo_h_lv.Text = dr["h_lv"].ToString();

                QuestInfo_open_date.Text = Utility.ParseRTDDate(dr["open_date"].ToString(), true);
                QuestInfo_close_date.Text = Utility.ParseRTDDate(dr["close_date"].ToString(), true);

                QuestInfo_opentype_content.Children.Clear();
                List<OpenType> opentypes = t.Result;
                if (opentypes.Count == 0)
                {
                    QuestInfo_opentype.Visibility = Visibility.Collapsed;
                }
                else
                {
                    QuestInfo_opentype.Visibility = Visibility.Visible;
                    foreach (OpenType type in t.Result)
                    {
                        QuestInfo_opentype_content.Children.Add(new TextBox()
                        {
                            Text = type.Type,
                            Width = 60
                        });
                        QuestInfo_opentype_content.Children.Add(new TextBox()
                        {
                            Text = type.Param,
                            Width = 200
                        });
                        QuestInfo_opentype_content.Children.Add(new TextBox()
                        {
                            Text = type.Group.ToString(),
                            Width = 40
                        });
                    }
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

                QuestInfo_bonus.Text = Utility.ParseBonustype(dr["bonus_type"].ToString());
                QuestInfo_bonus_start.Text = Utility.ParseRTDDate(dr["bonus_start"].ToString());
                QuestInfo_bonus_end.Text = Utility.ParseRTDDate(dr["bonus_end"].ToString());

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
                    QuestInfo_challenge_gold.Document = Utility.ParseTextToDocument(dr["challenge2"].ToString());
                    QuestInfo_challenge_silver.Document = Utility.ParseTextToDocument(dr["challenge1"].ToString());
                    QuestInfo_challenge_bronze.Document = Utility.ParseTextToDocument(dr["challenge0"].ToString());
                }

                QuestInfo_present_type.Text = Utility.ParsePresenttype(dr["present_type"].ToString());
                QuestInfo_present_param.Text = dr["present_param_name"].ToString();
                QuestInfo_present_param_1.Text = dr["present_param_1"].ToString();
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskParse.Start();
        }

        private void QuestTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=category) as category,
       ( CASE
                WHEN open_date<>0 THEN open_date
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                WHEN open_type_7 = 4 THEN open_param_7
                WHEN open_type_8 = 4 THEN open_param_8
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN close_date<>0 THEN close_date
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                WHEN open_type_7 = 5 THEN open_param_7
                WHEN open_type_8 = 5 THEN open_param_8
                ELSE 0 
       END ) AS [end]
  FROM quest_master
 ORDER BY start DESC,end DESC,id DESC;";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestTypeRadio_Daily_Checked(object sender, RoutedEventArgs e)
        {
            string today = DateTime.Today.AddHours(1).ToString("yyyyMMddHH");
            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=category) as category,
       ( CASE
                WHEN open_type_1 = 1 THEN open_param_1 
                WHEN open_type_2 = 1 THEN open_param_2 
                WHEN open_type_3 = 1 THEN open_param_3 
                WHEN open_type_4 = 1 THEN open_param_4 
                WHEN open_type_5 = 1 THEN open_param_5 
                WHEN open_type_6 = 1 THEN open_param_6 
                WHEN open_type_7 = 1 THEN open_param_7
                WHEN open_type_8 = 1 THEN open_param_8
                ELSE -1
       END ) AS DayOfWeek,
       ( CASE
                WHEN open_date<>0 THEN open_date
                WHEN open_type_1 = 4 THEN open_param_1 
                WHEN open_type_2 = 4 THEN open_param_2 
                WHEN open_type_3 = 4 THEN open_param_3 
                WHEN open_type_4 = 4 THEN open_param_4 
                WHEN open_type_5 = 4 THEN open_param_5 
                WHEN open_type_6 = 4 THEN open_param_6 
                WHEN open_type_7 = 4 THEN open_param_7
                WHEN open_type_8 = 4 THEN open_param_8
                ELSE 0 
       END ) AS start,
       ( CASE
                WHEN close_date<>0 THEN close_date
                WHEN open_type_1 = 5 THEN open_param_1 
                WHEN open_type_2 = 5 THEN open_param_2 
                WHEN open_type_3 = 5 THEN open_param_3 
                WHEN open_type_4 = 5 THEN open_param_4 
                WHEN open_type_5 = 5 THEN open_param_5 
                WHEN open_type_6 = 5 THEN open_param_6 
                WHEN open_type_7 = 5 THEN open_param_7
                WHEN open_type_8 = 5 THEN open_param_8
                ELSE 0 
       END ) AS [end],
       ( CASE
                WHEN open_type_1 = 6 THEN open_param_1 
                WHEN open_type_2 = 6 THEN open_param_2 
                WHEN open_type_3 = 6 THEN open_param_3 
                WHEN open_type_4 = 6 THEN open_param_4 
                WHEN open_type_5 = 6 THEN open_param_5 
                WHEN open_type_6 = 6 THEN open_param_6 
                WHEN open_type_7 = 6 THEN open_param_7
                WHEN open_type_8 = 6 THEN open_param_8
                ELSE 0 
       END ) AS isDisabled
FROM QUEST_MASTER
WHERE DayOfWeek>=0
AND isDisabled=0
AND ([end]>" + today + @" OR [end]=0)
ORDER BY DayOfWeek,id DESC";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestTypeRadio_Main_Checked(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT id,name,stamina,
(select parent_field_id from quest_area_master where quest_area_master.id=category) as hasParent
FROM QUEST_MASTER
WHERE hasParent>0
ORDER BY id DESC";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuestTypeRadio_Event.IsChecked = false;
            QuestTypeRadio_Daily.IsChecked = false;
            QuestTypeRadio_Main.IsChecked = false;

            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=category) as category_name
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
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestSearchClear_Click(object sender, RoutedEventArgs e)
        {
            QuestSearch_id.Text = String.Empty;
            QuestSearch_name.Text = String.Empty;
            QuestSearch_category.Text = String.Empty;
            QuestSearch_category_name.Text = String.Empty;
            QuestTypeRadio_Event.IsChecked = true;
        }

        private void SB_ShowMap_Completed(object sender, EventArgs e)
        {
            Map.Load(QuestInfo_id.Text);
        }
    }
}
