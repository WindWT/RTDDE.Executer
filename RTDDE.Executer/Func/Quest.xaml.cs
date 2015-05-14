using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    public partial class Quest : UserControl
    {
        public Quest()
        {
            InitializeComponent();
        }
        [DAL(UseProperty = true)]
        public class QuestMasterExtend : QuestMaster
        {
            public string parent_area_name { get; set; }
            public string parent_area_text { get; set; }
            public string sp_event_name { get; set; }
            public string open_sp_event_name { get; set; }
            public string present_param_name { get; set; }
        }
        async private void QuestDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string eqInfo_id = ((DataRowView)QuestDataGrid.SelectedItem).Row["id"].ToString();
            QuestInfo_id.Text = eqInfo_id;
            Task<QuestMasterExtend> questTask = Task.Run(() =>
            {
                string sql = @"SELECT *,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name,
(select text from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_text,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.sp_event_id) as sp_event_name,
(SELECT target_name FROM SP_EVENT_MASTER where SP_EVENT_MASTER.sp_event_id=quest_master.open_sp_event_id) as open_sp_event_name,
(case when present_type=4 then (select name from unit_master where unit_master.id=quest_master.present_param) else present_param end) as present_param_name
  FROM quest_master WHERE id={0}";
                return DAL.ToSingle<QuestMasterExtend>(String.Format(sql, eqInfo_id));
            });

            QuestMasterExtend quest = await questTask;
            Task<List<OpenType>> taskOpenType = Task.Run(() =>
            {
                List<OpenType> opentypeList = new List<OpenType>();
                if (quest == null) {
                    return null;
                }
                for (int i = 0; i < 8; i++) {
                    opentypeList.Add(Utility.ParseOpentype(quest.GetOpenType(i), quest.GetOpenParam(i), quest.GetGroupParam(i)));
                }
                opentypeList.Add(new OpenType(quest.open_sp_event_name, quest.open_sp_event_point.ToString(), 0));
                opentypeList.RemoveAll(o => string.IsNullOrEmpty(o.Type));
                return opentypeList;
            });
            Task<MapEventMaster> mapEventTask = Task.Run(() => DAL.ToSingle<MapEventMaster>("SELECT * FROM MAP_EVENT_MASTER WHERE id=" + quest.parent_map_event_id));

            string questDiff = string.Empty;
            for (int i = 0; i < 8; i++) {
                if (i < quest.quest_difficulty) {
                    questDiff += "★";
                }
                else {
                    questDiff += "☆";
                }
            }
            QuestInfo_quest_difficulty.Text = questDiff;
            QuestInfo_name.Text = quest.name;
            QuestInfo_name_sub.Text = quest.name_sub;
            QuestInfo_pt_num.Text = quest.pt_num.ToString();
            QuestInfo_difficulty.Text = quest.difficulty.ToString();
            QuestInfo_stamina.Text = quest.stamina.ToString();
            QuestInfo_distance.Text = quest.distance.ToString();

            QuestInfo_parent_area_id.Text = quest.parent_area_id.ToString();
            QuestInfo_parent_area_name.Text = quest.parent_area_name;
            QuestInfo_parent_area_text.Text = Utility.ParseText(quest.parent_area_text);

            QuestInfo_parent_map_event_id.Text = quest.parent_map_event_id.ToString();
            try {
                MapEventMaster mapEvent = await mapEventTask;
                if (mapEvent != null) {
                    QuestInfo_parent_map_event_name.Text = mapEvent.name;
                }
            }
            catch (Exception ex) {
                Utility.ShowException(ex.Message);
            }
            QuestInfo_display_order.Text = quest.display_order.ToString();
            QuestInfo_sp_guide_id.Text = quest.sp_guide_id.ToString();
            QuestInfo_event_effect_flag.Text = quest.event_effect_flag.ToString();
            QuestInfo_reward_money.Text = quest.reward_money.ToString();
            QuestInfo_reward_exp.Text = quest.reward_exp.ToString();
            QuestInfo_soul.Text = quest.soul.ToString();
            QuestInfo_reward_money_limit.Text = quest.reward_money_limit.ToString();
            QuestInfo_reward_exp_limit.Text = quest.reward_exp_limit.ToString();
            QuestInfo_reward_soul.Text = quest.reward_soul.ToString();
            QuestInfo_kind.Text = Utility.ParseQuestKind(quest.kind);
            QuestInfo_zbtn_kind.Text = Utility.ParseZBTNKind(quest.zbtn_kind);
            QuestInfo_bgm_f.Text = Utility.ParseBgmName(quest.bgm_f);
            QuestInfo_bgm_b.Text = Utility.ParseBgmName(quest.bgm_b);
            if (quest.h_id == 0) {
                QuestHelperGrid.Visibility = Visibility.Collapsed;
            }
            else {
                QuestHelperGrid.Visibility = Visibility.Visible;
                QuestInfo_h_id.Text = quest.h_id.ToString();
                QuestInfo_h_name.Text = Utility.ParseUnitName(quest.h_id);
                QuestInfo_h_lv.Text = quest.h_lv.ToString();
            }

            //if (quest.challenge_id_0==0
            //    && string.IsNullOrWhiteSpace(quest.challenge1)
            //    && string.IsNullOrWhiteSpace(quest.challenge2)) {
            //    QuestInfo_challenge.Visibility = Visibility.Collapsed;
            //}
            //else {
            //    QuestInfo_challenge.Visibility = Visibility.Visible;
            //    QuestInfo_challenge_gold.Document = Utility.ParseTextToDocument(quest.challenge2);
            //    QuestInfo_challenge_silver.Document = Utility.ParseTextToDocument(quest.challenge1);
            //    QuestInfo_challenge_bronze.Document = Utility.ParseTextToDocument(quest.challenge0);
            //}
            QuestInfo_challenge.Children.Clear();
            int[] challengeIds = new[] { quest.challenge_id_2, quest.challenge_id_1, quest.challenge_id_0 };
            foreach (var challengeId in challengeIds) {
                if (challengeId == 0) {
                    continue;
                }
                QuestChallengeMaster challenge = await ChallengeTask(challengeId);
                if (challenge == null) {
                    continue;
                }
                SolidColorBrush gradeBrush = new SolidColorBrush(challenge.grade == 2 ? Colors.Gold : (challenge.grade == 1 ? Colors.Silver : Colors.Brown));
                gradeBrush.Freeze();
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock textBlock = new TextBlock() { Text = challenge.grade == 2 ? "Gold" : (challenge.grade == 1 ? "Silver" : "Brown"), Foreground = gradeBrush };
                textBlock.SetValue(Grid.ColumnProperty, 0);
                grid.Children.Add(textBlock);
                TextBox textBox = new TextBox() { Text = Utility.ParseChallengeType(challenge.type), BorderBrush = gradeBrush };
                textBox.SetValue(Grid.ColumnProperty, 1);
                grid.Children.Add(textBox);
                QuestInfo_challenge.Children.Add(grid);

                RichTextBox richText = new RichTextBox(Utility.ParseTextToDocument(challenge.text)) { BorderBrush = gradeBrush };
                QuestInfo_challenge.Children.Add(richText);

                UniformGrid uniformGrid = new UniformGrid() { Rows = 1, Columns = 4 };
                uniformGrid.Children.Add(new TextBox() { Text = challenge.param_0.ToString(), BorderBrush = gradeBrush });
                uniformGrid.Children.Add(new TextBox() { Text = challenge.param_1.ToString(), BorderBrush = gradeBrush });
                uniformGrid.Children.Add(new TextBox() { Text = challenge.param_2.ToString(), BorderBrush = gradeBrush });
                uniformGrid.Children.Add(new TextBox() { Text = challenge.param_3.ToString(), BorderBrush = gradeBrush });
                QuestInfo_challenge.Children.Add(uniformGrid);
            }

            var openDate = Utility.ParseRTDDate(quest.open_date, true);
            QuestInfo_open_date.Text = openDate == DateTime.MinValue
                ? string.Empty
                : openDate.ToString("yyyy-MM-dd HH:mm ddd");
            var closeDate = Utility.ParseRTDDate(quest.close_date, true);
            QuestInfo_close_date.Text = closeDate == DateTime.MinValue
                ? string.Empty
                : closeDate.ToString("yyyy-MM-dd HH:mm ddd");

            QuestInfo_opentype_content.Children.Clear();
            List<OpenType> opentypes = await taskOpenType;
            if (opentypes.Count == 0) {
                QuestInfo_opentype.Visibility = Visibility.Collapsed;
            }
            else {
                QuestInfo_opentype.Visibility = Visibility.Visible;
                foreach (OpenType type in opentypes) {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(60) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    TextBox typeTextBox = new TextBox() { Text = type.Type };
                    typeTextBox.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(typeTextBox);
                    TextBox paramTextBox = new TextBox() { Text = type.Param };
                    paramTextBox.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(paramTextBox);
                    TextBox groupTextBox = new TextBox() { Text = type.Group.ToString() };
                    groupTextBox.SetValue(Grid.ColumnProperty, 2);
                    grid.Children.Add(groupTextBox);
                    QuestInfo_opentype_content.Children.Add(grid);
                }
            }

            if (quest.sp_event_id == 0) {
                QuestInfo_sp_event.Visibility = Visibility.Collapsed;
            }
            else {
                QuestInfo_sp_event.Visibility = Visibility.Visible;
                QuestInfo_sp_event_id.Text = quest.sp_event_id.ToString();
                QuestInfo_sp_event_name.Text = quest.sp_event_name;
            }

            QuestInfo_bonus.Text = Utility.ParseBonusType(quest.bonus_type);
            var bonusStart = Utility.ParseRTDDate((int)quest.bonus_start);
            QuestInfo_bonus_start.Text = bonusStart == DateTime.MinValue
                ? string.Empty
                : bonusStart.ToString("yyyy-MM-dd HH:mm");
            var bonusEnd = Utility.ParseRTDDate((int)quest.bonus_end);
            QuestInfo_bonus_end.Text = bonusEnd == DateTime.MinValue
                ? string.Empty
                : bonusEnd.ToString("yyyy-MM-dd HH:mm");

            QuestInfo_panel_sword.Text = quest.panel_sword.ToString();
            QuestInfo_panel_lance.Text = quest.panel_lance.ToString();
            QuestInfo_panel_archer.Text = quest.panel_archer.ToString();
            QuestInfo_panel_cane.Text = quest.panel_cane.ToString();
            QuestInfo_panel_heart.Text = quest.panel_heart.ToString();
            QuestInfo_panel_sp.Text = quest.panel_sp.ToString();

            QuestInfo_present_type.Text = Utility.ParsePresentType(quest.present_type);
            QuestInfo_present_param.Text = quest.present_param_name;
            QuestInfo_present_param_1.Text = quest.present_param_1.ToString();

            //advanced
            UnitInfo_event_cutin_id.Text = quest.event_cutin_id.ToString();
            UnitInfo_enemy_table_id.Text = quest.enemy_table_id.ToString();
            UnitInfo_banner.Text = quest.banner;
            UnitInfo_tflg_cmd_0.Text = quest.tflg_cmd_0.ToString();
            UnitInfo_tflg_idx_0.Text = quest.tflg_idx_0.ToString();
            UnitInfo_tflg_cmd_1.Text = quest.tflg_cmd_1.ToString();
            UnitInfo_tflg_idx_1.Text = quest.tflg_idx_1.ToString();
            UnitInfo_text.Text = quest.text;
            UnitInfo_map.Text = quest.map.ToString();
            UnitInfo_division.Text = quest.division.ToString();
            UnitInfo_kpi_class.Text = quest.kpi_class.ToString();
            UnitInfo_flag_no.Text = quest.flag_no.ToString();
        }
        async private Task<QuestChallengeMaster> ChallengeTask(int id)
        {
            return DAL.ToSingle<QuestChallengeMaster>("SELECT * FROM QUEST_CHALLENGE_MASTER WHERE id=" + id);
        }

        private void QuestTypeRadio_Event_Checked(object sender, RoutedEventArgs e)
        {
            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name,
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
            string today = Utility.ToRTDDate(DateTime.Now, false).ToString();
            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name,
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
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name
FROM QUEST_MASTER
WHERE (select parent_field_id from quest_area_master where quest_area_master.id=parent_area_id)>0
ORDER BY id DESC";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestTypeRadio_MapEvent_Checked(object sender, RoutedEventArgs e)
        {
            const string sql = @"SELECT id,name,stamina
FROM QUEST_MASTER
WHERE parent_area_id=0
ORDER BY id DESC";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuestTypeRadio_Event.IsChecked = false;
            QuestTypeRadio_Daily.IsChecked = false;
            QuestTypeRadio_Main.IsChecked = false;

            string sql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name
FROM QUEST_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(QuestSearch_id.Text) == false) {
                sql += "id=" + QuestSearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_name.Text) == false) {
                sql += "name LIKE '%" + QuestSearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_parent_area_id.Text) == false) {
                sql += "parent_area_id=" + QuestSearch_parent_area_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(QuestSearch_parent_area_name.Text) == false) {
                sql += "parent_area_name LIKE '%" + QuestSearch_parent_area_name.Text.Trim() + "%' AND ";
            }
            sql += " 1=1 ORDER BY id DESC";
            Utility.BindData(QuestDataGrid, sql);
        }

        private void QuestSearchClear_Click(object sender, RoutedEventArgs e)
        {
            QuestSearch_id.Text = String.Empty;
            QuestSearch_name.Text = String.Empty;
            QuestSearch_parent_area_id.Text = String.Empty;
            QuestSearch_parent_area_name.Text = String.Empty;
            QuestTypeRadio_Event.IsChecked = true;
        }

        private void SB_ShowMap_Completed(object sender, EventArgs e)
        {
            Map.Load(QuestInfo_id.Text);
        }

        private void QuestInfoHelperToUnitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Utility.GoToItemById("Unit", Convert.ToInt32(QuestInfo_h_id.Text));
        }
    }
}
