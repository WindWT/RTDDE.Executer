using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RTDDataProvider.MasterData;
using System;
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
        public MapData(Stream plistFileStream)
        {
            var reader = new System.Runtime.Serialization.Plists.BinaryPlistReader();
            var dict = reader.ReadObject(plistFileStream);

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
                    initMapData(json);
                }
            }
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
            string questFileName = "GAME.xml";
            string dropFileName = "com.prime31.UnityPlayerNativeActivity.xml";
            string iosFileName = "jp.co.acquire.RTD.plist";
            if (File.Exists(questFileName) && File.Exists(dropFileName))
            {
                string questXml = string.Empty, dropXml = string.Empty;
                using (StreamReader sr = new StreamReader(questFileName))
                {
                    questXml = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(dropFileName))
                {
                    dropXml = sr.ReadToEnd();
                }
                ei = ParseEnemyInfo(levelID, questXml, dropXml);

            }
            else if (File.Exists(iosFileName))
            {
                using (StreamReader sr = new StreamReader(iosFileName))
                {
                    //ei = FileParser.ParseEnemyInfo(levelID, sr.BaseStream);
                }
            }
            return ei;
        }
        private static List<EnemyInfo> ParseEnemyInfo(string questId, string xmlQuestString, string xmlEnemyInfoString)
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
            if (String.IsNullOrWhiteSpace(jsonQuest))   //iOS workaround
            {
                ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(jsonEnemyInfo);
            }
            else
            {
                string currentQuestId = JObject.Parse(jsonQuest)["m_QuestID"].ToString();
                if (questId == currentQuestId)
                {
                    ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(jsonEnemyInfo);
                }
            }
            return ei;
        }
        //public static List<EnemyInfo> ParseEnemyInfo(string questId, Stream plistFileStream)
        //{
        //    PListRoot plist = PListRoot.Load(plistFileStream);
        //    plist.Format = PListFormat.Binary;
        //    PListDict dict = (PListDict)plist.Root;
        //    string jsonQuest = String.Empty, jsonEnemyInfo = String.Empty;
        //    foreach (KeyValuePair<string, IPListElement> item in dict)
        //    {
        //        string key = item.Key;
        //        if (!string.IsNullOrWhiteSpace(key))
        //        {
        //            if (key == "RESTORE")   //this keyValue is not valid
        //            {
        //                jsonQuest = (PListString)item.Value;
        //            }
        //            else if (key == "QUEST_ENEMY_INFO")
        //            {
        //                jsonEnemyInfo = (PListString)item.Value;
        //            }
        //        }
        //    }
        //    return JSON.ParseEnemyInfo(questId, jsonQuest, jsonEnemyInfo);
        //}
        //public static List<EnemyInfo> ParseEnemyInfo(string questId, string quest, string enemyInfo)
        //{
        //    List<EnemyInfo> ei = new List<EnemyInfo>();
        //    if (String.IsNullOrWhiteSpace(quest))   //iOS workaround
        //    {
        //        ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(enemyInfo);
        //    }
        //    else
        //    {
        //        string currentQuestId = JObject.Parse(quest)["m_QuestID"].ToString();
        //        if (questId == currentQuestId)
        //        {
        //            ei = JsonConvert.DeserializeObject<List<EnemyInfo>>(enemyInfo);
        //        }
        //    }
        //    return ei;
        //}
    }
}
