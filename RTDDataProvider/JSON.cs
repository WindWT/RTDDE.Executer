using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RTDDataProvider.MasterData;

namespace RTDDataProvider
{
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
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.UNIT_MASTER:
                    {
                        UnitMaster[] o = JsonConvert.DeserializeObject<UnitMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.PARTY_SKILL_MASTER:
                    {
                        PartySkillMaster[] o = JsonConvert.DeserializeObject<PartySkillMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.PARTY_SKILL_RANK_MASTER:
                    {
                        PartySkillRankMaster[] o = JsonConvert.DeserializeObject<PartySkillRankMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ACTIVE_SKILL_MASTER:
                    {
                        ActiveSkillMaster[] o = JsonConvert.DeserializeObject<ActiveSkillMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ACTIVE_SKILL_RANK_MASTER:
                    {
                        ActiveSkillRankMaster[] o = JsonConvert.DeserializeObject<ActiveSkillRankMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_UNIT_MASTER:
                    {
                        EnemyUnitMaster[] o = JsonConvert.DeserializeObject<EnemyUnitMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_TABLE_MASTER:
                    {
                        EnemyTableMaster[] o = JsonConvert.DeserializeObject<EnemyTableMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ENEMY_DROP_MASTER:    //not exist
                    {
                        EnemyDropMaster[] o = JsonConvert.DeserializeObject<EnemyDropMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_MASTER:
                    {
                        QuestMaster[] o = JsonConvert.DeserializeObject<QuestMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CATEGORY_MASTER:
                    {
                        QuestCategoryMaster[] o = JsonConvert.DeserializeObject<QuestCategoryMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                //case MASTERDB.GACHA_ITEM_MASTER:    //not exist
                //case MASTERDB.GACHA_TABLE_MASTER:   //not exist
                case MASTERDB.SHOP_PRODUCT_MASTER:  //exist in iOS only
                    {
                        ShopProductMaster[] o = JsonConvert.DeserializeObject<ShopProductMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.SHOP_PRODUCT_MASTER_ANDROID:
                    {
                        ShopProductMaster[] o = JsonConvert.DeserializeObject<ShopProductMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.LOGIN_BONUS_MASTER:
                    {
                        LoginBonusMaster[] o = JsonConvert.DeserializeObject<LoginBonusMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.SEQUENCE_LOGIN_BONUS_MASTER:
                    {
                        SequenceLoginBonusMaster[] o = JsonConvert.DeserializeObject<SequenceLoginBonusMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.LEVELDATA_LIST_MASTER:  //not exist
                    {
                        LevelDataListMaster[] o = JsonConvert.DeserializeObject<LevelDataListMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.UNIT_TALK_MASTER:
                    {
                        UnitTalkMaster[] o = JsonConvert.DeserializeObject<UnitTalkMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.GLOBAL_PARAM_MASTER:
                    {
                        GlobalParamsMaster[] o = JsonConvert.DeserializeObject<GlobalParamsMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CHALLENGE_MASTER:
                    {
                        QuestChallengeMaster[] o = JsonConvert.DeserializeObject<QuestChallengeMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.QUEST_CHALLENGE_REWARD_MASTER:
                    {
                        QuestChallengeRewardMaster[] o = JsonConvert.DeserializeObject<QuestChallengeRewardMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.SP_EVENT_MASTER:
                    {
                        SpEventMaster[] o = JsonConvert.DeserializeObject<SpEventMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.PANEL_SKILL_MASTER:
                    {
                        PanelSkillMaster[] o = JsonConvert.DeserializeObject<PanelSkillMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.PANEL_SKILL_RANK_MASTER:
                    {
                        PanelSkillRankMaster[] o = JsonConvert.DeserializeObject<PanelSkillRankMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.ACCESSORY_MASTER:
                    {
                        AccessoryMaster[] o = JsonConvert.DeserializeObject<AccessoryMaster[]>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
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
            DataTable dt = ObjectConvert.ToDataTable(o);
            dt.PrimaryKey = new DataColumn[] { dt.Columns["level_data_id"] };
            return dt;
        }
        public static DataTable ParseJSONGAME(string json, MASTERDB MDBType)
        {
            JObject jo = JObject.Parse(json);
            DataTable dt = new DataTable();
            switch (MDBType)
            {
                case MASTERDB.ENEMY_TABLE_MASTER:
                    {
                        JToken jt = jo["enemy_table_master"];
                        string jtjson = JsonConvert.SerializeObject(jt);
                        EnemyTableMaster[] o = new EnemyTableMaster[1];
                        o[0] = JsonConvert.DeserializeObject<EnemyTableMaster>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                case MASTERDB.UNIT_TALK_MASTER:
                    {
                        JToken jt = jo["unit_talk_master"];
                        string jtjson = JsonConvert.SerializeObject(jt);
                        UnitTalkMaster[] o = new UnitTalkMaster[1];
                        o[0] = JsonConvert.DeserializeObject<UnitTalkMaster>(jtjson);
                        dt = ObjectConvert.ToDataTable(o);
                        break;
                    }
                default: break;
            }
            dt.TableName = MDBType.ToString();
            dt.PrimaryKey = new DataColumn[] { dt.Columns["id"] };
            return dt;
        }
        public static List<EnemyInfo> ParseEnemyInfo(string questId, string quest, string enemyInfo)
        {
            List<EnemyInfo> ei = new List<EnemyInfo>();
            if (String.IsNullOrWhiteSpace(quest))   //iOS workaround
            {
                ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(enemyInfo);
            }
            else
            {
                string currentQuestId = JObject.Parse(quest)["m_QuestID"].ToString();
                if (questId == currentQuestId)
                {
                    ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(enemyInfo);
                }
            }
            return ei;
        }
    }
}