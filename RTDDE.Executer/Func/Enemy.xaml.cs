using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            Utility.BindData(EnemyDataGrid, "SELECT id,name FROM Enemy_Unit_MASTER order by id");
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
        }

        private void EnemyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EnemyDataGrid.SelectedItem == null) {
                //avoid Exception
                return;
            }
            string Enemyid = ((DataRowView)EnemyDataGrid.SelectedItem).Row["id"].ToString();
            EnemyInfo_id.Text = Enemyid;
            EnemyInfo_lv.Text = "1";
            EnemyInfo_BindData(Enemyid);
        }
        private void EnemyInfo_lv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text)) {
                EnemyInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(EnemyInfo_lv.Text).Success) {
                EnemyInfo_lv.Text = "1";
                return;
            }
            EnemyInfo_BindData(EnemyInfo_id.Text);
        }

        public void EnemyInfo_BindData(string Enemyid)
        {
            if (string.IsNullOrWhiteSpace(EnemyInfo_lv.Text)) {
                return;
            }
            int thislevel = Convert.ToInt32(EnemyInfo_lv.Text);

            Task<EnemyUnitMaster> task = new Task<EnemyUnitMaster>(() =>
            {
                string sql = @"Select * from Enemy_unit_master WHERE id={0}";
                return DAL.ToSingle<EnemyUnitMaster>(String.Format(sql, Enemyid));
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null) {
                    Utility.ShowException(t.Exception.InnerException.Message);
                    return;
                }
                if (task.Result == null) {
                    return;
                }
                EnemyUnitMaster eum = task.Result;
                EnemyInfo_id.Text = eum.id.ToString();
                EnemyInfo_chara_flag_no.Text = eum.chara_flag_no == 0 ? string.Empty : eum.chara_flag_no.ToString();
                int rare = eum.chara_symbol - 9;    //chara_symbol start from 10, equal to rare 1
                string rareText = string.Empty;
                if (rare >= 1 && rare <= 6) {
                    for (int i = 0; i < rare; i++) {
                        rareText += "★";
                    }
                }
                EnemyInfo_chara_symbol.Text = rareText;
                EnemyInfo_name.Text = eum.name;
                EnemyInfo_attribute.Text = Utility.ParseAttributetype(eum.attribute);
                EnemyInfo_type.Text = Utility.ParseEnemyType(eum.type);
                EnemyInfo_chara_kind.Text = eum.chara_flag_no == 0 ? string.Empty : Utility.ParseUnitKind(eum.chara_kind).ToString();

                EnemyInfo_lv.Text = thislevel.ToString("0");
                EnemyInfo_HP.Text = Utility.RealCalc(eum.life, eum.up_life, thislevel).ToString();
                EnemyInfo_ATK.Text = Utility.RealCalc(eum.attack, eum.up_attack, thislevel).ToString();
                EnemyInfo_DEF.Text = Utility.RealCalc(eum.defense, eum.up_defense, thislevel).ToString();
                EnemyInfo_soul_pt.Text = eum.soul_pt.ToString();
                EnemyInfo_gold_pt.Text = eum.gold_pt.ToString();
                EnemyInfo_flag.Text = Convert.ToBoolean(eum.flag).ToString();
                EnemyInfo_isUnit.Text = Utility.IsUnitEnemy(eum.type).ToString();
                EnemyInfo_turn.Text = eum.turn.ToString();
                EnemyInfo_ui.Text = eum.ui.ToString();

                EnemyInfo_pat.Text = Utility.ParseAttackPattern(eum.pat);
                EnemyInfo_p0.Text = eum.p0.ToString();
                EnemyInfo_p1.Text = eum.p1.ToString();
                EnemyInfo_pat_01.Text = Utility.ParseAttackPattern(eum.pat_01);
                EnemyInfo_p0_01.Text = eum.p0_01.ToString();
                EnemyInfo_p1_01.Text = eum.p1_01.ToString();
                //Advanced
                EnemyInfo_model.Text = eum.model;
                EnemyInfo_texture.Text = eum.texture;
                EnemyInfo_icon.Text = eum.icon.ToString();
                EnemyInfo_shadow.Text = eum.shadow.ToString();
                EnemyInfo_up.Text = eum.up.ToString();
                EnemyInfo_atk_ef_id.Text = eum.atk_ef_id.ToString();
            }, MainWindow.UiTaskScheduler);    //this Task work on ui thread
            task.Start();
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
            Utility.BindData(EnemyDataGrid, "SELECT id,name FROM Enemy_Unit_MASTER order by id");
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
            string sql = @"SELECT id,name FROM Enemy_unit_MASTER WHERE ";
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
    }
}
