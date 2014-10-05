using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTDDataProvider.MasterData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace RTDDataProvider
{
    public class MapData
    {
        public string LDM { get; set; }
        public string ETM { get; set; }
        public string UTM { get; set; }

        public MapData(string json)
        {
            initMapData(json);
        }
        public static List<MapData> FromPlist(Stream plistFileStream)
        {
            var reader = new System.Runtime.Serialization.Plists.BinaryPlistReader();
            var dict = reader.ReadObject(plistFileStream);
            var list = new List<MapData>();

            Dictionary<int, int> objIndex = new Dictionary<int, int>();
            object[] NSKeyObjects = (((dict["$objects"] as Object[])[1] as IDictionary<object, object>)["NS.keys"] as object[]);
            object[] NSValueObjects = (((dict["$objects"] as Object[])[1] as IDictionary<object, object>)["NS.objects"] as object[]);
            for (int i = 0; i < NSKeyObjects.Length; i++)
            {
                objIndex.Add(Convert.ToInt32((NSKeyObjects[i] as Dictionary<String, UInt64>)["CF$UID"]), Convert.ToInt32((NSValueObjects[i] as Dictionary<String, UInt64>)["CF$UID"]));
            }
            foreach (KeyValuePair<int, int> o in objIndex)
            {
                if ((dict["$objects"] as Object[])[o.Key].ToString().StartsWith("LDBS"))
                {
                    string json = (dict["$objects"] as Object[])[o.Value].ToString();
                    list.Add(new MapData(json));
                }
            }
            return list;
        }
        private void initMapData(string json)
        {
            LDM = json;
            JObject jo = JObject.Parse(json);
            ETM = JsonConvert.SerializeObject(jo["enemy_table_master"]);
            UTM = JsonConvert.SerializeObject(jo["unit_talk_master"]);
        }
        public static List<EnemyInfo> GetEnemyInfo(string levelID)
        {
            List<EnemyInfo> ei = new List<EnemyInfo>();
            string questFileNameAndroid = "GAME.xml";
            string dropFileNameAndroid = "com.prime31.UnityPlayerNativeActivity.xml";
            string questFileNameiOS = "GAME";
            string dropFileNameiOS = "jp.co.acquire.RTD.plist";
            if (File.Exists(questFileNameAndroid) && File.Exists(dropFileNameAndroid))
            {
                string questXml = string.Empty, dropXml = string.Empty;
                using (StreamReader sr = new StreamReader(questFileNameAndroid))
                {
                    questXml = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(dropFileNameAndroid))
                {
                    dropXml = sr.ReadToEnd();
                }
                ei = ParseEnemyInfoAndroid(levelID, questXml, dropXml);

            }
            else if (File.Exists(questFileNameiOS) && File.Exists(dropFileNameiOS))
            {
                using (StreamReader srDrop = new StreamReader(dropFileNameiOS), srQuest = new StreamReader(questFileNameiOS))
                {
                    ei = ParseEnemyInfoiOS(levelID, srQuest.BaseStream, srDrop.BaseStream);
                }
            }
            return ei;
        }
        private static List<EnemyInfo> ParseEnemyInfoAndroid(string questId, string xmlQuestString, string xmlEnemyInfoString)
        {
            XmlDocument xmlQuestInfo = new XmlDocument();
            XmlDocument xmlEnemyInfo = new XmlDocument();
            string jsonQuest = String.Empty, jsonEnemyInfo = String.Empty;

            xmlQuestInfo.LoadXml(xmlQuestString);
            foreach (XmlNode xmlNode in xmlQuestInfo.GetElementsByTagName("string"))
            {
                var attr = xmlNode.Attributes["name"];
                if (attr != null && attr.Value == "RESTORE")
                {
                    jsonQuest = xmlNode.InnerText;
                    break;
                }
            }
            xmlEnemyInfo.LoadXml(xmlEnemyInfoString);
            foreach (XmlNode xmlNode in xmlEnemyInfo.GetElementsByTagName("string"))
            {
                var attr = xmlNode.Attributes["name"];
                if (attr != null && attr.Value == "QUEST_ENEMY_INFO")
                {
                    jsonEnemyInfo = xmlNode.InnerText;
                    break;
                }
            }
            List<EnemyInfo> ei = new List<EnemyInfo>();

            string currentQuestId = JObject.Parse(jsonQuest)["m_QuestID"].ToString();
            if (questId == currentQuestId)
            {
                ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(jsonEnemyInfo);
            }
            return ei;
        }
        private static List<EnemyInfo> ParseEnemyInfoiOS(string questId, Stream questStream, Stream dropStream)
        {
            var reader = new System.Runtime.Serialization.Plists.BinaryPlistReader();
            IDictionary dictQuest = reader.ReadObject(questStream);
            IDictionary dictDrop = reader.ReadObject(dropStream);
            string jsonEnemyInfo = String.Empty, currentQuestId = string.Empty;

            Dictionary<int, int> objIndex = new Dictionary<int, int>();
            object[] NSKeyObjects = (((dictQuest["$objects"] as Object[])[1] as IDictionary<object, object>)["NS.keys"] as object[]);
            object[] NSValueObjects = (((dictQuest["$objects"] as Object[])[1] as IDictionary<object, object>)["NS.objects"] as object[]);
            for (int i = 0; i < NSKeyObjects.Length; i++)
            {
                objIndex.Add(Convert.ToInt32((NSKeyObjects[i] as Dictionary<String, UInt64>)["CF$UID"]), Convert.ToInt32((NSValueObjects[i] as Dictionary<String, UInt64>)["CF$UID"]));
            }
            foreach (KeyValuePair<int, int> o in objIndex)
            {
                if ((dictQuest["$objects"] as Object[])[o.Key].ToString() == "RESTORE")
                {
                    string jsonQuest = (dictQuest["$objects"] as Object[])[o.Value].ToString();
                    currentQuestId = JObject.Parse(jsonQuest)["m_QuestID"].ToString();
                }
            }
            if (questId == currentQuestId)
            {
                jsonEnemyInfo = dictDrop["QUEST_ENEMY_INFO"].ToString();
            }
            return JsonConvert.DeserializeObject<List<EnemyInfo>>(jsonEnemyInfo);
        }
    }
}
