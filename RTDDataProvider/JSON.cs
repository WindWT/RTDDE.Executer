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
        /// <summary>
        /// 将JSON转换为实体List
        /// </summary>
        /// <typeparam name="T">JSON对应的MDB实体</typeparam>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static List<T> ToList<T>(string json)
        {
            JObject jo = JObject.Parse(json);
            JToken jt = jo["result"][0]["data_list"];   //只有MDB能这么玩，LDB不行
            return JsonConvert.DeserializeObject<List<T>>(jt.ToString());
        }
        public static T ToSingle<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static Dictionary<Type, string> GetJSONFromGAME(string json)
        {
            Dictionary<Type, string> dict = new Dictionary<Type, string>();
            dict.Add(typeof(LevelDataMaster), json);
            JObject jo = JObject.Parse(json);
            dict.Add(typeof(EnemyTableMaster), JsonConvert.SerializeObject(jo["enemy_table_master"]));
            dict.Add(typeof(UnitTalkMaster), JsonConvert.SerializeObject(jo["unit_talk_master"]));
            return dict;
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