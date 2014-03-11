using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Reflection;
using PList;
using System.IO;
using RTDDataProvider.MasterData;

namespace RTDDataProvider
{
    public static class FileParser
    {
        #region Android
        public static DataSet ParseXmlMDB(string xmlMDBString)
        {
            XmlDocument xmlMDB = new XmlDocument();
            DataSet ds = new DataSet("MDB");

            xmlMDB.LoadXml(xmlMDBString);
            foreach (XmlNode xmlNode in xmlMDB.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null)
                {
                    string MDBID = xmlNode.Attributes["name"].Value;
                    string MDBenumID = MDBID.Replace("MDBS", String.Empty);
                    string jsonMDB = xmlNode.InnerText;
                    DataTable dt = JSON.ParseJSONMDB(jsonMDB, (MASTERDB)Enum.Parse(typeof(MASTERDB), MDBenumID, true));
                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }
        public static DataTable ParseXmlLDB(string xmlLDBString)
        {
            XmlDocument xmlLDB = new XmlDocument();
            DataTable dt = new DataTable("LEVEL_DATA_MASTER");
            DataColumn dc = null;
            foreach (FieldInfo fi in typeof(LevelDataMaster).GetFields())
            {
                dc = new DataColumn();
                dc.ColumnName = fi.Name;
                dc.DataType = fi.FieldType;
                dt.Columns.Add(dc);
            }
            xmlLDB.LoadXml(xmlLDBString);
            foreach (XmlNode xmlNode in xmlLDB.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null && xmlNode.Attributes["name"].Value.StartsWith("LDBS"))
                {
                    string jsonLDB = xmlNode.InnerText;
                    DataTable dtTemp = JSON.ParseJSONLDB(jsonLDB);
                    object[] obj = new object[dtTemp.Columns.Count];
                    dtTemp.Rows[0].ItemArray.CopyTo(obj, 0);
                    dt.Rows.Add(obj);
                }
            }
            return dt;
        }
        public static DataSet ParseXmlGAME(string xmlGAMEString)
        {
            XmlDocument xmlGAME = new XmlDocument();
            DataSet ds = new DataSet("MDB");

            xmlGAME.LoadXml(xmlGAMEString);
            foreach (XmlNode xmlNode in xmlGAME.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null && xmlNode.Attributes["name"].Value.StartsWith("LDBS"))
                {
                    string json = xmlNode.InnerText;
                    ds.Tables.Add(JSON.ParseJSONGAME(json, MASTERDB.ENEMY_TABLE_MASTER));
                    ds.Tables.Add(JSON.ParseJSONGAME(json, MASTERDB.UNIT_TALK_MASTER));
                }
            }
            return ds;
        }
        public static List<EnemyInfo> ParseEnemyInfo(string questId, string xmlQuestString, string xmlEnemyInfoString)
        {
            XmlDocument xmlQuestInfo = new XmlDocument();
            XmlDocument xmlEnemyInfo = new XmlDocument();
            string jsonQuest = String.Empty, jsonEnemyInfo = String.Empty;

            xmlQuestInfo.LoadXml(xmlQuestString);
            foreach (XmlNode xmlNode in xmlQuestInfo.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null)
                {
                    if (xmlNode.Attributes["name"].Value == "RESTORE")
                    {
                        jsonQuest = xmlNode.InnerText;
                        break;
                    }
                }
            }
            xmlEnemyInfo.LoadXml(xmlEnemyInfoString);
            foreach (XmlNode xmlNode in xmlEnemyInfo.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null)
                {
                    if (xmlNode.Attributes["name"].Value == "QUEST_ENEMY_INFO")
                    {
                        jsonEnemyInfo = xmlNode.InnerText;
                        break;
                    }
                }
            }
            return JSON.ParseEnemyInfo(questId, jsonQuest, jsonEnemyInfo);
        }
        #endregion

        #region iOS
        public static DataSet ParsePlistMDB(Stream plistFileStream)
        {
            PListRoot plist = PListRoot.Load(plistFileStream);
            plist.Format = PListFormat.Binary;
            PListDict dict = (PListDict)plist.Root;
            DataSet ds = new DataSet("MDB");

            foreach (KeyValuePair<string, IPListElement> item in dict)
            {
                string key = item.Key;
                if (!string.IsNullOrWhiteSpace(key) && key.StartsWith("MDBS", StringComparison.Ordinal))
                {
                    string MDBID = key;
                    string MDBenumID = MDBID.Replace("MDBS", String.Empty);
                    string jsonMDB = (PListString)item.Value;
                    DataTable dt = JSON.ParseJSONMDB(jsonMDB, (MASTERDB)Enum.Parse(typeof(MASTERDB), MDBenumID, true));
                    ds.Tables.Add(dt);
                }
            }
            return ds;
        }
        public static DataTable ParsePlistLDB(Stream plistFileStream)
        {
            PListRoot plist = PListRoot.Load(plistFileStream);
            plist.Format = PListFormat.Binary;
            PListDict dict = (PListDict)plist.Root;
            DataTable dt = new DataTable("LEVEL_DATA_MASTER");
            DataColumn dc = null;
            foreach (FieldInfo fi in typeof(LevelDataMaster).GetFields())
            {
                dc = new DataColumn();
                dc.ColumnName = fi.Name;
                dc.DataType = fi.FieldType;
                dt.Columns.Add(dc);
            }
            foreach (KeyValuePair<string, IPListElement> item in dict)
            {
                string key = item.Key;
                if (!string.IsNullOrWhiteSpace(key) && key.StartsWith("LDBS", StringComparison.Ordinal))
                {
                    string jsonLDB = (PListString)item.Value;
                    DataTable dtTemp = JSON.ParseJSONLDB(jsonLDB);
                    object[] obj = new object[dtTemp.Columns.Count];
                    dtTemp.Rows[0].ItemArray.CopyTo(obj, 0);
                    dt.Rows.Add(obj);
                }
            }
            return dt;
        }
        public static DataTable ParsePlistFileMDB(Stream plistFileStream)
        {
            var reader = new System.Runtime.Serialization.Plists.BinaryPlistReader();
            var dict = reader.ReadObject(plistFileStream);
            string MDBenumID = (dict["$objects"] as Object[])[2].ToString().Replace("MDBS", String.Empty); ;
            string jsonMDB = (dict["$objects"] as Object[])[3].ToString();
            DataTable dt = JSON.ParseJSONMDB(jsonMDB, (MASTERDB)Enum.Parse(typeof(MASTERDB), MDBenumID, true));
            return dt;
        }
        public static DataTable ParsePlistFileLDB(Stream plistFileStream)
        {
            DataTable dt = new DataTable("LEVEL_DATA_MASTER");
            DataColumn dc = null;
            foreach (FieldInfo fi in typeof(LevelDataMaster).GetFields())
            {
                dc = new DataColumn();
                dc.ColumnName = fi.Name;
                dc.DataType = fi.FieldType;
                dt.Columns.Add(dc);
            }

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
                    string jsonLDB = (dict["$objects"] as Object[])[o.Value].ToString();
                    DataTable dtTemp = JSON.ParseJSONLDB(jsonLDB);
                    object[] obj = new object[dtTemp.Columns.Count];
                    dtTemp.Rows[0].ItemArray.CopyTo(obj, 0);
                    dt.Rows.Add(obj);
                }
            }
            return dt;
        }
        public static DataSet ParsePlistFileGAME(Stream plistFileStream)
        {
            DataSet ds = new DataSet("MDB");

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
                    ds.Tables.Add(JSON.ParseJSONGAME(json, MASTERDB.ENEMY_TABLE_MASTER));
                    ds.Tables.Add(JSON.ParseJSONGAME(json, MASTERDB.UNIT_TALK_MASTER));
                }
            }
            return ds;
        }
        public static List<EnemyInfo> ParseEnemyInfo(string questId, Stream plistFileStream)
        {
            PListRoot plist = PListRoot.Load(plistFileStream);
            plist.Format = PListFormat.Binary;
            PListDict dict = (PListDict)plist.Root;
            string jsonQuest = String.Empty, jsonEnemyInfo = String.Empty;
            foreach (KeyValuePair<string, IPListElement> item in dict)
            {
                string key = item.Key;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    if (key == "RESTORE")   //this keyValue is not valid
                    {
                        jsonQuest = (PListString)item.Value;
                    }
                    else if (key == "QUEST_ENEMY_INFO")
                    {
                        jsonEnemyInfo = (PListString)item.Value;
                    }
                }
            }
            return JSON.ParseEnemyInfo(questId, jsonQuest, jsonEnemyInfo);
        }
        #endregion


    }
}