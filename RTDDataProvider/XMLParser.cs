using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Data;
using System.Reflection;
using PList;
using System.IO;

namespace RTDDataProvider
{
    /// <summary>
    /// MDB对应的枚举
    /// </summary>
    public enum MASTERDB
    {
        USER_RANK_MASTER = 60,
        UNIT_MASTER = 10,
        PARTY_SKILL_MASTER,
        PARTY_SKILL_RANK_MASTER,
        ACTIVE_SKILL_MASTER,
        ACTIVE_SKILL_RANK_MASTER,
        ENEMY_UNIT_MASTER = 20,
        ENEMY_TABLE_MASTER,
        ENEMY_DROP_MASTER,  //not exist
        QUEST_MASTER = 30,
        QUEST_CATEGORY_MASTER,
        GACHA_ITEM_MASTER = 40, //not exist
        GACHA_TABLE_MASTER, //not exist
        SHOP_PRODUCT_MASTER,    //not exist
        SHOP_PRODUCT_MASTER_ANDROID,
        LOGIN_BONUS_MASTER = 51,
        SEQUENCE_LOGIN_BONUS_MASTER,
        LEVELDATA_LIST_MASTER = 70, //not exist
        UNIT_TALK_MASTER = 15,
        GLOBAL_PARAM_MASTER = 90,
        QUEST_CHALLENGE_MASTER = 32,
        QUEST_CHALLENGE_REWARD_MASTER,
        SP_EVENT_MASTER,
        PANEL_SKILL_MASTER = 16,
        PANEL_SKILL_RANK_MASTER,
        MAX = 22    //not exist
    }
    public static class XMLParser
    {
        public static DataSet ParseMDB(string xmlMDBstring)
        {
            XmlDocument xmlMDB = new XmlDocument();
            DataSet ds = new DataSet("MDB");

            xmlMDB.LoadXml(xmlMDBstring);
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
        public static DataTable ParseLDB(string xmlLDBstring)
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
            xmlLDB.LoadXml(xmlLDBstring);
            foreach (XmlNode xmlNode in xmlLDB.GetElementsByTagName("string"))
            {
                if (xmlNode.Attributes["name"] != null)
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
    }
}