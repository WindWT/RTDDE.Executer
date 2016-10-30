using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using RTDDE.Provider;
using RTDDE.Provider.Enums;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer.Func
{
    /// <summary>
    /// Enemy.xaml 的交互逻辑
    /// </summary>
    public partial class Enemy : UserControl
    {
        public Enemy()
        {
            InitializeComponent();
        }
        private void EnemyTab_Initialized(object sender, EventArgs e)
        {
            Utility.DisableBindData = true;
            EnemySearch_chara_symbol.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"★","10"},
                {"★★","11"},
                {"★★★","12"},
                {"★★★★","13"},
                {"★★★★★","14"},
                {"★×6","15"}
            };
            var attrDict = new Dictionary<string, string>()
            {
                {"------",""},
                {"NONE","1"},
                {"FIRE","2"},
                {"WATER","3"},
                {"LIGHT","4"},
                {"DARK","5"}
            };
            EnemySearch_attribute.ItemsSource = attrDict;
            var typeDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (ENEMY_TYPE type in Enum.GetValues(typeof(ENEMY_TYPE))) {
                string id = (Convert.ToInt32(type)).ToString();
                typeDict.Add(string.Format("{0}_{1}", id, type.ToString()), id);
            }
            EnemySearch_type.ItemsSource = typeDict;
            var kindDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AssignID kind in Enum.GetValues(typeof(AssignID))) {
                if (kind.ToString().StartsWith("MS0") == false) {
                    kindDict.Add(kind.ToString(), Utility.ParseAssignID(kind).ToString());
                }
            }
            EnemySearch_chara_kind.ItemsSource = kindDict;
            var patternDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AttackPattern pattern in Enum.GetValues(typeof(AttackPattern))) {
                string id = (Convert.ToInt32(pattern)).ToString();
                patternDict.Add(pattern.ToString(), id);
            }
            EnemySearch_pattern.ItemsSource = patternDict;
            Utility.DisableBindData = false;
            Utility.BindData(EnemyDataGrid, "SELECT id,attribute,name FROM Enemy_Unit_MASTER order by id");
        }

        private void EnemyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (EnemyDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string Enemyid = ((DataRowView) EnemyDataGrid.SelectedItem).Row["id"].ToString();
            EnemyInfo_id.Text = Enemyid;
            EnemyInfo_lv.Text = "1";
            EnemyInfo_BindData(Enemyid);
        }

        private void EnemyInfo_lv_LostFocus(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text)) {
                EnemyInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(EnemyInfo_lv.Text).Success) {
                EnemyInfo_lv.Text = "1";
                return;
            }
            if (string.IsNullOrWhiteSpace(EnemyInfo_id.Text)) {
                return;
            }
            EnemyInfo_BindData(EnemyInfo_id.Text);
        }

        private async void EnemyInfo_BindData(string Enemyid) {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text)) {
                return;
            }
            int thislevel = Convert.ToInt32(EnemyInfo_lv.Text);

            Task<EnemyUnitMaster> taskEnemy = Task.Run(() => {
                string sql = @"Select * from Enemy_unit_master WHERE id={0}";
                return DAL.ToSingle<EnemyUnitMaster>(String.Format(sql, Enemyid));
            });
            EnemyUnitMaster eum = await taskEnemy;

            Task<LogicGroupData> taskLogic = Task.Run(() => {
                string sql = @"Select * from LOGIC_GROUP_DATA WHERE logic_group_id={0}";
                try {
                    return DAL.ToSingle<LogicGroupData>(String.Format(sql, eum.logic_group_id));
                }
                catch (Exception) {
                    return null;
                }
            });
            
            EnemyInfo_id.Text = eum.id.ToString();
            EnemyInfo_chara_flag_no.Text = eum.chara_flag_no == 0 ? string.Empty : eum.chara_flag_no.ToString();
            int rare = eum.chara_symbol - 9; //chara_symbol start from 10, equal to rare 1
            string rareText = string.Empty;
            if (rare >= 1 && rare <= 6) {
                for (int i = 0; i < rare; i++) {
                    rareText += "★";
                }
            }
            EnemyInfo_chara_symbol.Text = rareText;
            EnemyInfo_name.Text = eum.name;
            EnemyInfo_attribute.Fill = Utility.ParseAttributeToBrush(Utility.ParseAttribute(eum.attribute));
            EnemyInfo_type.Text = Utility.ParseEnemyType(eum.type);
            EnemyInfo_turn_wait_sec.Text = eum.turn_wait_sec.ToString();
            EnemyInfoTurnWaitSecPath.Stroke = eum.turn_wait_sec > 0
                ? Brushes.Black
                : (Brush) this.FindResource("HighlightBrush");
            EnemyInfo_chara_kind.Text = eum.chara_flag_no == 0
                ? string.Empty
                : Utility.ParseUnitKind(eum.chara_kind).ToString();

            EnemyInfo_lv.Text = thislevel.ToString("0");
            EnemyInfo_HP.Text = Utility.RealCalc(eum.life, eum.up_life, thislevel).ToString();
            EnemyInfo_ATK.Text = Utility.RealCalc(eum.attack, eum.up_attack, thislevel).ToString();
            EnemyInfo_DEF.Text = Utility.RealCalc(eum.defense, eum.up_defense, thislevel).ToString();
            EnemyInfo_soul_pt.Text = eum.soul_pt.ToString();
            EnemyInfo_gold_pt.Text = eum.gold_pt.ToString();
            EnemyInfo_flag.Foreground = Convert.ToBoolean(eum.flag)
                ? Brushes.Black
                : (Brush) this.FindResource("HighlightBrush");
            EnemyInfo_isUnit.Foreground = Utility.IsUnitEnemy(eum.type)
                ? Brushes.Black
                : (Brush) this.FindResource("HighlightBrush");
            EnemyInfo_turn.Text = eum.turn.ToString();
            EnemyInfo_ui.Text = eum.ui.ToString();
            //skill
            EnemyInfo_pat.Text = Utility.ParseAttackPattern(eum.pat);
            EnemyInfo_p0.Text = eum.p0.ToString();
            EnemyInfo_p1.Text = eum.p1.ToString();
            EnemyInfo_pat_01.Text = Utility.ParseAttackPattern(eum.pat_01);
            EnemyInfo_p0_01.Text = eum.p0_01.ToString();
            EnemyInfo_p1_01.Text = eum.p1_01.ToString();
            //Logic
            EnemyLogicStackPanel.Children.Clear();
            if (eum.logic_group_id == 0) {
                EnemyLogicExpander.Visibility = Visibility.Collapsed;
                EnemyLogicExpander.Header = "Logic";
            }
            else {
                EnemyLogicExpander.Visibility=Visibility.Visible;
                EnemyLogicExpander.Header = $"Logic#{eum.logic_group_id}";
                LogicGroupData lgd = await taskLogic;
                if (lgd == null) {
                    EnemyLogicExpander.Header = $"Logic#{eum.logic_group_id}(UNKNOWN)";
                }
                else {
                    foreach (LogicData logicData in lgd.logic_data) {
                        Grid logicGrid=new Grid();
                        logicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                        logicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                        logicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                        logicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
                        logicGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(1, GridUnitType.Star)});
                        
                        var idTextbox = new TextBox() { Text = logicData.logic_id.ToString() };
                        idTextbox.SetValue(Grid.ColumnProperty, 0);
                        idTextbox.SetValue(Grid.RowProperty, 0);
                        logicGrid.Children.Add(idTextbox);

                        var typeTextbox = new TextBox() {
                            Text = Utility.ParseTriggerType(logicData.trigger_type),
                            ToolTip = logicData.cutin_type
                        };
                        typeTextbox.SetValue(Grid.ColumnProperty, 1);
                        typeTextbox.SetValue(Grid.ColumnSpanProperty, 2);
                        typeTextbox.SetValue(Grid.RowProperty, 0);
                        logicGrid.Children.Add(typeTextbox);

                        var paramTextbox = new TextBox() { Text = logicData.trigger_param.ToString() };
                        paramTextbox.SetValue(Grid.ColumnProperty, 3);
                        paramTextbox.SetValue(Grid.RowProperty, 0);
                        logicGrid.Children.Add(paramTextbox);

                        int actionCount = 1; //use for rowProperty
                        foreach (ActionData actionData in logicData.action_data) {
                            logicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

                            var actionTypeTextbox = new TextBox() { Text = Utility.ParseAttackPattern(actionData.action_type) };
                            actionTypeTextbox.SetValue(Grid.ColumnProperty, 1);
                            actionTypeTextbox.SetValue(Grid.RowProperty, actionCount);
                            logicGrid.Children.Add(actionTypeTextbox);

                            var param0Textbox = new TextBox() { Text = actionData.action_param_0.ToString() };
                            param0Textbox.SetValue(Grid.ColumnProperty, 2);
                            param0Textbox.SetValue(Grid.RowProperty, actionCount);
                            logicGrid.Children.Add(param0Textbox);

                            var param1Textbox = new TextBox() { Text = actionData.action_param_1.ToString() };
                            param1Textbox.SetValue(Grid.ColumnProperty, 3);
                            param1Textbox.SetValue(Grid.RowProperty, actionCount);
                            logicGrid.Children.Add(param1Textbox);

                            actionCount++;
                        }
                        EnemyLogicStackPanel.Children.Add(logicGrid);
                    }
                }
            }
            //Advanced
            EnemyInfo_model.Text = eum.model;
            EnemyInfo_texture.Text = eum.texture;
            EnemyInfo_icon.Text = eum.icon.ToString();
            EnemyInfo_shadow.Text = eum.shadow.ToString();
            EnemyInfo_up.Text = eum.up.ToString();
            EnemyInfo_atk_ef_id.Text = eum.atk_ef_id.ToString();
            EnemyInfo_survive.Text = eum.survive.ToString();
        }

        private void EnemySearchClear_Click(object sender, RoutedEventArgs e)
        {
            EnemySearch_id.Text = string.Empty;
            EnemySearch_name.Text = string.Empty;
            EnemySearch_chara_symbol.SelectedIndex = 0;
            EnemySearch_chara_kind.SelectedIndex = 0;
            EnemySearch_type.SelectedIndex = 0;
            EnemySearch_attribute.SelectedIndex = 0;
            EnemySearch_pattern.SelectedIndex = 0;
            Utility.BindData(EnemyDataGrid, "SELECT id,attribute,name FROM Enemy_Unit_MASTER order by id");
        }
        private void EnemySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(EnemyDataGrid, EnemySearch_BuildSQL());
        }
        private void EnemySearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(EnemyDataGrid, EnemySearch_BuildSQL());
        }
        private string EnemySearch_BuildSQL()
        {
            string sql = @"SELECT id,attribute,name FROM Enemy_unit_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(EnemySearch_id.Text) == false) {
                sql += "id=" + EnemySearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(EnemySearch_name.Text) == false) {
                sql += "name LIKE '%" + EnemySearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_chara_symbol.SelectedValue) == false) {
                sql += "chara_symbol=" + EnemySearch_chara_symbol.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_chara_kind.SelectedValue) == false) {
                sql += "chara_kind=" + EnemySearch_chara_kind.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_type.SelectedValue) == false) {
                sql += "type=" + EnemySearch_type.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_attribute.SelectedValue) == false) {
                sql += "attribute=" + EnemySearch_attribute.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)EnemySearch_pattern.SelectedValue) == false) {
                string pat = EnemySearch_pattern.SelectedValue.ToString();
                sql += string.Format("(pat={0} OR pat_01={0})", pat) + " AND ";
            }
            sql += " 1=1 ORDER BY id";
            return sql;
        }

        public void SelectEnemyById(int id)
        {
            foreach (var item in EnemyDataGrid.Items) {
                if ((item as DataRowView).Row["id"].ToString() == id.ToString()) {
                    EnemyDataGrid.SelectedItem = item;
                    EnemyDataGrid.ScrollIntoView(item);
                    break;
                }
            }
        }

        private void EnemyLvButton_OnClick(object sender, RoutedEventArgs e) {
            EnemyInfo_lv.Text = ((Button) sender).Content.ToString();
            EnemyInfo_lv_LostFocus(null, null);
        }
    }
}
