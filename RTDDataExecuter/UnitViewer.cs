using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RTDDataExecuter
{
    public partial class MainWindow : Window
    {
        private void UnitViewerTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            Task<DataTable> task = new Task<DataTable>(() =>
            {
                DB db = new DB();
                return db.GetData("SELECT id,g_id,name FROM UNIT_MASTER order by g_id");
            });
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                UnitViewerDataGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);    //this Task work on ui thread
            task.Start();
        }
        private void UnitViewerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitViewerDataGrid.SelectedItem == null)
            {
                //avoid Exception
                return;
            }
            string unitid = ((DataRowView)UnitViewerDataGrid.SelectedItem).Row["id"].ToString();
            UnitInfo_id.Text = unitid;
            UnitInfo_lv.Text = "1";
            UnitInfo_BindData(unitid);
        }
        private void UnitInfo_lv_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UnitInfo_lv.Text))
            {
                UnitInfo_lv.Text = "1";
            }
            Regex r = new Regex("[^0-9]");
            if (r.Match(UnitInfo_lv.Text).Success)
            {
                UnitInfo_lv.Text = "1";
            }
            UnitInfo_BindData(UnitInfo_id.Text);
        }
        private void UnitInfo_g_id_TextChanged(object sender, TextChangedEventArgs e)
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
        }

        private static readonly object UnitInfo_BindingDataSyncObject = new object();
        private void UnitInfo_BindData(string unitid)
        {
            double thislevel = Convert.ToDouble(Convert.ToInt32(UnitInfo_lv.Text));

            Task<DataTable> task = new Task<DataTable>(() =>
            {
                string sql = @"SELECT * FROM unit_master WHERE id={0}";
                DB db = new DB();
                return db.GetData(String.Format(sql, unitid));
            });
            Task<List<SkillMaster>> taskParse = new Task<List<SkillMaster>>(() =>
            {
                List<SkillMaster> skillList = new List<SkillMaster>();
                Task.WaitAll(task);
                DataRow unitData = task.Result.Rows[0];
                if (unitData == null || unitData.ItemArray.Length == 0)
                {
                    return null;
                }
                int partyRankSkillId = Convert.ToInt32(unitData["p_skill_id"]);
                int activeRankSkillId = Convert.ToInt32(unitData["a_skill_id"]);
                int panelRankSkillId = Convert.ToInt32(unitData["panel_skill_id"]);

                skillList.Add(getSkillFromRankSkill("PARTY_SKILL", partyRankSkillId, (int)thislevel));
                skillList.Add(getSkillFromRankSkill("ACTIVE_SKILL", activeRankSkillId, (int)thislevel));
                skillList.Add(getSkillFromRankSkill("PANEL_SKILL", panelRankSkillId, (int)thislevel));
                return skillList;
            }
            );
            taskParse.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                DataRow unitData = task.Result.Rows[0];
                if (unitData == null || unitData.ItemArray.Length == 0)
                {
                    return;
                }
                UnitInfo_g_id.Text = unitData["g_id"].ToString();
                UnitInfo_name.Text = unitData["name"].ToString();
                string rare = string.Empty;
                for (int i = 0; i < Convert.ToInt32(unitData["category"]); i++)
                {
                    rare += "★";
                }
                UnitInfo_category.Text = rare;
                UnitInfo_style.Text = parseStyletype(Convert.ToInt32(unitData["style"]));
                UnitInfo_attribute.Text = parseAttributetype(Convert.ToByte(unitData["attribute"]));
                UnitInfo_sub_a1.Text = parseAttributetype(Convert.ToByte(unitData["sub_a1"]));
                UnitInfo_model.Text = unitData["model"].ToString();
                UnitInfo_kind.Text = parseUnitKind(Convert.ToInt32(unitData["kind"]));

                double maxlevel = Convert.ToDouble(unitData["lv_max"]);
                UnitInfo_lv_max.Text = maxlevel.ToString("0");
                if (thislevel > maxlevel)
                {
                    thislevel = maxlevel;
                    UnitInfo_lv.Text = maxlevel.ToString("0");
                }

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
                UnitInfo_story.Document = parseTextToDocument(unitData["story"].ToString());
                //cutin
                UnitInfo_ct_text.Text = unitData["ct_text"].ToString();
                UnitInfo_sct_text.Text = unitData["sct_text"].ToString();
                UnitInfo_sct6_text.Text = unitData["sct6_text"].ToString();
                UnitInfo_a_skill_text.Text = unitData["a_skill_text"].ToString();
                //skill
                partySkill_BindData(t.Result[0]);
                activeSkill_BindData(t.Result[1]);
                panelSkill_BindData(t.Result[2]);

            }, uiTaskScheduler);    //this Task work on ui thread
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
            partySkill_text.Document = parseTextToDocument(skill.text);
            partySkill_attribute.Text = parseAttributetype(skill.attribute);
            partySkill_style.Text = parseStyletype(skill.style);
            partySkill_type.Text = parseSkillType((PartySkillType)skill.type);
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
            activeSkill_text.Document = parseTextToDocument(skill.text);
            activeSkill_attribute.Text = parseAttributetype(skill.attribute);
            activeSkill_style.Text = parseStyletype(skill.style);
            activeSkill_type.Text = parseSkillType((ActiveSkillType)skill.type);
            activeSkill_num.Text = skill.num.ToString();
            activeSkill_num_01.Text = skill.num_01.ToString();
            activeSkill_num_02.Text = skill.num_02.ToString();
            activeSkill_num_03.Text = skill.num_03.ToString();
            activeSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
            activeSkill_soul.Text = skill.soul.ToString();
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
            panelSkill_text.Document = parseTextToDocument(skill.text);
            panelSkill_attribute.Text = parseAttributetype(skill.attribute);
            panelSkill_style.Text = parseStyletype(skill.style);
            panelSkill_type.Text = parseSkillType((PanelSkillType)skill.type);
            panelSkill_num.Text = skill.num.ToString();
            panelSkill_num_01.Text = skill.num_01.ToString();
            panelSkill_num_02.Text = skill.num_02.ToString();
            panelSkill_num_03.Text = skill.num_03.ToString();
            panelSkill_phase.Text = ((SkillPhase)skill.phase).ToString();
            panelSkill_duplication.Text = skill.duplication == 1 ? "重複可" : skill.duplication == 2 ? "重複不可" : String.Empty;
        }
        public class SkillMaster
        {
            public int id;
            public string name;
            public int type;
            public int attribute;
            public int style;
            public int num;
            public int num_01;
            public int num_02;
            public int num_03;
            public int phase;
            public int soul;
            public int duplication;
            public string text;
            public SkillMaster()
            {
                id = 0;
                name = String.Empty;
                text = String.Empty;
            }
        }
        private SkillMaster getSkillFromRankSkill(string tableName, int rankSkillId, int thislevel = 1)
        {
            DB db = new DB();
            int skillId = 0;
            SkillMaster sm = new SkillMaster();
            DataTable rankSkillTable = db.GetData("SELECT * FROM " + tableName + "_RANK_MASTER WHERE id=" + rankSkillId);
            if (rankSkillTable.Rows.Count == 0)
            {
                skillId = 0;
            }
            if (thislevel < 10)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_01_09"]);
            }
            else if (thislevel < 20)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_10_19"]);
            }
            else if (thislevel < 30)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_20_29"]);
            }
            else if (thislevel < 40)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_30_39"]);
            }
            else if (thislevel < 50)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_40_49"]);
            }
            else if (thislevel < 60)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_50_59"]);
            }
            else if (thislevel < 70)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_60_69"]);
            }
            else if (thislevel < 80)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_70_79"]);
            }
            else if (thislevel < 90)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_80_89"]);
            }
            else if (thislevel < 100)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_90_99"]);
            }
            else if (thislevel == 100)
            {
                skillId = Convert.ToInt32(rankSkillTable.Rows[0]["skill_100"]);
            }
            else
            {
                skillId = 0;
            }
            DataTable skillTable = db.GetData("SELECT * FROM " + tableName + "_MASTER WHERE id=" + skillId);
            if (skillTable.Rows.Count != 0)
            {
                sm.id = Convert.ToInt32(skillTable.Rows[0]["id"]);
                sm.name = Convert.ToString(skillTable.Rows[0]["name"]);
                sm.type = Convert.ToInt32(skillTable.Rows[0]["type"]);
                sm.attribute = Convert.ToInt32(skillTable.Rows[0]["attribute"]);
                sm.style = Convert.ToInt32(skillTable.Rows[0]["style"]);
                sm.num = Convert.ToInt32(skillTable.Rows[0]["num"]);
                sm.num_01 = Convert.ToInt32(skillTable.Rows[0]["num_01"]);
                sm.num_02 = Convert.ToInt32(skillTable.Rows[0]["num_02"]);
                sm.num_03 = Convert.ToInt32(skillTable.Rows[0]["num_03"]);
                sm.text = Convert.ToString(skillTable.Rows[0]["text"]);
                if (skillTable.Columns.Contains("phase"))
                {
                    sm.phase = Convert.ToInt32(skillTable.Rows[0]["phase"]);
                }
                if (skillTable.Columns.Contains("soul"))
                {
                    sm.soul = Convert.ToInt32(skillTable.Rows[0]["soul"]);
                }
                if (skillTable.Columns.Contains("duplication"))
                {
                    sm.duplication = Convert.ToInt32(skillTable.Rows[0]["duplication"]);
                }
            }
            return sm;
        }
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
    }
}
