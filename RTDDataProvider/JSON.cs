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