using RTDDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RTDDataExecuter
{
    public partial class MainWindow : Window
    {
        private void InitMap(string levelID, int repeat = 1)
        {
            bool isShowDropInfo = (bool)IsShowDropInfo.IsChecked;

            Task<DataTable> initMonsterTask = new Task<DataTable>(GetMonsterData, levelID);
            initMonsterTask.ContinueWith(t =>
            {
                MapMonsterGrid.ItemsSource = t.Result.DefaultView;
            }, uiTaskScheduler);

            Task<MapTable> task = new Task<MapTable>(() =>
            {
                DB db = new DB();
                DataTable dt = db.GetData("SELECT a.*,b.distance FROM level_data_master a left join quest_master b on a.level_data_id=b.id WHERE a.level_data_id=" + levelID);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("数据库取数失败。");
                }
                DataRow levelData = dt.Rows[0];
                if (isShowDropInfo == false)
                {
                    return BindMonsterDataToMap(InitMapData(
                    levelData["map_data"].ToString(),
                    Convert.ToInt32(levelData["width"]),
                    Convert.ToInt32(levelData["height"]),
                    Convert.ToInt32(levelData["start_x"]),
                    Convert.ToInt32(levelData["start_y"]),
                    Convert.ToInt32(levelData["distance"]),
                    repeat
                    ), initMonsterTask.Result);
                }
                else
                {
                    return BindDropDataToMap(BindMonsterDataToMap(InitMapData(
                    levelData["map_data"].ToString(),
                    Convert.ToInt32(levelData["width"]),
                    Convert.ToInt32(levelData["height"]),
                    Convert.ToInt32(levelData["start_x"]),
                    Convert.ToInt32(levelData["start_y"]),
                    Convert.ToInt32(levelData["distance"]),
                    repeat
                    ), initMonsterTask.Result), GetEnenyInfo(levelID));
                }
            }
            );
            task.ContinueWith(t =>
            {
                ClearMap();
                if (t.Exception != null)
                {
                    StatusBarExceptionMessage.Text = t.Exception.InnerException.Message;
                    return;
                }
                else
                {
                    DrawMap(t.Result);
                }
            }, uiTaskScheduler);
            task.Start();
            initMonsterTask.Start();
        }

        /*按列生成地图
        private void DrawMap(string mapData, int w, int h, int x, int y, int distance, int repeat)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            var mapTable = new MapTable();
            //循环生成列
            //-1列用于序号
            for (int j = -1; j < h; j++)
            {
                var mapColumn = new MapColumn();
                for (int i = 0; i < (w * repeat) + 1; i++)
                {
                    var mapCell = new MapCell();
                    if (j == -1)    //序号列
                    {
                        mapCell.CellData = ((y - i) % w).ToString();
                    }
                    else if (i % w == y && j == x)  //起点
                    {
                        mapCell.CellData = "★";
                    }
                    else if (i == w * repeat)   //序号行
                    {
                        mapCell.CellData = (distance - j + 3).ToString();    //magic number 3!
                    }
                    else
                    {
                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                        int num = 7 & cellDataInt >> 5;
                        if (num > 0)
                        {
                            switch (1 << (num - 1))
                            {
                                //AttributeTypeLight,
                                case 1:
                                    {
                                        mapCell.Background = Brushes.Yellow;
                                        break;
                                    }
                                //AttributeTypeDark,
                                case 2:
                                    {
                                        mapCell.Background = Brushes.Purple;
                                        mapCell.Foreground = Brushes.White;
                                        break;
                                    }
                                //AttributeTypeFire = 4,
                                case 4:
                                    {
                                        mapCell.Background = Brushes.Pink;
                                        break;
                                    }
                                //AttributeTypeWater = 8,
                                case 8:
                                    {
                                        mapCell.Background = Brushes.Aqua;
                                        break;
                                    }
                                //AttributeTypeBossStart = 16,
                                case 16:
                                    {
                                        mapCell.Background = Brushes.Silver;
                                        break;
                                    }
                                //AttributeTypeBoss = 32,
                                case 32:
                                    {
                                        mapCell.Background = Brushes.Black;
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
                                //td.Attributes.Add("enemy", cellData);
                            }
                            if (num2 >= 17)
                            {
                                //E17以上一般都是史莱姆
                                mapCell.Foreground = Brushes.Red;
                                mapCell.fontWeight = FontWeights.Bold;
                            }
                        }
                        if ((cellDataInt == 0) || (num2 == 0))
                        {
                            cellData = "";
                        }
                        mapCell.CellData = cellData;
                    }
                    mapColumn.MapCells.Add(mapCell);
                }
                mapTable.MapColumns.Add(mapColumn);
            }
            MapDataGrid.DataContext = mapTable;
        }*/

        private MapTable InitMapData(string mapData, int w, int h, int x, int y, int distance, int repeat)
        {
            if (repeat < 1)
            {
                repeat = 1;
            }
            var mapTable = new MapTable();
            mapTable.x = x;
            mapTable.y = y;
            mapTable.h = h;
            mapTable.w = w;
            mapTable.repeat = repeat;

            for (int r = 0; r < repeat; r++)
            {
                for (int i = 0; i < w; i++)
                {
                    var mapRow = new MapRow();
                    for (int j = 0; j < h; j++)
                    {
                        var mapCell = new MapCell();

                        string cellData = mapData.Substring((j * w + i) * 2, 2);
                        int cellDataInt = int.Parse(cellData, System.Globalization.NumberStyles.HexNumber);
                        int num = 7 & cellDataInt >> 5;
                        if (num > 0)
                        {
                            switch (1 << (num - 1))
                            {
                                //AttributeTypeLight,
                                case 1:
                                    {
                                        mapCell.Background = Brushes.Yellow;
                                        break;
                                    }
                                //AttributeTypeDark,
                                case 2:
                                    {
                                        mapCell.Background = Brushes.Purple;
                                        mapCell.Foreground = Brushes.White;
                                        break;
                                    }
                                //AttributeTypeFire = 4,
                                case 4:
                                    {
                                        mapCell.Background = Brushes.Pink;
                                        break;
                                    }
                                //AttributeTypeWater = 8,
                                case 8:
                                    {
                                        mapCell.Background = Brushes.Aqua;
                                        break;
                                    }
                                //AttributeTypeBossStart = 16,
                                case 16:
                                    {
                                        mapCell.Background = Brushes.Silver;
                                        break;
                                    }
                                //AttributeTypeBoss = 32,
                                case 32:
                                    {
                                        mapCell.Background = Brushes.Black;
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
                                //td.Attributes.Add("enemy", cellData);
                            }
                            if (num2 >= 17)
                            {
                                //E17以上一般都是史莱姆
                                mapCell.Foreground = Brushes.Red;
                                mapCell.fontWeight = FontWeights.Bold;
                            }
                        }
                        if ((cellDataInt == 0) || (num2 == 0))
                        {
                            cellData = "";
                        }
                        if (i == y && j == x)
                        {
                            cellData = "★";
                        }

                        mapCell.CellData = cellData;
                        mapCell.x = i;
                        mapCell.y = j;
                        mapRow.Cells.Add(mapCell);
                    }
                    mapTable.Rows.Add(mapRow);
                }
            }

            //添加底部标记
            var mapMarkRow = new MapRow();
            for (int j = 0; j < h; j++)
            {
                var mapCellMark = new MapCell((distance - j + 3).ToString());    //magic number 3!
                mapMarkRow.Cells.Add(mapCellMark);
            }
            mapTable.Rows.Add(mapMarkRow);

            return mapTable;
        }

        private void ClearMap()
        {
            MapGrid.Children.Clear();
            MapGrid.ColumnDefinitions.Clear();
            MapGrid.RowDefinitions.Clear();
            MapMarkGrid.Children.Clear();
            MapMarkGrid.ColumnDefinitions.Clear();
            MapMarkGrid.RowDefinitions.Clear();
        }

        private void DrawMap(MapTable map)
        {
            bool isFirstColDef = false;
            int row = 0;
            foreach (MapRow r in map.Rows)
            {
                int col = 0;
                MapGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(25)
                });
                foreach (MapCell c in r.Cells)
                {
                    if (!isFirstColDef)
                    {
                        MapGrid.ColumnDefinitions.Add(new ColumnDefinition()
                        {
                            Width = new GridLength(25)
                        });
                    }
                    TextBox tb = new TextBox()
                    {
                        Text = c.CellData,
                        Foreground = c.Foreground,
                        Background = c.Background,
                        FontWeight = c.fontWeight,
                        BorderBrush = c.BorderBrush,
                        BorderThickness = c.BorderThickness
                    };
                    MapGrid.Children.Add(tb);

                    tb.SetValue(Grid.RowProperty, row);
                    tb.SetValue(Grid.ColumnProperty, col);

                    if (String.IsNullOrWhiteSpace(c.drop_unit_id) == false)
                    {
                        if (c.drop_unit_id != "0")
                        {
                            tb.ToolTip = new TextBlock() { Text = parseUnitName(c.drop_unit_id) + "\n" + "觉醒pt:" + c.add_attribute_exp };
                        }
                        else
                        {
                            tb.ToolTip = new TextBlock() { Text = "觉醒pt:" + c.add_attribute_exp };
                        }
                    }
                    col++;
                }
                isFirstColDef = true;
                row++;
            }
            //绘制冻结行标记
            MapMarkGrid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(25)
            });
            for (int i = 0; i < row; i++)
            {
                MapMarkGrid.RowDefinitions.Add(new RowDefinition()
                {
                    Height = new GridLength(25)
                });
                TextBox tb = new TextBox()
                {
                    Text = (i == (row - 1)) ? string.Empty : ((map.y - i) % map.h).ToString()
                };
                MapMarkGrid.Children.Add(tb);

                tb.SetValue(Grid.RowProperty, i);
                tb.SetValue(Grid.ColumnProperty, 0);
            }
        }

        private DataTable GetMonsterData(object param)
        {
            string levelID = param.ToString();
            DB db = new DB();
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

            if (questData.Rows.Count == 0)
            {
                return monsterData;
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

            return monsterData;
        }

        private MapTable BindMonsterDataToMap(MapTable map, DataTable monsterData)
        {
            foreach (MapRow r in map.Rows)
            {
                foreach (MapCell c in r.Cells)
                {
                    string enemyNo = c.CellData;
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
                                c.CellData = enemyDropId.ToString() + "↑";
                                break;
                            }
                        case "65003":   //移動床_直進
                            {
                                int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                enemyDropId -= 299;
                                c.CellData = enemyDropId.ToString() + "→";
                                break;
                            }
                        case "65004":   //移動床_下
                            {
                                int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                enemyDropId -= 199;
                                c.CellData = enemyDropId.ToString() + "↓";
                                break;
                            }
                        case "22000":   //宝箱
                            {
                                c.CellData = "箱";
                                c.Foreground = Brushes.Red;
                                c.fontWeight = FontWeights.Bold;
                                break;
                            }
                        case "40100":   //上り階段
                            {
                                int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                //td.Text = "↗" + enemyDropId.ToString();
                                //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↗</a>";
                                c.CellData = "↗";
                                break;
                            }
                        case "65001":   //下り階段
                            {
                                int enemyDropId = Convert.ToInt32(foundRow[0]["drop_id"]);
                                //td.Text = "↘" + enemyDropId.ToString();
                                //tb.Text = "<a href='Map.aspx?id=" + enemyDropId + "'>↘</a>";
                                c.CellData = "↘";
                                break;
                            }
                        default: break;
                    }
                }
            }
            return map;
        }

        private MapTable BindDropDataToMap(MapTable map, List<EnemyInfo> DropData)
        {
            if (DropData == null || DropData.Count == 0)
            {
                return map;
            }
            foreach (MapRow r in map.Rows)
            {
                foreach (MapCell c in r.Cells)
                {
                    EnemyInfo ei = DropData.Find(o => { return o.x == c.x && o.y == c.y && !(o.x == 0 && o.y == 0); });
                    if (ei != null)
                    {
                        c.drop_unit_id = ei.drop_unit_id.ToString();
                        c.add_attribute_exp = ei.add_attribute_exp.ToString();
                        switch (ei.drop_unit_id)
                        {
                            case 15004:
                            case 15005:
                            case 15006:
                            case 15007:
                                {
                                    c.BorderBrush = Brushes.Green;
                                    c.BorderThickness = new Thickness(2.5);
                                    break;
                                }
                            case 15022:
                                {
                                    c.BorderBrush = Brushes.Black;
                                    c.BorderThickness = new Thickness(2.5);
                                    break;
                                }
                            case 15025:
                            case 15026:
                            case 15027:
                                {
                                    c.BorderBrush = Brushes.MediumTurquoise;
                                    c.BorderThickness = new Thickness(2.5);
                                    break;
                                }
                        }
                    }
                }
            }
            MapCell lastCell = map.Rows.Find(o => { return true; }).Cells.FindLast(o => { return true; });
            EnemyInfo bossInfo = DropData.Find(o => { return o.enemy_id == 0; });
            lastCell.drop_unit_id = bossInfo.drop_unit_id.ToString();
            lastCell.add_attribute_exp = bossInfo.add_attribute_exp.ToString();
            return map;
        }

        private List<EnemyInfo> GetEnenyInfo(string levelID)
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
                try
                {
                    ei = XMLParser.ParseEnemyInfo(levelID, questXml, dropXml);
                }
                catch (Exception ex)
                {
                    StatusBarExceptionMessage.Text = ex.Message;
                }
            }
            else if (File.Exists(iosFileName))
            {
                using (StreamReader sr = new StreamReader(iosFileName))
                {
                    try
                    {
                        ei = XMLParser.ParseEnemyInfo(levelID, sr.BaseStream);
                    }
                    catch (Exception ex)
                    {
                        StatusBarExceptionMessage.Text = ex.Message;
                    }
                }
            }
            return ei;
        }
    }
}
