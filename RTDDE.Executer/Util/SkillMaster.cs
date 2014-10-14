using RTDDE.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace RTDDE.Executer
{
    public class SkillMaster
    {
        public int id;
        public string name;
        public int type;
        public int attribute;
        public int sub_attr;
        public int style;
        public int num;
        public int num_01;
        public int num_02;
        public int num_03;
        public int soul;
        public int phase;
        public int limit_num;
        public int duplication;
        public string text;
        public SkillMaster()
        {
            id = 0;
            name = String.Empty;
            text = String.Empty;
        }
        public SkillMaster(string tableName, int rankSkillId, int thislevel = 1)
        {
            int skillId = 0;
            DataTable rankSkillTable = DAL.GetDataTable("SELECT * FROM " + tableName + "_RANK_MASTER WHERE id=" + rankSkillId);
            if (rankSkillTable.Rows.Count == 0)
            {
                skillId = 0;
            }
            else if (thislevel < 10)
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
            DataTable skillTable = DAL.GetDataTable("SELECT * FROM " + tableName + "_MASTER WHERE id=" + skillId);
            if (skillTable.Rows.Count != 0)
            {
                this.id = Convert.ToInt32(skillTable.Rows[0]["id"]);
                this.name = Convert.ToString(skillTable.Rows[0]["name"]);
                this.type = Convert.ToInt32(skillTable.Rows[0]["type"]);
                this.attribute = Convert.ToInt32(skillTable.Rows[0]["attribute"]);
                if (skillTable.Columns.Contains("sub_attr"))
                {
                    this.sub_attr = Convert.ToInt32(skillTable.Rows[0]["sub_attr"]);
                }
                this.style = Convert.ToInt32(skillTable.Rows[0]["style"]);
                this.num = Convert.ToInt32(skillTable.Rows[0]["num"]);
                this.num_01 = Convert.ToInt32(skillTable.Rows[0]["num_01"]);
                this.num_02 = Convert.ToInt32(skillTable.Rows[0]["num_02"]);
                this.num_03 = Convert.ToInt32(skillTable.Rows[0]["num_03"]);
                this.text = Convert.ToString(skillTable.Rows[0]["text"]);
                if (skillTable.Columns.Contains("soul"))
                {
                    this.soul = Convert.ToInt32(skillTable.Rows[0]["soul"]);
                }
                if (skillTable.Columns.Contains("phase"))
                {
                    this.phase = Convert.ToInt32(skillTable.Rows[0]["phase"]);
                }
                if (skillTable.Columns.Contains("limit_num"))
                {
                    this.limit_num = Convert.ToInt32(skillTable.Rows[0]["limit_num"]);
                }
                if (skillTable.Columns.Contains("duplication"))
                {
                    this.duplication = Convert.ToInt32(skillTable.Rows[0]["duplication"]);
                }
            }
        }
    }
}
