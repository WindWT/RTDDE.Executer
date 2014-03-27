using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Unit.xaml 的交互逻辑
    /// </summary>
    public partial class Unit : UserControl
    {
        public Unit()
        {
            InitializeComponent();
        }
        public void Refresh()
        {
            UnitTab_Initialized(null, null);
        }
        private void UnitTab_Initialized(object sender, EventArgs e)
        {
            Utility.BindData(UnitDataGrid, "SELECT id,g_id,name FROM UNIT_MASTER order by g_id");
            UnitSearch_category.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"★","1"},
                {"★★","2"},
                {"★★★","3"},
                {"★★★★","4"},
                {"★★★★★","5"},
                {"★×6","6"}
            };
            UnitSearch_style.ItemsSource = new Dictionary<string, string>()
            {
                {"------",""},
                {"KNIGHT","1"},
                {"LANCER","2"},
                {"ARCHER","3"},
                {"WIZARD","4"}
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
            UnitSearch_attribute.ItemsSource = attrDict;
            UnitSearch_sub_a1.ItemsSource = attrDict;
            var kindDict = new Dictionary<string, string>()
            {
                {"------",""},
            };
            foreach (AssignID kind in Enum.GetValues(typeof(AssignID)))
            {
                if (kind.ToString().StartsWith("MS0") == false)
                {
                    kindDict.Add(kind.ToString(), Utility.ParseAssignID(kind).ToString());
                }
            }
            UnitSearch_kind.ItemsSource = kindDict;
        }

        private void UnitDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string unitid = ((DataRowView)UnitDataGrid.SelectedItem).Row["id"].ToString();
            UnitInfo_id.Text = unitid;

            if (Settings.IsDefaultLvMax)
            {
                UnitInfo_lv.Text = "99";
            }
            else
            {
                UnitInfo_lv.Text = "1";
            }

            UnitInfo_BindData(unitid);
        }
        private void UnitInfo_lv_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UnitInfo_lv.Text))
            {
                UnitInfo_lv.Text = "";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(UnitInfo_lv.Text).Success)
            {
                if (Settings.IsDefaultLvMax)
                {
                    UnitInfo_lv.Text = "99";     //This will trigger itself again
                    return;
                }
                else
                {
                    UnitInfo_lv.Text = "1";
                    return;
                }
            }
            UnitInfo_BindData(UnitInfo_id.Text);
        }
        /*private void UnitInfo_g_id_TextChanged(object sender, TextChangedEventArgs e)
        {
            //防止死循环触发事件
            if (UnitInfo_g_id.IsFocused)
            {
                if (string.IsNullOrWhiteSpace(UnitInfo_g_id.Text))
                {
                    UnitInfo_g_id.Text = "1";
                }
                Regex r = new Regex("[^0-9]");
                if (r.Match(UnitInfo_g_id.Text).Success)
                {
                    UnitInfo_g_id.Text = "1";
                }
                string g_id = UnitInfo_g_id.Text;
                if (IsDefaultLvMax)
                {
                    UnitInfo_lv.Text = "99"; 
                }
                else
                {
                    UnitInfo_lv.Text = "1";
                }
                Task<string> task = new Task<string>(() =>
                {
                    string sql = @"SELECT id FROM unit_master WHERE g_id={0}";
                    DB db = new DB();
                    return db.GetString(String.Format(sql, g_id));
                });
                task.ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(t.Result) == false)
                    {
                        UnitInfo_id.Text = t.Result;
                        UnitInfo_BindData(t.Result);
                    }
                }, uiTaskScheduler);
                task.Start();
            }
        }*/

        public void UnitInfo_BindData(string unitid)
        {
            if (string.IsNullOrWhiteSpace(UnitInfo_lv.Text))
            {
                return;
            }
            double thislevel = Convert.ToDouble(Convert.ToInt32(UnitInfo_lv.Text));

            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"
Select *,
(SELECT NAME FROM UNIT_MASTER as ui WHERE uo.rev_unit_id=ui.id) as rev_unit_name,
(SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_fire=ui.id) as ultimate_rev_unit_name_fire,
(SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_water=ui.id) as ultimate_rev_unit_name_water,
(SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_shine=ui.id) as ultimate_rev_unit_name_shine,
(SELECT NAME FROM UNIT_MASTER as ui WHERE uo.ultimate_rev_unit_id_dark=ui.id) as ultimate_rev_unit_name_dark
from unit_master as uo
WHERE uo.id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, unitid));
            });
            Task<List<SkillMaster>> taskParse = new Task<List<SkillMaster>>(() =>
            {
                List<SkillMaster> skillList = new List<SkillMaster>();
                Task.WaitAll(task);
                if (task.Result == null || task.Result.Rows.Count == 0)
                {
                    return null;
                }
                DataRow unitData = task.Result.Rows[0];

                double maxlevel = Convert.ToDouble(unitData["lv_max"]);
                if (Settings.IsEnableLevelLimiter && (thislevel > maxlevel))
                {
                    thislevel = maxlevel;
                }

                int partyRankSkillId = Convert.ToInt32(unitData["p_skill_id"]);
                int activeRankSkillId = Convert.ToInt32(unitData["a_skill_id"]);
                int panelRankSkillId = Convert.ToInt32(unitData["panel_skill_id"]);

                skillList.Add(new SkillMaster("PARTY_SKILL", partyRankSkillId, (int)thislevel));
                skillList.Add(new SkillMaster("ACTIVE_SKILL", activeRankSkillId, (int)thislevel));
                skillList.Add(new SkillMaster("PANEL_SKILL", panelRankSkillId, (int)thislevel));
                return skillList;
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
                DataRow unitData = task.Result.Rows[0];
                UnitInfo_g_id.Text = unitData["g_id"].ToString();
                UnitInfo_ui_id.Text = unitData["ui_id"].ToString();
                UnitInfo_flag_no.Text = unitData["flag_no"].ToString();
                UnitInfo_name.Text = unitData["name"].ToString();
                string rare = string.Empty;
                for (int i = 0; i < Convert.ToInt32(unitData["category"]); i++)
                {
                    rare += "★";
                }
                UnitInfo_category.Text = rare;
                UnitInfo_style.Text = Utility.ParseStyletype(Convert.ToInt32(unitData["style"]));
                UnitInfo_attribute.Text = Utility.ParseAttributetype(Convert.ToByte(unitData["attribute"]));
                UnitInfo_sub_a1.Text = Utility.ParseAttributetype(Convert.ToByte(unitData["sub_a1"]));
                UnitInfo_model.Text = unitData["model"].ToString();
                UnitInfo_kind.Text = Utility.ParseUnitKind(Convert.ToInt32(unitData["kind"]));

                UnitInfo_need_pt.Text = unitData["need_pt"].ToString();
                UnitInfo_bonus_limit_base.Text = unitData["bonus_limit_base"].ToString();
                UnitInfo_rev_unit_id.Text = unitData["rev_unit_id"].ToString();
                UnitInfo_rev_unit_name.Text = unitData["rev_unit_name"].ToString();

                if (Convert.ToInt32(unitData["ultimate_rev_unit_id_fire"]) == 0
                    && Convert.ToInt32(unitData["ultimate_rev_unit_id_water"]) == 0
                    && Convert.ToInt32(unitData["ultimate_rev_unit_id_shine"]) == 0
                    && Convert.ToInt32(unitData["ultimate_rev_unit_id_dark"]) == 0)
                {
                    UnitInfo_Panel_ultimate_rev.Visibility = Visibility.Collapsed;
                }
                else
                {
                    UnitInfo_Panel_ultimate_rev.Visibility = Visibility.Visible;
                    UnitInfo_max_attribute_exp.Text = unitData["max_attribute_exp"].ToString();
                    UnitInfo_rev_unit_name_fire.Text = unitData["ultimate_rev_unit_name_fire"].ToString();
                    UnitInfo_rev_unit_name_water.Text = unitData["ultimate_rev_unit_name_water"].ToString();
                    UnitInfo_rev_unit_name_shine.Text = unitData["ultimate_rev_unit_name_shine"].ToString();
                    UnitInfo_rev_unit_name_dark.Text = unitData["ultimate_rev_unit_name_dark"].ToString();
                }

                double maxlevel = Convert.ToDouble(unitData["lv_max"]);
                UnitInfo_lv_max.Text = maxlevel.ToString("0");
                UnitInfo_lv.Text = thislevel.ToString("0");

                //calc hp atk heal
                double y = UnitParam_CategoryPer[Convert.ToInt32(unitData["category"]) - 1];
                int num = (Convert.ToInt32(unitData["style"]) - 1) * 3;
                double y2 = UnitParam_StylePer[num];
                double y3 = UnitParam_StylePer[num + 1];
                double y4 = UnitParam_StylePer[num + 2];

                double thishp = (int)(Math.Pow(Math.Pow(thislevel, y), y2) * Convert.ToDouble(unitData["life"]));
                double thisattack = (int)(Math.Pow(Math.Pow(thislevel, y), y3) * Convert.ToDouble(unitData["attack"]));
                double thisheal = (int)(Math.Pow(Math.Pow(thislevel, y), y4) * Convert.ToDouble(unitData["heal"]));
                UnitInfo_HP.Text = thishp.ToString("0");
                UnitInfo_ATK.Text = thisattack.ToString("0");
                UnitInfo_HEAL.Text = thisheal.ToString("0");

                //calc exp pt
                int num2 = Convert.ToInt32(unitData["category"]) - 1;
                float perMax = (float)(Math.Pow(thislevel, UnitParam_NextExp_Per[num2]) * (double)(Convert.ToSingle(unitData["grow"])));
                float perMin = (float)(Math.Pow(thislevel - 1, UnitParam_NextExp_Per[num2]) * (double)(Convert.ToSingle(unitData["grow"])));
                if (thislevel == maxlevel)
                {
                    UnitInfo_EXP.Text = perMin.ToString("0");
                }
                else
                {
                    UnitInfo_EXP.Text = perMin.ToString("0") + "~" + perMax.ToString("0");
                }
                UnitInfo_EXP_max.Text = ((float)(Math.Pow(maxlevel - 1, UnitParam_NextExp_Per[num2]) * (double)(Convert.ToSingle(unitData["grow"])))).ToString("0");

                int set_pt = Convert.ToInt32(unitData["set_pt"]);
                UnitInfo_pt.Text = ((int)((float)(thislevel - 1) * Math.Pow((float)set_pt, 0.5f) + (float)set_pt)).ToString("0");
                //story
                var storyDoc = Utility.parseTextToDocument(unitData["story"].ToString());
                storyDoc.TextAlignment = TextAlignment.Center;
                UnitInfo_story.Padding = new Thickness(8, 0, 8, 0);
                UnitInfo_story.Document = storyDoc;
                //cutin
                UnitInfo_ct_text.Text = unitData["ct_text"].ToString();
                UnitInfo_sct_text.Text = unitData["sct_text"].ToString();
                UnitInfo_sct6_text.Text = unitData["sct6_text"].ToString();
                UnitInfo_a_skill_text.Text = unitData["a_skill_text"].ToString();
                //skill
                partySkill_BindData(t.Result[0]);
                activeSkill_BindData(t.Result[1]);
                panelSkill_BindData(t.Result[2]);

            }, MainWindow.uiTaskScheduler);    //this Task work on ui thread
            task.Start();
            taskParse.Start();
        }
        private void partySkill_BindData(SkillMaster skill)
        {
            if (skill.id == 0)
            {
                UnitInfo_PartySkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_PartySkill.Visibility = Visibility.Visible;

            partySkill_id.Text = skill.id.ToString();
            partySkill_name.Text = skill.name;
            partySkill_text.Document = Utility.parseTextToDocument(skill.text);
            partySkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
            partySkill_sub_attr.Text = Utility.ParseAttributetype(skill.sub_attr);
            partySkill_style.Text = Utility.ParseStyletype(skill.style);
            partySkill_type.Text = Utility.ParseSkillType((PassiveSkillType)skill.type);
            partySkill_num.Text = skill.num.ToString();
            partySkill_num_01.Text = skill.num_01.ToString();
            partySkill_num_02.Text = skill.num_02.ToString();
            partySkill_num_03.Text = skill.num_03.ToString();
        }
        private void activeSkill_BindData(SkillMaster skill)
        {
            if (skill.id == 0)
            {
                UnitInfo_ActiveSkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_ActiveSkill.Visibility = Visibility.Visible;

            activeSkill_id.Text = skill.id.ToString();
            activeSkill_name.Text = skill.name;
            activeSkill_text.Document = Utility.parseTextToDocument(skill.text);
            activeSkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
            activeSkill_sub_attr.Text = Utility.ParseAttributetype(skill.sub_attr);
            activeSkill_style.Text = Utility.ParseStyletype(skill.style);
            activeSkill_type.Text = Utility.ParseSkillType((ActiveSkillType)skill.type);
            activeSkill_num.Text = skill.num.ToString();
            activeSkill_num_01.Text = skill.num_01.ToString();
            activeSkill_num_02.Text = skill.num_02.ToString();
            activeSkill_num_03.Text = skill.num_03.ToString();
            activeSkill_soul.Text = skill.soul.ToString();
            activeSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
            activeSkill_limit_num.Text = skill.limit_num.ToString();
        }
        private void panelSkill_BindData(SkillMaster skill)
        {
            if (skill.id == 0)
            {
                UnitInfo_PanelSkill.Visibility = Visibility.Collapsed;
                return;
            }
            UnitInfo_PanelSkill.Visibility = Visibility.Visible;

            panelSkill_id.Text = skill.id.ToString();
            panelSkill_name.Text = skill.name;
            panelSkill_text.Document = Utility.parseTextToDocument(skill.text);
            panelSkill_attribute.Text = Utility.ParseAttributetype(skill.attribute);
            panelSkill_style.Text = Utility.ParseStyletype(skill.style);
            panelSkill_type.Text = Utility.ParseSkillType((PanelSkillType)skill.type);
            panelSkill_num.Text = skill.num.ToString();
            panelSkill_num_01.Text = skill.num_01.ToString();
            panelSkill_num_02.Text = skill.num_02.ToString();
            panelSkill_num_03.Text = skill.num_03.ToString();
            panelSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
            panelSkill_duplication.Text = skill.duplication == 1 ? "重複可" : skill.duplication == 2 ? "重複不可" : String.Empty;
        }

        private void UnitSearchClear_Click(object sender, RoutedEventArgs e)
        {
            UnitSearch_id.Text = string.Empty;
            UnitSearch_g_id.Text = string.Empty;
            UnitSearch_name.Text = string.Empty;
            UnitSearch_category.SelectedIndex = 0;
            UnitSearch_style.SelectedIndex = 0;
            UnitSearch_attribute.SelectedIndex = 0;
            UnitSearch_sub_a1.SelectedIndex = 0;
            Utility.BindData(UnitDataGrid, "SELECT id,g_id,name FROM UNIT_MASTER order by g_id");
        }
        private void UnitSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Utility.BindData(UnitDataGrid, UnitSearch_BuildSQL());
        }
        private void UnitSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Utility.BindData(UnitDataGrid, UnitSearch_BuildSQL());
        }
        private string UnitSearch_BuildSQL()
        {
            string sql = @"SELECT id,g_id,name FROM UNIT_MASTER WHERE ";
            if (String.IsNullOrWhiteSpace(UnitSearch_id.Text) == false)
            {
                sql += "id=" + UnitSearch_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(UnitSearch_g_id.Text) == false)
            {
                sql += "g_id=" + UnitSearch_g_id.Text + " AND ";
            }
            if (String.IsNullOrWhiteSpace(UnitSearch_name.Text) == false)
            {
                sql += "name LIKE '%" + UnitSearch_name.Text.Trim() + "%' AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_category.SelectedValue) == false)
            {
                sql += "category=" + UnitSearch_category.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_kind.SelectedValue) == false)
            {
                sql += "kind=" + UnitSearch_kind.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_style.SelectedValue) == false)
            {
                sql += "style=" + UnitSearch_style.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_attribute.SelectedValue) == false)
            {
                sql += "attribute=" + UnitSearch_attribute.SelectedValue.ToString() + " AND ";
            }
            if (String.IsNullOrWhiteSpace((string)UnitSearch_sub_a1.SelectedValue) == false)
            {
                sql += "sub_a1=" + UnitSearch_sub_a1.SelectedValue.ToString() + " AND ";
            }
            sql += " 1=1 ORDER BY g_id";
            return sql;
        }
        #region Magic Numbers
        private static readonly double[] UnitParam_CategoryPer = new double[]
{
	0.38999998569488525,
	0.40000000596046448,
	0.40999999642372131,
	0.41999998688697815,
	0.43000000715255737,
	0.40999999642372131
};
        public static readonly double[] UnitParam_NextExp_Per = new double[]
{
	2.380000114440918,
	2.4200000762939453,
	2.4800000190734863,
	2.5799999237060547,
	2.6600000858306885,
	2.5999999046325684
};

        private static readonly double[] UnitParam_StylePer = new double[]
{
	1.1000000238418579,
	1.1000000238418579,
	0.699999988079071,
	0.800000011920929,
	1.25,
	0.85000002384185791,
	0.949999988079071,
	1.0,
	0.949999988079071,
	1.0,
	0.60000002384185791,
	1.2999999523162842
};
        #endregion
    }
}
