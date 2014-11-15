using RTDDE.Provider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using RTDDE.Provider.MasterData;

namespace RTDDE.Executer
{
    public class Skills
    {
        public PartySkillMaster partySkill;
        public ActiveSkillMaster activeSkill;
        public PanelSkillMaster panelSkill;
        public LimitSkillMaster limitSkill;
        public ActiveSkillMaster[] limitActiveSkill;

        public Skills()
        {

        }
        public Skills(int unitid, int level = 1)
        {
            UnitMaster um = DAL.ToSingle<UnitMaster>(string.Format("SELECT * FROM UNIT_MASTER WHERE id={0}", unitid));
            InitSkills(um.p_skill_id, um.a_skill_id, um.panel_skill_id, um.limit_skill_id, level);
        }
        public Skills(int p_skill_id, int a_skill_id, int panel_skill_id, int limit_skill_id, int level = 1)
        {
            InitSkills(p_skill_id, a_skill_id, panel_skill_id, limit_skill_id, level);
        }
        public static Skills FromLimitSkillId(int limitSkillId)
        {
            Skills s = new Skills();
            s.limitSkill = DAL.ToSingle<LimitSkillMaster>(string.Format("SELECT * FROM LIMIT_SKILL_MASTER WHERE id={0}", limitSkillId));
            s.limitActiveSkill = new ActiveSkillMaster[3];
            if ((s.limitSkill == null || s.limitSkill.id == 0) == false)
            {
                s.limitActiveSkill[0] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", s.limitSkill.skill_id_00));
                s.limitActiveSkill[1] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", s.limitSkill.skill_id_01));
                s.limitActiveSkill[2] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", s.limitSkill.skill_id_02));
            }
            return s;
        }
        private void InitSkills(int p_skill_id, int a_skill_id, int panel_skill_id, int limit_skill_id, int level = 1)
        {
            PartySkillRankMaster partyRank = DAL.ToSingle<PartySkillRankMaster>(string.Format("SELECT * FROM PARTY_SKILL_RANK_MASTER WHERE id={0}", p_skill_id));
            ActiveSkillRankMaster activeRank = DAL.ToSingle<ActiveSkillRankMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_RANK_MASTER WHERE id={0}", a_skill_id));
            PanelSkillRankMaster panelRank = DAL.ToSingle<PanelSkillRankMaster>(string.Format("SELECT * FROM PANEL_SKILL_RANK_MASTER WHERE id={0}", panel_skill_id));
            LimitSkillRankMaster limitRank = DAL.ToSingle<LimitSkillRankMaster>(string.Format("SELECT * FROM LIMIT_SKILL_RANK_MASTER WHERE id={0}", limit_skill_id));
            //get real skill id from rank
            int partySkillId = 0, activeSkillId = 0, panelSkillId = 0, limitSkillId = 0;
            //party
            if (partyRank == null) { partySkillId = 0; }
            else if (level < 10) { partySkillId = partyRank.skill_01_09; }
            else if (level < 20) { partySkillId = partyRank.skill_10_19; }
            else if (level < 30) { partySkillId = partyRank.skill_20_29; }
            else if (level < 40) { partySkillId = partyRank.skill_30_39; }
            else if (level < 50) { partySkillId = partyRank.skill_40_49; }
            else if (level < 60) { partySkillId = partyRank.skill_50_59; }
            else if (level < 70) { partySkillId = partyRank.skill_60_69; }
            else if (level < 80) { partySkillId = partyRank.skill_70_79; }
            else if (level < 90) { partySkillId = partyRank.skill_80_89; }
            else if (level < 100) { partySkillId = partyRank.skill_90_99; }
            else if (level == 100) { partySkillId = partyRank.skill_100; }
            else { partySkillId = 0; }
            //active
            if (activeRank == null) { activeSkillId = 0; }
            else if (level < 10) { activeSkillId = activeRank.skill_01_09; }
            else if (level < 20) { activeSkillId = activeRank.skill_10_19; }
            else if (level < 30) { activeSkillId = activeRank.skill_20_29; }
            else if (level < 40) { activeSkillId = activeRank.skill_30_39; }
            else if (level < 50) { activeSkillId = activeRank.skill_40_49; }
            else if (level < 60) { activeSkillId = activeRank.skill_50_59; }
            else if (level < 70) { activeSkillId = activeRank.skill_60_69; }
            else if (level < 80) { activeSkillId = activeRank.skill_70_79; }
            else if (level < 90) { activeSkillId = activeRank.skill_80_89; }
            else if (level < 100) { activeSkillId = activeRank.skill_90_99; }
            else if (level == 100) { activeSkillId = activeRank.skill_100; }
            else { activeSkillId = 0; }
            //panel
            if (panelRank == null) { panelSkillId = 0; }
            else if (level < 10) { panelSkillId = panelRank.skill_01_09; }
            else if (level < 20) { panelSkillId = panelRank.skill_10_19; }
            else if (level < 30) { panelSkillId = panelRank.skill_20_29; }
            else if (level < 40) { panelSkillId = panelRank.skill_30_39; }
            else if (level < 50) { panelSkillId = panelRank.skill_40_49; }
            else if (level < 60) { panelSkillId = panelRank.skill_50_59; }
            else if (level < 70) { panelSkillId = panelRank.skill_60_69; }
            else if (level < 80) { panelSkillId = panelRank.skill_70_79; }
            else if (level < 90) { panelSkillId = panelRank.skill_80_89; }
            else if (level < 100) { panelSkillId = panelRank.skill_90_99; }
            else if (level == 100) { panelSkillId = panelRank.skill_100; }
            else { panelSkillId = 0; }
            //limit
            if (limitRank == null) { limitSkillId = 0; }
            else if (level < 10) { limitSkillId = limitRank.skill_01_09; }
            else if (level < 20) { limitSkillId = limitRank.skill_10_19; }
            else if (level < 30) { limitSkillId = limitRank.skill_20_29; }
            else if (level < 40) { limitSkillId = limitRank.skill_30_39; }
            else if (level < 50) { limitSkillId = limitRank.skill_40_49; }
            else if (level < 60) { limitSkillId = limitRank.skill_50_59; }
            else if (level < 70) { limitSkillId = limitRank.skill_60_69; }
            else if (level < 80) { limitSkillId = limitRank.skill_70_79; }
            else if (level < 90) { limitSkillId = limitRank.skill_80_89; }
            else if (level < 100) { limitSkillId = limitRank.skill_90_99; }
            else if (level == 100) { limitSkillId = limitRank.skill_100; }
            else { limitSkillId = 0; }
            partySkill = DAL.ToSingle<PartySkillMaster>(string.Format("SELECT * FROM PARTY_SKILL_MASTER WHERE id={0}", partySkillId));
            activeSkill = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", activeSkillId));
            panelSkill = DAL.ToSingle<PanelSkillMaster>(string.Format("SELECT * FROM PANEL_SKILL_MASTER WHERE id={0}", panelSkillId));
            limitSkill = DAL.ToSingle<LimitSkillMaster>(string.Format("SELECT * FROM LIMIT_SKILL_MASTER WHERE id={0}", limitSkillId));
            limitActiveSkill = new ActiveSkillMaster[3];
            if ((limitSkill == null || limitSkill.id == 0) == false)
            {
                limitActiveSkill[0] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", limitSkill.skill_id_00));
                limitActiveSkill[1] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", limitSkill.skill_id_01));
                limitActiveSkill[2] = DAL.ToSingle<ActiveSkillMaster>(string.Format("SELECT * FROM ACTIVE_SKILL_MASTER WHERE id={0}", limitSkill.skill_id_02));
            }
        }
    }
}
