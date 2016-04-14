using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using RTDDE.Executer.Util;
using RTDDE.Provider;
using RTDDE.Provider.MasterData;
using RTDDE.Provider.Util;

namespace RTDDE.Executer.Func
{
    public partial class Quest : UserControl, IRedirectable
    {
        public Quest()
            : this(false)
        {
        }
        public Quest(bool disableAutoLoad)
        {
            InitializeComponent();
            if (disableAutoLoad == false) {
                QuestTypeRadio_Event.IsChecked = true;
                QuestTypeSwitch(QuestType.Event);
            }
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
            Task<List<QuestOpenType>> taskOpenType = Task.Run(() =>
            {
                List<QuestOpenType> opentypeList = new List<QuestOpenType>();
                if (quest == null) {
                    return null;
                }
                for (int i = 0; i < 8; i++) {
                    opentypeList.Add(Utility.GetQuestOpenType(quest.GetOpenType(i), quest.GetOpenParam(i), quest.GetGroupParam(i)));
                }
                opentypeList.Add(new QuestOpenType(quest.open_sp_event_name, quest.open_sp_event_point.ToString(), 0));
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
            this.DataContext = quest;
            QuestInfo_quest_difficulty.Text = questDiff;
            QuestInfo_name.Text = quest.name;
            QuestInfo_name_sub.Text = quest.name_sub;
            QuestInfo_pt_num.Text = quest.pt_num.ToString();
            QuestInfo_difficulty.Text = quest.difficulty.ToString();
            QuestInfo_stamina.Text = quest.stamina.ToString();
            QuestInfo_distance.Text = quest.distance.ToString();

            QuestInfo_parent_area_id.Text = quest.parent_area_id.ToString();
            QuestInfo_parent_area_name.Text = quest.parent_area_name;
            QuestInfo_parent_area_text.Document = Utility.ParseTextToDocument(quest.parent_area_text);

            QuestInfo_parent_map_event_id.Text = quest.parent_map_event_id.ToString();
            try {
                MapEventMaster mapEvent = await mapEventTask;
                if (mapEvent != null) {
                    QuestInfo_parent_map_event_name.Text = mapEvent.name;
                }
            }
            catch (Exception ex) {
                Utility.ShowException(ex);
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
            //Challenge
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
            //Multi
            QuestMultiConditionStackPanel.Children.Clear();
            QuestMultiRewardStackPanel.Children.Clear();
            if (quest.multi_quest_id != 0) {
                MultiQuestMaster mqm = await MultiQuestTask((int) quest.multi_quest_id);
                QuestInfo_multi_footprint_exp.Text = mqm.footprint_exp.ToString();
                QuestInfo_multi_host_ticket.Text = mqm.host_ticket.ToString();
                QuestInfo_multi_guest_ticket.Text = mqm.guest_ticket.ToString();
                //Multi Contribution
                foreach (uint contributionId in mqm.GetMedals()) {
                    if (contributionId == 0) {
                        continue;
                    }
                    MultiContributionMaster mcm = await MultiContributionTask((int) contributionId);
                    if (mcm == null) {
                        continue;
                    }
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                    TextBox textBoxId = new TextBox() { Text = mcm.id.ToString() };
                    textBoxId.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(textBoxId);

                    TextBox textBoxType = new TextBox() { Text = Utility.ParseMultiConditionType(mcm.type) };
                    textBoxType.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(textBoxType);

                    string pointText = string.Empty;
                    if (mcm.point < 5) {
                        for (int i = 0; i < mcm.point; i++) {
                            pointText += "●";
                        }
                    }
                    else {
                        pointText = $"●×{mcm.point}";
                    }

                    TextBox textBoxPoint = new TextBox() { Text = pointText };
                    textBoxPoint.SetValue(Grid.ColumnProperty, 0);
                    textBoxPoint.SetValue(Grid.RowProperty, 1);
                    grid.Children.Add(textBoxPoint);

                    TextBox textBoxText = new TextBox() { Text = mcm.text };
                    textBoxText.SetValue(Grid.ColumnProperty, 1);
                    textBoxText.SetValue(Grid.RowProperty, 1);
                    grid.Children.Add(textBoxText);

                    TextBox textBoxCutin = new TextBox() { Text = mcm.cutin_text };
                    textBoxCutin.SetValue(Grid.ColumnProperty, 1);
                    textBoxCutin.SetValue(Grid.RowProperty, 2);
                    grid.Children.Add(textBoxCutin);

                    TextBox textBoxParam0 = new TextBox() { Text = mcm.param_0.ToString() };
                    textBoxParam0.SetValue(Grid.ColumnProperty, 2);
                    textBoxParam0.SetValue(Grid.RowProperty, 0);
                    grid.Children.Add(textBoxParam0);

                    TextBox textBoxParam1 = new TextBox() { Text = mcm.param_1.ToString() };
                    textBoxParam1.SetValue(Grid.ColumnProperty, 2);
                    textBoxParam1.SetValue(Grid.RowProperty, 1);
                    grid.Children.Add(textBoxParam1);

                    TextBox textBoxParam2 = new TextBox() { Text = mcm.param_2.ToString() };
                    textBoxParam2.SetValue(Grid.ColumnProperty, 2);
                    textBoxParam2.SetValue(Grid.RowProperty, 2);
                    grid.Children.Add(textBoxParam2);

                    QuestMultiConditionStackPanel.Children.Add(grid);
                }
                //Multi Reward
                MultiRewardMaster mrm = await MultiRewardTask((int)mqm.reward_id);
                for (int i = 0; i < 11; i++) {
                    if (mrm.GetUnitID(i) == 0) {
                        continue;
                    }
                    Grid grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1,GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    TextBlock textBlockIndex = new TextBlock() { Text = i == 10 ? "R" : i.ToString() };
                    textBlockIndex.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(textBlockIndex);

                    TextBox textBoxId = new TextBox() { Text = mrm.GetUnitID(i).ToString() };
                    textBoxId.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(textBoxId);

                    TextBlock textBlockLv = new TextBlock() { Text = "lv" };
                    textBlockLv.SetValue(Grid.ColumnProperty, 2);
                    grid.Children.Add(textBlockLv);

                    TextBox textBoxLv = new TextBox() { Text =mrm.GetUnitLv(i).ToString() };
                    textBoxLv.SetValue(Grid.ColumnProperty, 3);
                    grid.Children.Add(textBoxLv);

                    int rarity = mrm.GetUnitRarity(i);
                    TextBox textBoxRarity = new TextBox() {
                        Text = rarity == 2 ? "SR" : rarity == 1 ? "R" : rarity == 0 ? "N" : "?"
                    };
                    textBoxRarity.SetValue(Grid.ColumnProperty, 4);
                    grid.Children.Add(textBoxRarity);

                    TextBox textBoxName = new TextBox() { Text = Utility.ParseUnitName((int) mrm.GetUnitID(i)) };
                    textBoxName.SetValue(Grid.ColumnProperty, 5);
                    grid.Children.Add(textBoxName);

                    Button button = new Button()
                    {
                        Content = "→",
                        Style = FindResource("InlineButton") as Style
                    };
                    var index = i;
                    button.Click += (s, arg) => {
                        Utility.GoToItemById<Unit>((int)mrm.GetUnitID(index));
                    };
                    button.SetValue(Grid.ColumnProperty, 6);
                    grid.Children.Add(button);

                    QuestMultiRewardStackPanel.Children.Add(grid);
                }
            }
            //Open&Close
            var openDate = Utility.ParseRTDDate(quest.open_date, true);
            QuestInfo_open_date.Text = openDate == DateTime.MinValue
                ? string.Empty
                : openDate.ToString("yyyy-MM-dd HH:mm ddd");
            var closeDate = Utility.ParseRTDDate(quest.close_date, true);
            QuestInfo_close_date.Text = closeDate == DateTime.MinValue
                ? string.Empty
                : closeDate.ToString("yyyy-MM-dd HH:mm ddd");

            QuestInfo_opentype_content.Children.Clear();
            List<QuestOpenType> opentypes = await taskOpenType;
            if (opentypes.Count == 0) {
                QuestInfo_opentype.Visibility = Visibility.Collapsed;
            }
            else {
                QuestInfo_opentype.Visibility = Visibility.Visible;
                foreach (QuestOpenType type in opentypes) {
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
            //Sp event
            if (quest.sp_event_id == 0) {
                QuestInfo_sp_event.Visibility = Visibility.Collapsed;
            }
            else {
                QuestInfo_sp_event.Visibility = Visibility.Visible;
                QuestInfo_sp_event_id.Text = quest.sp_event_id.ToString();
                QuestInfo_sp_event_name.Text = quest.sp_event_name;
            }
            //bonus
            QuestInfo_bonus.Text = Utility.ParseBonusType(quest.bonus_type);
            var bonusStart = Utility.ParseRTDDate((int)quest.bonus_start);
            QuestInfo_bonus_start.Text = bonusStart == DateTime.MinValue
                ? string.Empty
                : bonusStart.ToString("yyyy-MM-dd HH:mm");
            var bonusEnd = Utility.ParseRTDDate((int)quest.bonus_end);
            QuestInfo_bonus_end.Text = bonusEnd == DateTime.MinValue
                ? string.Empty
                : bonusEnd.ToString("yyyy-MM-dd HH:mm");
            //panel rate
            QuestInfo_panel_sword.Text = quest.panel_sword.ToString();
            QuestInfo_panel_lance.Text = quest.panel_lance.ToString();
            QuestInfo_panel_archer.Text = quest.panel_archer.ToString();
            QuestInfo_panel_cane.Text = quest.panel_cane.ToString();
            QuestInfo_panel_heart.Text = quest.panel_heart.ToString();
            QuestInfo_panel_sp.Text = quest.panel_sp.ToString();
            //present
            string presentType = Utility.ParsePresentType(quest.present_type);
            if (string.Compare(presentType, "UNIT", StringComparison.OrdinalIgnoreCase) == 0) {
                QuestInfoPresentToUnitButton.Visibility = Visibility.Visible;
            }
            else {
                QuestInfoPresentToUnitButton.Visibility = Visibility.Collapsed;
            }
            QuestInfo_present_type.Text = Utility.ParsePresentType(quest.present_type);
            QuestInfo_present_param_name.Text = quest.present_param_name;
            QuestInfo_present_param.Text = quest.present_param.ToString();
            QuestInfo_present_param_1.Text = quest.present_param_1.ToString();
        }
        async private Task<QuestChallengeMaster> ChallengeTask(int id)
        {
           return DAL.ToSingle<QuestChallengeMaster>("SELECT * FROM QUEST_CHALLENGE_MASTER WHERE id=" + id);
        }
        async private Task<MultiQuestMaster> MultiQuestTask(int id) {
            return DAL.ToSingle<MultiQuestMaster>("SELECT * FROM MULTI_QUEST_MASTER WHERE id=" + id);
        }
        async private Task<MultiContributionMaster> MultiContributionTask(int id) {
            return DAL.ToSingle<MultiContributionMaster>("SELECT * FROM MULTI_CONTRIBUTION_MASTER WHERE id=" + id);
        }
        async private Task<MultiRewardMaster> MultiRewardTask(int id) {
            return DAL.ToSingle<MultiRewardMaster>("SELECT * FROM MULTI_REWARD_MASTER WHERE id=" + id);
        }

        private enum QuestType
        {
            Event,
            Main,
            Daily,
            MapEvent,
            Multi
        }
        #region DataGridSql
        private const string EventSql = @"SELECT id,name,stamina,
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
        private const string DailySql = @"SELECT id,name,stamina,
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
AND ([end]>{0} OR [end]=0)
ORDER BY DayOfWeek,id DESC";
        private const string MainSql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name
FROM QUEST_MASTER
WHERE (select parent_field_id from quest_area_master where quest_area_master.id=parent_area_id)>0 AND multi_quest_id=0
ORDER BY id DESC";
        private const string MapEventSql = @"SELECT id,name,stamina
FROM QUEST_MASTER
WHERE parent_area_id=0
ORDER BY id DESC";
        private const string MultiSql = @"SELECT id,name,stamina,
(select name from quest_area_master where quest_area_master.id=parent_area_id) as parent_area_name
FROM QUEST_MASTER
WHERE multi_quest_id<>0
ORDER BY id DESC";
        private const string GetQuestTypeSql = @"SELECT CASE 
WHEN {0} IN (SELECT id FROM QUEST_MASTER WHERE (select parent_field_id from quest_area_master where quest_area_master.id=parent_area_id)>0 AND multi_quest_id=0) 
THEN 1 
WHEN {0} IN (SELECT ID FROM (SELECT id,name,stamina,
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
FROM QUEST_MASTER WHERE DayOfWeek>=0 AND isDisabled=0 AND ([end]>{1} OR [end]=0)))
THEN 2
WHEN {0} IN (SELECT id FROM QUEST_MASTER WHERE parent_area_id=0)
THEN 3
WHEN {0} IN (SELECT id FROM QUEST_MASTER WHERE multi_quest_id<>0)
THEN 4
ELSE 0
END";
        #endregion
        private void QuestTypeSwitch(QuestType type)
        {
            switch (type) {
                case QuestType.Event: Utility.BindData(QuestDataGrid, EventSql); break;
                case QuestType.Daily: Utility.BindData(QuestDataGrid, string.Format(DailySql, Utility.ToRTDDate(DateTime.Now, false).ToString())); break;
                case QuestType.Main: Utility.BindData(QuestDataGrid, MainSql); break;
                case QuestType.MapEvent: Utility.BindData(QuestDataGrid, MapEventSql); break;
                case QuestType.Multi: Utility.BindData(QuestDataGrid, MultiSql); break;
                default: break;
            }
        }
        private void QuestTypeRadio_Event_OnClick(object sender, RoutedEventArgs e)
        {
            QuestTypeSwitch(QuestType.Event);
        }
        private void QuestTypeRadio_Daily_OnClick(object sender, RoutedEventArgs e)
        {
            QuestTypeSwitch(QuestType.Daily);
        }
        private void QuestTypeRadio_Main_OnClick(object sender, RoutedEventArgs e)
        {
            QuestTypeSwitch(QuestType.Main);
        }
        private void QuestTypeRadio_MapEvent_OnClick(object sender, RoutedEventArgs e)
        {
            QuestTypeSwitch(QuestType.MapEvent);
        }

        private void QuestTypeRadio_Multi_OnClick(object sender, RoutedEventArgs e) {
            QuestTypeSwitch(QuestType.Multi);
        }

        private void QuestSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            QuestTypeRadio_Event.IsChecked = false;
            QuestTypeRadio_Daily.IsChecked = false;
            QuestTypeRadio_Main.IsChecked = false;
            QuestTypeRadio_MapEvent.IsChecked = false;
            QuestTypeRadio_Multi.IsChecked = false;

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
            QuestTypeSwitch(QuestType.Event);
        }

        private void SB_ShowMap_Completed(object sender, EventArgs e) {
            MapGrid.Width = UnitGrid.ActualWidth;
            Map.Load(QuestInfo_id.Text);
        }

        private void QuestInfoHelperToUnitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Utility.GoToItemById<Unit>(Convert.ToInt32(QuestInfo_h_id.Text));
        }
        private void QuestInfoPresentToUnitButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QuestInfo_present_param.Text)) {
                return;
            }
            int id = Convert.ToInt32(QuestInfo_present_param.Text);
            if (id == 0) {
                return;
            }
            Utility.GoToItemById<Unit>(id);
        }
        private void QuestToQuestAreaButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QuestInfo_parent_area_id.Text)) {
                return;
            }
            int id = Convert.ToInt32(QuestInfo_parent_area_id.Text);
            if (id == 0) {
                return;
            }
            Utility.GoToItemById<QuestArea>(id);
        }
        public DataGrid GetTargetDataGrid(int firstId, int lastId = -1, string type = null)
        {
            Task<QuestType> task = Task.Run(() => (QuestType)DAL.Get<int>(string.Format(GetQuestTypeSql, firstId, Utility.ToRTDDate(DateTime.Now, false).ToString())));
            switch (task.Result) {
                case QuestType.Event: QuestTypeRadio_Event.IsChecked = true; break;
                case QuestType.Main: QuestTypeRadio_Main.IsChecked = true; break;
                case QuestType.Daily: QuestTypeRadio_Daily.IsChecked = true; break;
                case QuestType.MapEvent: QuestTypeRadio_MapEvent.IsChecked = true; break;
                case QuestType.Multi: QuestTypeRadio_Multi.IsChecked = true; break;
                default: break;
            }
            QuestTypeSwitch(task.Result);
            return QuestDataGrid;
        }

        private void QuestToMapEventButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(QuestInfo_parent_map_event_id.Text)) {
                return;
            }
            int id = Convert.ToInt32(QuestInfo_parent_map_event_id.Text);
            if (id == 0) {
                return;
            }
            Utility.GoToItemById<Event>(id);
        }

        private void UnitGrid_OnSizeChanged(object sender, SizeChangedEventArgs e) {
            if (MapGrid.Width > 0) {
                MapGrid.Width = UnitGrid.ActualWidth;
            }
        }
    }
}
