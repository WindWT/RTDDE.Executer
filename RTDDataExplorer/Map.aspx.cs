using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace RTDDataExplorer
{
    public partial class Map : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string levelID = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(levelID))
            {
                InitMap(levelID);
                InitQuestGrid(levelID);
                InitMonsterGrid(levelID);
                ReDrawMap(levelID);
            }
            else
            {
                Response.Write("<script language:javascript>javascript:window.close();</script>");
            }
        }
        private void InitMap(string levelID)
        {
            DB db = new DB(false);
            DataTable dt = db.GetData("SELECT a.*,b.distance FROM level_data_master a left join quest_master b on a.level_data_id=b.id WHERE a.level_data_id=" + levelID);
            if (dt.Rows.Count == 0)
            {
                map.Text = "无地图";
            }
            else
            {
                DataRow levelData = dt.Rows[0];
                map.Controls.AddAt(0,
                    DrawMap(
                    levelData["map_data"].ToString(),
                    Convert.ToInt32(levelData["width"]),
                    Convert.ToInt32(levelData["height"]),
                    Convert.ToInt32(levelData["start_x"]),
                    Convert.ToInt32(levelData["start_y"]),
                    Convert.ToInt32(levelData["distance"])
                    ));
            }
        }
        private Table DrawMap(string mapData, int w, int h, int x, int y, int distance, int repeat = 1)
        {
            Table mapTable = new Table();
            mapTable.CssClass = "map";
            for (int r = 0; r < repeat; r++)
            {
                for (int i = 0; i < w; i++)
                {
                    TableRow tr = new TableRow();
                    tr.CssClass = "map";
                    for (int j = 0; j < h; j++)
                    {
                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        if (i == y && j == x)
                        {
                            cellData = "★";
                        }
                        TableCell td = new TableCell();
                        td.CssClass = "map";
                        if (cellData != "★")
                        {
                            int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                            int num = 7 & cellDataInt >> 5;
                            if (num > 0)
                            {
                                switch (1 << (num - 1))
                                {
                                    //AttributeTypeLight,
                                    case 1:
                                        {
                                            td.BackColor = System.Drawing.Color.Yellow;
                                            break;
                                        }
                                    //AttributeTypeDark,
                                    case 2:
                                        {
                                            td.BackColor = System.Drawing.Color.Purple;
                                            td.ForeColor = System.Drawing.Color.White;
                                            break;
                                        }
                                    //AttributeTypeFire = 4,
                                    case 4:
                                        {
                                            td.BackColor = System.Drawing.Color.Pink;
                                            break;
                                        }
                                    //AttributeTypeWater = 8,
                                    case 8:
                                        {
                                            td.BackColor = System.Drawing.Color.Aqua;
                                            break;
                                        }
                                    //AttributeTypeBossStart = 16,
                                    case 16:
                                        {
                                            td.BackColor = System.Drawing.Color.Silver;
                                            break;
                                        }
                                    //AttributeTypeBoss = 32,
                                    case 32:
                                        {
                                            td.BackColor = System.Drawing.Color.Black;
                                            break;
                                        }
                                }
                            }

                            var num2 = 31 & cellDataInt;
                            if (num2 >= 24)
                            {
                                //this.TreasureID = num2 - 23;
                                //this.EnemyUnitID = 0;
                                //cellData = "箱" + (num2 - 23);
                                cellData = "箱";
                            }
                            else
                            {
                                //this.EnemyUnitID = num2;
                                //this.TreasureID = 0;
                                cellData = "E" + (num2);
                                if (num2 != 0)
                                {
                                    td.Attributes.Add("enemy", cellData);
                                }
                                if (num2 >= 17)
                                {
                                    //E17以上一般都是史莱姆
                                    td.ForeColor = System.Drawing.Color.Red;
                                    td.Font.Bold = true;
                                }
                            }
                            if ((cellDataInt == 0) || (num2 == 0))
                            {
                                cellData = "";
                            }
                        }
                        td.Text = cellData;
                        tr.Cells.Add(td);
                    }
                    //添加行标记
                    TableCell tdMark = new TableCell();
                    tdMark.CssClass = "map";
                    tdMark.Text = (y - i).ToString();
                    tr.Cells.AddAt(0, tdMark);

                    TableCell tdMarkEnd = new TableCell();
                    tdMarkEnd.CssClass = "map";
                    tdMarkEnd.Text = (y - i).ToString();
                    tr.Cells.Add(tdMarkEnd);

                    mapTable.Rows.Add(tr);
                }
            }
            //添加标记
            TableRow trMark = new TableRow();
            trMark.CssClass = "map";
            for (int j = 0; j < h; j++)
            {
                TableCell tdMark = new TableCell();
                tdMark.CssClass = "map";
                tdMark.Text = (distance - j + 3).ToString();  //magic number 3!
                trMark.Cells.Add(tdMark);
            }
            TableCell tdEmpty = new TableCell();
            tdEmpty.CssClass = "map";

            TableCell tdEmptyEnd = new TableCell();
            tdEmptyEnd.CssClass = "map";

            trMark.Cells.AddAt(0, tdEmpty);
            trMark.Cells.Add(tdEmptyEnd);

            mapTable.Rows.Add(trMark);
            return mapTable;
        }
        private void InitQuestGrid(string levelID)
        {
            DB db = new DB(false);
            DataTable questData = db.GetData("SELECT * FROM quest_master WHERE id=" + levelID);
            questGrid.DataSource = questData;
            questGrid.DataBind();
        }
        private void InitMonsterGrid(string levelID)
        {
            DB db = new DB(false);
            DataTable questData = db.GetData("SELECT * FROM quest_master WHERE id=" + levelID);
            DataTable monsterData = new DataTable();
            monsterData.Columns.Add("#", typeof(string));
            monsterData.Columns.Add("id", typeof(int));
            monsterData.Columns.Add("lv_min", typeof(int));
            monsterData.Columns.Add("lv_max", typeof(int));
            monsterData.Columns.Add("rate", typeof(int));
            monsterData.Columns.Add("drop_id", typeof(int));
            monsterData.Columns.Add("isDragon", typeof(int));
            monsterData.Columns.Add("name", typeof(string));
            monsterData.Columns.Add("model", typeof(string));
            monsterData.Columns.Add("texture", typeof(string));
            monsterData.Columns.Add("type", typeof(int));
            monsterData.Columns.Add("attribute", typeof(int));
            monsterData.Columns.Add("soul_pt", typeof(int));
            monsterData.Columns.Add("gold_pt", typeof(int));
            monsterData.Columns.Add("turn", typeof(int));
            //monsterData.Columns.Add("LvMinLife", typeof(int));
            monsterData.Columns.Add("LvMaxLife", typeof(int));
            //monsterData.Columns.Add("LvMinATK", typeof(int));
            monsterData.Columns.Add("LvMaxATK", typeof(int));
            //monsterData.Columns.Add("LvMinDEF", typeof(int));
            monsterData.Columns.Add("LvMaxDEF", typeof(int));

            questMonsterGrid.DataSource = monsterData;

            if (questData.Rows.Count == 0)
            {
                questMonsterGrid.DataBind();
                return;
            }
            string enemy_table_id = questData.Rows[0]["enemy_table_id"].ToString();
            DataTable enemyTableData = db.GetData("SELECT * FROM enemy_table_master WHERE id=" + enemy_table_id);
            for (int i = 1; i <= 18; i++)
            {
                if (enemyTableData.Rows[0]["enemy" + i + "_set_id"].ToString() != "0")
                {
                    monsterData.Rows.Add(
                    new object[]{
                        "E"+enemyTableData.Rows[0]["enemy"+i+"_id"],
                        enemyTableData.Rows[0]["enemy"+i+"_set_id"],
                        enemyTableData.Rows[0]["enemy"+i+"_lv_min"],
                        enemyTableData.Rows[0]["enemy"+i+"_lv_max"],
                        enemyTableData.Rows[0]["enemy"+i+"_rate"],
                        enemyTableData.Rows[0]["enemy"+i+"_drop_id"]
                    });
                }
            }
            monsterData.Rows.Add(
               new object[]{
                "DEATH",//实际上应该是DEATH，不知道为什么表名是boss
                enemyTableData.Rows[0]["boss_set_id"],
                enemyTableData.Rows[0]["boss_lv_min"],
                enemyTableData.Rows[0]["boss_lv_max"],
                enemyTableData.Rows[0]["boss_rate"],
                enemyTableData.Rows[0]["boss_drop_id"]
                });
            monsterData.Rows.Add(
               new object[]{
                "BOSS",//实际上应该是BOSS，不知道为什么表名是death
                enemyTableData.Rows[0]["death_set_id"],
                enemyTableData.Rows[0]["death_lv_min"],
                enemyTableData.Rows[0]["death_lv_max"],
                enemyTableData.Rows[0]["death_rate"],
                enemyTableData.Rows[0]["death_drop_id"]
                });
            foreach (DataRow dr in monsterData.Rows)
            {
                DataTable enemyUnitTable = db.GetData("SELECT * FROM enemy_unit_master WHERE id=" + dr["id"]);
                if (enemyUnitTable.Rows.Count == 0)
                {
                    continue;
                }
                DataRow enemyUnitRow = enemyUnitTable.Rows[0];
                dr["isDragon"] = enemyUnitRow["flag"];
                dr["name"] = enemyUnitRow["name"];
                dr["model"] = enemyUnitRow["model"];
                dr["texture"] = enemyUnitRow["texture"];
                dr["type"] = enemyUnitRow["type"];
                dr["attribute"] = enemyUnitRow["attribute"];
                dr["soul_pt"] = enemyUnitRow["soul_pt"];
                dr["gold_pt"] = enemyUnitRow["gold_pt"];
                dr["turn"] = enemyUnitRow["turn"];
                //dr["LvMinLife"] = RealCalc(Convert.ToInt32(enemyUnitRow["life"]), Convert.ToInt32(enemyUnitRow["up_life"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxLife"] = RealCalc(Convert.ToInt32(enemyUnitRow["life"]), Convert.ToInt32(enemyUnitRow["up_life"]), Convert.ToInt32(dr["lv_max"]));
                //dr["LvMinATK"] = RealCalc(Convert.ToInt32(enemyUnitRow["attack"]), Convert.ToInt32(enemyUnitRow["up_attack"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxATK"] = RealCalc(Convert.ToInt32(enemyUnitRow["attack"]), Convert.ToInt32(enemyUnitRow["up_attack"]), Convert.ToInt32(dr["lv_max"]));
                //dr["LvMinDEF"] = RealCalc(Convert.ToInt32(enemyUnitRow["defense"]), Convert.ToInt32(enemyUnitRow["up_defense"]), Convert.ToInt32(dr["lv_min"]));
                dr["LvMaxDEF"] = RealCalc(Convert.ToInt32(enemyUnitRow["defense"]), Convert.ToInt32(enemyUnitRow["up_defense"]), Convert.ToInt32(dr["lv_max"]));
            }

            questMonsterGrid.DataBind();
        }
        private void ReDrawMap(string levelID)
        {
            if (map.Text == "无地图")
                return;
            Table mapTable = (Table)map.Controls[0];
            DataTable monsterData = (DataTable)questMonsterGrid.DataSource;
            foreach (TableRow tr in mapTable.Rows)
            {
                foreach (TableCell td in tr.Cells)
                {
                    if (!String.IsNullOrWhiteSpace(td.Attributes["enemy"]))
                    {
                        string enemyNo = td.Attributes["enemy"];
                        DataRow[] foundRow = monsterData.Select("[#] = '" + enemyNo + "'");
                        if (foundRow.Length < 1)
                            continue;
                        string enemyId = foundRow[0]["id"].ToString();
                        switch (enemyId)
                        {
                            case "65002":   //移動床_上
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    enemyDropId -= 99;
                                    td.Text = enemyDropId.ToString() + "↑";
                                    break;
                                }
                            case "65003":   //移動床_直進
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    enemyDropId -= 299;
                                    td.Text = enemyDropId.ToString() + "→";
                                    break;
                                }
                            case "65004":   //移動床_下
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    enemyDropId -= 199;
                                    td.Text = enemyDropId.ToString() + "↓";
                                    break;
                                }
                            case "22000":   //宝箱
                                {
                                    td.Text = "箱";
                                    td.ForeColor = System.Drawing.Color.Red;
                                    td.Font.Bold = true;
                                    break;
                                }
                            case "40100":   //上り階段
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    //td.Text = "↗" + enemyDropId.ToString();
                                    td.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↗</a>";
                                    break;
                                }
                            case "65001":   //下り階段
                                {
                                    int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                    //td.Text = "↘" + enemyDropId.ToString();
                                    td.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↘</a>";
                                    break;
                                }
                            default: break;
                        }
                    }
                }
            }
        }
        private int RealCalc(int baseAttr, int up, int lv)
        {
            return (int)Math.Round(baseAttr * ((lv - 1) * (up * 0.01) + 1));
        }
    }
    [Serializable]
    public class EnemyArrange
    {
        public int enemy_id;
        public int arrange_id;
        public int lv_min;
        public int lv_max;
        public int arrange_rate;
        public int drop_id;
        public void SetMasterRecord()
        {
            //this.enemyData = ClientData.GetEnemyDataFromMasterID(this.enemy_id);
        }
        public bool IsDeath()
        {
            return this.arrange_id == 0;
        }
        public bool IsBoss()
        {
            return this.arrange_id == 99;
        }
    }
}