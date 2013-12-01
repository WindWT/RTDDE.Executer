using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text.RegularExpressions;

namespace RTDDataExplorer
{
    public partial class Calc : System.Web.UI.Page
    {
        private DataRow unitData;
        protected void Page_Load(object sender, EventArgs e)
        {
            string gID = Request.QueryString["g_id"];
            if (!string.IsNullOrEmpty(gID))
            {
                DB db = new DB(false);
                DataTable dt = db.GetData("SELECT * FROM UNIT_MASTER WHERE g_id=" + gID);
                if (dt.Rows.Count == 0)
                {
                    Response.Write("<script language:javascript>javascript:window.close();</script>");
                }
                else
                {
                    unitData = dt.Rows[0];
                    g_id.Text = unitData["g_id"].ToString();
                    name.Text = unitData["name"].ToString();
                    rare.Text = "";
                    for (int i = 0; i < Convert.ToInt32(unitData["category"]); i++)
                    {
                        rare.Text += "★";
                    }
                }
            }
            else
            {
                Response.Write("<script language:javascript>javascript:window.close();</script>");
            }
        }

        protected void calc_Click(object sender, EventArgs e)
        {
            //get level
            double thislevel = Convert.ToDouble(Convert.ToInt32(level.Text));
            double maxlevel = Convert.ToDouble(unitData["lv_max"]);
            if (thislevel > maxlevel)
            {
                thislevel = maxlevel;
                level.Text = maxlevel.ToString("0");
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
            HP.Text = thishp.ToString("0");
            ATK.Text = thisattack.ToString("0");
            HEAL.Text = thisheal.ToString("0");

            //calc exp pt
            int num2 = Convert.ToInt32(unitData["category"]) - 1;
            float perMax = (float)(Math.Pow(thislevel, UnitParam_NextExp_Per[num2]) * (double)(Convert.ToSingle(unitData["grow"])));
            float perMin = (float)(Math.Pow(thislevel - 1, UnitParam_NextExp_Per[num2]) * (double)(Convert.ToSingle(unitData["grow"])));
            if (thislevel == maxlevel)
            {
                EXP.Text = perMin.ToString("0");
            }
            else
            {
                EXP.Text = perMin.ToString("0") + "~" + perMax.ToString("0");
            }

            int set_pt = Convert.ToInt32(unitData["set_pt"]);
            pt.Text = ((int)((float)(thislevel - 1) * Math.Pow((float)set_pt, 0.5f) + (float)set_pt)).ToString("0");

            int partyRankSkillId = Convert.ToInt32(unitData["p_skill_id"]);
            int activeRankSkillId = Convert.ToInt32(unitData["a_skill_id"]);
            int panelRankSkillId = Convert.ToInt32(unitData["panel_skill_id"]);

            SkillMaster partySkill = getSkillFromRankSkill("PARTY_SKILL", partyRankSkillId, (int)thislevel);
            SkillMaster activeSkill = getSkillFromRankSkill("ACTIVE_SKILL", activeRankSkillId, (int)thislevel);
            SkillMaster panelSkill = getSkillFromRankSkill("PANEL_SKILL", panelRankSkillId, (int)thislevel);

            passiveSkillName.Text = partySkill.name;
            passiveSkillText.Text = parseText(partySkill.text);
            activeSkillName.Text = String.Format("{0}({1})", activeSkill.name, activeSkill.soul);
            activeSkillText.Text = parseText(activeSkill.text);
            panelSkillName.Text = panelSkill.name;
            panelSkillText.Text = parseText(panelSkill.text);
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
        }
        public static string parseText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return String.Empty;
            }
            text = text.Replace(@"\n", string.Empty);
            Regex r = new Regex(@"(\[[a-zA-Z0-9]{6}\])(.*?)(\[-\])");
            return r.Replace(text, new MatchEvaluator(parseTextEvaluator));
        }
        public static string parseTextEvaluator(Match m)
        {
            string color = m.Groups[1].Value.Trim(new char[] { '[', ']' });
            return String.Format("<span style='color:#{0}'>{1}</span>", color, m.Groups[2].Value);
        }
        private SkillMaster getSkillFromRankSkill(string tableName, int rankSkillId, int thislevel = 1)
        {
            DB db = new DB(false);
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
                if (skillTable.Columns.Contains("soul"))
                {
                    sm.soul = Convert.ToInt32(skillTable.Rows[0]["soul"]);
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