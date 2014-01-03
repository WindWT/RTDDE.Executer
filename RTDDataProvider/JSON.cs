using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RTDDataProvider
{
    #region JSON反序列化用的类
    [Serializable]
    public class ActiveSkillMaster
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
        public int soul;
        public int phase;
        public string text;
    }
    [Serializable]
    public class ActiveSkillRankMaster
    {
        public int id;
        public int skill_01_09;
        public int skill_10_19;
        public int skill_20_29;
        public int skill_30_39;
        public int skill_40_49;
        public int skill_50_59;
        public int skill_60_69;
        public int skill_70_79;
        public int skill_80_89;
        public int skill_90_99;
        public int skill_100;
    }
    [Serializable]
    public class EnemyDropMaster
    {
        public int id;
        public int unit1_id;
        public int unit1_lv_min;
        public int unit1_lv_max;
        public int unit1_rate;
        public int unit2_id;
        public int unit2_lv_min;
        public int unit2_lv_max;
        public int unit2_rate;
        public int unit3_id;
        public int unit3_lv_min;
        public int unit3_lv_max;
        public int unit3_rate;
        public int unit4_id;
        public int unit4_lv_min;
        public int unit4_lv_max;
        public int unit4_rate;
        public int sp_event_point;
        public int add_attribute_exp;
    }
    [Serializable]
    public class EnemyTableMaster
    {
        public int id;
        public int enemy1_id;
        public int enemy1_set_id;
        public int enemy1_lv_min;
        public int enemy1_lv_max;
        public int enemy1_rate;
        public int enemy1_drop_id;
        public int enemy2_id;
        public int enemy2_set_id;
        public int enemy2_lv_min;
        public int enemy2_lv_max;
        public int enemy2_rate;
        public int enemy2_drop_id;
        public int enemy3_id;
        public int enemy3_set_id;
        public int enemy3_lv_min;
        public int enemy3_lv_max;
        public int enemy3_rate;
        public int enemy3_drop_id;
        public int enemy4_id;
        public int enemy4_set_id;
        public int enemy4_lv_min;
        public int enemy4_lv_max;
        public int enemy4_rate;
        public int enemy4_drop_id;
        public int enemy5_id;
        public int enemy5_set_id;
        public int enemy5_lv_min;
        public int enemy5_lv_max;
        public int enemy5_rate;
        public int enemy5_drop_id;
        public int enemy6_id;
        public int enemy6_set_id;
        public int enemy6_lv_min;
        public int enemy6_lv_max;
        public int enemy6_rate;
        public int enemy6_drop_id;
        public int enemy7_id;
        public int enemy7_set_id;
        public int enemy7_lv_min;
        public int enemy7_lv_max;
        public int enemy7_rate;
        public int enemy7_drop_id;
        public int enemy8_id;
        public int enemy8_set_id;
        public int enemy8_lv_min;
        public int enemy8_lv_max;
        public int enemy8_rate;
        public int enemy8_drop_id;
        public int enemy9_id;
        public int enemy9_set_id;
        public int enemy9_lv_min;
        public int enemy9_lv_max;
        public int enemy9_rate;
        public int enemy9_drop_id;
        public int enemy10_id;
        public int enemy10_set_id;
        public int enemy10_lv_min;
        public int enemy10_lv_max;
        public int enemy10_rate;
        public int enemy10_drop_id;
        public int enemy11_id;
        public int enemy11_set_id;
        public int enemy11_lv_min;
        public int enemy11_lv_max;
        public int enemy11_rate;
        public int enemy11_drop_id;
        public int enemy12_id;
        public int enemy12_set_id;
        public int enemy12_lv_min;
        public int enemy12_lv_max;
        public int enemy12_rate;
        public int enemy12_drop_id;
        public int enemy13_id;
        public int enemy13_set_id;
        public int enemy13_lv_min;
        public int enemy13_lv_max;
        public int enemy13_rate;
        public int enemy13_drop_id;
        public int enemy14_id;
        public int enemy14_set_id;
        public int enemy14_lv_min;
        public int enemy14_lv_max;
        public int enemy14_rate;
        public int enemy14_drop_id;
        public int enemy15_id;
        public int enemy15_set_id;
        public int enemy15_lv_min;
        public int enemy15_lv_max;
        public int enemy15_rate;
        public int enemy15_drop_id;
        public int enemy16_id;
        public int enemy16_set_id;
        public int enemy16_lv_min;
        public int enemy16_lv_max;
        public int enemy16_rate;
        public int enemy16_drop_id;
        public int enemy17_id;
        public int enemy17_set_id;
        public int enemy17_lv_min;
        public int enemy17_lv_max;
        public int enemy17_rate;
        public int enemy17_drop_id;
        public int enemy18_id;
        public int enemy18_set_id;
        public int enemy18_lv_min;
        public int enemy18_lv_max;
        public int enemy18_rate;
        public int enemy18_drop_id;
        public int boss_id;
        public int boss_set_id;
        public int boss_lv_min;
        public int boss_lv_max;
        public int boss_rate;
        public int boss_drop_id;
        public int death_id;
        public int death_set_id;
        public int death_lv_min;
        public int death_lv_max;
        public int death_rate;
        public int death_drop_id;
    }
    [Serializable]
    public class EnemyUnitMaster
    {
        public int id;
        public int flag;
        public string name;
        public string model;
        public string texture;
        public int icon;
        public int shadow;
        public int chara_kind;
        public int chara_symbol;
        public int chara_g_id;
        public int type;
        public int attribute;
        public int up;
        public int soul_pt;
        public int gold_pt;
        public int up_life;
        public int up_attack;
        public int up_defense;
        public int life;
        public int attack;
        public int defense;
        public int turn;
        public int ui;
        public short atk_ef_id;
    }
    [Serializable]
    public class GlobalParamsMaster
    {
        public int id;
        public int val;
    }
    [Serializable]
    public class LevelDataListMaster
    {
        public int level_data_id;
    }
    [Serializable]
    public class LevelDataMaster
    {
        public int level_data_id;
        public int format;
        public int start_x;
        public int start_y;
        public int width;
        public int height;
        public string map_data;
        public string hash;
    }
    [Serializable]
    public class LoginBonusMaster
    {
        public int id;
        public string day;
        public int present_type;
        public int present_param;
    }
    [Serializable]
    public class PanelSkillMaster
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
        public int duplication;
        public string text;
    }
    [Serializable]
    public class PanelSkillRankMaster
    {
        public int id;
        public int skill_01_09;
        public int skill_10_19;
        public int skill_20_29;
        public int skill_30_39;
        public int skill_40_49;
        public int skill_50_59;
        public int skill_60_69;
        public int skill_70_79;
        public int skill_80_89;
        public int skill_90_99;
        public int skill_100;
    }
    [Serializable]
    public class PartySkillMaster
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
        public string text;
    }
    [Serializable]
    public class PartySkillRankMaster
    {
        public int id;
        public int skill_01_09;
        public int skill_10_19;
        public int skill_20_29;
        public int skill_30_39;
        public int skill_40_49;
        public int skill_50_59;
        public int skill_60_69;
        public int skill_70_79;
        public int skill_80_89;
        public int skill_90_99;
        public int skill_100;
    }
    [Serializable]
    public class QuestCategoryMaster
    {
        public int id;
        public int kind;
        public int zbtn_kind;
        public int pt_num;
        public string icon;
        public string name;
        public string text;
        public int display_order;
    }
    [Serializable]
    public class QuestChallengeMaster
    {
        public int id;
        public int grade;
        public int type;
        public int param_0;
        public int param_1;
        public int param_2;
        public int param_3;
        public string text;
    }
    [Serializable]
    public class QuestChallengeRewardMaster
    {
        public int id;
        public int category;
        public int point;
        public int present_type;
        public int present_param_0;
        public int present_param_1;
    }
    [Serializable]
    public class QuestMaster
    {
        public int id;
        public string name;
        public int division;
        public int category;
        public int map;
        public int stamina;
        public int soul;
        public int kind;
        public int zbtn_kind;
        public int pt_num;
        public int kpi_class;
        public int flag_no;
        public int display_order;
        public int difficulty;
        public int distance;
        public int sp_event_id;
        public int open_type_1;
        public int open_param_1;
        public int open_type_2;
        public int open_param_2;
        public int open_type_3;
        public int open_param_3;
        public int open_type_4;
        public int open_param_4;
        public int open_type_5;
        public int open_param_5;
        public int open_type_6;
        public int open_param_6;
        public int open_sp_event_id;
        public int open_sp_event_point;
        public uint bonus_start;
        public uint bonus_end;
        public int bonus_type;
        public int panel_sword;
        public int panel_lance;
        public int panel_archer;
        public int panel_cane;
        public int panel_heart;
        public int panel_sp;
        public int reward_money;
        public int reward_exp;
        public int reward_soul;
        public int reward_money_limit;
        public int reward_exp_limit;
        public int enemy_table_id;
        public int enemy_table_hash;
        public string text;
        public int present_type;
        public int present_param;
        public int present_param_1;
        public int challenge_id_0;
        public int challenge_id_1;
        public int challenge_id_2;
        public int tflg_cmd_0;
        public int tflg_idx_0;
        public int tflg_cmd_1;
        public int tflg_idx_1;
        public int sp_guide_id;
    }
    [Serializable]
    public class SequenceLoginBonusMaster
    {
        public int id;
        public int type;
        public int num;
    }
    [Serializable]
    public class ShopProductMaster
    {
        public string id;
        public string name;
        public uint price;
        public uint point;
    }
    [Serializable]
    public class SpEventMaster
    {
        public int sp_event_id;
        public string icon;
        public string target_name;
        public string info_message_0;
        public string info_message_1;
        public string lock_message;
        public string unlock_message;
    }
    [Serializable]
    public class UnitMaster
    {
        public int id;
        public int g_id;
        public int flag_no;
        public string name;
        public string model;
        public int shadow;
        public int category;
        public int attribute;
        public int sub_a1;
        public int style;
        public int sub_c1;
        public int sub_c2;
        public int sub_c3;
        public int sub_c4;
        public int kind;
        public int lv_max;
        public int rev_unit_id;
        public int ultimate_rev_unit_id_fire;
        public int ultimate_rev_unit_id_water;
        public int ultimate_rev_unit_id_shine;
        public int ultimate_rev_unit_id_dark;
        public int max_attribute_exp;
        public int max_attribute_exp_fire;
        public int max_attribute_exp_water;
        public int max_attribute_exp_shine;
        public int max_attribute_exp_dark;
        public int need_pt;
        public int set_pt;
        public int grow;
        public int mix;
        public int cost;
        public int sale;
        public int ap_rec_val;
        public int bonus_limit_base;
        public int yorisiro;
        public int present;
        public int material_type;
        public int life;
        public int attack;
        public int heal;
        public int p_skill_id;
        public int a_skill_id;
        public int panel_skill_id;
        public string story;
        public string ct_text;
        public string sct_text;
        public string sct6_text;
        public string a_skill_text;
        public string icon_name;
        //public UIAtlas cutin_atlas;
        public string cutin_name1;
        public string cutin_name2;
    }
    [Serializable]
    public class UnitTalkMaster
    {
        public int id;
        public string icon;
        public string message_00;
        public string message_01;
        public string message_02;
        public string message_03;
        public string message_04;
        public string message_05;
        public string message_06;
        public string message_07;
        public string message_08;
        public string message_09;
        public string message_10;
        public string message_11;
        public string message_12;
        public string message_13;
        public string message_14;
        public string message_15;
        public string message_16;
        public string message_17;
        public string message_18;
        public string message_19;
        public string message_20;
        public string message_21;
        public string message_22;
        public string message_23;
        public string message_24;
        public string message_25;
        public string message_26;
        public string message_27;
        public string message_28;
        public string message_29;
        public string message_30;
        public string message_31;
        public string message_32;
        public string message_33;
        public string message_34;
        public string message_35;
        public string message_36;
        public string message_37;
        public string message_38;
        public string message_39;
        public string message_40;
        public string message_41;
        public string message_42;
        public string message_43;
        public string message_44;
        public string message_45;
        public string message_46;
        public string message_47;
        public string message_48;
        public string message_49;
        public string message_50;
        public string message_51;
        public string message_52;
        public string message_53;
        public string message_54;
        public string message_55;
        public string message_56;
        public string message_57;
        public string message_58;
        public string message_59;
        public string message_60;
        public string message_61;
        public string message_62;
        public string message_63;
        public string message_64;
        public string message_65;
        public string message_66;
        public string message_67;
        public string message_68;
        public string message_69;
        public string message_70;
        public string message_71;
        public string message_72;
        public string message_73;
        public string message_74;
        public string message_75;
        public string message_76;
        public string message_77;
        public string message_78;
        public string message_79;
        public string message_80;
        public string message_81;
        public string message_82;
        public string message_83;
        public string message_84;
        public string message_85;
        public string message_86;
        public string message_87;
        public string message_88;
        public string message_89;
        public string message_90;
        public string message_91;
        public string message_92;
        public string message_93;
        public string message_94;
        public string message_95;
        public string message_96;
        public string message_97;
        public string message_98;
        public string message_99;
        public string message_100;
        public string message_101;
        public string message_102;
        public string message_103;
        public string message_104;
        public string message_105;
        public string message_106;
        public string message_107;
        public string message_108;
        public string message_109;
        public string message_110;
        public string message_111;
        public string message_112;
        public string message_113;
        public string message_114;
        public string message_115;
        public string message_116;
        public string message_117;
        public string message_118;
        public string message_119;
        public string message_120;
        public string message_121;
        public string message_122;
        public string message_123;
        public string message_124;
        public string message_125;
        public string message_126;
        public string message_127;
    }
    [Serializable]
    public class UserRankMaster
    {
        public int rank;
        public int need_exp;
        public int flags;
        public int game_money;
        public int friend_point;
        public int unit_id;
    }
    [Serializable]
    public class EnemyInfo
    {
        public int enemy_id;
        public int enemy_set_id;
        public int x;
        public int y;
        public bool flag;
        public int drop_data_id;
        public int drop_unit_id;
        public int drop_unit_level;
        public int sp_event_point;
        public int add_attribute_exp;
    }
    #endregion

    public static class JSON
    {
        public static DataTable ParseJSONMDB(string json, MASTERDB MDBType)
        {
            JObject jo = JObject.Parse(json);
            JToken jt = jo["result"][0]["data_list"];   //只有MDB能这么玩，LDB不行
            string jtjson = JsonConvert.SerializeObject(jt);
            DataTable dt = new DataTable();
            switch (MDBType)
            {
                case MASTERDB.USER_RANK_MASTER:
                    {
                        UserRankMaster[] o = JsonConvert.DeserializeObject<UserRankMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.UNIT_MASTER:
                    {
                        UnitMaster[] o = JsonConvert.DeserializeObject<UnitMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.PARTY_SKILL_MASTER:
                    {
                        PartySkillMaster[] o = JsonConvert.DeserializeObject<PartySkillMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.PARTY_SKILL_RANK_MASTER:
                    {
                        PartySkillRankMaster[] o = JsonConvert.DeserializeObject<PartySkillRankMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.ACTIVE_SKILL_MASTER:
                    {
                        ActiveSkillMaster[] o = JsonConvert.DeserializeObject<ActiveSkillMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.ACTIVE_SKILL_RANK_MASTER:
                    {
                        ActiveSkillRankMaster[] o = JsonConvert.DeserializeObject<ActiveSkillRankMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_UNIT_MASTER:
                    {
                        EnemyUnitMaster[] o = JsonConvert.DeserializeObject<EnemyUnitMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_TABLE_MASTER:
                    {
                        EnemyTableMaster[] o = JsonConvert.DeserializeObject<EnemyTableMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_DROP_MASTER:    //not exist
                    {
                        EnemyDropMaster[] o = JsonConvert.DeserializeObject<EnemyDropMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_MASTER:
                    {
                        QuestMaster[] o = JsonConvert.DeserializeObject<QuestMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CATEGORY_MASTER:
                    {
                        QuestCategoryMaster[] o = JsonConvert.DeserializeObject<QuestCategoryMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                //case MASTERDB.GACHA_ITEM_MASTER:    //not exist
                //case MASTERDB.GACHA_TABLE_MASTER:   //not exist
                case MASTERDB.SHOP_PRODUCT_MASTER:  //not exist   
                    {
                        ShopProductMaster[] o = JsonConvert.DeserializeObject<ShopProductMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.SHOP_PRODUCT_MASTER_ANDROID:
                    {
                        ShopProductMaster[] o = JsonConvert.DeserializeObject<ShopProductMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.LOGIN_BONUS_MASTER:
                    {
                        LoginBonusMaster[] o = JsonConvert.DeserializeObject<LoginBonusMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.SEQUENCE_LOGIN_BONUS_MASTER:
                    {
                        SequenceLoginBonusMaster[] o = JsonConvert.DeserializeObject<SequenceLoginBonusMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.LEVELDATA_LIST_MASTER:  //not exist
                    {
                        LevelDataListMaster[] o = JsonConvert.DeserializeObject<LevelDataListMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.UNIT_TALK_MASTER:
                    {
                        UnitTalkMaster[] o = JsonConvert.DeserializeObject<UnitTalkMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.GLOBAL_PARAM_MASTER:
                    {
                        GlobalParamsMaster[] o = JsonConvert.DeserializeObject<GlobalParamsMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CHALLENGE_MASTER:
                    {
                        QuestChallengeMaster[] o = JsonConvert.DeserializeObject<QuestChallengeMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CHALLENGE_REWARD_MASTER:
                    {
                        QuestChallengeRewardMaster[] o = JsonConvert.DeserializeObject<QuestChallengeRewardMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.SP_EVENT_MASTER:
                    {
                        SpEventMaster[] o = JsonConvert.DeserializeObject<SpEventMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.PANEL_SKILL_MASTER:
                    {
                        PanelSkillMaster[] o = JsonConvert.DeserializeObject<PanelSkillMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                case MASTERDB.PANEL_SKILL_RANK_MASTER:
                    {
                        PanelSkillRankMaster[] o = JsonConvert.DeserializeObject<PanelSkillRankMaster[]>(jtjson);
                        dt = ConvertToDataTable(o);
                        break;
                    }
                //case MASTERDB.MAX   //not exist
                default:
                    {
                        break;
                    }
            }
            dt.TableName = MDBType.ToString();
            dt.PrimaryKey = new DataColumn[] { dt.Columns["id"] };
            return dt;
        }
        public static DataTable ParseJSONLDB(string json)
        {
            LevelDataMaster ldm = JsonConvert.DeserializeObject<LevelDataMaster>(json);
            LevelDataMaster[] o = new LevelDataMaster[1];
            o[0] = ldm;
            DataTable dt = ConvertToDataTable(o);
            dt.PrimaryKey = new DataColumn[] { dt.Columns["level_data_id"] };
            return dt;
        }
        public static List<EnemyInfo> ParseEnemyInfo(string questId, string quest, string enemyInfo)
        {
            List<EnemyInfo> ei = new List<EnemyInfo>();
            string currentQuestId = JObject.Parse(quest)["m_QuestID"].ToString();
            if (questId == currentQuestId)
            {
                ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(enemyInfo);
            }
            return ei;
        }
        #region Converting ObjectArray to Datatable
        private static DataTable ConvertToDataTable(Object[] array)
        {
            FieldInfo[] fields = array.GetType().GetElementType().GetFields();
            DataTable dt = CreateDataTable(fields);
            if (array.Length != 0)
            {
                foreach (object o in array)
                    FillData(fields, dt, o);
            }
            return dt;
        }

        private static DataTable CreateDataTable(FieldInfo[] fields)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;
            foreach (FieldInfo fi in fields)
            {
                dc = new DataColumn();
                dc.ColumnName = fi.Name;
                dc.DataType = fi.FieldType;
                dt.Columns.Add(dc);
            }
            return dt;
        }

        private static void FillData(FieldInfo[] fields, DataTable dt, Object o)
        {
            DataRow dr = dt.NewRow();
            foreach (FieldInfo fi in fields)
            {
                dr[fi.Name] = fi.GetValue(o);
            }
            dt.Rows.Add(dr);
        }

        #endregion
    }
}