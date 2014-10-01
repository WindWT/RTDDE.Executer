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

        public class JsonGAME
        {
            public string LDM { get; set; }
            public string ETM { get; set; }
            public string UTM { get; set; }
        }
        public static JsonGAME GetJsonFromGAME(string json)
        {
            JsonGAME game = new JsonGAME();
            game.LDM = json;
            JObject jo = JObject.Parse(json);
            game.ETM = JsonConvert.SerializeObject(jo["enemy_table_master"]);
            game.UTM = JsonConvert.SerializeObject(jo["unit_talk_master"]);
            return game;
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